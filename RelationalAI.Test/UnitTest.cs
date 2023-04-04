using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class UnitTest : IAsyncLifetime
    {
        private static readonly RAILog4NetProvider _loggerProvider = new RAILog4NetProvider();
        private readonly ILogger _logger;

        public UnitTest()
        { }

        public UnitTest(ITestOutputHelper testOutputHelper)
        {
            _loggerProvider.AddRAITestOutputAppender(testOutputHelper);
            _logger = _loggerProvider.CreateLogger("RAI-SDK");
        }

        public Dictionary<string, object> GetConfig()
        {
            Dictionary<string, object> config;

            if (File.Exists(Config.GetRaiConfigPath()))
            {
                config = Config.Read(profile: "default");
            }
            else
            {
                var clientId = GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = GetEnvironmentVariable("CLIENT_SECRET");
                var clientCredentialsUrl = GetEnvironmentVariable("CLIENT_CREDENTIALS_URL");
                var raiHost = GetEnvironmentVariable("HOST", "");
                if (raiHost == "")
                {
                    raiHost = "azure.relationalai.com";
                }

                var configStr = $@"
                [default]
                host={raiHost}
                region=us-east
                port=443
                scheme=https
                client_id={clientId}
                client_secret={clientSecret}
                client_credentials_url={clientCredentialsUrl}
                ";
                config = Config.Read(new MemoryStream(Encoding.UTF8.GetBytes(configStr)));
            }

            return config;
        }

        public Client.Context CreateContext(Dictionary<string, object> config)
        {
            return new Client.Context(config);
        }

        public Client CreateClient()
        {
            var customHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetEnvironmentVariable("CUSTOM_HEADERS"));
            var config = GetConfig();
            var ctx = CreateContext(config);

            Client testClient;
            if (_logger != null)
            {
                testClient = new Client(ctx, _logger);
            }
            else
            {
                testClient = new Client(ctx);
            }

            var httpClient = testClient.HttpClient;
            foreach (var header in customHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            return testClient;
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual Task DisposeAsync() => Task.CompletedTask;

        public Relation FindRelation(Relation[] relations, string colName)
        {
            return relations
                .FirstOrDefault(relation => relation.RelKey.Keys.Length != 0 && relation.RelKey.Keys[0].Equals(colName));
        }

        public string GetEnvironmentVariable(string name, string defaultValue = "{}")
            => Environment.GetEnvironmentVariable(name) ?? defaultValue;
    }
}
