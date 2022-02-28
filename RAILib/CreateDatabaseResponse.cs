namespace RAILib
{
    using Newtonsoft.Json;

    public class CreateDatabaseResponse : Entity
    {
        [JsonProperty("database", Required = Required.Always)]
        public Database Database { get; set; }
    }
}