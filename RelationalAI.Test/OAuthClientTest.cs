using System;
using Xunit;

namespace RelationalAI.Test
{
    public class OAuthClientTests : UnitTest
    {
        [Fact]
        public void OAuthClientTest()
        {
            Client client = CreateClient();

            Assert.Throws<SystemException>( () => client.FindOAuthClient(OAuthClientName) );

            var rsp = client.CreateOAuthClient(OAuthClientName);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clientId = rsp.ID;

            rsp = client.FindOAuthClient(OAuthClientName);
            Assert.Equal(clientId, rsp.ID);
            Assert.Equal(OAuthClientName, rsp.Name);

            rsp = client.GetOAuthClient(clientId);
            Assert.Equal(clientId, rsp.ID);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clients = client.ListOAuthClients();
            var found = clients.Find( item => item.ID == clientId );
            Assert.NotNull(found);
            Assert.Equal(clientId, found.ID);
            Assert.Equal(OAuthClientName, found.Name);

            var deleteRsp = client.DeleteOAuthClient(clientId);
            Assert.Equal(clientId, deleteRsp.ID);

            Assert.Throws<SystemException>( () => client.FindOAuthClient(OAuthClientName) );
        }
    }
}
