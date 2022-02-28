namespace RAILib
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class ListDatabasesResponse : Entity
    {
        [JsonProperty("databases", Required = Required.Always)]
        public List<Database> Databases { get; set; }
    }
}