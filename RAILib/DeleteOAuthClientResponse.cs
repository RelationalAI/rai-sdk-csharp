namespace RAILib
{
    using Newtonsoft.Json;

    public class DeleteOAuthClientResponse : Entity
    {
        [JsonProperty("client_id", Required = Required.Always)]
        public string ID { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }
    }
}