namespace RAILib.Examples
{
    using System;
    using System.Collections.Generic;
    using System.CommandLine;
    using System.CommandLine.NamingConventionBinder;
    using Newtonsoft.Json;
    using RAILib;
    public class GetUserID
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetUserID", "--email <Email> --profile <Profile name>"){
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


        private static void Run(string email, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.FindUser(email));
        }
    }
}