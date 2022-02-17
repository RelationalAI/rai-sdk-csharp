using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

using RAILib.Credentials;

namespace RAILib
{
    public class Api
    {
        private Rest _rest;
        private Api.Context _context;
        private static string PathEngine = "/compute";
        private static string PathDatabae = "/database";
        private static string PathTransaction = "/transaction";
        private static string PathTransactions = "/transactions";
        private static string PathUsers = "/users";
        private static string PathOAuthClients = "/oauth-clients";
        public Api(Api.Context context)
        {
            _context = context;
            _rest = new Rest(context);
        }
        public string CreateDatabase(string database, string source = null)
        {
            var data = new Dictionary<string, string>(){ {"name", database} };
            
            return FormatResponse(_rest.Put(MakeUrl(Api.PathDatabae), data));
        }
        public string CreateEngine(string engine, EngineSize size = EngineSize.XS)
        {
            var data = new Dictionary<string, string>()
            {
                {"region", _context.Region},
                {"name", engine},
                {"size", size.ToString()}
            };
            
            return FormatResponse(_rest.Put(MakeUrl(Api.PathEngine), data));
        }
        public string CreateOAuthClient(string name, List<Permission> permissions = null)
        {
            HashSet<string> uniquePermissions = new HashSet<string>();
            if(null != permissions) 
                permissions.ForEach(p => uniquePermissions.Add(p.Value()));

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"name", name},
                {"permissions", uniquePermissions}
            };

