using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class DeleteModels
    {
        public static Command GetCommand()
        {
            var cmd = new Command("DeleteModels", "--database <Database> --engine <Engine> --model <Model name> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "Database name."
                },

                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Engine name."
                },

                new Option<string>("--model"){
                    IsRequired = true,
                    Description = "Model name."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Deletes a user by ID.";
            cmd.Handler = CommandHandler.Create<string, string, string, string>(Run);
            return cmd;
        }

        private static async Task Run(string database, string engine, string model, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.DeleteModelsAsync(database, engine, new List<string> { model }));
        }

    }
}