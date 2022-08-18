using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class CreateDatabase
    {
        public static Command GetCommand()
        {
            var cmd = new Command("CreateDatabase", "--database <Database name> --engine <Engine name> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "database name to create."
                },

                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Engine name to use."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Creates a new database.";
            cmd.Handler = CommandHandler.Create<string, string, string>(Run);
            return cmd;
        }

        private static async Task Run(string database, string engine, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine("Creating Database: "+ database);
            Console.WriteLine(await client.CreateDatabaseAsync(database, engine));
        }

    }
}
