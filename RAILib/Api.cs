namespace RAILib
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json.Linq;
    using RAILib.Credentials;

    public class Api
    {
        private static readonly string PathEngine = "/compute";
        private static readonly string PathDatabae = "/database";
        private static readonly string PathTransaction = "/transaction";
        private static readonly string PathTransactions = "/transactions";
        private static readonly string PathUsers = "/users";
        private static readonly string PathOAuthClients = "/oauth-clients";
        private readonly Rest rest;
        private readonly Api.Context context;

        public Api(Api.Context context)
        {
            this.context = context;
            this.rest = new Rest(context);
        }

        public string CreateDatabase(string database, string source = null)
        {
            var data = new Dictionary<string, string>() { { "name", database } };
            return FormatResponse(this.rest.Put(this.MakeUrl(Api.PathDatabae), data));
        }

        public string CreateEngine(string engine, EngineSize size = EngineSize.XS)
        {
            var data = new Dictionary<string, string>()
            {
                {"region", context.Region},
                {"name", engine},
                {"size", size.ToString()}
            };
            return FormatResponse(this.rest.Put(this.MakeUrl(Api.PathEngine), data));
        }

        public string CreateOAuthClient(string name, List<Permission> permissions = null)
        {
            HashSet<string> uniquePermissions = new HashSet<string>();
            if (permissions != null)
            {
                permissions.ForEach(p => uniquePermissions.Add(p.Value()));
            }

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"name", name},
                {"permissions", uniquePermissions}
            };

            return FormatResponse(this.rest.Post(this.MakeUrl(Api.PathOAuthClients), data), "clients");
        }

        public string CreateUser(string email, List<Role> roles = null)
        {
            HashSet<string> uniqueRoles = new HashSet<string>();
            if (roles != null)
            {
                roles.ForEach(r => uniqueRoles.Add(r.Value()));
            }

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                {"email", email},
                {"roles", uniqueRoles}
            };

            return FormatResponse(this.rest.Post(this.MakeUrl(Api.PathUsers), data));
        }

        public string DeleteDatabase(string database)
        {
            var data = new Dictionary<string, string>()
            {
                {"name", database} 
            };
            return FormatResponse(this.rest.Delete(this.MakeUrl(Api.PathDatabae), data));
        }

        public string DeleteEngine(string engine)
        {
            var data = new Dictionary<string, string>()
            {
                {"name", engine} 
            };
            return FormatResponse(this.rest.Delete(this.MakeUrl(Api.PathEngine), data));
        }

        public string DeleteOAuthClient(string id)
        {
            return FormatResponse(this.rest.Delete(this.MakeUrl(String.Format("{0}/{1}", Api.PathOAuthClients, id))));
        }

        public string DeleteTransaction(string id)
        {
            return FormatResponse(this.rest.Delete(this.MakeUrl(String.Format("{0}/{1}", Api.PathTransactions, id))));
        }

        public string DeleteUser(string id)
        {
            return FormatResponse(this.rest.Delete(this.MakeUrl(String.Format("{0}/{1}", Api.PathUsers, id))));
        }

        public string DisableUser(string id)
        {
            return FormatResponse(this.UpdateUser(id, UserStatus.InActive));
        }

        public string EnableUser(string id)
        {
            return FormatResponse(this.UpdateUser(id, UserStatus.Active));
        }

        public string GetDatabase(string database)
        {
            var parameters = new Dictionary<string, string>() 
            {
                {"name", database}
            };
            return this.GetResource(Api.PathDatabae, "databases", parameters);
        }

        public string GetEngine(string engine)
        {
            var parameters = new Dictionary<string, string>()
            {
                {"name", engine},
                {"deleted_on", ""}
            };
            return this.GetResource(Api.PathEngine, "computes", parameters);
        }

        public string GetOAuthClient(string id)
        {
            return this.GetResource(String.Format("{0}/{1}", Api.PathOAuthClients, id), "client");
        }

        public string GetTransaction(string id)
        {
            return this.GetResource(String.Format("{0}/{1}", Api.PathTransactions, id), "transaction");
        }

        public string GetUser(string userId)
        {
            return this.GetResource(String.Format("{0}/{1}", Api.PathUsers, userId));
        }

        public string ListDatabases(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            return this.ListCollections(Api.PathDatabae, "databases", parameters);
        }

        public string ListEngines(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            return this.ListCollections(Api.PathEngine, "computes", parameters);
        }

        public string ListOAuthClients()
        {
            return this.ListCollections(Api.PathOAuthClients, "clients");
        }

        public string ListUsers()
        {
            return this.ListCollections(Api.PathUsers, "users");
        }

        public string UpdateUser(string id, UserStatus status = UserStatus.None, List<Role> roles = null)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            if (roles != null)
            {
                HashSet<string> uniqueRoles = new HashSet<string>();
                roles.ForEach(r => uniqueRoles.Add(r.Value()));
                data.Add("roles", uniqueRoles);
            }

            if (status != UserStatus.None)
            {
                data.Add("status", status.Value());
            }

            return FormatResponse(this.rest.Patch(this.MakeUrl(String.Format("{0}/{1}", Api.PathUsers, id)), data));
        }

        private static string FormatResponse(string response, string key = null)
        {
            try
            {
                // to return the formatted JSON
                var json = JObject.Parse(response);
                JToken result = json;
                if (key != null  && json.ContainsKey(key))
                {
                    result = json.GetValue(key);
                }

                return result.ToString();
            }
            catch
            {
                // ignore exception
            }

            return response;
        }

        private string GetResource(string path, string key = null, Dictionary<string, string> parameters = null)
        {
            var url = this.MakeUrl(path);
            var resp = this.rest.Get(url, null, null, parameters);
            try
            {
                var json = JObject.Parse(resp);
                JToken result = json;
                if (key != null && json.ContainsKey(key))
                {
                    result = json.GetValue(key);
                }

                if (result != null && result.Type is JTokenType.Array && result.HasValues)
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

        private string ListCollections(string path, string key = null, Dictionary<string, string> parameters = null)
        {
            var url = this.MakeUrl(path);
            var resp = this.rest.Get(url, null, null, parameters);
            try
            {
                var json = JObject.Parse(resp);
                JToken result = json;
                if (key != null && json.ContainsKey(key))
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
            return String.Format("{0}://{1}:{2}{3}", this.context.Scheme, this.context.Host, this.context.Port, path);
        }

        public class Context : Rest.Context
        {
            private string host;
            private string port;
            private string scheme;

            public Context(
                string host = null,
                string port = null,
                string scheme = null,
                string region = null,
                ICredentials credentials = null)
            {
                this.Host = host;
                this.Port = port;
                this.Scheme = scheme;
                this.Region = region;
                this.Credentials = credentials;
            }

            public Context(Dictionary<string, object> config)
            {
                if (config != null)
                {
                    this.Host = (config.ContainsKey("host") && config["host"] != null) ? (string)config["host"] : null;
                    this.Port = (config.ContainsKey("port") && config["port"] != null) ? (string)config["port"] : null;
                    this.Scheme = (config.ContainsKey("scheme") && config["scheme"] != null) ? (string)config["scheme"] : null;
                    this.Region = (config.ContainsKey("region") && config["region"] != null) ? (string)config["region"] : null;
                    this.Credentials = (config.ContainsKey("credentials") && config["credentials"] != null) ?
                        (ICredentials)config["credentials"] : null;
                }
            }

            public string Host
            {
                get => this.host;
                set => this.host = !string.IsNullOrEmpty(value) ? value : "localhost";
            }

            public string Port
            {
                get => this.port;
                set => this.port = !string.IsNullOrEmpty(value) ? value : "443";
            }

            public string Scheme
            {
                get => this.scheme;
                set => this.scheme = !string.IsNullOrEmpty(value) ? value : "https";
            }
        }
    }
}