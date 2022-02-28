namespace RAILib
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    
    public class Json<T>
    {
        public static T Deserialize(string data, string key = null)
        {
            if (string.IsNullOrEmpty(data) || data == "[]")
            {
                throw new SystemException("404 not found");
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(data);
            }
            catch
            {
                throw new SystemException(data);
            }
        }
    }
}