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
namespace RelationalAI.Credentials
{
    using System;

    public class ClientCredentials : ICredentials
    {
        public static string defaultClientCredentialsURL = "https://login.relationalai.com/oauth/token";
        private string _clientID;
        private string _clientSecret;
        private string _clientCredentialsURL = ClientCredentials.defaultClientCredentialsURL;
        private AccessToken _accessToken;

        public ClientCredentials(string clientID, string clientSecret)
        {
            ClientID = clientID;
            ClientSecret = clientSecret;
        }
        public ClientCredentials(string clientID, string clientSecret, string clientCredentialsURL): 
            this(clientID, clientSecret)
        {
            ClientCredentialsURL = clientCredentialsURL;
        }
        public string ClientID
        {
            get => _clientID;
            set => _clientID = !String.IsNullOrEmpty(value) ? value : 
                throw new ArgumentException("ClientID cannot be null or empty"); 
        }
        public string ClientSecret
        {
            get => _clientSecret;
            set => _clientSecret =  !String.IsNullOrEmpty(value) ? value : 
                throw new ArgumentException("ClientSecret cannot be null or empty"); 
        }
        public string ClientCredentialsURL
        {
            get => _clientCredentialsURL;
            set => _clientCredentialsURL = !String.IsNullOrEmpty(value) ? value : defaultClientCredentialsURL;
        }
        public AccessToken AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }

    }
}