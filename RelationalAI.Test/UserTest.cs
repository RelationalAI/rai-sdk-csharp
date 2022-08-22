using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RelationalAI.Models.User;
using Xunit;

namespace RelationalAI.Test
{
    public class UserTest : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string UserEmail = $"csharp-sdk-{Uuid}@example.com";

        [Fact]
        public async Task TestUser()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<SystemException>(async () => await client.FindUserAsync(UserEmail));

            var rsp = await client.CreateUserAsync(UserEmail);
            Assert.Equal(UserEmail, rsp.Email);
            Assert.Equal("ACTIVE", rsp.Status);
            Assert.Equal(new List<string> { "user" }, rsp.Roles);

            rsp = await client.FindUserAsync(UserEmail);
            var userId = rsp.Id;
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserEmail, rsp.Email);

            rsp = await client.GetUserAsync(userId);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserEmail, rsp.Email);

            var users = await client.ListUsersAsync();
            var user = users.Find(user => user.Id == userId);
            Assert.Equal(userId, user.Id);
            Assert.Equal(UserEmail, user.Email);

            rsp = await client.DisableUserAsync(userId);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.Active);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal("ACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, roles: new List<Role> { Role.Admin, Role.User });
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(new List<string> { "admin", "user" }, rsp.Roles);

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive, new List<Role> { Role.User });
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(new List<string> { "user" }, rsp.Roles);

            // cleanup
            await client.DeleteUserAsync(userId);
            Assert.Equal(userId, rsp.Id);
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                var oauthClient = await client.FindUserAsync(UserEmail);
                await client.DeleteUserAsync(oauthClient.Id);
            }
            catch { }
        }
    }
}
