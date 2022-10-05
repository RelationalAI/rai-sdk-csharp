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

namespace RelationalAI
{
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
        UpdateUser
    }

    public static class Permissions
    {
        public static string Value(this Permission permission)
        {
            return permission switch
            {
                Permission.CreateAccessKey => "create:accesskey",
                Permission.ListAccessKeys => "list:accesskey",
                Permission.ReadCreditsUsage => "read:credits_usage",
                Permission.CreateDatabase => "create:database",
                Permission.DeleteDatabase => "delete:database",
                Permission.ListDatabases => "list:database",
                Permission.UpdateDatabase => "update:database",
                Permission.CreateCompute => "create:compute",
                Permission.DeleteCompute => "delete:compute",
                Permission.ListComputes => "list:compute",
                Permission.ReadCompute => "read:compute",
                Permission.CreateOAuthClient => "create:oauth_client",
                Permission.DeleteOAuthClient => "delete:oauth_client",
                Permission.ListOAuthClient => "list:oauth_client",
                Permission.ReadOAuthClient => "read:oauth_client",
                Permission.RotateOAuthClientSecret => "rotate:oauth_client",
                Permission.UpdateOAuthClient => "update:oauth_client",
                Permission.ListPermissions => "list:permission",
                Permission.ListRoles => "list:role",
                Permission.ReadRole => "read:role",
                Permission.RunTransaction => "run:transaction",
                Permission.ReadTransaction => "read:transaction",
                Permission.DeleteTransaction => "delete:transaction",
                Permission.CreateUser => "create:user",
                Permission.ListUsers => "list:user",
                Permission.ReadUser => "read:user",
                Permission.UpdateUser => "update:user",
                _ => throw new ArgumentOutOfRangeException(nameof(permission), permission, "Permission is not supported")
            };
        }
    }
}