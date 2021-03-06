using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RelationalAI;


namespace RelationalAI.Examples
{
    public class GetUser
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetUser", "--id <User ID> --profile <Profile name>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "User's ID to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets a user's details by ID.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static void Run(string id, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine(client.GetUser(id));
        }

    }
}