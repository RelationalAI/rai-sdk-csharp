using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;

using RAILib;


namespace RAILib.Examples
{
    public class RunQuery
    {
        public static Command GetCommand()
        {
            var cmd = new Command("RunQuery", "--database <Database name> --engine <Compute name> --query <Query> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "Database to run query."
                },

                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Compute name of the database."
                },

                new Option<string>("--query"){
                    IsRequired = true,
                    Description = "Query to run."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets a user's details by ID.";
            cmd.Handler = CommandHandler.Create<string, string, string, string>(Run);
            return cmd;
        }

        private static void Run(string database, string engine, string query, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Api.Context context = new Api.Context(config);
            Api api = new Api(context);
            Console.WriteLine(api.Execute(database, engine, query));
        }

    }
}