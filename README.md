Request0r
=========
A wrapper around .Net System.Net.HttpWebRequest classes with support for authentication and a fluent interface.

Handy to scrape websites that require authentication.

Examples
--------

### Juicy Ads

**Downloads a users site list**

```
	var email      = "address@email.com";
	var password   = "password";
	var postData	 = string.Format("username={0}&userpass={1}", email, password);
	var pattern	 = @"<td width=200><font size=3 face=arial><b>(?<name>[^<]+?)</b>[\s\S]*?<input type=hidden name=siteid value=(?<siteId>[0-9]+)>";
	var loginUri	 = new Uri("http://www.juicyads.com/login2.php");
	var uri		 = new Uri("http://www.juicyads.com/adminb.php");

	var underTest  = new Request0r().LogIn(loginUri, postData).DownloadString(uri);

	StringAssert.IsMatch(pattern, underTest.LastResponseContent);
```

Change Log
----------
* 0.2.0 Form upload
* 0.1.0 Login and download string