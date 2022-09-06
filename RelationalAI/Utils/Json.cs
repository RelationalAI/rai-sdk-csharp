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

using Newtonsoft.Json;
using RelationalAI.Errors;

namespace RelationalAI.Utils
{
    public class Json<T>
        where T : class
    {
        public static T Deserialize(string data, string key = null)
        {
            if (string.IsNullOrEmpty(data))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                // TODO: add a proper handling of 4xx status responses at Rest service level, currently they end up here
                throw new InvalidResponseException($"Failed to deserialize response into type {typeof(T).Name}", data);
            }
        }
    }
}