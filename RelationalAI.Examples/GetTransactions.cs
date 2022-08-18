using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;

namespace RelationalAI.Examples
{
    public class GetTransactions
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetTransactions"){

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Get an asynchronous transactions information.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            Dictionary<string, object> config = Config.Read("", profile);
            Client.Context context = new Client.Context(config);
            Client client = new Client(context);
            var transactions = await client.GetTransactionsAsync();
            foreach (var transaction in transactions.Transactions)
            {
                Console.WriteLine(transaction);
            }
        }
    }
}
