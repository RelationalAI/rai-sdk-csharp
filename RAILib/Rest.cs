namespace RAILib
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Web;
    using Newtonsoft.Json;
    using RAILib.Credentials;

    public class Rest
    {
        private Rest.Context context;

        public Rest(Rest.Context context)
        {
            this.context = context;
        }

        public static string EncodeQueryString(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            return string.Join("&", parameters.Select(kvp =>
                string.Format("{0}={1}", HttpUtility.UrlEncode(kvp.Key), HttpUtility.UrlEncode(kvp.Value))));
        }

        public string Delete(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.Request("DELETE", url, data, headers, parameters);
        }

        public string Get(
        string url,
        object data = null,
        Dictionary<string, string> headers = null,
        Dictionary<string, string> parameters = null)
        {
            return this.Request("GET", url, data, headers, parameters);
        }

        public string Patch(
        string url,
        object data = null, 
        Dictionary<string, string> headers = null,
        Dictionary<string, string> parameters = null)
        { 
            return this.Request("PATCH", url, data, headers, parameters);
        }

        public string Post(
        string url,
        object data = null, 
        Dictionary<string, string> headers = null,
        Dictionary<string, string> parameters = null)
        {
            return this.Request("POST", url, data, headers, parameters);
        }

        public string Put(
        string url,
        object data = null, 
        Dictionary<string, string> headers = null,
        Dictionary<string, string> parameters = null)
        {
            return this.Request("PUT", url, data, headers, parameters);
        }

        public string Request(
            string method,
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            Dictionary<string, string> caseInsensitiveHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers != null)
            {
                foreach (var kv in headers)
                {
                   caseInsensitiveHeaders.Add(kv.Key, kv.Value);
                }
            }

            caseInsensitiveHeaders.Add("Authorization", string.Format("Bearer {0}", this.GetAccessToken(this.GetHost(url))));
            return this.RequestHelper(method, url, data, caseInsensitiveHeaders, parameters);
        }

        private HttpContent EncodeContent(object body)
        {
            if (body == null)
            {
                return null;
            }

            if (!(body is string))
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(body));
                return new ByteArrayContent(stringContent.ReadAsByteArrayAsync().Result);
            }

            return new ByteArrayContent(System.Text.Encoding.UTF8.GetBytes((string)body));
        }

        private string GetAccessToken(string host)
        {
            if (!(this.context.Credentials is ClientCredentials))
            {
                throw new SystemException("credential not supported");
            }

            ClientCredentials creds = (ClientCredentials)this.context.Credentials;
            if (creds.AccessToken == null || creds.AccessToken.IsExpired)
            {
                creds.AccessToken = this.RequestAccessToken(host, creds);
            }

            return creds.AccessToken.Token;
        }

        private string GetHost(string url)
        {
            return new Uri(url).Host;
        }

        private string GetUserAgent()
        {
            // TODO: add version here
            return "rai-sdk-csharp";
        }

        private Dictionary<string, string> GetDefaultHeaders(Uri uri, Dictionary<string, string> headers = null)
        {
            headers = headers != null ? headers : new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!headers.ContainsKey("accept"))
            {
                headers.Add("Accept", "application/json");
            }

            if (!headers.ContainsKey("content-type"))
            {
                headers.Add("Content-Type", "application/json");
            }

            if (!headers.ContainsKey("host"))
            {
                headers.Add("Host", uri.Host);
            }

            if (!headers.ContainsKey("user-agent"))
            {
                headers.Add("User-Agent", this.GetUserAgent());
            }

            return headers;
        }

        private HttpRequestMessage PrepareHttpRequest(
            string method,
            Uri uri,
            HttpContent content,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters = null)
        {
            var uriBuilder = new UriBuilder(uri);
            if (parameters != null)
            {
                uriBuilder.Query = Rest.EncodeQueryString(parameters);
            }

            headers = this.GetDefaultHeaders(uri, headers);
            var request = new HttpRequestMessage(new HttpMethod(method), uriBuilder.Uri);
            request.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse(headers["accept"]));
            request.Content = content;

            // HttpClient does not allow to set content-type header if there is no body
            if (content != null)
            {
                request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(headers["content-type"]);
            }

            request.Headers.Host = headers["host"];
            request.Headers.Add("User-Agent", headers["user-agent"]);
            headers.Remove("accept");
            headers.Remove("content-type");
            headers.Remove("host");
            headers.Remove("user-agent");

            foreach (KeyValuePair<string, string> kv in headers)
            {
                request.Headers.TryAddWithoutValidation(kv.Key, kv.Value);
            }

            return request;
        }

        private AccessToken RequestAccessToken(string host, ClientCredentials creds)
        {
            // Form the API request body.
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"client_id", creds.ClientID},
                {"client_secret", creds.ClientSecret},
                {"audience", String.Format("https://{0}", host)},
                {"grant_type", "client_credentials"}
            };
            string resp = this.RequestHelper("POST", creds.ClientCredentialsURL, data);
            Dictionary<string, string> result =
                (Dictionary<string, string>)JsonConvert.DeserializeObject(resp, typeof(Dictionary<string, string>));

            return new AccessToken(result["access_token"], int.Parse(result["expires_in"]));
        }

        public string RequestHelper(
            string method,
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            Uri uri = new Uri(url);
            using (var client = new HttpClient())
            {
                // Set the API url
                client.BaseAddress = uri;
                // Create the POST request
                var request = this.PrepareHttpRequest(method, client.BaseAddress, this.EncodeContent(data), headers, parameters);
                // Get the result back or throws an exception.
                var httpRespTask = client.SendAsync(request);
                httpRespTask.Wait();
                var resultTask = httpRespTask.Result.Content.ReadAsStringAsync();
                resultTask.Wait();

                return resultTask.Result;
            }
        }

        public class Context
        {
            private ICredentials credentials;
            private string region;
            private string service = "transaction";

            public Context(string region = null, ICredentials credentials = null)
            {
                this.Region = region;
                this.Credentials = credentials;
            }

            public ICredentials Credentials
            {
                get => this.credentials;
                set => this.credentials = value;
            }

            public string Region
            {
                get => this.region;
                set => this.region = !String.IsNullOrEmpty(value) ? value : "us-east";
            }
        }
    }
}