            return FormatResponse(_rest.Post(MakeUrl(Api.PathOAuthClients), data), "clients");
        }
        public string CreateUser(string email, List<Role> roles = null)
        {
            HashSet<string> uniqueRoles = new HashSet<string>();
            if(null != roles) 
                roles.ForEach(r => uniqueRoles.Add(r.Value()));
            
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"email", email},
                {"roles", uniqueRoles}
            };

            return FormatResponse(_rest.Post(MakeUrl(Api.PathUsers), data));
        }
        public string DeleteDatabase(string database)
        {
            var data = new Dictionary<string, string>(){ {"name", database} };
            
            return FormatResponse(_rest.Delete(MakeUrl(Api.PathDatabae), data));
        }
        public string DeleteEngine(string engine)
        {
            var data = new Dictionary<string, string>(){ {"name", engine} };
            
            return FormatResponse(_rest.Delete(MakeUrl(Api.PathEngine), data));
        }
        public string DeleteOAuthClient(string id)
        {
            return FormatResponse(_rest.Delete(MakeUrl(String.Format("{0}/{1}", Api.PathOAuthClients, id))));
        }
        public string DeleteTransaction(string id)
        {
            return FormatResponse(_rest.Delete(MakeUrl(String.Format("{0}/{1}", Api.PathTransactions, id))));
        }
        public string DeleteUser(string id)
        {
            return FormatResponse(_rest.Delete(MakeUrl(String.Format("{0}/{1}", Api.PathUsers, id))));
        }
        public string DisableUser(string id)
        {   
            return FormatResponse(UpdateUser(id, UserStatus.InActive));
        }
        public string EnableUser(string id)
        {   
            return FormatResponse(UpdateUser(id, UserStatus.Active));
        }
        public string GetDatabase(string database)
        {
            var parameters = new Dictionary<string, string>() { {"name", database}};

            return GetResource(Api.PathDatabae, "databases", parameters);
        }
        public string GetEngine(string engine)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"name", engine},
                {"deleted_on", ""}
            };

            return GetResource(Api.PathEngine, "computes", parameters);
        }
        public string GetOAuthClient(string id)
        {
            return GetResource(String.Format("{0}/{1}", Api.PathOAuthClients, id), "client");
        }
        public string GetTransaction(string id)
        {
            return GetResource(String.Format("{0}/{1}", Api.PathTransactions, id), "transaction");
        }
        public string GetUser(string userId)
        {
            return GetResource(String.Format("{0}/{1}", Api.PathUsers, userId));
        }
        public string ListDatabases(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if(state != null)
                parameters.Add("state", state);
            
            return ListCollections(Api.PathDatabae, "databases", parameters);
        }
        public string ListEngines(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if(state != null)
                parameters.Add("state", state);
            
            return ListCollections(Api.PathEngine, "computes", parameters);
        }
        public string ListOAuthClients()
        { 
            return ListCollections(Api.PathOAuthClients, "clients");
        }
        public string ListUsers()
        { 
            return ListCollections(Api.PathUsers, "users");
        }
        public string UpdateUser(string id, UserStatus status = UserStatus.None, List<Role> roles = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if(null != roles)
            { 
                HashSet<string> uniqueRoles = new HashSet<string>();
                roles.ForEach(r => uniqueRoles.Add(r.Value()));
                data.Add("roles", uniqueRoles);
            }
            if(status != UserStatus.None)
            {
                data.Add("status", status.Value());
            }

            return FormatResponse(_rest.Patch(MakeUrl(String.Format("{0}/{1}", Api.PathUsers, id)), data));
        }
        private static string FormatResponse(string response, string key = null)
        {
            try 
            {
                // to return the formatted JSON
                var json = JObject.Parse(response);
                JToken result = json;
                if(key != null && json.ContainsKey(key))
                    result = json.GetValue(key);

                return result.ToString();
            }
            catch{}
            
            return response;
        }
        private string GetResource(string path, string key=null, Dictionary<string, string> parameters=null)
        {
            var url = MakeUrl(path);
            var resp = _rest.Get(url, null, null, parameters);
            try
            {
                var json = JObject.Parse(resp);
                JToken result = json;
                if (null != key && json.ContainsKey(key))
                {
                    result = json.GetValue(key);
                }
                if (null != result && result.Type is JTokenType.Array && result.HasValues)
                {
                    // making sure there aren't more than one value
                    if (result.First != result.Last)
                    {
                        throw new SystemException("more than one resources found");
                    }

                    result = result.First;
                }

                return result.ToString();
            }
            catch
            {
                // ignore exception
            }

            return resp;
        }
        private string ListCollections(string path, string key=null, Dictionary<string, string> parameters=null)
        {
            var url = MakeUrl(path);
            var resp = _rest.Get(url, null, null, parameters);
            try
            {
                var json = JObject.Parse(resp);
                JToken result = json;
                if (null != key && json.ContainsKey(key))
                {
                    result = json.GetValue(key);
                }

                return result.ToString();
            }
            catch
            {
                // ignore exception
            }

            return resp;
        }
        private string MakeUrl(string path)
        {
            return String.Format("{0}://{1}:{2}{3}", _context.Scheme, _context.Host, _context.Port, path);
        }

        public class Context : Rest.Context
        {
            string _host;
            string _port;
            string _scheme;

            public Context(string host = null, string port = null, string scheme = null, string region = null,
                ICredentials credentials = null)
            {
                Host = host;
                Port = port;
                Scheme = scheme;
                Region = region;
                Credentials = credentials;
            }
            public Context(Dictionary<string, object> config)
            {
                if (config != null)
                {
                    Host = (config.ContainsKey("host") && config["host"] != null) ? (string)config["host"] : null;
                    Port = (config.ContainsKey("port") && config["port"] != null) ? (string)config["port"] : null;
                    Scheme = (config.ContainsKey("scheme") && config["scheme"] != null) ? (string)config["scheme"] : null;
                    Region = (config.ContainsKey("region") && config["region"] != null) ? (string)config["region"] : null;
                    Credentials = (config.ContainsKey("credentials") && config["credentials"] != null) ?
                        (ICredentials)config["credentials"] : null;
                }
            }
            public string Host
            {
                get => _host;
                set => _host = !String.IsNullOrEmpty(value) ? value : "localhost";
            }
            public string Port
            {
                get => _port;
                set => _port = !String.IsNullOrEmpty(value) ? value : "443";
            }
            public string Scheme
            {
                get => _scheme;
                set => _scheme = !String.IsNullOrEmpty(value) ? value : "https";
            }
        }
    }
}