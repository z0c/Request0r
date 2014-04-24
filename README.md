Request0r
=========
A wrapper around .Net System.Net.HttpWebRequest classes with support for authentication and a fluent interface.

Handy to crawl websites that require authentication.

Examples
--------

### Juicy Ads

**Downloads a users site list**

```
	var email      = "address@email.com";
	var password   = "password";
	var postData = string.Format("username={0}&userpass={1}", email, password);
	var pattern = @"<td width=200><font size=3 face=arial><b>(?<name>[^<]+?)</b>[\s\S]*?<input type=hidden name=siteid value=(?<siteId>[0-9]+)>";
	var loginUri = new Uri("http://www.juicyads.com/login2.php");
	var uri = new Uri("http://www.juicyads.com/adminb.php");

	var underTest = new Request0r().LogIn(loginUri, postData).DownloadString(uri);

	StringAssert.IsMatch(pattern, underTest.LastResponseContent);
```

**Adds a new site**

```
	var email      = "address@email.com";
	var password   = "password";	
	var postData = string.Format("username={0}&userpass={1}", email, password);
	var title = Guid.NewGuid().ToString();                
	var loginUri = new Uri("http://www.juicyads.com/login2.php");
	var addUri = new Uri("http://www.juicyads.com/admin-addwebsite.php");
	var form = new NameValueCollection();
	form.Add("url", "http://" + title + ".com");
	form.Add("title", title);
	form.Add("desc", title);
	form.Add("category", "tgppics");
	form.Add("niche", "other");
	form.Add("niche2", string.Empty);
	form.Add("rating", "400");
	form.Add("method", "step2");
	form.Add("siteid", string.Empty);

	var underTest = new Request0r().LogIn(loginUri, postData).UploadValues(addUri, form);

	StringAssert.Contains(title, underTest.LastResponseContent);
```

Change Log
----------
0.2.0 Form upload
0.1.0 Login and download string