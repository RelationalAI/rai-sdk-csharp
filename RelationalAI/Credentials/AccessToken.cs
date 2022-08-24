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

namespace RelationalAI.Credentials
{
    public class AccessToken
    {
        private readonly DateTime _createdOn;
        private int _expiresIn;

        public AccessToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
            _createdOn = DateTime.Now;
        }

        public bool IsExpired => (DateTime.Now - _createdOn).TotalSeconds >= ExpiresIn - 5; // Anticipate access token expiration by 5 seconds

        public string Token { get; set; }

        public int ExpiresIn
        {
            get => _expiresIn;
            set => _expiresIn = value > 0 ? value : throw new ArgumentException("ExpiresIn should be greater than 0 ");
        }
    }
}