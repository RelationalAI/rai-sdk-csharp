using System;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    public class OAuthClientTests : UnitTest
    {
        public static string Uuid = Guid.NewGuid().ToString();
        public static string OAuthClientName = $"csharp-sdk-{Uuid}";
        private readonly ITestOutputHelper outputHelper;

        public OAuthClientTests(ITestOutputHelper output)
        {
            outputHelper = output;
        }

        [Fact]
        public async Task OAuthClientTest()
        {
            var client = CreateClient();

            await Assert.ThrowsAsync<HttpError>(async () => await client.FindOAuthClientAsync(OAuthClientName));

            var rsp = await client.CreateOAuthClientAsync(OAuthClientName);
            Assert.Equal(OAuthClientName, rsp.Name);

            var clientId = rsp.Id;

            rsp = await client.GetOAuthClientAsync(clientId);
            Assert.Equal(clientId, rsp.Id);
            Assert.Equal(OAuthClientName, rsp.Name);

            var deleteRsp = await client.DeleteOAuthClientAsync(clientId);
            Assert.Equal(clientId, deleteRsp.Id);

            await Assert.ThrowsAsync<HttpError>(async () => await client.GetOAuthClientAsync(clientId));
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
