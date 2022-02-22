using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RAILib;

namespace RAILib.Examples
{
    public class GetDatabase
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetDatabase", "--database <Database name> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "Database name to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets a database's details by name.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            
            return cmd;
        }

        private static void Run(string database, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.GetDatabase(database));
        }

    }
}