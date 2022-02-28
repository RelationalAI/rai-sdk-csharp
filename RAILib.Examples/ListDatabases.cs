using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RAILib;


namespace RAILib.Examples
{
    public class ListDatabases
    {
        public static Command GetCommand()
        {
            var cmd = new Command("ListDatabases", "--state <State> --profile <Profile name>"){
                new Option<string>("--state"){
                    IsRequired = false,
                    Description = "To list databases in a paricular state. For example, CREATED"
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "List databases";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static void Run(string state = null, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            List<Database> databases = api.ListDatabases(state);
            foreach(var database in databases) 
            {
                Console.WriteLine(database);
            }
        }

    }
}