using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelationalAI.Models.Transaction;
using RelationalAI.Services;
using RelationalAI.Utils;
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
                var clientId = Environment.GetEnvironmentVariable("CLIENT_ID");
                var clientSecret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var clientCredentialsUrl = Environment.GetEnvironmentVariable("CLIENT_CREDENTIALS_URL");

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

            var ctx = new Client.Context(config);
            return new Client(ctx);
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual Task DisposeAsync() => Task.CompletedTask;

        public Relation FindRelation(Relation[] relations, string colName)
        {
            return relations
                .FirstOrDefault(relation => relation.RelKey.Keys.Length != 0 && relation.RelKey.Keys[0].Equals(colName));
        }
    }
}
