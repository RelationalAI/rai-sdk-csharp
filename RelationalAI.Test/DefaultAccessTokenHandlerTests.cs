using System;
using System.Reflection;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class DefaultAccessTokenHandlerTests : UnitTest
    {
        [Fact]
        public async Task DefaultAccessTokenHandlerTestAsync()
        {
            var accessToken = new AccessToken("test_access_token", new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds(), 60, "test_access_token_scope");
            var creds = new ClientCredentials("test_client_id", "test_client_secret")
            {
                AccessToken = accessToken
            };

            var accessTokenHandler = new DefaultAccessTokenHandler(null, "test_tokens.json");

            // Get Private methods using System.Reflection
            MethodInfo writeAccessTokenAsync = accessTokenHandler.GetType().GetMethod("WriteAccessTokenAsync", BindingFlags.NonPublic | BindingFlags.Instance);
            writeAccessTokenAsync.Invoke(accessTokenHandler, new object[2] { creds, creds.AccessToken });

            var token = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().BeEquivalentTo(creds.AccessToken);
        }
    }
}