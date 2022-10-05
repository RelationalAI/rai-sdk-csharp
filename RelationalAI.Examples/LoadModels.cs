using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.IO;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class LoadModels
    {
        public static Command GetCommand()
        {
            var cmd = new Command("LoadModels", "--database <Database> --engine <Engine> --file <File> --profile <Profile name>"){
                new Option<string>("--database"){
                    IsRequired = true,
                    Description = "To list models in a paricular database."
                },

                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Engine to use to list models."
                },

                new Option<string>("--file"){
                    IsRequired = true,
                    Description = "Model file."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "List databases";
            cmd.Handler = CommandHandler.Create<string, string, string, string>(Run);
            return cmd;
        }

        private static async Task Run(string database, string engine, string file, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);

            var name = Path.GetFileNameWithoutExtension(file);
            var value = File.ReadAllText(file);
            var models = new Dictionary<string, string> { { name, value } };
            var resp = await client.LoadModelsAsync(database, engine, models);
            Console.WriteLine(resp);
        }

    }
}