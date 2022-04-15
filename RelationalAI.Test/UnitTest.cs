using System;
using System.Collections.Generic;

namespace RelationalAI.Test
{
    public class UnitTest : IDisposable
    {
        public Client CreateClient()
        {
            Dictionary<string, object> config = Config.Read("", "default");
            Client.Context ctx = new Client.Context(config);
            return new Client(ctx);
        }

        public virtual void Dispose() {}

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
