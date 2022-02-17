using System;

namespace RAILib.Credentials
{
    public class AccessToken
    {
        private DateTime _createdOn;
        private int _expiresIn;
        private string _token;

        public AccessToken(string token, int expiresIn)
        {
            Token = token;
            ExpiresIn = expiresIn;
            _createdOn = DateTime.Now;
        }

        public int ExpiresIn
        {
            get => _expiresIn;
            set => _expiresIn = value > 0 ? value : throw new ArgumentException("ExpiresIn should be greater than 0 ");  
        }

        public bool IsExpired
        {
            get => (DateTime.Now - _createdOn).TotalSeconds >= ExpiresIn; 
        }
        public string Token 
        {
            get => _token;
            set => _token = value;
        } 
        
    }
}