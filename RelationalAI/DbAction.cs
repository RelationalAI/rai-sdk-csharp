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

using System;
using System.Collections.Generic;
using System.Linq;

namespace RelationalAI
{
    /// <summary>
    /// Represents a "database action", which is an argument to a transaction.
    /// </summary>
    public class DbAction : Dictionary<string, object>
    {
        public DbAction()
        {
        }

        public DbAction(string type)
        {
            Add("type", type);
        }

        /// <summary>
        /// Wraps each of the given action in a LabeledAction.
        /// </summary>
        /// <param name="actions">The actions to wrap.</param>
        /// <returns>The list of wrapped actions.</returns>
        public static List<DbAction> MakeActions(List<DbAction> actions)
        {
            var ix = 0;
            var result = new List<DbAction>();
            if (actions != null)
            {
                result.AddRange(actions.Select(action => new DbAction("LabeledAction")
                    { { "name", $"action{ix++}" }, { "action", action } }));
            }

            return result;
        }

        public static DbAction MakeDeleteModelAction(string name)
        {
            return MakeDeleteModelsAction(new[] { name });
        }

        /// <summary>
        /// Create a DbAction for installing the single named model.
        /// </summary>
        /// <param name="name">The name of the model to install.</param>
        /// <param name="model">The model to install.</param>
        /// <returns>The install action.</returns>
        public static DbAction MakeInstallAction(string name, string model)
        {
            var result = new DbAction("InstallAction")
            {
                { "sources", new[] { MakeQuerySource(name, model) } }
            };
            return result;
        }

        /// <summary>
        /// Created a DbAction for installing the set of name => model pairs.
        /// </summary>
        /// <param name="models">Dictionary of the named models to install.</param>
        /// <returns>The install action.</returns>
        public static DbAction MakeInstallAction(Dictionary<string, string> models)
        {
            var sources = models.Select(entry => MakeQuerySource(entry.Key, entry.Value)).ToArray();
            var result = new DbAction("InstallAction")
            {
                { "sources", sources }
            };
            return result;
        }

        public static DbAction MakeListModelsAction()
        {
            return new DbAction("ListSourceAction");
        }

        public static DbAction MakeListEdbAction()
        {
            return new DbAction("ListEdbAction");
        }

        public static DbAction MakeQueryAction(string source, Dictionary<string, string> inputs)
        {
            var actionInputs = new List<DbAction>();
            if (inputs != null)
            {
                actionInputs.AddRange(inputs.Select(entry => MakeQueryActionInput(entry.Key, entry.Value)));
            }

            string[] empty = { };
            var result = new DbAction("QueryAction")
            {
                { "source", MakeQuerySource("query", source) },
                { "inputs", actionInputs },
                { "outputs", empty },
                { "persist", empty }
            };
            return result;
        }

        public static DbAction MakeQueryActionInput(string name, string value)
        {
            string[,] columns = { { value } };
            var typename = Reltype(value);
            var result = new DbAction("Relation")
            {
                { "columns", columns },
                { "rel_key", MakeRelKey(name, typename) }
            };
            return result;
        }

        private static DbAction MakeDeleteModelsAction(string[] names)
        {
            var result = new DbAction("ModifyWorkspaceAction")
            {
                { "delete_source", names }
            };
            return result;
        }

        private static DbAction MakeRelKey(string name, string key)
        {
            string[] keys = { key };
            string[] values = { };
            var result = new DbAction("RelKey")
            {
                { "name", name },
                { "keys", keys },
                { "value", values }
            };
            return result;
        }

        private static DbAction MakeQuerySource(string name, string model)
        {
            var result = new DbAction("Source")
            {
                { "name", name },
                { "path", string.Empty },
                { "value", model }
            };
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
