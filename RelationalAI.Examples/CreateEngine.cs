using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI;


namespace RelationalAI.Examples
{
    public class CreateEngine
    {
        public static Command GetCommand()
        {
            var cmd = new Command("CreateEngine", "--engine <Engine name> --profile <Profile name>"){
                new Option<string>("--engine"){
                    IsRequired = true,
                    Description = "Engine name to create."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Creates a new RAI engine.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string engine, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            Console.WriteLine("Creating Engine: "+ engine);
            Console.WriteLine(await client.CreateEngineWaitAsync(engine));
        }

    }
}