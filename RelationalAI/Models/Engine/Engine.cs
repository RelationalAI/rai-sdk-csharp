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
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace RelationalAI.Models.Engine
{
    public class Engine : Entity
    {
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; set; }

        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("region", Required = Required.Always)]
        public string Region { get; set; }

        [JsonProperty("account_name", Required = Required.Always)]
        public string AccountName { get; set; }

        [JsonProperty("created_by", Required = Required.Always)]
        public string CreatedBy { get; set; }

        [JsonProperty("deleted_on")]
        public string DeletedOn { get; set; }

        [JsonProperty("size")]
        [JsonConverter(typeof(StringEnumConverter))]
        public EngineSize Size { get; set; }

        [JsonProperty("state")]
        [JsonConverter(typeof(StringEnumConverter), typeof(SnakeCaseNamingStrategy))]
        public EngineState State { get; set; }
    }
}
