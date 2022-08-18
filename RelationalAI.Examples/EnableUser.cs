using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class EnableUser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("EnableUser", "--id <User ID>  --profile <Profile name>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "User's ID to enable."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Enables a user by ID.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.EnableUserAsync(id));
        }

    }
}