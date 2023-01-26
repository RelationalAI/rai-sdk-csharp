using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class UnitTest : IAsyncLifetime
    {
        public Client CreateClient(ITestOutputHelper testOutputHelper = null)
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

            var customHeaders = JsonConvert.DeserializeObject<Dictionary<string, string>>(GetEnvironmentVariable("CUSTOM_HEADERS"));

            var ctx = new Client.Context(config);
            var testClient = new Client(ctx);
            var httpClient = testClient.HttpClient;
            foreach (var header in customHeaders)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }

            // Logging configuration
            if (testOutputHelper != null)
            {
                var logger = testClient.Logger as RAITraceSourceLogger;
                logger.AddListener(new RAITestTraceListener(testOutputHelper));
                logger.SwitchLogLevel(TraceEventType.Verbose);
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
