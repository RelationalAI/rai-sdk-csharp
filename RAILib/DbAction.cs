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

namespace RAILib
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    // Represents a "database action", which is an argument to a transaction.
    public class DbAction : Dictionary<string, object>
    {
        public DbAction() { }

        public DbAction(string type)
        {
            this.Add("type", type);
        }

        // Wrapps each of the given action in a LabeledAction.
        public static List<DbAction> MakeActions(List<DbAction> actions)
        {
            int ix = 0;
            var result = new List<DbAction>();
            if (actions != null)
            {
                foreach (var action in actions)
                {
                    var item = new DbAction("LabeledAction");
                    item.Add("name", string.Format("action{0}", ix++));
                    item.Add("action", action);
                    result.Add(item);
                }
            }

            return result;
        }

        private static DbAction MakeDeleteModelAction(string name)
        {
            return MakeDeleteModelsAction(new string[] { name });
        }

        private static DbAction MakeDeleteModelsAction(string[] names)
        {
            var result = new DbAction("ModifyWorkspaceAction");
            result.Add("delete_source", names);
            return result;
        }

        // Return a DbAction for installing the single named model. 
        private static DbAction MakeInstallAction(string name, string model)
        {
            var result = new DbAction("InstallAction");
            result.Add("sources", new DbAction[] { MakeQuerySource(name, model) });
            return result;
        }

        // Return a DbAction for isntalling the set of name => model pairs.
        private static DbAction MakeInstallAction(Dictionary<string, string> models)
        {
            int i = 0;
            var size = models.Count;
            var sources = new DbAction[size];
            foreach (var entry in models)
            {
                sources[i++] = MakeQuerySource(entry.Key, entry.Value);
            }
            var result = new DbAction("InstallAction");
            result.Add("sources", sources);
            return result;
        }

        private static DbAction MakeListModelsAction()
        {
            return new DbAction("ListSourceAction");
        }

        private static DbAction MakeListEdbAction()
        {
            return new DbAction("ListEdbAction");
        }

        private static DbAction MakeRelKey(string name, string key)
        {
            string[] keys = { key };
            string[] values = { };
            var result = new DbAction("RelKey");
            result.Add("name", name);
            result.Add("keys", keys);
            result.Add("value", values);
            return result;
        }

        public static DbAction MakeQueryAction(string source, Dictionary<string, string> inputs)
        {
            var actionInputs = new List<DbAction>();
            if (inputs != null)
            {
                foreach (var entry in inputs)
                {
                    var actionInput = MakeQueryActionInput(entry.Key, entry.Value);
                    actionInputs.Add(actionInput);
                }
            }
            string[] empty = { };
            var result = new DbAction("QueryAction");
            result.Add("source", MakeQuerySource("query", source));
            result.Add("inputs", actionInputs);
            result.Add("outputs", empty);
            result.Add("persist", empty);
            return result;
        }

        private static DbAction MakeQueryActionInput(string name, string value)
        {
            string[,] columns = { { value } };
            var typename = Reltype(value);
            var result = new DbAction("Relation");
            result.Add("columns", columns);
            result.Add("rel_key", MakeRelKey(name, typename));
            return result;
        }

        private static DbAction MakeQuerySource(string name, string model)
        {
            var result = new DbAction("Source");
            result.Add("name", name);
            result.Add("path", "");
            result.Add("value", model);
            return result;
        }

        private static string Reltype(object value)
        {
            if (value is string)
            {
                return "RAI_VariableSizeStrings.VariableSizeString";
            }

            throw new ArgumentException("bad query input type");
        }
    }
}
