// <copyright file="AccessToken.cs" company="PlaceholderCompany">
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

    public class AccessToken
    {
        private readonly DateTime createdOn;
        private int expiresIn;
        private string token;

        public AccessToken(string token, int expiresIn)
        {
            this.Token = token;
            this.ExpiresIn = expiresIn;
            this.createdOn = DateTime.Now;
        }

        public int ExpiresIn
        {
            get => this.expiresIn;
            set => this.expiresIn = value > 0 ? value : throw new ArgumentException("ExpiresIn should be greater than 0 ");
        }

        public bool IsExpired
        {
            get => (DateTime.Now - this.createdOn).TotalSeconds >= this.ExpiresIn - 5; // Anticipate access token expiration by 5 seconds
        }

        public string Token
        {
            get => this.token;
            set => this.token = value;
        }
    }
}