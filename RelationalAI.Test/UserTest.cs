using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class UserTest : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string UserEmail = $"csharp-sdk-{Uuid}@example.com";

        private readonly ITestOutputHelper testOutput;

        public UserTest(ITestOutputHelper output)
        {
            testOutput = output;
        }

        [Fact]
        public async Task TestUser()
        {
            var client = CreateClient();

            testOutput.WriteLine($"user email: {UserEmail}");

            await Assert.ThrowsAsync<HttpError>(async () => await client.FindUserAsync(UserEmail));

            var rsp = await client.CreateUserAsync(UserEmail);
            Assert.Equal(UserEmail, rsp.Email);
            Assert.Equal(UserStatus.Active, rsp.Status);
            Assert.True(new List<Role> { Role.User }.SequenceEqual(rsp.Roles));

            var user = await client.GetUserAsync(rsp.Id);
            var userId = user.Id;
            Assert.Equal(user.Id, rsp.Id);
            Assert.Equal(UserEmail, user.Email);

            rsp = await client.GetUserAsync(userId);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserEmail, rsp.Email);

            rsp = await client.DisableUserAsync(userId);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserStatus.InActive, rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserStatus.InActive, rsp.Status);

            rsp = await client.UpdateUserAsync(userId, UserStatus.Active);
            Assert.Equal(userId, rsp.Id);
            Assert.Equal(UserStatus.Active, rsp.Status);

            rsp = await client.UpdateUserAsync(userId, roles: new List<Role> { Role.Admin, Role.User });
            Assert.Equal(userId, rsp.Id);
            Assert.True(new List<Role> { Role.Admin, Role.User }.SequenceEqual(rsp.Roles));

            rsp = await client.UpdateUserAsync(userId, UserStatus.InActive, new List<Role> { Role.User });
            Assert.Equal(userId, rsp.Id);
            Assert.True(new List<Role> { Role.User }.SequenceEqual(rsp.Roles));

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
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
