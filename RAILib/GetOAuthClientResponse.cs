namespace RAILib
{
    using Newtonsoft.Json;

    public class GetOAuthClientResponse : Entity
    {
        [JsonProperty("client", Required = Required.Always)]
        public OAuthClientEx Client { get; set; }
    }
}