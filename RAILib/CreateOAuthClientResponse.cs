namespace RAILib
{
    using Newtonsoft.Json;

    public class CreateOAuthClientResponse : Entity
    {
        [JsonProperty("client", Required = Required.Always)]
        public OAuthClient OAuthClient { get; set; }
    }
}