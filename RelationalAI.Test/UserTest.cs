using System;
using Xunit;
using System.Collections.Generic;

namespace RelationalAI.Test
{
    public class UserTest : UnitTest
    {
        [Fact]
        public void TestUser()
        {
            Client client = CreateClient();

            Assert.Throws<SystemException>( () => client.FindUser(UserEmail) );

            var rsp = client.CreateUser(UserEmail);
            Assert.Equal(UserEmail, rsp.Email);
            Assert.Equal("ACTIVE", rsp.Status);
            Assert.Equal( new List<string> {"user"}, rsp.Roles);

            rsp = client.FindUser(UserEmail);
            var userId = rsp.ID;
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(UserEmail, rsp.Email);

            rsp = client.GetUser(userId);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(UserEmail, rsp.Email);

            var users = client.ListUsers();
            var user = users.Find( user => user.ID == userId);
            Assert.Equal(userId, user.ID);
            Assert.Equal(UserEmail, user.Email);

            rsp = client.DisableUser(userId);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = client.UpdateUser(userId, UserStatus.InActive);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("INACTIVE", rsp.Status);

            rsp = client.UpdateUser(userId, UserStatus.Active);
            Assert.Equal(userId, rsp.ID);
            Assert.Equal("ACTIVE", rsp.Status);

            rsp = client.UpdateUser(userId, roles: new List<Role> {Role.Admin, Role.User});
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(new List<string> {"admin", "user"}, rsp.Roles);

            rsp = client.UpdateUser(userId, UserStatus.InActive, new List<Role>{Role.User});
            Assert.Equal(userId, rsp.ID);
            Assert.Equal(new List<string>{"user"}, rsp.Roles);

            // cleanup
            var deleteRsp = client.DeleteUser(userId);
            Assert.Equal(userId, rsp.ID);
        }
    }
}
