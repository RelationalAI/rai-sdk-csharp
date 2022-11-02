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
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RelationalAI
{
    // This handler caches tokens in ~/.rai/tokens.json. It will attempt to load
    // a token from the cache file and if it is not found or has expired, it will
    // delegate to rest.RequestAccessTokenAsync to retrieve a new token and will save it
    // in the cache file.
    public class DefaultAccessTokenHandler : IAccessTokenHandler
    {
        public async Task<AccessToken> GetAccessTokenAsync(Rest rest, string host, ClientCredentials creds)
        {
            var token = ReadAccessToken(creds);
            if (token != null && !token.IsExpired)
            {
                return token;
            }

            token = await rest.RequestAccessTokenAsync(host, creds);
            if (token != null)
            {
                WriteAccessToken(creds, token);
            }

            return token;
        }

        private string CacheName()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);

            return Path.Join(home, ".rai", "tokens.json");
        }

        private AccessToken ReadAccessToken(ClientCredentials creds)
        {
            var cache = ReadTokenCache();

            if (cache.ContainsKey(creds.ClientId))
            {
                return cache[creds.ClientId];
            }

            return null;
        }

        private Dictionary<string, AccessToken> ReadTokenCache()
        {
            try
            {
                var data = File.ReadAllText(CacheName());
                var cache = JsonConvert.DeserializeObject<Dictionary<string, AccessToken>>(data);
                return cache;
            }
            catch (IOException)
            {
            }

            return new Dictionary<string, AccessToken>();
        }

        private void WriteAccessToken(ClientCredentials creds, AccessToken token)
        {
            try
            {
                var cache = ReadTokenCache();
                var dict = cache ?? new Dictionary<string, AccessToken>();
                if (dict.ContainsKey(creds.ClientId))
                {
                    dict[creds.ClientId] = token;
                }
                else
                {
                    dict.Add(creds.ClientId, token);
                }

                File.WriteAllText(CacheName(), JsonConvert.SerializeObject(dict));
            }
            catch (IOException)
            {
            }
        }
    }
}
