namespace RAILib
{
    using Newtonsoft.Json;

    public class GetUserResponse : Entity
    {
        [JsonProperty("user", Required = Required.Always)]
        public User User { get; set; }
    }
}