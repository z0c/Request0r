Request0r
=========
A wrapper around .Net System.Net.HttpWebRequest classes with support for authentication and a fluent interface.

Handy to crawl websites that require authentication.

Examples
--------

###Juicy Ads
The following example downloads a users site list from the Juicy Ads website
```
	var email      = "address@email.com";
	var password   = "password";
	var postData   = string.Format("username={0}&userpass={1}", email, password);
	var loginUri   = new Uri("http://www.juicyads.com/login2.php");
	var sellAdsUri = new Uri("http://www.juicyads.com/adminb.php");
	var result     = new Request0r().LogIn(loginUri, postData).DownloadString(sellAdsUri).LastResponseContent;
```

Change Log
----------
1.0.0 Basic functionality