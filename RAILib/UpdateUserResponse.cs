namespace RAILib
{
    using Newtonsoft.Json;

    public class UpdateUserResponse : Entity
    {
        [JsonProperty("user", Required = Required.Always)]
        public User User { get; set; }
    }
}