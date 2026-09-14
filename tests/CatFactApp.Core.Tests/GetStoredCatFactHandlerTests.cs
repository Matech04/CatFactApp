using System.Text;
using CatFactFetcher.Core.Share.Storage;
using CatFactFetcher.Functions.Features.GetStoredCatFacts;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFactApp.Core.Tests;

public class GetStoredCatFactsHandlerTests
{
    private readonly Mock<ILogger<GetStoredCatFactsHandler>> _loggerMock;
    private readonly Mock<IFileStorage> _storageMock;
    private readonly IMemoryCache _memoryCache;
    private readonly GetStoredCatFactsHandler _handler;

    public GetStoredCatFactsHandlerTests()
    {
        _loggerMock = new Mock<ILogger<GetStoredCatFactsHandler>>();
        _storageMock = new Mock<IFileStorage>();
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
        _handler = new GetStoredCatFactsHandler(_loggerMock.Object, _storageMock.Object, _memoryCache);
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnFail_WhenFileStreamIsNull()
    {
        // Arrange
        _storageMock
            .Setup(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((Stream?)null);

        // Act
        var result = await _handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsFailed.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Error while trying to read File");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnParsedRecords_WhenFileContainsValidData()
    {
        // Arrange
        var content = "2026-03-01T10:00:00Z | Cats can jump high | 18\n" +
                      "2026-03-02T12:00:00Z | Purring is therapeutic | 23";

        _storageMock
            .Setup(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(Encoding.UTF8.GetBytes(content)));

        // Act
        var result = await _handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Fact.Should().Be("Cats can jump high");
        result.Value[0].Length.Should().Be(18);
        result.Value[1].Fact.Should().Be("Purring is therapeutic");
        result.Value[1].Length.Should().Be(23);
    }

    [Fact]
    public async Task HandleAsync_ShouldIgnoreInvalidAndEmptyLines()
    {
        // Arrange
        var content = "2026-03-01T10:00:00Z | Valid fact | 10\n" +
                      "\n" +
                      "line without separator\n" +
                      "2026-03-01 | Invalid length | abc\n" +
                      "NotADate | Invalid date | 10\n" +
                      "2026-03-02T12:00:00Z | Another valid fact | 18";

        _storageMock
            .Setup(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(Encoding.UTF8.GetBytes(content)));

        // Act
        var result = await _handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
        result.Value[0].Fact.Should().Be("Valid fact");
        result.Value[1].Fact.Should().Be("Another valid fact");
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnEmptyArray_WhenStreamIsEmpty()
    {
        // Arrange
        _storageMock
            .Setup(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(Encoding.UTF8.GetBytes(string.Empty)));

        // Act
        var result = await _handler.HandleAsync(CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task HandleAsync_ShouldReturnDataFromCache_OnSecondCallWithoutCallingStorage()
    {
        // Arrange
        var content = "2026-03-01T10:00:00Z | Cats can jump high | 18";

        _storageMock
            .Setup(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => new MemoryStream(Encoding.UTF8.GetBytes(content)));

        // Act - Pierwsze wywołanie (czyta ze Storage i zapisuje w cache)
        var firstResult = await _handler.HandleAsync(CancellationToken.None);

        // Act - Drugie wywołanie (powinno pobrać dane z cache)
        var secondResult = await _handler.HandleAsync(CancellationToken.None);

        // Assert
        firstResult.IsSuccess.Should().BeTrue();
        secondResult.IsSuccess.Should().BeTrue();
        secondResult.Value.Should().BeEquivalentTo(firstResult.Value);

        // Weryfikujemy, że Storage zostało wywołane TYLKO RAZ mimo dwóch wywołań handlera
        _storageMock.Verify(s => s.GetFileStreamAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}