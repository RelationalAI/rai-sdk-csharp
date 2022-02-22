using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RAILib;


namespace RAILib.Examples
{
    public class GetOAuthClient
    {   
        public static Command GetCommand()
        {
            var cmd = new Command("GetOAuthClient", "--id <Client ID> --profile <Profile name>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "oAuth client's id to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets an oAuth client's details by name.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            
            return cmd;
        }

        private static void Run(string id, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.GetOAuthClient(id));
        }

    }
}