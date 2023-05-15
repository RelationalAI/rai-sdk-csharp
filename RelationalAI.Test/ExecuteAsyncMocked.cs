using System;
using System.Collections.Generic;
using System.IO;
using Moq;
using Relationalai.Protocol;
using Xunit;
using System.Threading.Tasks;
using FluentAssertions;
using Xunit.Abstractions;
using System.Net.Http;
using System.Threading;
using Moq.Protected;
using System.Net;

namespace RelationalAI.Test
{
    [Collection("RelationalAI.Test")]
    public class ExecuteAsyncMockedTests : UnitTest
    {

        public static string Uuid = Guid.NewGuid().ToString();
        public static string Dbname = $"csharp-sdk-{Uuid}";

        public ExecuteAsyncMockedTests(ITestOutputHelper output) : base(output)
        { }

        [Fact]
        public async Task ExecuteAsyncTest()
        {
            var client = CreateClient();

            var handlerMock = MockHttpResponse();
            client.HttpClient = new HttpClient(handlerMock.Object);

            var query = "x, x^2, x^3, x^4 from x in {1; 2; 3; 4; 5}";
            var rsp = await client.ExecuteWaitAsync("ExecuteAsyncMockedDB", "ExecuteAsyncMockedEngine", query, true);

            var results = new List<ArrowRelation>
            {
                new ArrowRelation("", new List<object> {1L, 2L, 3L, 4L, 5L} ),
                new ArrowRelation("", new List<object> {1L, 4L, 9L, 16L, 25L} ),
                new ArrowRelation("", new List<object> {1L, 8L, 27L, 64L, 125L} ),
                new ArrowRelation("", new List<object> {1L, 16L, 81L, 256L, 625L} )
            };

            var metadata = MetadataInfo.Parser.ParseFrom(File.ReadAllBytes("metadata.pb"));

            var problems = new List<object>();

            rsp.Results.Should().Equal(results);
            rsp.Metadata.ToString().Should().Be(metadata.ToString());
            rsp.Problems.Should().Equal(problems);
        }

        public Mock<HttpMessageHandler> MockHttpResponse()
        {
            var mockResponse = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new ByteArrayContent(File.ReadAllBytes("multipart.data"))
            };

            mockResponse.Content.Headers.ContentType =
                new System.Net.Http.Headers.MediaTypeHeaderValue("multipart/form-data");

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(mockResponse)
                .Verifiable();

            return handlerMock;
        }
    }
}
