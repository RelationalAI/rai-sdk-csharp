namespace RAILib
{
    using Newtonsoft.Json;

    public class CreateUserResponse : Entity
    {
        [JsonProperty("user", Required = Required.Always)]
        public User User { get; set; }
    }
}