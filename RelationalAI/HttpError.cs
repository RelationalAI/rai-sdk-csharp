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

namespace RelationalAI
{
    [Serializable]
    public class HttpError : Exception
    {
        private static Dictionary<int, string> statusText = new Dictionary<int, string>()
        {
            { 200, "OK" },
            { 201, "Created" },
            { 202, "Accepted" },
            { 204, "No Content" },
            { 400, "Bad Request" },
            { 401, "Unauthorized" },
            { 403, "Forbidden" },
            { 404, "Not Found" },
            { 405, "Method Not Allowed" },
            { 409, "Conflict" },
            { 410, "Gone" },
            { 500, "Internal Server Error" },
            { 501, "Not Implemented" },
            { 502, "Bad Gateway" },
            { 503, "Service Unavailable" },
            { 504, "Gateway Timeout" },
        };

        public HttpError(int statusCode)
        {
            StatusCode = statusCode;
        }

        public HttpError(int statusCode, String message)
        : base(message)
        {
            StatusCode = statusCode;
        }

        public int StatusCode { get; set; }

        public override string Message { get; }

        public override String ToString()
        {
            String result = StatusCode.ToString();
            if (statusText.ContainsKey(StatusCode))
            {
                result += " " + statusText[StatusCode];
            }

            if (Message != null)
            {
                result += "\n" + Message;
            }

            return result;
        }
    }
}