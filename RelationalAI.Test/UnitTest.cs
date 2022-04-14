using System;
using System.Collections.Generic;

namespace RelationalAI.Test
{
    public class UnitTest : IDisposable
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{UUID}";
        public static string EngineName = $"csharp-sdk-{UUID}";
        public static string UserEmail = $"csharp-sdk-{UUID}@relational.ai";
        public static string OAuthClientName = $"csharp-sdk-{UUID}";
        public Client CreateClient()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            return new Client(ctx);
        }

        public void Dispose()
        {
            Client client = CreateClient();

            try { client.DeleteDatabase(Dbname); } catch {}
            try { client.DeleteEngineWait(EngineName); } catch {}
            try { client.DeleteUser(client.FindUser(UserEmail).ID); } catch {}
            try { client.DeleteOAuthClient(client.FindOAuthClient(OAuthClientName).ID); } catch {}
        }

        public Relation findRelation(Relation[] relations, string colName)
        {
            foreach(var relation in relations)
            {
                var keys = relation.RelKey.Keys;
                if (keys.Length == 0)
                    continue;
                var name = keys[0];
                if (name.Equals(colName))
                    return relation;
            }
            return null;
        }
    }
}
