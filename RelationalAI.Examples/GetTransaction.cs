using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class GetTransaction
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetTransaction", "--id <Transaction id>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "Transaction id."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Get an asynchronous transaction information.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            Console.WriteLine(await client.GetTransactionAsync(id));
        }
    }
}
