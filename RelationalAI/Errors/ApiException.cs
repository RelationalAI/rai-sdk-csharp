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
using System.Net;
using System.Net.Http.Headers;

namespace RelationalAI.Errors
{
    /// <summary>
    /// Represents error thrown when RAI API request failed with error status code, etc.
    /// </summary>
    public class ApiException : Exception
    {
        public ApiException(
            string message,
            HttpStatusCode statusCode,
            string response,
            HttpResponseHeaders headers)
            : base($"Request failed with error: {message}")
        {
            StatusCode = statusCode;
            Response = response;
            Headers = headers;
        }

        /// <summary>
        /// Gets the status code of the RAI API response.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        /// <summary>
        /// Gets the raw RAI API response content.
        /// </summary>
        public string Response { get; }

        /// <summary>
        /// Gets the headers of the RAI API response.
        /// </summary>
        public HttpResponseHeaders Headers { get; }

        /// <summary>
        /// Gets string representation of the error details, including Status Code, Headers and Response content.
        /// </summary>
        /// <returns>String representation of the exception.</returns>
        public override string ToString()
        {
            return $"StatusCode: {StatusCode}, Headers: {Headers}," + Environment.NewLine +
                   $"Response: {Response}" + Environment.NewLine
                   + base.ToString();
        }
    }
}
