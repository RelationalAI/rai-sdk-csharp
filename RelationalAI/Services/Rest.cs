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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Apache.Arrow;
using Apache.Arrow.Ipc;
using HttpMultipartParser;
using Microsoft.Data.Analysis;
using Newtonsoft.Json;
using RelationalAI.Credentials;
using RelationalAI.Models.Transaction;
using Relationalai.Protocol;

namespace RelationalAI.Services
{
    public class Rest
    {
        private readonly Context _context;

        public Rest(Context context)
        {
            _context = context;
        }

        public static string EncodeQueryString(Dictionary<string, string> parameters)
        {
            if (parameters == null)
            {
                return string.Empty;
            }

            return string.Join("&", parameters.Select(kvp =>
                $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        }

        public Task<object> DeleteAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return RequestAsync("DELETE", url, data, headers, parameters);
        }

        public Task<object> GetAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return RequestAsync("GET", url, data, headers, parameters);
        }

        public Task<object> PatchAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return RequestAsync("PATCH", url, data, headers, parameters);
        }

        public Task<object> PostAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return RequestAsync("POST", url, data, headers, parameters);
        }

        public Task<object> PutAsync(
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            return RequestAsync("PUT", url, data, headers, parameters);
        }

        public async Task<object> RequestAsync(
            string method,
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            var caseInsensitiveHeaders = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (headers != null)
            {
                foreach (var (key, value) in headers)
                {
                    caseInsensitiveHeaders.Add(key, value);
                }
            }

            var accessToken = await GetAccessTokenAsync(GetHost(url));
            caseInsensitiveHeaders.Add("Authorization", $"Bearer {accessToken}");
            return await RequestHelperAsync(method, url, data, caseInsensitiveHeaders, parameters);
        }

        public string ReadString(byte[] data)
        {
            return Encoding.UTF8.GetString(data, 0, data.Length);
        }

        /*public List<ArrowRelation> ReadArrowFiles(List<TransactionAsyncFile> files)
        {
            var output = new List<ArrowRelation>();
            foreach (var file in files)
            {
                if ("application/vnd.apache.arrow.stream".Equals(file.ContentType.ToLower()))
                {
                    var memoryStream = new MemoryStream(file.Data)
                    {
                        Position = 0,
                    };

                    var reader = new ArrowStreamReader(memoryStream);
                    RecordBatch recordBatch;
                    while ((recordBatch = reader.ReadNextRecordBatch()) != null)
                    {
                        var df = DataFrame.FromArrowRecordBatch(recordBatch);
                        output.AddRange(df.Columns.Select(col => col.Cast<object>().ToList())
                            .Select(values => new ArrowRelation(file.Name, values)));
                    }
                }
            }

            return output;
        }*/
        public List<ArrowResult> ReadArrowFiles(List<TransactionAsyncFile> files)
        {
            var output = new List<ArrowResult>();
            foreach (var file in files)
            {
                if ("application/vnd.apache.arrow.stream".Equals(file.ContentType.ToLower()))
                {
                    var memoryStream = new MemoryStream(file.Data)
                    {
                        Position = 0,
                    };

                    var reader = new ArrowStreamReader(memoryStream);
                    RecordBatch record;
                    List<RecordBatch> records = new List<RecordBatch>();

                    while ((record = reader.ReadNextRecordBatch()) != null)
                    {
                        records.Add(record);
                    }

                    var table = Table.TableFromRecordBatches(records.First().Schema, records);

                    output.Add(new ArrowResult(file.Name, file.Filename, table));
                }
            }

            return output;
        }

        public MetadataInfo ReadMetadataProtobuf(byte[] data)
        {
            return MetadataInfo.Parser.ParseFrom(data);
        }

        private static HttpContent EncodeContent(object body)
        {
            if (body == null)
            {
                return null;
            }

            if (!(body is string s))
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(body));
                return new ByteArrayContent(stringContent.ReadAsByteArrayAsync().Result);
            }

            return new ByteArrayContent(Encoding.UTF8.GetBytes(s));
        }

        private static string GetHost(string url)
        {
            return new Uri(url).Host;
        }

        private static string GetUserAgent()
        {
            return $"rai-sdk-csharp/{SdkProperties.Version}";
        }

        private static Dictionary<string, string> GetDefaultHeaders(Uri uri, Dictionary<string, string> headers = null)
        {
            headers ??= new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
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
                headers.Add("User-Agent", GetUserAgent());
            }

            return headers;
        }

        private static HttpRequestMessage PrepareHttpRequest(
            string method,
            Uri uri,
            HttpContent content,
            Dictionary<string, string> headers,
            Dictionary<string, string> parameters = null)
        {
            var uriBuilder = new UriBuilder(uri);
            if (parameters != null)
            {
                uriBuilder.Query = EncodeQueryString(parameters);
            }

            headers = GetDefaultHeaders(uri, headers);
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

            foreach (var (key, value) in headers)
            {
                request.Headers.TryAddWithoutValidation(key, value);
            }

            return request;
        }

        private static List<TransactionAsyncFile> ParseMultipartResponse(byte[] content)
        {
            var output = new List<TransactionAsyncFile>();

            var parser = MultipartFormDataParser.Parse(new MemoryStream(content));

            foreach (var file in parser.Files)
            {
                var memoryStream = new MemoryStream();
                file.Data.CopyTo(memoryStream);
                var buffer = memoryStream.ToArray();
                var txnAsyncFile = new TransactionAsyncFile(file.Name, buffer, file.FileName, file.ContentType);
                output.Add(txnAsyncFile);
            }

            return output;
        }

        private async Task<string> GetAccessTokenAsync(string host)
        {
            if (!(_context.Credentials is ClientCredentials creds))
            {
                throw new SystemException("credential not supported");
            }

            if (creds.AccessToken == null || creds.AccessToken.IsExpired)
            {
                creds.AccessToken = await RequestAccessTokenAsync(host, creds);
            }

            return creds.AccessToken.Token;
        }

        private async Task<AccessToken> RequestAccessTokenAsync(string host, ClientCredentials creds)
        {
            // Form the API request body.
            var data = new Dictionary<string, string>
            {
                { "client_id", creds.ClientId },
                { "client_secret", creds.ClientSecret },
                { "audience", $"https://{host}" },
                { "grant_type", "client_credentials" }
            };
            var resp = await RequestHelperAsync("POST", creds.ClientCredentialsUrl, data);
            if (!(resp is string stringResponse))
            {
                throw new SystemException("Unexpected response type");
            }

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(stringResponse);

            if (result == null)
            {
                throw new SystemException("Unexpected access token response format");
            }

            return new AccessToken(result["access_token"], int.Parse(result["expires_in"]));
        }

        private async Task<object> RequestHelperAsync(
            string method,
            string url,
            object data = null,
            Dictionary<string, string> headers = null,
            Dictionary<string, string> parameters = null)
        {
            var uri = new Uri(url);
            using var client = new HttpClient();

            // Set the API url
            client.BaseAddress = uri;

            // Create the POST request
            var request = PrepareHttpRequest(method, client.BaseAddress, EncodeContent(data), headers, parameters);

            // Get the result back or throws an exception.
            var httpResponse = await client.SendAsync(request);
            var content = await httpResponse.Content.ReadAsByteArrayAsync();
            var contentType = httpResponse.Content.Headers.ContentType.MediaType;

            return contentType.ToLower() switch
            {
                "application/json" => ReadString(content),
                "application/x-protobuf" => ReadMetadataProtobuf(content),
                "multipart/form-data" => ParseMultipartResponse(content),
                _ => throw new SystemException($"unsupported content-type: {contentType}")
            };
        }

        public class Context
        {
            private string _region;

            public Context(string region = null, ICredentials credentials = null)
            {
                Region = region;
                Credentials = credentials;
            }

            public ICredentials Credentials { get; set; }

            public string Region
            {
                get => _region;
                set => _region = !string.IsNullOrEmpty(value) ? value : "us-east";
            }
        }
    }
}