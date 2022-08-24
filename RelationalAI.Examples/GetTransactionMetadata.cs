using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Threading.Tasks;
using RelationalAI.Services;
using RelationalAI.Utils;

namespace RelationalAI.Examples
{
    public class GetTransactionMetadata
    {
        public static Command GetCommand()
        {
            var cmd = new Command("GetTransactionMetadata", "--id <Transaction id>"){
                new Option<string>("--id"){
                    IsRequired = true,
                    Description = "Transaction id."
                },

                new Option<string>("--profile"){
                    IsRequired = false,
                    Description = "Profile name from .rai/config to connect to RAI Cloud."
                }
            };
            cmd.Description = "Get an asynchronous transaction protobuf metadata.";
            cmd.Handler = CommandHandler.Create<string, string>(Run);
            return cmd;
        }

        private static async Task Run(string id, string profile = "default")
        {
            var config = Config.Read("", profile);
            var context = new Client.Context(config);
            var client = new Client(context);

            var metadata = await client.GetTransactionMetadataAsync(id);
            Console.WriteLine(metadata);
        }
    }
}
