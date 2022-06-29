using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RelationalAI;


namespace RelationalAI.Examples
{
    public class Execute
    {
        public static Command GetCommand()
        {
            var cmd = new Command("Execute", "--database <Database name> --engine <Compute name> --command <Command text> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "Database to run query."
                },

                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Compute name of the database."
                },

                new Option<string>("--command"){
                    IsRequired = true,
                    Description = "Command to execute."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Execute a synchronous transaction.";
            cmd.Handler = CommandHandler.Create<string, string, string, string>(Run);
            return cmd;
        }

        private static void Run(string database, string engine, string command, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine(client.ExecuteV1(database, engine, command));
        }

    }
}