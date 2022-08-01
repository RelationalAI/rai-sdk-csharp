/*
 * Copyright 2022 RelationalAI, Inc.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace RelationalAI
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Threading;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RelationalAI.Credentials;
    using Relationalai.Protocol;

    public class Client
    {
        private static readonly string PathEngine = "/compute";
        private static readonly string PathDatabae = "/database";
        private static readonly string PathTransaction = "/transaction";
        private static readonly string PathTransactions = "/transactions";
        private static readonly string PathUsers = "/users";
        private static readonly string PathOAuthClients = "/oauth-clients";
        private readonly Rest rest;
        private readonly Client.Context context;

        public Client(Client.Context context)
        {
            this.context = context;
            this.rest = new Rest(context);
        }

        public Database CreateDatabase(string database, string engine)
        {
            return this.CreateDatabase(database, engine, false);
        }

        public Database CreateDatabase(string database, string engine, bool overwrite)
        {
            var mode = CreateMode(null, overwrite);
            var transaction = new Transaction(this.context.Region, database, engine, mode);
            string resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), transaction.Payload(null), null, transaction.QueryParams()) as string;
            return this.GetDatabase(database);
        }

        public Database GetDatabase(string database)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "name", database },
            };

            string resp = this.GetResource(Client.PathDatabae, null, parameters);
            List<Database> dbs = Json<GetDatabaseResponse>.Deserialize(resp).Databases;
            return dbs.Count > 0 ? dbs[0] : throw new SystemException("not found");
        }

        public List<Database> ListDatabases(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            string resp = this.ListCollections(Client.PathDatabae, null, parameters);
            return Json<ListDatabasesResponse>.Deserialize(resp).Databases;
        }

        public DeleteDatabaseResponse DeleteDatabase(string database)
        {
            var data = new Dictionary<string, string>()
            {
                { "name", database },
            };
            string resp = this.rest.Delete(this.MakeUrl(Client.PathDatabae), data) as string;
            return Json<DeleteDatabaseResponse>.Deserialize(resp);
        }

        public Engine CreateEngine(string engine, EngineSize size = EngineSize.XS)
        {
            var data = new Dictionary<string, string>()
            {
                { "region", this.context.Region },
                { "name", engine },
                { "size", size.ToString() },
            };
            string resp = this.rest.Put(this.MakeUrl(Client.PathEngine), data) as string;
            return Json<CreateEngineResponse>.Deserialize(resp).Engine;
        }

        public Engine CreateEngineWait(string engine, EngineSize size = EngineSize.XS)
        {
            var resp = this.CreateEngine(engine, size);
            while (!IsTerminalState(resp.State, "PROVISIONED"))
            {
                Thread.Sleep(2000);
                resp = this.GetEngine(resp.Name);
            }

            return resp;
        }

        public Engine GetEngine(string engine)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "name", engine },
                { "deleted_on", string.Empty },
            };
            var resp = this.GetResource(Client.PathEngine, null, parameters);
            List<Engine> engines = Json<GetEngineResponse>.Deserialize(resp).Engines;
            return engines.Count > 0 ? engines[0] : throw new SystemException("not found");
        }

        public List<Engine> ListEngines(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            string resp = this.ListCollections(Client.PathEngine, null, parameters);
            return Json<ListEnginesResponse>.Deserialize(resp).Engines;
        }

        public DeleteEngineResponse DeleteEngine(string engine)
        {
            var data = new Dictionary<string, string>()
            {
                { "name", engine },
            };
            string resp = this.rest.Delete(this.MakeUrl(Client.PathEngine), data) as string;
            return Json<DeleteEngineResponse>.Deserialize(resp);
        }

        public DeleteEngineResponse DeleteEngineWait(string engine)
        {
            var resp = this.DeleteEngine(engine);
            var status = resp.Status.State;
            while (!IsTerminalState(status, "DELETED"))
            {
                Thread.Sleep(2000);
                status = this.GetEngine(resp.Status.Name).State;
            }
            return resp;
        }

        public OAuthClient CreateOAuthClient(string name, List<Permission> permissions = null)
        {
            HashSet<string> uniquePermissions = new HashSet<string>();
            if (permissions != null)
            {
                permissions.ForEach(p => uniquePermissions.Add(p.Value()));
            }
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "name", name },
                { "permissions", uniquePermissions },
            };
            string resp = this.rest.Post(this.MakeUrl(Client.PathOAuthClients), data) as string;
            return Json<CreateOAuthClientResponse>.Deserialize(resp).OAuthClient;
        }

        public OAuthClient FindOAuthClient(string name)
        {
            List<OAuthClient> clients = this.ListOAuthClients();
            foreach (var client in clients)
            {
                if (client.Name == name)
                {
                    return client;
                }
            }

            throw new SystemException("not found");
        }

        public OAuthClientEx GetOAuthClient(string id)
        {
            string resp = this.GetResource(string.Format("{0}/{1}", Client.PathOAuthClients, id));
            return Json<GetOAuthClientResponse>.Deserialize(resp).Client;
        }

        public List<OAuthClient> ListOAuthClients()
        {
            string resp = this.ListCollections(Client.PathOAuthClients);
            return Json<ListOAuthClientResponse>.Deserialize(resp).Clients;
        }

        public DeleteOAuthClientResponse DeleteOAuthClient(string id)
        {
            string resp = this.rest.Delete(this.MakeUrl(string.Format("{0}/{1}", Client.PathOAuthClients, id))) as string;
            return Json<DeleteOAuthClientResponse>.Deserialize(resp);
        }

        public User CreateUser(string email, List<Role> roles = null)
        {
            HashSet<string> uniqueRoles = new HashSet<string>();
            if (roles != null)
            {
                roles.ForEach(r => uniqueRoles.Add(r.Value()));
            }
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "email", email },
                { "roles", uniqueRoles },
            };
            string resp = this.rest.Post(this.MakeUrl(Client.PathUsers), data) as string;
            return Json<CreateUserResponse>.Deserialize(resp).User;
        }

        public User UpdateUser(string id, UserStatus status = UserStatus.None, List<Role> roles = null)
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
            string resp = this.rest.Patch(this.MakeUrl(string.Format("{0}/{1}", Client.PathUsers, id)), data) as string;
            return Json<UpdateUserResponse>.Deserialize(resp).User;
        }

        public User FindUser(string email)
        {
            List<User> users = this.ListUsers();
            foreach (var user in users)
            {
                if (user.Email == email)
                {
                    return user;
                }
            }

            throw new SystemException("not found");
        }

        public User GetUser(string userId)
        {
            string resp = this.GetResource(string.Format("{0}/{1}", Client.PathUsers, userId));
            return Json<GetUserResponse>.Deserialize(resp).User;
        }

        public List<User> ListUsers()
        {
            string resp = this.ListCollections(Client.PathUsers);
            return Json<ListUsersResponse>.Deserialize(resp).Users;
        }

        public DeleteUserResponse DeleteUser(string id)
        {
            string resp = this.rest.Delete(this.MakeUrl(string.Format("{0}/{1}", Client.PathUsers, id))) as string;
            return Json<DeleteUserResponse>.Deserialize(resp);
        }

        public User DisableUser(string id)
        {
            return this.UpdateUser(id, UserStatus.InActive);
        }

        public User EnableUser(string id)
        {
            return this.UpdateUser(id, UserStatus.Active);
        }

        public TransactionsAsyncMultipleResponses GetTransactions()
        {
            var rsp = this.rest.Get(this.MakeUrl(Client.PathTransactions)) as string;
            return Json<TransactionsAsyncMultipleResponses>.Deserialize(rsp);
        }

        public TransactionAsyncSingleResponse GetTransaction(string id)
        {
            var rsp = this.rest.Get(this.MakeUrl(string.Format("{0}/{1}", Client.PathTransactions, id))) as string;
            return Json<TransactionAsyncSingleResponse>.Deserialize(rsp);
        }

        public List<ArrowRelation> GetTransactionResults(string id)
        {
            var files = this.rest.Get(this.MakeUrl(string.Format("{0}/{1}/results", Client.PathTransactions, id))) as List<TransactionAsyncFile>;
            return this.rest.ReadArrowFiles(files);
        }

        public List<TransactionAsyncMetadataResponse> GetTransactionMetadata(string id)
        {
            var rsp = this.rest.Get(this.MakeUrl(string.Format("{0}/{1}/metadata", Client.PathTransactions, id))) as string;
            return Json<List<TransactionAsyncMetadataResponse>>.Deserialize(rsp);
        }

        public MetadataInfo GetTransactionMetadataInfo(string id)
        {
            var headers = new Dictionary<string, string>()
            {
                { "accept", "application/x-protobuf" }
            };

            var rsp = this.rest.Get(this.MakeUrl(string.Format("{0}/{1}/metadata", Client.PathTransactions, id)), headers: headers) as MetadataInfo;
            return rsp;
        }

        public List<object> GetTransactionProblems(string id)
        {
            var rsp = this.rest.Get(this.MakeUrl(string.Format("{0}/{1}/problems", Client.PathTransactions, id))) as string;
            return ParseProblemsResult(rsp);
        }

        public TransactionAsyncCancelResponse CancelTransaction(string id)
        {
            var rsp = this.rest.Post(this.MakeUrl(string.Format("{0}/{1}/cancel", Client.PathTransactions, id)), new Dictionary<string, object>() { }) as string;
            return Json<TransactionAsyncCancelResponse>.Deserialize(rsp);
        }

        private List<object> ParseProblemsResult(string rsp)
        {
            var output = new List<object>();

            var problems = JsonConvert.DeserializeObject(rsp);
            foreach (var problem in problems as JArray)
            {
                var data = JsonConvert.SerializeObject(problem);
                try
                {
                    output.Add(Json<IntegrityConstraintViolation>.Deserialize(data));
                }
                catch (SystemException)
                {
                    output.Add(Json<ClientProblem>.Deserialize(data));
                }
            }

            return output;
        }

        public string DeleteTransaction(string id)
        {
            return FormatResponse(this.rest.Delete(this.MakeUrl(string.Format("{0}/{1}", Client.PathTransactions, id))) as string);
        }

        private static string CreateMode(string source, bool overwrite)
        {
            if (source != null)
            {
                return overwrite ? "CLONE_OVERWRITE" : "CLONE";
            }
            else
            {
                return overwrite ? "CREATE_OVERWRITE" : "CREATE";
            }
        }

        public List<Edb> ListEdbs(string database, string engine)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeListEdbAction() };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            var actionsResp = Json<ListEdbsResponse>.Deserialize(resp).actions;
            if (actionsResp.Count == 0)
                return new List<Edb>();
            return actionsResp[0].result.rels;
        }

        public TransactionResult LoadModel(
            string database,
            string engine,
            string name,
            string model)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", false);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeInstallAction(name, model) };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public TransactionResult LoadModels(
            string database,
            string engine,
            Dictionary<string, string> models)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", false);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeInstallAction(models) };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public List<Model> ListModels(string database, string engine)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeListModelsAction() };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            var actionsResp = Json<ListModelsResponse>.Deserialize(resp).actions;
            if (actionsResp.Count == 0)
                return new List<Model>();
            return actionsResp[0].result.models;
        }

        public List<string> ListModelNames(string database, string engine)
        {
            var models = ListModels(database, engine);
            List<string> result = new List<string>();
            for (var i = 0; i < models.Count; i++)
                result.Add(models[i].Name);
            return result;
        }

        public Model GetModel(string database, string engine, string name)
        {
            var models = ListModels(database, engine);
            foreach (var model in models)
                if (model.Name.Equals(name))
                    return model;
            throw new SystemException($"model {name} not found.");
        }

        public TransactionResult DeleteModel(string database, string engine, string name)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeDeleteModelAction(name) };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        // Query
        public TransactionResult ExecuteV1(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", readOnly);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeQueryAction(source, inputs) };
            var body = tx.Payload(actions);
            var resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public TransactionAsyncResult Execute(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var id = ExecuteAsync(database, engine, source, readOnly, inputs).Transaction.ID;

            var transaction = GetTransaction(id).Transaction;

            while (!(transaction.State.Equals("COMPLETED") || transaction.State.Equals("ABORTED")))
            {
                Thread.Sleep(2000);
                transaction = GetTransaction(id).Transaction;
            }

            var results = GetTransactionResults(id);
            var metadata = GetTransactionMetadata(id);
            var metadataInfo = GetTransactionMetadataInfo(id);
            var problems = GetTransactionProblems(id);

            return new TransactionAsyncResult(transaction, results, metadata, metadataInfo, problems);
        }

        public TransactionAsyncResult ExecuteAsync(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var tx = new TransactionAsync(database, engine, readOnly, source, inputs);
            var body = tx.Payload();
            var rsp = this.rest.Post(this.MakeUrl(Client.PathTransactions), body, null, tx.QueryParams());

            if (rsp is string)
            {
                var txn = Json<TransactionAsyncCompactResponse>.Deserialize(rsp as string);
                return new TransactionAsyncResult(txn, new List<ArrowRelation>(), new List<TransactionAsyncMetadataResponse>(), null, new List<object>());
            }

            return ReadTransactionAsyncResults(rsp as List<TransactionAsyncFile>);
        }

        private TransactionAsyncResult ReadTransactionAsyncResults(List<TransactionAsyncFile> files)
        {
            var transaction = files.Find(f => f.Name == "transaction");
            var metadata = files.Find(f => f.Name == "metadata");
            var metadataInfo = files.Find(f => f.Name == "metadata_info");
            var problems = files.Find(f => f.Name == "problems");

            if (transaction == null)
            {
                throw new SystemException("transaction part not found");
            }
            TransactionAsyncCompactResponse transactionResult = Json<TransactionAsyncCompactResponse>.Deserialize(this.rest.ReadString(transaction.Data));

            if (metadata == null)
            {
                throw new SystemException("metadata part not found");
            }
            List<TransactionAsyncMetadataResponse> metadataResult = Json<List<TransactionAsyncMetadataResponse>>.Deserialize(this.rest.ReadString(metadata.Data));

            if (metadataInfo == null)
            {
                throw new SystemException("metadata info part not found");
            }

            var metadataInfoResult = this.rest.ReadMetadataInfo(metadataInfo.Data);

            List<object> problemsResult = null;
            if (problems != null)
            {
                problemsResult = ParseProblemsResult(this.rest.ReadString(problems.Data));
            }

            var results = this.rest.ReadArrowFiles(files);

            return new TransactionAsyncResult(transactionResult, results, metadataResult, metadataInfoResult, problemsResult);
        }

        private string GenLoadJson(string relation)
        {
            var builder = new StringBuilder();
            builder.Append("\ndef config:data = data\n");
            builder.AppendFormat("def insert:{0} = load_json[config]\n", relation);
            return builder.ToString();
        }

        public TransactionResult LoadJson(
            string database,
            string engine,
            string relation,
            string data)
        {
            var inputs = new Dictionary<string, string>();
            inputs.Add("data", data);
            var source = GenLoadJson(relation);
            return ExecuteV1(database, engine, source, false, inputs);
        }

        private void GenSchemaConfig(StringBuilder builder, CsvOptions options)
        {
            if (options == null)
                return;
            var schema = options.Schema;

            if (schema == null)
                return;

            var isEmpty = true;

            foreach (var entry in schema)
                isEmpty = false;

            if (isEmpty)
                return;

            var count = 0;
            builder.Append("def config:schema =");
            foreach (var entry in schema)
            {
                if (count > 0)
                    builder.Append(';');
                builder.AppendFormat("\n    :{0}, \"{1}\"", entry.Key, entry.Value);
                count++;
            }

            builder.Append('\n');
        }

        private string GenLiteral(Int32 value)
        {
            return value.ToString();
        }

        private string GenLiteral(char value)
        {
            if (value == '\'')
                return "'\\''";
            return $"'{value}'";
        }
        private string GenLiteral(object value)
        {
            if (value == null)
                throw new SystemException("Cannot generate literal from null value");

            if (
                value is int
                || value is Int16
                || value is Int32
                || value is Int64
            )
                return GenLiteral(Convert.ToInt32(value));

            if (value is char)
                return GenLiteral(Convert.ToChar(value));

            throw new SystemException($"Cannot generate type from {value.GetType()} value");
        }
        private void GenSyntaxOption(StringBuilder builder, string name, object value)
        {
            if (value == null)
                return;

            var lit = GenLiteral(value);
            var def = $"def config:syntax:{name} = {lit}\n";
            builder.Append(def);
        }
        private void GenSyntaxConfig(StringBuilder builder, CsvOptions options)
        {
            if (options == null)
                return;
            GenSyntaxOption(builder, "header_row", options.HeaderRow);
            GenSyntaxOption(builder, "delim", options.Delim);
            GenSyntaxOption(builder, "escapechar", options.EscapeChar);
            GenSyntaxOption(builder, "quotechar", options.QuoteChar);
        }
        private string GenLoadCsv(string relation, CsvOptions options)
        {
            var builder = new StringBuilder();
            GenSchemaConfig(builder, options);
            GenSyntaxConfig(builder, options);
            builder.Append("\n def config:data = data\n");
            builder.AppendFormat("def insert:{0} = load_csv[config]\n", relation);

            return builder.ToString();
        }
        public TransactionResult LoadCsv(
            string database,
            string engine,
            string relation,
            string data,
            CsvOptions options = null)
        {
            var source = GenLoadCsv(relation, options);
            var inputs = new Dictionary<string, string>();
            inputs.Add("data", data);
            return ExecuteV1(database, engine, source, false, inputs);
        }

        public Database CloneDatabase(
            string database,
            string engine,
            string source,
            bool overwrite = false)
        {
            var mode = CreateMode(source, overwrite);
            var tx = new Transaction(this.context.Region, database, engine, mode, false, source);
            string resp = this.rest.Post(this.MakeUrl(Client.PathTransaction), tx.Payload(null), null, tx.QueryParams()) as string;
            return this.GetDatabase(database);
        }

        private static bool IsTerminalState(string state, string targetState)
        {
            return state == "FAILED" || state == targetState;
        }

        private static string FormatResponse(string response, string key = null)
        {
            try
            {
                // to return the formatted JSON
                var json = JObject.Parse(response);
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

            return response;
        }

        private string GetResource(string path, string key = null, Dictionary<string, string> parameters = null)
        {
            var url = this.MakeUrl(path);
            var resp = this.rest.Get(url, null, null, parameters) as string;
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
            var resp = this.rest.Get(url, null, null, parameters) as string;
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