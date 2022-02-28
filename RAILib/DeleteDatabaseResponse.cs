namespace RAILib
{
    using Newtonsoft.Json;

    public class DeleteDatabaseResponse : Entity
    {
        [JsonProperty("name", Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }
    }
}