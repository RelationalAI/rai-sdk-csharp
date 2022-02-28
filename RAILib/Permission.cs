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
    public enum Permission
    {
        CreateAccessKey,
        ListAccessKeys,
        ReadCreditsUsage,
        CreateDatabase,
        DeleteDatabase,
        ListDatabases,
        UpdateDatabase,
        CreateCompute,
        DeleteCompute,
        ListComputes,
        ReadCompute,
        CreateOAuthClient,
        DeleteOAuthClient,
        ListOAuthClient,
        ReadOAuthClient,
        RotateOAuthClientSecret,
        UpdateOAuthClient,
        ListPermissions,
        ListRoles,
        ReadRole,
        RunTransaction,
        ReadTransaction,
        DeleteTransaction,
        CreateUser,
        ListUsers,
        ReadUser,
        UpdateUser,
    }

    static class Permissions
    {

        public static string Value(this Permission permission)
        {
            switch (permission)
            {
                case Permission.CreateAccessKey:
                    return "create:accesskey";
                case Permission.ListAccessKeys:
                    return "list:accesskey";
                case Permission.ReadCreditsUsage:
                    return "read:credits_usage";
                case Permission.CreateDatabase:
                    return "create:database";
                case Permission.DeleteDatabase:
                    return "delete:database"; ;
                case Permission.ListDatabases:
                    return "list:database"; ;
                case Permission.UpdateDatabase:
                    return "update:database"; ;
                case Permission.CreateCompute:
                    return "create:compute";
                case Permission.DeleteCompute:
                    return "delete:compute";
                case Permission.ListComputes:
                    return "list:compute";
                case Permission.ReadCompute:
                    return "read:compute";
                case Permission.CreateOAuthClient:
                    return "create:oauth_client";
                case Permission.DeleteOAuthClient:
                    return "delete:oauth_client";
                case Permission.ListOAuthClient:
                    return "list:oauth_client";
                case Permission.ReadOAuthClient:
                    return "read:oauth_client";
                case Permission.RotateOAuthClientSecret:
                    return "rotate:oauth_client";
                case Permission.UpdateOAuthClient:
                    return "update:oauth_client";
                case Permission.ListPermissions:
                    return "list:permission";
                case Permission.ListRoles:
                    return "list:role";
                case Permission.ReadRole:
                    return "read:role";
                case Permission.RunTransaction:
                    return "run:transaction";
                case Permission.ReadTransaction:
                    return "read:transaction";
                case Permission.DeleteTransaction:
                    return "delete:transaction";
                case Permission.CreateUser:
                    return "create:user";
                case Permission.ListUsers:
                    return "list:user";
                case Permission.ReadUser:
                    return "read:user";
                case Permission.UpdateUser:
                    return "update:user";
                default:
                    throw new ArgumentException(string.Format("permission '{0}' not supported", permission));
            }
        }
    }
}