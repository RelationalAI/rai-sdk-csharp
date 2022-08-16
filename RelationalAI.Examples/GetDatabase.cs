using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI;

namespace RelationalAI.Examples
{
    public class GetDatabase
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetDatabase", "--database <Database name> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "Database name to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets a database's details by name.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string database, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine(await client.GetDatabaseAsync(database));
        }

    }
}