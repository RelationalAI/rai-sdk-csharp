using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Xunit;
using Xunit.Abstractions;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class DefaultAccessTokenHandlerTests : UnitTest
    {
        private string TestCachePath()
        {
            var envHome = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "HOMEPATH" : "HOME";
            var home = Environment.GetEnvironmentVariable(envHome);

            return Path.Join(home, ".rai", "test_tokens.json");
        }

        public DefaultAccessTokenHandlerTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task DefaultAccessTokenHandlerTestAsync()
        {
            var ctx = CreateContext(GetConfig());
            var creds = ctx.Credentials as ClientCredentials;
            var rest = new Rest(ctx);

            // should generate token if cache path doesn't exist
            var accessTokenHandler = new DefaultAccessTokenHandler(rest, "/fake/path/tokens.json", _logger);
            var token = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().NotBeNull();
            token.Should().BeEquivalentTo(creds.AccessToken);

            // should generate and cache token if the path exists
            accessTokenHandler = new DefaultAccessTokenHandler(rest, TestCachePath(), _logger);
            token = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().NotBeNull();
            token.Should().BeEquivalentTo(creds.AccessToken);

            accessTokenHandler = new DefaultAccessTokenHandler(null, TestCachePath(), _logger);
            var cachedToken = await accessTokenHandler.GetAccessTokenAsync(creds);
            token.Should().NotBeNull();
            token.Should().BeEquivalentTo(cachedToken);
            token.Should().BeEquivalentTo(creds.AccessToken);
        }
    }
}