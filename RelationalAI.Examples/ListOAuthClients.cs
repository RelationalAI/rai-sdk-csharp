using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI;


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
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine(await client.ListOAuthClientsAsync());
        }

    }
}