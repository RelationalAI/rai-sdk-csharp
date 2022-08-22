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

namespace RelationalAI.Models.Transaction
{
    public class ClientProblem : Entity
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("error_code", Required = Required.Always)]
        public string ErrorCode { get; set; }

        [JsonProperty("is_error", Required = Required.Always)]
        public bool IsError { get; set; }

        [JsonProperty("is_exception", Required = Required.Always)]
        public bool IsException { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

        [JsonProperty("report", Required = Required.Always)]
        public string Report { get; set; }
    }
}