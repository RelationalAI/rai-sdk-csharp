using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI;

namespace RelationalAI.Examples
{
    public class DeleteOAuthClient
    {
        public static Command GetCommand()
        {
            var cmd = new Command("DeleteOAuthClient", "--id <Client ID> --profile <Profile name>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "oAuth client's id to delete."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Deletes an oAuth client by id.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.DeleteOAuthClientAsync(id));
        }

    }
}