using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RAILib;


namespace RAILib.Examples
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

        private static void Run(string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.ListOAuthClients());
        }

    }
}