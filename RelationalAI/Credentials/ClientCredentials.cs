// <copyright file="ClientCredentials.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

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
        private static readonly string DefaultClientCredentialsURL = "https://login.relationalai.com/oauth/token";
        private string clientID;
        private string clientSecret;
        private string clientCredentialsURL = ClientCredentials.DefaultClientCredentialsURL;
        private AccessToken accessToken;

        public ClientCredentials(string clientID, string clientSecret)
        {
            this.ClientID = clientID;
            this.ClientSecret = clientSecret;
        }

        public ClientCredentials(string clientID, string clientSecret, string clientCredentialsURL)
            : this(clientID, clientSecret)
        {
            this.ClientCredentialsURL = clientCredentialsURL;
        }

        public string ClientID
        {
            get => this.clientID;
            set => this.clientID = !string.IsNullOrEmpty(value) ? value :
                throw new ArgumentException("ClientID cannot be null or empty");
        }

        public string ClientSecret
        {
            get => this.clientSecret;
            set => this.clientSecret = !string.IsNullOrEmpty(value) ? value :
                throw new ArgumentException("ClientSecret cannot be null or empty");
        }

        public string ClientCredentialsURL
        {
            get => this.clientCredentialsURL;
            set => this.clientCredentialsURL = !string.IsNullOrEmpty(value) ? value : DefaultClientCredentialsURL;
        }

        public AccessToken AccessToken
        {
            get => this.accessToken;
            set => this.accessToken = value;
        }
    }
}