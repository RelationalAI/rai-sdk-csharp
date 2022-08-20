using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

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

        private static async Task Run(string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            var users = await client.ListUsersAsync();
            foreach (var user in users)
            {
                Console.WriteLine(user.ToString(true));
            }
        }

    }
}