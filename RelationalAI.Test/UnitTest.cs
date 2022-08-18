using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class UnitTest : IAsyncLifetime
    {
        public Client CreateClient()
        {
            Dictionary<string, object> config;
            if(File.Exists(Config.GetRAIConfigPath()))
            {
                config = Config.Read(profile: "default");
            }
            else
            {
                var client_id = Environment.GetEnvironmentVariable("CLIENT_ID");
                var client_secret = Environment.GetEnvironmentVariable("CLIENT_SECRET");
                var client_credentials_url = Environment.GetEnvironmentVariable("CLIENT_CREDENTIALS_URL");

                var configStr = $@"
                [default]
                host=azure.relationalai.com
                region=us-east
                port=443
                scheme=https
                client_id={client_id}
                client_secret={client_secret}
                client_credentials_url={client_credentials_url}
                ";
                config = Config.Read(new MemoryStream(Encoding.UTF8.GetBytes(configStr)));
            }

            var ctx = new Client.Context(config);
            return new Client(ctx);
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;

        public virtual Task DisposeAsync() => Task.CompletedTask;

        public Relation findRelation(Relation[] relations, string colName)
        {
            return relations
                .FirstOrDefault(relation => relation.RelKey.Keys.Length != 0 && relation.RelKey.Keys[0].Equals(colName));
        }
    }
}
