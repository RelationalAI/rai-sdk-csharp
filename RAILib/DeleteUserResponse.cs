namespace RAILib
{
    using Newtonsoft.Json;

    public class DeleteUserResponse : Entity
    {
        [JsonProperty("user_id", Required = Required.Always)]
        public string ID { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }
    }
}