using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using IniParser;
using IniParser.Model;
using RAILib.Credentials;

namespace RAILib
{
    public class Config
    {
        private static FileIniDataParser parser = new FileIniDataParser();
        
        public static Dictionary<string, object> Read(string path=null, string profile="default" )
        {
            IniData data = Config.LoadIniConfig(path);
            
            string[] keys = {"host", "port", "scheme", "region"};
            Dictionary<string, object> config = new Dictionary<string, object>();
            foreach (string key in keys)
                config.Add(key, Config.GetIniValue(data, profile, key, null));
            
            ICredentials clientCredentials = Config.ReadClientCredentials(data, profile);
            if(null != clientCredentials)
                config.Add("credentials", clientCredentials);
            

            return config;
        }
        private static ICredentials ReadClientCredentials(IniData data, string profile)
        {
            var clientID = Config.GetIniValue(data, profile, "client_id", null);
            var clientSecret = Config.GetIniValue(data, profile, "client_secret", null);
            var clientCredentialsURL = Config.GetIniValue(data, profile, "client_credentials_url", null);
            if (null != clientID && null != clientSecret)
                return new ClientCredentials(clientID, clientSecret, clientCredentialsURL);

            return null;
        }
        private static string GetRAIConfigPath()
        {
            return Path.Combine(GetRAIDir(), "config");
        }
        private static string GetRAIDir()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                home = homeDrive + home;
            }

            return Path.Combine(home, ".rai");
        }
        private static IniData LoadIniConfig(string path)
        {
            path = !String.IsNullOrEmpty(path) ? path : Config.GetRAIConfigPath();
            if (!File.Exists(path)) 
                throw new FileNotFoundException(path + " does not exist");

            return parser.ReadFile(path);
        }
        private static string GetIniValue(IniData ini, string profile, string key, string defaultValue="notfound")
        {
            var KeyData = ini[profile].GetKeyData(key);
            return null == KeyData ? defaultValue : KeyData.Value;
        }
    }
}
