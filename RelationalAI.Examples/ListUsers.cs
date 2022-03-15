using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RelationalAI;


namespace RelationalAI.Examples
{
    public class ListUsers
    {
        public static Command GetCommand()
        {
            var cmd = new Command("ListUsers", "--profile <Profile name>"){
                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Lists Users.";
            cmd.Handler = CommandHandler.Create<string>(Run);
            return cmd;
        }

        private static void Run(string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            List<User> users = client.ListUsers();
            foreach(var user in users)
            {
                Console.WriteLine(user.ToString(true));
            }
        }

    }
}