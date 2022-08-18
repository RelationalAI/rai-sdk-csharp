using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class GetEngine
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetEngine", "--engine <Engine name> --profile <Profile name>"){
                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Engine name to get the details."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Gets an engine's details by name.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string engine, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.GetEngineAsync(engine));
        }

    }
}