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
        public static string Uuid = Guid.NewGuid().ToString();
        public static string EngineName = $"csharp-sdk-{Uuid}";

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
       
        public string GetEngineName() 
        {
            var engineName = GetEnvironmentVariable("ENGINE_NAME", "");
            return engineName != "" ? engineName : EngineName;
        }

        public async Task<Engine> CreateEngineWaitAsync(Client client, string size = "XS")
        {
            try
            { 
                return await client.GetEngineAsync(GetEngineName());    
            }
            catch (NotFoundException)
            {
                return await client.CreateEngineWaitAsync(GetEngineName(), size);
            }

        }
    }
}
