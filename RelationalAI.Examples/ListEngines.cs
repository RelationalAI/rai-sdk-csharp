using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RelationalAI;


namespace RelationalAI.Examples
{
    public class ListEngines
    {
        public static Command GetCommand()
        {
            var cmd = new Command("ListEngines", "--state <State> --profile <Profile name>"){
                new Option<string>("--state"){
                    IsRequired = false,
                    Description = "To list engines in a paricular state. For example, DELETED"
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Lists engines.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static void Run(string state = null, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            List<Engine> engines = client.ListEngines(state);
            foreach(var engine in engines)
            {
                Console.WriteLine(engine.ToString(true));
            } 
            
        }

    }
}