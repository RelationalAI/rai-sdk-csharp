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

using System.Collections.Generic;
using System.Text;
using System.Web;

namespace RelationalAI
{
    /// <summary>
    /// Represents a collection of query parameters. The values may be String,
    /// String[], int, Boolean or null.
    /// </summary>
    public class QueryParams : Dictionary<string, object>
    {
        /// <summary>
        /// Creates a query string encoded in the format the RAI REST API expects.
        /// </summary>
        /// <returns>The RAI REST API formatted query string.</returns>
        public string Encode()
        {
            var result = new StringBuilder();
            foreach (var (key, value) in this)
            {
                Append(result, key, value);
            }

            return result.ToString();
        }

        // Encode the given k, v pair, and append to the given builder.
        private static void Append(StringBuilder builder, string k, object v)
        {
            if (v is string[] strings)
            {
                foreach (var vv in strings)
                {
                    Append(builder, k, vv);
                }

                return;
            }

            var value = v.ToString();
            if (builder.Length > 0)
            {
                builder.Append('&');
            }

            builder.Append(EncodeValue(k));
            builder.Append('=');
            builder.Append(EncodeValue(value));
        }

        // Encode an element of a query parameter.
        private static string EncodeValue(string value)
        {
            return HttpUtility.UrlEncode(value);
        }
    }
}