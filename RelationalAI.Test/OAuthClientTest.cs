using System;
using System.Threading.Tasks;
using Xunit;

namespace RelationalAI.Test
{
    public class OAuthClientTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string OAuthClientName = $"csharp-sdk-{Uuid}";
        
        [Fact]
        public async Task OAuthClientTest()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.FindOAuthClientAsync(OAuthClientName));

            var rsp = await client.CreateOAuthClientAsync(OAuthClientName);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clientId = rsp.Id;

            rsp = await client.FindOAuthClientAsync(OAuthClientName);
            Assert.Equal(clientId, rsp.Id);
            Assert.Equal(OAuthClientName, rsp.Name);

            rsp = await client.GetOAuthClientAsync(clientId);
            Assert.Equal(clientId, rsp.Id);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clients = await client.ListOAuthClientsAsync();
            var found = clients.Find(item => item.Id == clientId);
            Assert.NotNull(found);
            Assert.Equal(clientId, found.Id);
            Assert.Equal(OAuthClientName, found.Name);

            var deleteRsp = await client.DeleteOAuthClientAsync(clientId);
            Assert.Equal(clientId, deleteRsp.Id);

            await Assert.ThrowsAsync<NotFoundException>(async () => await client.FindOAuthClientAsync(OAuthClientName));
        }

        public override async Task DisposeAsync()
        {
            var client = CreateClient();

            try
            {
                var oauthClient = await client.FindOAuthClientAsync(OAuthClientName);
                await client.DeleteOAuthClientAsync(oauthClient.Id);
            }
            catch (Exception e)
            {
                await Console.Error.WriteLineAsync(e.ToString());
            }
        }
    }
}
