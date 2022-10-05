using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI;

namespace RelationalAI.Examples
{
    public class CancelTransaction
    {
        public static Command GetCommand()
        {
            var cmd = new Command("CancelTransaction", "--id <Transaction id>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "Transaction id."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Cancel an asynchronous transaction.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);
            var rsp = await client.CancelTransactionAsync(id);
            Console.WriteLine(rsp);
        }
    }
}
