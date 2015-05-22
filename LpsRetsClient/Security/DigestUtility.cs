using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace LpsRetsClient.Security
{
    public class DigestUtility
    {
        private string _uri;
        private string _username;
        private string _password;
        private string _realm;
        private string _nonce;
        private string _qop;
        private string _cnonce;
        private int _nc;
        private string _opaque;

        private static string CalculateMd5Hash(string input)
        {
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = MD5.Create().ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (var b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        private static string GetHeaderVarString(string varName, string header)
        {
            var regHeader = new Regex(string.Format(@"{0}=""([^""]*)""", varName));
            var matchHeader = regHeader.Match(header);
            return matchHeader.Success ? matchHeader.Groups[1].Value : null;
        }

        private static string GetHeaderVarNumeric(string varName, string header)
        {
            var regHeader = new Regex(string.Format(@"{0}=([0-9]*)", varName));
            var matchHeader = regHeader.Match(header);
            return matchHeader.Success ? matchHeader.Groups[1].Value : null;
        }

        private static int GetHeaderVarInteger(string varName, string header)
        {
            var valueString = GetHeaderVarNumeric(varName, header);
            var valueInt =0;
            if (!string.IsNullOrWhiteSpace(valueString))
            {
                valueString = valueString.TrimStart(new[] {'0'});
                if (!int.TryParse(valueString, out valueInt))
                    valueInt = 0;
            }
            return valueInt;
        }

        public static bool IsDigest(string header)
        {
            return (header.IndexOf("Digest", StringComparison.InvariantCultureIgnoreCase) >= 0);
        }

        private string GetDigestHeader(string uri, string method = "GET")
        {
            var list = new List<string>();
            _nc++;

            // Build digest response
            var ha1 = CalculateMd5Hash(string.Format("{0}:{1}:{2}", _username, _realm, _password));
            var ha2 = CalculateMd5Hash(string.Format("{0}:{1}", method, uri));
            var digestResponse = CalculateMd5Hash(string.Format("{0}:{1}:{2:00000000}:{3}:{4}:{5}", ha1, _nonce, _nc, _cnonce, _qop, ha2));
            
            // Build the new authorization header
            list.Add(string.Format("username=\"{0}\"", _username));
            list.Add(string.Format("realm=\"{0}\"", _realm));
            list.Add(string.Format("nonce=\"{0}\"", _nonce));
            list.Add(string.Format("uri=\"{0}\"", uri));
            list.Add(string.Format("cnonce=\"{0}\"", _cnonce));
            list.Add(string.Format("nc={0:00000000}", _nc)); // "nc" value isn't surrounded with quotes.
            if (_qop != null)
                list.Add(string.Format("qop=\"{0}\"", _qop));
            list.Add(string.Format("response=\"{0}\"", digestResponse));
            if (_opaque != null)
                list.Add(string.Format("opaque=\"{0}\"", _opaque));
            return string.Format("Digest {0}", string.Join(",", list));
        }

        private string GetCnonce()
        {
            if (string.IsNullOrWhiteSpace(_cnonce))
              _cnonce = Guid.NewGuid().ToString().Replace("-", "");
            return _cnonce;
        }
        
        public string UpdateAuthorizationHeader(string authorization, string uri, string password, string method)
        {
            if (!IsDigest(authorization))
                return string.Empty;

            _uri = uri;
            _password = password;
            _username = GetHeaderVarString("username", authorization);
            _realm = GetHeaderVarString("realm", authorization);
            _nonce = GetHeaderVarString("nonce", authorization);
            _qop = GetHeaderVarString("qop", authorization);
            _nc = GetHeaderVarInteger("nc", authorization);
            _cnonce = GetHeaderVarString("cnonce", authorization);
            _cnonce = GetCnonce(); // This ensures one is created if not already present
            _opaque = GetHeaderVarString("opaque", authorization);

            return GetDigestHeader(_uri, method);
        }

        public string CreateAuthorizationHeader(string wwwAuthenticateHeader, string uri, string username, string password, string method)
        {
            if (!IsDigest(wwwAuthenticateHeader))
                return string.Empty;

            _uri = uri;
            _username = username;
            _password = password;
            _realm = GetHeaderVarString("realm", wwwAuthenticateHeader);
            _nonce = GetHeaderVarString("nonce", wwwAuthenticateHeader);
            _qop = GetHeaderVarString("qop", wwwAuthenticateHeader);
            _opaque = GetHeaderVarString("opaque", wwwAuthenticateHeader);
            _nc = 0;
            _cnonce = GetCnonce();

            return GetDigestHeader(_uri, method);
        }
    }
}
