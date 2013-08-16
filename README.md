# retsdotnet

A .NET client library for RETS.

# Get the code
    git clone git://github.com/lpsmls/retsdotnet.git

# Build
* Ensure you have "Allow NuGet to download missing packages during build" enabled.  This can be found under Tools...Options...Package Manager...General.
* Open LpsRetsClient.sln in Visual Studio 2012.
* Build.

# Prerequisites for Usage
* You must have obtained access to a RETS server through an agreement with a MLS.
* You must be familiar with the RETS standard.  Visit http://www.reso.org/specifications for more information.

# Additional Notes
* This library was originally created to facilitate testing of the PostObject transaction.  Although that transaction is fully implemented, the library is not yet complete.

# Supported Transactions
* Login
* Logout
* PostObject

# Examples
## Tests
The LpsRetsClient.Tests project contains integration tests.  To utilize these tests, you must modify the config file as needed to define the RETS server to use for testing purposes.

## PostObject Command Line Client
The PostObject project is a command line program that will perform the PostObject transaction against a RETS server.  Run the program with no arguments to see the help.

## Login transaction
```csharp
var uri = new Uri(options.LoginUrl);

var session = RetsSession.Create(
    uri.GetLeftPart(UriPartial.Authority), 
    uri.GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped), 
    port, 
    userName, 
    password, 
    userAgent, 
    userAgentAuth, 
    retsVersion,
    AuthenticationMethod.Digest);

var loginResponse = session.Login();
if (loginResponse.IsSuccessful)
{
    // Do stuff with session
}
```

## PostObject
```csharp
response = session.PostObject(new RetsPostObjectParams
{
	ContentType = options.ContentType,
	Accessibility = options.Accessibility,
	ContentDescription = options.Description,
	ContentLabel = options.Label,
	FileContents = options.FileContents,
	ObjectId = options.ObjectId,
	Resource = options.Resource,
	ResourceId = options.ResourceId,
	Type = options.Type,
	UpdateAction = options.Action
});

```

# License
MIT
