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
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RelationalAI
{
    // This handler caches tokens in ~/.rai/tokens.json. It will attempt to load
    // a token from the cache file and if it is not found or has expired, it will
    // delegate to rest.RequestAccessTokenAsync to retrieve a new token and will save it
    // in the cache file.
    public class DefaultAccessTokenHandler : IAccessTokenHandler
    {
        private static readonly SemaphoreSlim SemaphoreSlim = new SemaphoreSlim(1);
        private readonly Rest _rest;
        private readonly ILogger _logger;
        private readonly string _cachePath;

        public DefaultAccessTokenHandler(Rest rest, string cachePath = null, ILogger logger = null)
        {
            _rest = rest;
            _cachePath = cachePath ?? DefaultCachePath();
            _logger = logger ?? new LoggerFactory().CreateLogger("RAI-SDK");
        }

        public async Task<AccessToken> GetAccessTokenAsync(ClientCredentials creds)
        {
            var token = ReadAccessToken(creds);
            if (token != null && !token.IsExpired)
            {
                creds.AccessToken = token;
                return creds.AccessToken;
            }

            token = await _rest.RequestAccessTokenAsync(creds);
            if (token != null)
            {
                creds.AccessToken = token;
                await WriteAccessTokenAsync(creds, token);
            }

            return creds.AccessToken;
        }

        private string DefaultCachePath()
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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
                var data = File.ReadAllText(_cachePath);
                var cache = JsonConvert.DeserializeObject<Dictionary<string, AccessToken>>(data);
                return cache;
            }
            catch (IOException ex)
            {
                _logger.LogInformation($"Unable to read from local cache, fallback to memory-based cache. {ex.Message}");
            }

            return new Dictionary<string, AccessToken>();
        }

        private async Task WriteAccessTokenAsync(ClientCredentials creds, AccessToken token)
        {
            try
            {
                await SemaphoreSlim.WaitAsync();

                var dict = ReadTokenCache();

                if (dict.ContainsKey(creds.ClientId))
                {
                    dict[creds.ClientId] = token;
                }
                else
                {
                    dict.Add(creds.ClientId, token);
                }

                File.WriteAllText(_cachePath, JsonConvert.SerializeObject(dict));
            }
            catch (IOException ex)
            {
                _logger.LogWarning($"Unable to write to local cache. {ex.Message}");
            }
            finally
            {
                SemaphoreSlim.Release();
            }
        }
    }
}
