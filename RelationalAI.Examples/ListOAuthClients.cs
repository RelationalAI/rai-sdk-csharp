using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class ListOAuthClients
    {
        public static Command GetCommand()
        {
            var cmd = new Command("ListOAuthClients", "--profile <Profile name>"){
                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Lists OAuthClients.";
            cmd.Handler = CommandHandler.Create<string>(Run);
            return cmd;
        }

        private static async Task Run(string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.ListOAuthClientsAsync());
        }

    }
}