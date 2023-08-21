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
using Newtonsoft.Json;

namespace RelationalAI
{
    public class AccessToken : Entity
    {
        [JsonProperty("expires_in")]
        private int _expiresIn;

        public AccessToken(string token, long createdOn, int expiresIn, string scope)
        {
            Token = token;
            Scope = scope;
            CreatedOn = createdOn;
            _expiresIn = expiresIn;
        }

        [JsonProperty("access_token", Required = Required.Always)]
        public string Token { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("created_on")]
        public long CreatedOn { get; set; }

        public int ExpiresIn
        {
            get => _expiresIn;
            set => _expiresIn = value > 0 ? value : throw new ArgumentException("ExpiresIn should be greater than 0 ");
        }

        public double ExpiresOn
        {
            get => CreatedOn + _expiresIn;
        }

        public bool IsExpired => new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds() >= ExpiresOn - 60; // Anticipate access token expiration by 60 seconds
    }
}
