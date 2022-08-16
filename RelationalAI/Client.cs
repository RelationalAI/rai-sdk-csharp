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
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Polly;
    using RelationalAI.Credentials;
    using RelationalAI.Utils;

    public class Client
    {
        private static readonly string PathEngine = "/compute";
        private static readonly string PathDatabase = "/database";
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

        public Task<Database> CreateDatabaseAsync(string database, string engine)
        {
            return this.CreateDatabaseAsync(database, engine, false);
        }

        public async Task<Database> CreateDatabaseAsync(string database, string engine, bool overwrite)
        {
            var mode = CreateMode(null, overwrite);
            var transaction = new Transaction(this.context.Region, database, engine, mode);
            await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), transaction.Payload(null), null, transaction.QueryParams());
            return await this.GetDatabaseAsync(database);
        }

        public async Task<Database> GetDatabaseAsync(string database)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "name", database },
            };

            string resp = await this.GetResourceAsync(Client.PathDatabase, null, parameters);
            List<Database> dbs = Json<GetDatabaseResponse>.Deserialize(resp).Databases;
            return dbs.Count > 0 ? dbs[0] : throw new SystemException("not found");
        }

        public async Task<List<Database>> ListDatabasesAsync(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            string resp = await this.ListCollectionsAsync(Client.PathDatabase, null, parameters);
            return Json<ListDatabasesResponse>.Deserialize(resp).Databases;
        }

        public async Task<DeleteDatabaseResponse> DeleteDatabaseAsync(string database)
        {
            var data = new Dictionary<string, string>()
            {
                { "name", database },
            };
            string resp = await this.rest.DeleteAsync(this.MakeUrl(Client.PathDatabase), data) as string;
            return Json<DeleteDatabaseResponse>.Deserialize(resp);
        }

        public async Task<Engine> CreateEngineAsync(string engine, EngineSize size = EngineSize.XS)
        {
            var data = new Dictionary<string, string>()
            {
                { "region", this.context.Region },
                { "name", engine },
                { "size", size.ToString() },
            };
            string resp = await this.rest.PutAsync(this.MakeUrl(Client.PathEngine), data) as string;
            return Json<CreateEngineResponse>.Deserialize(resp).Engine;
        }

        public async Task<Engine> CreateEngineWaitAsync(string engine, EngineSize size = EngineSize.XS)
        {
            await CreateEngineAsync(engine, size);
            var resp = await Policy
                    .HandleResult<Engine>(e => !IsTerminalState(e.State, "PROVISIONED"))
                    .Retry30Min()
                    .ExecuteAsync(() => GetEngineAsync(engine));

            if (resp.State != "PROVISIONED")
            {
                // TODO: replace with a better error during introducing the exceptions hierarchy
                throw new SystemException("Failed to provision engine");
            }

            return resp;
        }

        public async Task<Engine> GetEngineAsync(string engine)
        {
            var parameters = new Dictionary<string, string>()
            {
                { "name", engine },
                { "deleted_on", string.Empty },
            };
            var resp = await this.GetResourceAsync(Client.PathEngine, null, parameters);
            List<Engine> engines = Json<GetEngineResponse>.Deserialize(resp).Engines;
            return engines.Count > 0 ? engines[0] : throw new SystemException("not found");
        }

        public async Task<List<Engine>> ListEnginesAsync(string state = null)
        {
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (state != null)
            {
                parameters.Add("state", state);
            }

            string resp = await this.ListCollectionsAsync(Client.PathEngine, null, parameters);
            return Json<ListEnginesResponse>.Deserialize(resp).Engines;
        }

        public async Task<DeleteEngineResponse> DeleteEngineAsync(string engine)
        {
            var data = new Dictionary<string, string>()
            {
                { "name", engine },
            };
            string resp = await this.rest.DeleteAsync(this.MakeUrl(Client.PathEngine), data) as string;
            return Json<DeleteEngineResponse>.Deserialize(resp);
        }

        public async Task<DeleteEngineResponse> DeleteEngineWaitAsync(string engine)
        {
            var resp = await DeleteEngineAsync(engine);
            var engineResponse = await Policy
                .HandleResult<Engine>(e => !IsTerminalState(e.State, "DELETED"))
                .Retry15Min()
                .ExecuteAsync(() => GetEngineAsync(engine));
            resp.Status.State = engineResponse.State;
            return resp;
        }

        public async Task<OAuthClient> CreateOAuthClientAsync(string name, List<Permission> permissions = null)
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
            string resp = await this.rest.PostAsync(this.MakeUrl(Client.PathOAuthClients), data) as string;
            return Json<CreateOAuthClientResponse>.Deserialize(resp).OAuthClient;
        }

        public async Task<OAuthClient> FindOAuthClientAsync(string name)
        {
            List<OAuthClient> clients = await this.ListOAuthClientsAsync();
            foreach (var client in clients)
            {
                if (client.Name == name)
                {
                    return client;
                }
            }

            throw new SystemException("not found");
        }

        public async Task<OAuthClientEx> GetOAuthClientAsync(string id)
        {
            string resp = await this.GetResourceAsync(string.Format("{0}/{1}", Client.PathOAuthClients, id));
            return Json<GetOAuthClientResponse>.Deserialize(resp).Client;
        }

        public async Task<List<OAuthClient>> ListOAuthClientsAsync()
        {
            string resp = await this.ListCollectionsAsync(Client.PathOAuthClients);
            return Json<ListOAuthClientResponse>.Deserialize(resp).Clients;
        }

        public async Task<DeleteOAuthClientResponse> DeleteOAuthClientAsync(string id)
        {
            string resp = await this.rest.DeleteAsync(this.MakeUrl(string.Format("{0}/{1}", Client.PathOAuthClients, id))) as string;
            return Json<DeleteOAuthClientResponse>.Deserialize(resp);
        }

        public async Task<User> CreateUserAsync(string email, List<Role> roles = null)
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
            string resp = await this.rest.PostAsync(this.MakeUrl(Client.PathUsers), data) as string;
            return Json<CreateUserResponse>.Deserialize(resp).User;
        }

        public async Task<User> UpdateUserAsync(string id, UserStatus status = UserStatus.None, List<Role> roles = null)
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

            string resp = await this.rest.PatchAsync(this.MakeUrl(string.Format("{0}/{1}", Client.PathUsers, id)), data) as string;
            return Json<UpdateUserResponse>.Deserialize(resp).User;
        }

        public async Task<User> FindUserAsync(string email)
        {
            List<User> users = await this.ListUsersAsync();
            foreach (var user in users)
            {
                if (user.Email == email)
                {
                    return user;
                }
            }

            throw new SystemException("not found");
        }

        public async Task<User> GetUserAsync(string userId)
        {
            string resp = await this.GetResourceAsync(string.Format("{0}/{1}", Client.PathUsers, userId));
            return Json<GetUserResponse>.Deserialize(resp).User;
        }

        public async Task<List<User>> ListUsersAsync()
        {
            string resp = await this.ListCollectionsAsync(Client.PathUsers);
            return Json<ListUsersResponse>.Deserialize(resp).Users;
        }

        public async Task<DeleteUserResponse> DeleteUserAsync(string id)
        {
            string resp = await this.rest.DeleteAsync(this.MakeUrl(string.Format("{0}/{1}", Client.PathUsers, id))) as string;
            return Json<DeleteUserResponse>.Deserialize(resp);
        }

        public Task<User> DisableUserAsync(string id)
        {
            return this.UpdateUserAsync(id, UserStatus.InActive);
        }

        public Task<User> EnableUserAsync(string id)
        {
            return this.UpdateUserAsync(id, UserStatus.Active);
        }

        public async Task<TransactionsAsyncMultipleResponses> GetTransactionsAsync()
        {
            var rsp = await this.rest.GetAsync(this.MakeUrl(Client.PathTransactions)) as string;
            return Json<TransactionsAsyncMultipleResponses>.Deserialize(rsp);
        }

        public async Task<TransactionAsyncSingleResponse> GetTransactionAsync(string id)
        {
            var rsp = await this.rest.GetAsync(this.MakeUrl(string.Format("{0}/{1}", Client.PathTransactions, id))) as string;
            return Json<TransactionAsyncSingleResponse>.Deserialize(rsp);
        }

        public async Task<List<ArrowRelation>> GetTransactionResultsAsync(string id)
        {
            var files = await this.rest.GetAsync(this.MakeUrl(string.Format("{0}/{1}/results", Client.PathTransactions, id))) as List<TransactionAsyncFile>;
            return this.rest.ReadArrowFiles(files);
        }

        public async Task<List<TransactionAsyncMetadataResponse>> GetTransactionMetadataAsync(string id)
        {
            var rsp = await this.rest.GetAsync(this.MakeUrl(string.Format("{0}/{1}/metadata", Client.PathTransactions, id))) as string;
            return Json<List<TransactionAsyncMetadataResponse>>.Deserialize(rsp);
        }

        public async Task<List<object>> GetTransactionProblemsAsync(string id)
        {
            var rsp = await this.rest.GetAsync(this.MakeUrl(string.Format("{0}/{1}/problems", Client.PathTransactions, id))) as string;
            return ParseProblemsResult(rsp);
        }

        public async Task<TransactionAsyncCancelResponse> CancelTransactionAsync(string id)
        {
            var rsp = await this.rest.PostAsync(this.MakeUrl(string.Format("{0}/{1}/cancel", Client.PathTransactions, id)), new Dictionary<string, object>()) as string;
            return Json<TransactionAsyncCancelResponse>.Deserialize(rsp);
        }

        private List<object> ParseProblemsResult(string rsp)
        {
            var output = new List<object>();

            var problems = JsonConvert.DeserializeObject(rsp);
            foreach(var problem in problems as JArray)
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

        public async Task<string> DeleteTransactionAsync(string id)
        {
            var resp = await this.rest.DeleteAsync(this.MakeUrl(string.Format("{0}/{1}", Client.PathTransactions, id))) as string;
            return FormatResponse(resp);
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

        public async Task<List<Edb>> ListEdbsAsync(string database, string engine)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeListEdbAction() };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            var actionsResp = Json<ListEdbsResponse>.Deserialize(resp).actions;
            if (actionsResp.Count == 0)
                return new List<Edb>();
            return actionsResp[0].result.rels;
        }

        public async Task<TransactionResult> LoadModelAsync(
            string database,
            string engine,
            string name,
            string model)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", false);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeInstallAction(name, model) };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public async Task<TransactionResult> LoadModelsAsync(
            string database,
            string engine,
            Dictionary<string, string> models)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", false);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeInstallAction(models) };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public async Task<List<Model>> ListModelsAsync(string database, string engine)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeListModelsAction() };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            var actionsResp = Json<ListModelsResponse>.Deserialize(resp).actions;
            if (actionsResp.Count == 0)
                return new List<Model>();
            return actionsResp[0].result.models;
        }

        public async Task<List<string>> ListModelNamesAsync(string database, string engine)
        {
            var models = await ListModelsAsync(database, engine);
            List<string> result = new List<string>();
            for (var i = 0; i < models.Count; i++)
                result.Add(models[i].Name);
            return result;
        }

        public async Task<Model> GetModelAsync(string database, string engine, string name)
        {
            var models = await ListModelsAsync(database, engine);
            foreach (var model in models)
                if (model.Name.Equals(name))
                    return model;
            throw new SystemException($"model {name} not found.");
        }

        public async Task<TransactionResult> DeleteModelAsync(string database, string engine, string name)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN");
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeDeleteModelAction(name) };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        // Query
        public async Task<TransactionResult> ExecuteV1Async(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var tx = new Transaction(this.context.Region, database, engine, "OPEN", readOnly);
            List<DbAction> actions = new List<DbAction>() { DbAction.MakeQueryAction(source, inputs) };
            var body = tx.Payload(actions);
            var resp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), body, null, tx.QueryParams()) as string;
            return Json<TransactionResult>.Deserialize(resp);
        }

        public async Task<TransactionAsyncResult> ExecuteWaitAsync(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var rsp = await ExecuteAsync(database, engine, source, readOnly, inputs);
            var id = rsp.Transaction.ID;

            // fast-path
            if (rsp.GotCompleteResult)
                return rsp;

            // slow-path
            var transactionResponse = await Policy
                .HandleResult<TransactionAsyncSingleResponse>(r =>
                    !(r.Transaction.State.Equals("COMPLETED") || r.Transaction.State.Equals("ABORTED")))
                .RetryForeverWithBoundedDelay()
                .ExecuteAsync(() => GetTransactionAsync(id));

            var transaction = transactionResponse.Transaction;
            var results = await GetTransactionResultsAsync(id);
            var metadata = await GetTransactionMetadataAsync(id);
            var problems = await GetTransactionProblemsAsync(id);

            return new TransactionAsyncResult(
                transaction,
                results,
                metadata,
                problems,
                true
            );
        }

        public async Task<TransactionAsyncResult> ExecuteAsync(
            string database,
            string engine,
            string source,
            bool readOnly = false,
            Dictionary<string, string> inputs = null)
        {
            var tx = new TransactionAsync(database, engine, readOnly, source, inputs);
            var body = tx.Payload();
            var rsp = await this.rest.PostAsync(this.MakeUrl(Client.PathTransactions), body, null, tx.QueryParams());

            if (rsp is string)
            {
                var txn = Json<TransactionAsyncCompactResponse>.Deserialize(rsp as string);
                return new TransactionAsyncResult(txn, new List<ArrowRelation>(), new List<TransactionAsyncMetadataResponse>(), new List<object>());
            }

            return ReadTransactionAsyncResults(rsp as List<TransactionAsyncFile>);
        }

        private TransactionAsyncResult ReadTransactionAsyncResults(List<TransactionAsyncFile> files)
        {
            var transaction = files.Find(f => f.Name == "transaction");
            var metadata = files.Find(f => f.Name == "metadata");
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

            List<object> problemsResult = null;
            if (problems != null)
            {
                problemsResult = ParseProblemsResult(this.rest.ReadString(problems.Data));
            }

            var results = this.rest.ReadArrowFiles(files);

            return new TransactionAsyncResult(
                transactionResult,
                results,
                metadataResult,
                problemsResult,
                true
            );
        }

        private string GenLoadJson(string relation)
        {
            var builder = new StringBuilder();
            builder.Append("\ndef config:data = data\n");
            builder.AppendFormat("def insert:{0} = load_json[config]\n", relation);
            return builder.ToString();
        }

        public Task<TransactionResult> LoadJsonAsync(
            string database,
            string engine,
            string relation,
            string data)
        {
            var inputs = new Dictionary<string, string>();
            inputs.Add("data", data);
            var source = GenLoadJson(relation);
            return ExecuteV1Async(database, engine, source, false, inputs);
        }

        private void GenSchemaConfig(StringBuilder builder, CsvOptions options)
        {
            if (options == null)
                return;
            var schema = options.Schema;

            if (schema == null)
                return;

            var isEmpty = true;

            foreach(var entry in schema)
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

        public Task<TransactionResult> LoadCsvAsync(
            string database,
            string engine,
            string relation,
            string data,
            CsvOptions options = null)
        {
            var source = GenLoadCsv(relation, options);
            var inputs = new Dictionary<string, string>();
            inputs.Add("data", data);
            return ExecuteV1Async(database, engine, source, false, inputs);
        }

        public async Task<Database> CloneDatabaseAsync(
            string database,
            string engine,
            string source,
            bool overwrite = false)
        {
            var mode = CreateMode(source, overwrite);
            var tx = new Transaction(this.context.Region, database, engine, mode, false, source);
            await this.rest.PostAsync(this.MakeUrl(Client.PathTransaction), tx.Payload(null), null, tx.QueryParams());
            return await this.GetDatabaseAsync(database);
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

        private async Task<string> GetResourceAsync(string path, string key = null, Dictionary<string, string> parameters = null)
        {
            var url = this.MakeUrl(path);
            var resp = await this.rest.GetAsync(url, null, null, parameters) as string;
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

        private async Task<string> ListCollectionsAsync(string path, string key = null, Dictionary<string, string> parameters = null)
        {
            var url = this.MakeUrl(path);
            var resp = await this.rest.GetAsync(url, null, null, parameters) as string;
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