using System;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace RelationalAI.Test
{
    public class RelationalAIUnitTests : IDisposable
    {
        static string UUID = Guid.NewGuid().ToString();
        static string Dbname = $"csharp-sdk-{UUID}";
        static string EngineName = $"csharp-sdk-{UUID}";
        static string UserEmail = $"csharp-sdk-{UUID}@relational.ai";
        static string OAuthClientName =  $"csharp-sdk-{UUID}";

        public void Dispose()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            try { client.DeleteDatabase(Dbname); } catch {}
            try { client.DeleteEngineWait(EngineName); } catch {}
        }

        [Fact]
        public void UserTests()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            List<User> users = client.ListUsers();
            Console.WriteLine(users[0]);
            Console.WriteLine(client.GetUser(users[0].ID));
            User user = client.CreateUser(UserEmail, new List<Role>(){ Role.Admin });
            Console.WriteLine(user);
            Console.WriteLine(client.GetUser(user.ID));
            Console.WriteLine(client.DisableUser(user.ID));
            Console.WriteLine(client.EnableUser(user.ID));
            Console.WriteLine(client.DeleteUser(user.ID));
        }

        [Fact]
        public void EngineTests()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            //List<Engine> engines = client.ListEngines();
            //Console.WriteLine(engines[0]);
            Engine engine = client.CreateEngineWait(EngineName, EngineSize.XS);
            Console.WriteLine(client.GetEngine(engine.Name));
            Console.WriteLine(client.DeleteEngine(EngineName));
        }

        [Fact]
        public void OAuthClientTests()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            List<OAuthClient> clients = client.ListOAuthClients();
            OAuthClient oAuthClient = client.CreateOAuthClient(OAuthClientName, new List<Permission>(){Permission.CreateDatabase, Permission.ListDatabases, 
                Permission.CreateCompute, Permission.DeleteCompute, Permission.ListComputes, Permission.DeleteDatabase});
            Console.WriteLine(oAuthClient);
            clients = client.ListOAuthClients();
            Console.WriteLine(clients[0]);
            Console.WriteLine(client.GetOAuthClient(clients[0].ID));
            Console.WriteLine(client.FindOAuthClient(clients[0].Name));
        }

        [Fact]
        public void ExecuteAsyncTest()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);

            client.CreateEngineWait(EngineName);
            client.CreateDatabase(Dbname, EngineName);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = client.ExecuteAsyncWait(Dbname, EngineName, query, true);

            JObject expected = new JObject()
            {
                {
                    "results",
                    new JArray()
                    {
                        new JObject()
                        {
                            {"v1", new JArray() {1, 2, 3, 4, 5} },
                            {"v2", new JArray() {1, 4, 9, 16, 25} },
                            {"v3", new JArray() {1, 8, 27, 64, 125} },
                            {"v4", new JArray() {1, 16, 81, 256, 625} }
                        }
                    }
                },
                {
                    "metadata",
                    new JArray()
                    {
                        new JObject()
                        {
                            {"relationId", "/:output/Int64/Int64/Int64/Int64"},
                            {"types", new JArray() {":output", "Int64", "Int64", "Int64", "Int64"} }
                        }
                    }
                },
                {
                    "problems",
                    new JArray()
                }
            };

            Assert.Equal(rsp, expected);
        }

        [Fact]

        public void DatabaseTests()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            List<Database> databases = client.ListDatabases();
            foreach(var db in databases)
            {
                Console.WriteLine(db.ToString(true));
            }
            Console.WriteLine(client.CreateDatabase($"csharp-sdk-{UUID}", $"csharp-sdk-{UUID}"));
        }

         [Fact]
        public void TransactionTests()
        {
            Dictionary<string, object> config = Config.Read("", "latest");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            List<Database> databases = client.ListDatabases();
            Console.WriteLine(client.Execute("ia-test-db", "ia-test-engine", "1 + 1"));
        }
    }

    public class Student
    {
        [JsonProperty("student_name", Required = Required.Always)]
        public string Name {
            get;
            set;
        }
        [JsonPropertyName("student_address")]
        public string Address {
            get;
            set;
        }

        public override string ToString(){
            return JObject.FromObject(this).ToString();
        }
    }
}
