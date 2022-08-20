using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class GetTransactionResults
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetTransactionResults", "--id <Transaction id>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "Transaction id."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Get an asynchronous transaction results.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            foreach (var result in await client.GetTransactionResultsAsync(id))
            {
                Console.WriteLine(result);
            }
        }
    }
}
