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

namespace RelationalAI
{
    public class ClientCredentials : ICredentials
    {
        private const string DefaultClientCredentialsUrl = "https://login.relationalai.com/oauth/token";
        private string _clientId;
        private string _clientSecret;
        private string _audience;
        private string _clientCredentialsUrl = DefaultClientCredentialsUrl;

        public ClientCredentials(string clientId, string clientSecret)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
        }

        public ClientCredentials(string clientId, string clientSecret, string clientCredentialsUrl, string audience)
            : this(clientId, clientSecret)
        {
            ClientCredentialsUrl = clientCredentialsUrl;
            Audience = audience;
        }

        public string ClientId
        {
            get => _clientId;
            set => _clientId = !string.IsNullOrEmpty(value) ? value :
                throw new ArgumentException("ClientID cannot be null or empty");
        }

        public string ClientSecret
        {
            get => _clientSecret;
            set => _clientSecret = !string.IsNullOrEmpty(value) ? value :
                throw new ArgumentException("ClientSecret cannot be null or empty");
        }

        public string ClientCredentialsUrl
        {
            get => _clientCredentialsUrl;
            set => _clientCredentialsUrl = !string.IsNullOrEmpty(value) ? value : DefaultClientCredentialsUrl;
        }

        public string Audience
        {
            get => _audience;
            set => _audience = !string.IsNullOrEmpty(value) ? value :
                throw new ArgumentException("Audience cannot be null or empty");
        }

        public AccessToken AccessToken { get; set; }
    }
}