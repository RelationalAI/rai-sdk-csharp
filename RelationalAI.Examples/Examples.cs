using System.CommandLine;
using System.CommandLine.Parsing;


namespace RelationalAI.Examples
{
    public class Examples
    {
        public static int Main(string[] args)
        {
            // setup the example commands CLI
            return BuildCommandLine().Invoke(args);
        }

        private static RootCommand BuildCommandLine()
        {
            // init the root command
            var root = new RootCommand("RAI examples CLI");
            // add various examples as commands
            root.AddCommand(CreateEngine.GetCommand());
            root.AddCommand(DeleteEngine.GetCommand());
            root.AddCommand(GetDatabase.GetCommand());
            root.AddCommand(GetEngine.GetCommand());
            root.AddCommand(GetOAuthClient.GetCommand());
            root.AddCommand(GetUser.GetCommand());
            root.AddCommand(FindUser.GetCommand());
            root.AddCommand(EnableUser.GetCommand());
            root.AddCommand(DisableUser.GetCommand());
            root.AddCommand(DeleteUser.GetCommand());
            root.AddCommand(ListDatabases.GetCommand());
            root.AddCommand(ListEngines.GetCommand());
            root.AddCommand(ListOAuthClients.GetCommand());
            root.AddCommand(ListUsers.GetCommand());
            root.AddCommand(Execute.GetCommand());
            root.AddCommand(ExecuteAsync.GetCommand());
            root.AddCommand(GetTransaction.GetCommand());
            root.AddCommand(GetTransactionResults.GetCommand());
            root.AddCommand(GetTransactionMetadata.GetCommand());
            root.AddCommand(GetTransactionProblems.GetCommand());
            //return the root command
            return root;
        }
    }
}