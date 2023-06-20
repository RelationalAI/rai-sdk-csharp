using System.CommandLine;
using System;
using System.Collections.Generic;

namespace RelationalAI.Examples
{
    public class Examples
    {
        public static void Main(string[] args)
        {
            GetListRun();
            
            //return BuildCommandLine().Invoke(args);

            Console.Read();
        }

        private static void GetListRun(DatabaseState? state = null, string profile = "latest")//,string profile = "latest-engine") //string profile = "dev1")//string profile = "latest-engine")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            for(int i = 1; i < 10; i++)
            {
                try
                {
                    var res = client.CreateEngineAsync("wei-fullengine-xl","XL").Result;
                                    //var res1 = client.CreateEngineAsync("wei-spot-test").Result;

                    //var test =  client.CreateDatabaseAsync("weitest10","wei-benchmark-test").Result;
                    //var test2 = client.ListEnginesAsync().Result;
                    //var index = i + 100000;
                    //string query = $"def insert:integers = range[1, {index}, 1] def output = count[integers]";
                    //query = "def output = {1;2;3}";

                    //var test =  client.ExecuteWaitAsync("Test2", "wei-test-m1", query).Result;

                    //Console.WriteLine("test:" + i);
        
                }
                catch(Exception ex)
                {
                    Console.WriteLine("exception");
                }
            }

        }

        private static RootCommand BuildCommandLine()
        {
            // init the root command
            var root = new RootCommand("RAI examples CLI");
            // add various examples as commands
            root.AddCommand(CreateEngine.GetCommand());
            root.AddCommand(CreateDatabase.GetCommand());
            root.AddCommand(DeleteEngine.GetCommand());
            root.AddCommand(GetDatabase.GetCommand());
            root.AddCommand(GetEngine.GetCommand());
            root.AddCommand(GetOAuthClient.GetCommand());
            root.AddCommand(GetModel.GetCommand());
            root.AddCommand(GetUser.GetCommand());
            root.AddCommand(FindUser.GetCommand());
            root.AddCommand(EnableUser.GetCommand());
            root.AddCommand(DisableUser.GetCommand());
            root.AddCommand(DeleteUser.GetCommand());
            root.AddCommand(ListDatabases.GetCommand());
            root.AddCommand(ListEngines.GetCommand());
            root.AddCommand(ListOAuthClients.GetCommand());
            root.AddCommand(ListUsers.GetCommand());
            root.AddCommand(ListModels.GetCommand());
            root.AddCommand(LoadModels.GetCommand());
            root.AddCommand(DeleteModels.GetCommand());
            root.AddCommand(ExecuteV1.GetCommand());
            root.AddCommand(Execute.GetCommand());
            root.AddCommand(ExecuteAsync.GetCommand());
            root.AddCommand(GetTransaction.GetCommand());
            root.AddCommand(GetTransactions.GetCommand());
            root.AddCommand(GetTransactionResults.GetCommand());
            root.AddCommand(GetTransactionMetadata.GetCommand());
            root.AddCommand(GetTransactionProblems.GetCommand());
            root.AddCommand(CancelTransaction.GetCommand());
            //return the root command
            return root;
        }
    }
}