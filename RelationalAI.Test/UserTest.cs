using System;
using Xunit;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RelationalAI.Test
{
    public class UserTest : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string UserEmail = $"csharp-sdk-{UUID}@example.com";

        [Fact]
        public async Task TestUser()
        {
            Client client = CreateClient();

            await Assert.ThrowsAsync<SystemException>(async () => await client.FindUserAsync(UserEmail) );

            var rsp = await client.CreateUserAsync(UserEmail);
            Assert.Equal(UserEmail, rsp.Email);
            Assert.Equal("ACTIVE", rsp.Status);
            Assert.Equal( new List<string> {"user"}, rsp.Roles);

            rsp = await client.FindUserAsync(UserEmail);
            var userId = rsp.ID;
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(UserEmail, rsp.Email);

            rsp = await client.GetUserAsync(userId);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(UserEmail, rsp.Email);

            var users = await client.ListUsersAsync();
            var user = users.Find( user => user.ID == userId);
            Assert.Equal(userId, user.ID);
            Assert.Equal(UserEmail, user.Email);

            rsp = await client.DisableUserAsync(userId);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.Active);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("ACTIVE", rsp.Status);

            rsp = await client.UpdateUserAsync(userId, roles: new List<Role> {Role.Admin, Role.User});
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(new List<string> {"admin", "user"}, rsp.Roles);

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive, new List<Role>{Role.User});
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(new List<string>{"user"}, rsp.Roles);

            // cleanup
            var deleteRsp = await client.DeleteUserAsync(userId);
            Assert.Equal(userId, rsp.ID);
        }

        public override void Dispose()
        {
            var client = CreateClient();

            try { client.DeleteUserAsync(client.FindUserAsync(UserEmail).Result.ID).Wait(); } catch {}
        }
    }
}
