using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace Request0r
{
// ReSharper disable once InconsistentNaming
    public class Request0r
    {
        public CookieContainer CookieContainer;
        public string UserAgent { get; set; }
        public Response LastResponse { get; private set; }

        public Request0r()
        {
            UserAgent = "Request0r";
        }

        /// <summary>
        /// Opens a uri and stores the content in LastResponseContent
        /// </summary>
        /// <param name="uri">Page Uri</param>
        /// <param name="followRedirections">True if redirections should be followed</param>
        /// <returns>this</returns>
        public Request0r DownloadString(Uri uri, bool followRedirections = true)
        {
            if (CookieContainer == null) CookieContainer = new CookieContainer();

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.UserAgent = UserAgent;
            request.Method = "GET";
            request.CookieContainer = CookieContainer;
            request.AllowAutoRedirect = followRedirections;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.Headers["Set-Cookie"] != null)
                    CookieContainer.SetCookies(new Uri(uri.Scheme + "://" + uri.Host), response.Headers["Set-Cookie"]);

// ReSharper disable once AssignNullToNotNullAttribute
                using (var stream = new StreamReader(response.GetResponseStream(), new UTF8Encoding()))
                    LastResponse = new Response
                    {
                        Content = stream.ReadToEnd(),
                        StatusCode = response.StatusCode
                    };
            }

            return this;
        }

        /// <summary>
        /// Posts crediantls to a url and stores the authentication cookies 
        /// </summary>
        /// <param name="uri">Uri to post</param>
        /// <param name="post">Credentials post data</param>
        /// <returns>The response content after rediractions</returns>
        public Request0r LogIn(Uri uri, string post)
        {
            if (CookieContainer == null) CookieContainer = new CookieContainer();

            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.CookieContainer = CookieContainer;
            request.UserAgent = UserAgent;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.AllowAutoRedirect = false;
            byte[] bytedata = Encoding.UTF8.GetBytes(post);
            request.ContentLength = bytedata.Length;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytedata, 0, bytedata.Length);
                requestStream.Close();
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                if (response.Headers["Set-Cookie"] != null)
                    CookieContainer.SetCookies(new Uri(uri.Scheme + "://" + uri.Host), response.Headers["Set-Cookie"]);

                if (!string.IsNullOrEmpty(response.Headers["Location"]))
                {
                    request = (HttpWebRequest) WebRequest.Create(
                        response.Headers["Location"].ToLowerInvariant().StartsWith("http")
                            ? response.Headers["Location"]
                            : uri.Scheme + "://" + uri.Host + "/" + response.Headers["Location"]);
                    request.Referer = uri.ToString();
                    request.CookieContainer = CookieContainer;
                    request.UserAgent = UserAgent;
                    request.Method = "GET";
                    request.AllowAutoRedirect = true;
                    using (var redirectResponse = (HttpWebResponse)request.GetResponse())
// ReSharper disable once AssignNullToNotNullAttribute
                    using (var stream = new StreamReader(redirectResponse.GetResponseStream(), new UTF8Encoding()))
                    {
                        LastResponse = new Response
                        {
                            Content = stream.ReadToEnd(),
                            StatusCode = response.StatusCode
                        };
                        return this;
                    }
                }

// ReSharper disable once AssignNullToNotNullAttribute
                using (var stream = new StreamReader(response.GetResponseStream(), new UTF8Encoding()))
                {
                    LastResponse = new Response
                    {
                        Content = stream.ReadToEnd(),
                        StatusCode = response.StatusCode
                    };
                    return this;
                }
            }
        }        

        /// <summary>
        /// Posts a form to a uri
        /// </summary>
        /// <param name="uri">Target uri</param>
        /// <param name="form">A collection for the form data</param>
        /// <returns>this</returns>
        public Request0r UploadValues(Uri uri, NameValueCollection form)
        {
            var client = new WebClient();
            client.Headers.Add(HttpRequestHeader.UserAgent, UserAgent);
            client.Headers.Add(HttpRequestHeader.Cookie, CookieContainer.GetCookieHeader(new Uri(uri.Scheme + "://" + uri.Host)));
            client.Headers.Add(HttpRequestHeader.ContentType, "application/x-www-form-urlencoded");
            var responseData = client.UploadValues(uri, form);
            LastResponse = new Response
            {
                Content = Encoding.ASCII.GetString(responseData)
                //,StatusCode = response.StatusCode
            };
            return this;
        }
    }
}
