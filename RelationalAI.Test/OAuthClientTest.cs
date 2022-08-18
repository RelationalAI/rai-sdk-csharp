using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class OAuthClientTests : UnitTest
    {
        public static string UUID = Guid.NewGuid().ToString();
        public static string OAuthClientName = $"csharp-sdk-{UUID}";
        [Fact]
        public async Task OAuthClientTest()
        {
            Client client = CreateClient();

            await Assert.ThrowsAsync<SystemException>(async () => await client.FindOAuthClientAsync(OAuthClientName));

            var rsp = await client.CreateOAuthClientAsync(OAuthClientName);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clientId = rsp.ID;

            rsp = await client.FindOAuthClientAsync(OAuthClientName);
            Assert.Equal(clientId, rsp.ID);
            Assert.Equal(OAuthClientName, rsp.Name);

            rsp = await client.GetOAuthClientAsync(clientId);
            Assert.Equal(clientId, rsp.ID);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clients = await client.ListOAuthClientsAsync();
            var found = clients.Find(item => item.ID == clientId);
            Assert.NotNull(found);
            Assert.Equal(clientId, found.ID);
            Assert.Equal(OAuthClientName, found.Name);

            var deleteRsp = await client.DeleteOAuthClientAsync(clientId);
            Assert.Equal(clientId, deleteRsp.ID);

            await Assert.ThrowsAsync<SystemException>(async () => await client.FindOAuthClientAsync(OAuthClientName));
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                var oauthClient = await client.FindOAuthClientAsync(OAuthClientName);
                await client.DeleteOAuthClientAsync(oauthClient.ID);
            }
            catch { }
        }
    }
}
