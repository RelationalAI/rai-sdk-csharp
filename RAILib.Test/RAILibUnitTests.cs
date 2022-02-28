using System;
using Xunit;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using RAILib;

namespace RAILib.Test
{
    public class RAILibUnitTests
    {
        [Fact]
        public void UserTests()
        {
           try
           {
                Dictionary<string, object> config = Config.Read("", "latest");
                Api.Context ctx = new Api.Context(config);
                Api api = new Api(ctx);
                List<User> users = api.ListUsers();
                Console.WriteLine(users[0]);
                Console.WriteLine(api.GetUser(users[0].ID));
                User user = api.CreateUser("csharpsdk@relational.ai", new List<Role>(){ Role.Admin });
                Console.WriteLine(user);
                Console.WriteLine(api.GetUser(user.ID));
                Console.WriteLine(api.DisableUser(user.ID));
                Console.WriteLine(api.EnableUser(user.ID));
            
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
                Api.Context ctx = new Api.Context(config);
                Api api = new Api(ctx);
                //List<Engine> engines = api.ListEngines();
                //Console.WriteLine(engines[0]);
                Engine engine = api.CreateEngineWait("ia-test-engine8", EngineSize.XS);
                Console.WriteLine(api.GetEngine(engine.Name));
                Console.WriteLine(api.DeleteEngine("ia-test-engine8"));
            
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
                Api.Context ctx = new Api.Context(config);
                Api api = new Api(ctx);
                List<OAuthClient> clients = api.ListOAuthClients();
                OAuthClient client = api.CreateOAuthClient("irfan1234", new List<Permission>(){Permission.CreateDatabase, Permission.ListDatabases, 
                    Permission.CreateCompute, Permission.DeleteCompute, Permission.ListComputes, Permission.DeleteDatabase});
                Console.WriteLine(client);
                clients = api.ListOAuthClients();
                Console.WriteLine(clients[0]);
                Console.WriteLine(api.GetOAuthClient(clients[0].ID));
                Console.WriteLine(api.FindOAuthClient(clients[0].Name));
                
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
            Api.Context ctx = new Api.Context(config);
            Api api = new Api(ctx);
            List<Database> databases = api.ListDatabases();
            foreach(var db in databases)
            {
                Console.WriteLine(db.ToString(true));
            }
            Console.WriteLine(api.CreateDatabase("ia-test-db4", "ia-test-engine"));
        }

         [Fact]
        public void TransactionTests()
        {
            Dictionary<string, object> config = Config.Read("", "latest");
            Api.Context ctx = new Api.Context(config);
            Api api = new Api(ctx);
            List<Database> databases = api.ListDatabases();
            Console.WriteLine(api.Execute("ia-test-db", "ia-test-engine", "1 + 1"));
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
