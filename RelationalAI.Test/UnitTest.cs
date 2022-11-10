using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xunit;

namespace RelationalAI.Test
{
    public class UnitTest : IAsyncLifetime
    {
        public Client CreateClient()
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

                var configStr = $@"
                [default]
                host=azure.relationalai.com
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
        
        public async Task<Engine> CreateEngineWaitAsync(string engineName)
        {
            var engine = GetEngineName(engineName);
            var client = CreateClient();
            Console.WriteLine("Engine-Name: " + engine);
            if (engine != engineName)
            {
                Console.WriteLine("Getting Engine");
                return await client.GetEngineAsync(engine);
            }
            else
            {
                Console.WriteLine("Creating Engine");
                return await client.CreateEngineWaitAsync(engine);
            }
        }

        public async void DeleteEngineWaitAsync(string engineName)
        {
            if (GetEngineName(engineName) == engineName)
            {
                Console.WriteLine("Deleting Engine: " + engineName);
                var client = CreateClient();
                await client.DeleteEngineWaitAsync(engineName);
            }
        }

        public string GetEngineName(string engineName)
        {
            return GetEnvironmentVariable("ENGINE_NAME", engineName);    
        }
    }
}
