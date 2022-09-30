using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class ListDatabases
    {
        public static Command GetCommand()
        {
            var cmd = new Command("ListDatabases", "--state <State> --profile <Profile name>"){
                new Option<DatabaseState>("--state"){
                    IsRequired = false,
                    Description = "To list databases in a paricular state. For example, CREATED"
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "List databases";
            cmd.Handler = CommandHandler.Create<DatabaseState?, string>(Run);
            return cmd;
        }

        private static async Task Run(DatabaseState? state = null, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            var databases = await client.ListDatabasesAsync(state);
            foreach (var database in databases)
            {
                Console.WriteLine(database);
            }
        }

    }
}