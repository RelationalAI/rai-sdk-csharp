/*
 * Copyright 2022 RelationalAI, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace RelationalAI
{
    using Apache.Arrow;
    using Apache.Arrow.Ipc;
    using HttpMultipartParser;
    using Microsoft.Data.Analysis;
    using Newtonsoft.Json;
    using RelationalAI.Credentials;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;
    using System.Web;
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

        public Task<object> DeleteAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.RequestAsync("DELETE", url, data, headers, parameters);
        }

        public Task<object> GetAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.RequestAsync("GET", url, data, headers, parameters);
        }

        public Task<object> PatchAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.RequestAsync("PATCH", url, data, headers, parameters);
        }

        public Task<object> PostAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.RequestAsync("POST", url, data, headers, parameters);
        }

        public Task<object> PutAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return this.RequestAsync("PUT", url, data, headers, parameters);
        }

        public async Task<object> RequestAsync(
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

            var accessToken = await this.GetAccessTokenAsync(this.GetHost(url));
            caseInsensitiveHeaders.Add("Authorization", string.Format("Bearer {0}", accessToken));
            return await this.RequestHelperAsync(method, url, data, caseInsensitiveHeaders, parameters);
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

        private async Task<string> GetAccessTokenAsync(string host)
        {
            if (!(this.context.Credentials is ClientCredentials))
            {
                throw new SystemException("credential not supported");
            }

            ClientCredentials creds = (ClientCredentials)this.context.Credentials;
            if (creds.AccessToken == null || creds.AccessToken.IsExpired)
            {
                creds.AccessToken = await this.RequestAccessTokenAsync(host, creds);
            }

            return creds.AccessToken.Token;
        }

        private string GetHost(string url)
        {
            return new Uri(url).Host;
        }

        private string GetUserAgent()
        {
            return $"rai-sdk-csharp/{SdkProperties.Version}";
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

        private async Task<AccessToken> RequestAccessTokenAsync(string host, ClientCredentials creds)
        {
            // Form the API request body.
            Dictionary<string, string> data = new Dictionary<string, string>()
            {
                {"client_id", creds.ClientID},
                {"client_secret", creds.ClientSecret},
                {"audience", String.Format("https://{0}", host)},
                {"grant_type", "client_credentials"}
            };
            string resp = await this.RequestHelperAsync("POST", creds.ClientCredentialsURL, data) as string;
            Dictionary<string, string> result =
                (Dictionary<string, string>)JsonConvert.DeserializeObject(resp, typeof(Dictionary<string, string>));

            return new AccessToken(result["access_token"], int.Parse(result["expires_in"]));
        }

        public async Task<object> RequestHelperAsync(
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
                var httpResponse = await client.SendAsync(request);
                var content = await httpResponse.Content.ReadAsByteArrayAsync();
                var contentType = httpResponse.Content.Headers.ContentType.MediaType;

                return contentType.ToLower() switch
                {
                    "application/json" => ReadString(content),
                    "multipart/form-data" => ParseMultipartResponse(content),
                    _ => throw new SystemException($"unsupported content-type: {contentType}")
                };
            }
        }

        public List<TransactionAsyncFile> ParseMultipartResponse(byte[] content)
        {
            var output = new List<TransactionAsyncFile>();

            var parser = MultipartFormDataParser.Parse(new MemoryStream(content));

            foreach (var file in parser.Files)
            {
                MemoryStream memoryStream = new MemoryStream();
                file.Data.CopyTo(memoryStream);
                byte[] buffer = memoryStream.ToArray();
                var txnAsyncFile = new TransactionAsyncFile(file.Name, buffer, file.FileName, file.ContentType);
                output.Add(txnAsyncFile);
            }
            return output;
        }

        public string ReadString(byte[] data)
        {
            return System.Text.Encoding.UTF8.GetString(data, 0, data.Length);
        }

        public List<ArrowRelation> ReadArrowFiles(List<TransactionAsyncFile> files)
        {
            var output = new List<ArrowRelation> ();
            foreach(var file in files)
            {
                if ("application/vnd.apache.arrow.stream".Equals(file.ContentType.ToLower()))
                {
                    MemoryStream memoryStream = new MemoryStream(file.Data);
                    memoryStream.Position = 0;

                    ArrowStreamReader reader = new ArrowStreamReader(memoryStream);
                    RecordBatch recordBatch;
                    while((recordBatch = reader.ReadNextRecordBatch()) != null)
                    {
                       var df = DataFrame.FromArrowRecordBatch(recordBatch);
                       foreach(var col in df.Columns)
                       {
                           List<Object> values = new List<object>();
                           for (var i = 0; i < col.Length; i++)
                           {
                               values.Add(col[i]);
                           }
                           output.Add(new ArrowRelation(col.Name, values));
                       }
                    }
                }
            } 

            return output;
        }

        public class Context
        {
            private ICredentials credentials;
            private string region;
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