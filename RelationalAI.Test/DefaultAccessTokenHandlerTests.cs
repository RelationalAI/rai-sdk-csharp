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
            var ctx = CreateContext(GetConfig());
            var creds = ctx.Credentials as ClientCredentials;
            var rest = new Rest(ctx, null);

            var accessTokenHandler = new DefaultAccessTokenHandler(rest, "test_tokens.json");
            var token = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().BeEquivalentTo(creds.AccessToken);

            // check if the cached token is the same as the fetched token
            // set rest to null to force fetching the cached token
            accessTokenHandler = new DefaultAccessTokenHandler(null, "test_tokens.json");
            var cachedToken = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().BeEquivalentTo(cachedToken);
            token.Should().BeEquivalentTo(creds.AccessToken);
        }
    }
}