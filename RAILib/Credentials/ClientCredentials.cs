using System;

namespace RAILib.Credentials
{
    public class ClientCredentials : ICredentials
    {
        public static string defaultClientCredentialsURL = "https://login.relationalai.com/oauth/token";
        private string _clientID;
        private string _clientSecret;
        private string _clientCredentialsURL = ClientCredentials.defaultClientCredentialsURL;
        private AccessToken _accessToken;

        public ClientCredentials(string clientID, string clientSecret)
        {
            ClientID = clientID;
            ClientSecret = clientSecret;
        }
        public ClientCredentials(string clientID, string clientSecret, string clientCredentialsURL): 
            this(clientID, clientSecret)
        {
            ClientCredentialsURL = clientCredentialsURL;
        }
        public string ClientID
        {
            get => _clientID;
            set => _clientID = !String.IsNullOrEmpty(value) ? value : 
                throw new ArgumentException("ClientID cannot be null or empty"); 
        }
        public string ClientSecret
        {
            get => _clientSecret;
            set => _clientSecret =  !String.IsNullOrEmpty(value) ? value : 
                throw new ArgumentException("ClientSecret cannot be null or empty"); 
        }
        public string ClientCredentialsURL
        {
            get => _clientCredentialsURL;
            set => _clientCredentialsURL = !String.IsNullOrEmpty(value) ? value : defaultClientCredentialsURL;
        }
        public AccessToken AccessToken
        {
            get => _accessToken;
            set => _accessToken = value;
        }

    }
}