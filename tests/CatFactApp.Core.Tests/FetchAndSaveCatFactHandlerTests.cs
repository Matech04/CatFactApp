using System.Net;
using System.Net.Http.Json;
using CatFactFetcher.Core.Share.Storage;
using CatFactFetcher.Functions.Features.FetchAndSaveCatFact;
using FluentAssertions;
using Moq;
using Moq.Protected;
using Xunit;

namespace CatFactApp.Core.Tests;

public class FetchAndSaveCatFactHandlerTests
{
    private readonly Mock<IFileStorage> _storageMock;

    public FetchAndSaveCatFactHandlerTests()
    {
        _storageMock = new Mock<IFileStorage>();
    }

    private FetchAndSaveCatFactHandler CreateHandler(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        var httpClient = new HttpClient(handlerMock.Object);
        return new FetchAndSaveCatFactHandler(httpClient, _storageMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnSuccessAndSaveLine_WhenApiReturnsValidFact()
    {
        // Arrange
        var jsonResponse = """{"fact": "Cats sleep 16 hours a day.", "length": 26}""";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        var handler = CreateHandler(httpResponse);

        // Act
        var result = await handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value.Fact.Should().Be("Cats sleep 16 hours a day.");
        result.Value.Length.Should().Be(26);

        _storageMock.Verify(s => s.SaveLineAsync(
            It.Is<string>(line => line.Contains("Cats sleep 16 hours a day.") && line.Contains("26")),
            It.IsAny<CancellationToken>()
        ), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenApiReturnsNullContent()
    {
        // Arrange
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent("null", System.Text.Encoding.UTF8, "application/json")
        };

        var handler = CreateHandler(httpResponse);

        // Act
        var result = await handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "External server answered with empty fact");
        _storageMock.Verify(s => s.SaveLineAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenHttpClientThrowsException()
    {
        // Arrange
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("Network error"));

        var httpClient = new HttpClient(handlerMock.Object);
        var handler = new FetchAndSaveCatFactHandler(httpClient, _storageMock.Object);

        // Act
        var result = await handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message.Contains("Network error"));
        _storageMock.Verify(s => s.SaveLineAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}