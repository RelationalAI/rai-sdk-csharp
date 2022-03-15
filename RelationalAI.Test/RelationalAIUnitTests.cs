using System;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using RelationalAI;

namespace RelationalAI.Test
{
    public class RelationalAIUnitTests
    {
        [Fact]
        public void UserTests()
        {
           try
           {
                Dictionary<string, object> config = Config.Read("", "latest");
                Client.Context ctx = new Client.Context(config);
                Client client = new Client(ctx);
                List<User> users = client.ListUsers();
                Console.WriteLine(users[0]);
                Console.WriteLine(client.GetUser(users[0].ID));
                User user = client.CreateUser("csharpsdk@relational.ai", new List<Role>(){ Role.Admin });
                Console.WriteLine(user);
                Console.WriteLine(client.GetUser(user.ID));
                Console.WriteLine(client.DisableUser(user.ID));
                Console.WriteLine(client.EnableUser(user.ID));
            
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex);
           }
        }

        [Fact]
        public void EngineTests()
        {
           try
           {
                Dictionary<string, object> config = Config.Read("", "default");
                Client.Context ctx = new Client.Context(config);
                Client client = new Client(ctx);
                //List<Engine> engines = client.ListEngines();
                //Console.WriteLine(engines[0]);
                Engine engine = client.CreateEngineWait("ia-test-engine8", EngineSize.XS);
                Console.WriteLine(client.GetEngine(engine.Name));
                Console.WriteLine(client.DeleteEngine("ia-test-engine8"));
            
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex);
           }
        }

        [Fact]
        public void OAuthClientTests()
        {
           try
           {
                Dictionary<string, object> config = Config.Read("", "default");
                Client.Context ctx = new Client.Context(config);
                Client client = new Client(ctx);
                List<OAuthClient> clients = client.ListOAuthClients();
                OAuthClient oAuthClient = client.CreateOAuthClient("irfan1234", new List<Permission>(){Permission.CreateDatabase, Permission.ListDatabases, 
                    Permission.CreateCompute, Permission.DeleteCompute, Permission.ListComputes, Permission.DeleteDatabase});
                Console.WriteLine(oAuthClient);
                clients = client.ListOAuthClients();
                Console.WriteLine(clients[0]);
                Console.WriteLine(client.GetOAuthClient(clients[0].ID));
                Console.WriteLine(client.FindOAuthClient(clients[0].Name));
                
           }
           catch(Exception ex)
           {
               Console.WriteLine(ex);
           }
        }

        [Fact]

        public void DatabaseTests()
        {
            Dictionary<string, object> config = Config.Read("", "latest");
            Client.Context ctx = new Client.Context(config);
            Client client = new Client(ctx);
            List<Database> databases = client.ListDatabases();
            foreach(var db in databases)
            {
                Console.WriteLine(db.ToString(true));
            }
            Console.WriteLine(client.CreateDatabase("ia-test-db4", "ia-test-engine"));
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
