using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class FindUser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("FindUser", "--email <Email> --profile <Profile name>"){
                new Option<string>("--email"){
                    IsRequired = true,
                    Description = "User's email to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets a user's ID by email.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }


        private static async Task Run(string email, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine(await client.FindUserAsync(email));
        }
    }
}