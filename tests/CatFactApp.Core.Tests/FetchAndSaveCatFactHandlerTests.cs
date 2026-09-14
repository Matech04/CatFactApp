using System.Net;
using Castle.Core.Logging;
using CatFactFetcher.Core.Share.Storage;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Moq.Protected;
using Xunit;
using Microsoft.Extensions.Logging;
using CatFactApp.Core.Features.FetchAndSaveCatFact;
using CatFactApp.Core.Features.GetStoredCatFacts;

namespace CatFactApp.Core.Tests;

public class FetchAndSaveCatFactHandlerTests
{
    private readonly Mock<IFileStorage> _storageMock;
    private readonly Mock<ILogger<FetchAndSaveCatFactHandler>> _loggerMock; // 2. Dodaj pole dla mocka logera
    private readonly IMemoryCache _memoryCache;

    public FetchAndSaveCatFactHandlerTests()
    {
        _storageMock = new Mock<IFileStorage>();
        _loggerMock = new Mock<ILogger<FetchAndSaveCatFactHandler>>(); // 3. Zainicjalizuj mock
        _memoryCache = new MemoryCache(new MemoryCacheOptions());
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
        
        // 4. Przekaż _loggerMock.Object jako 4. argument
        return new FetchAndSaveCatFactHandler(
            httpClient, 
            _storageMock.Object, 
            _memoryCache, 
            _loggerMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_ShouldInvalidateCache_WhenNewFactIsSaved()
    {
        // Arrange
        var jsonResponse = """{"fact": "Cats sleep 16 hours a day.", "length": 26}""";
        var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(jsonResponse, System.Text.Encoding.UTF8, "application/json")
        };

        // Symulujemy, że w cache znajdują się już jakieś nieaktualne dane
        _memoryCache.Set(GetStoredCatFactsHandler.CacheKey, Array.Empty<object>());

        var handler = CreateHandler(httpResponse);

        // Act
        await handler.HandleAsync(CancellationToken.None);

        // Assert
        // Sprawdzamy, czy klucz został usunięty z pamięci podręcznej po zapisie
        _memoryCache.TryGetValue(GetStoredCatFactsHandler.CacheKey, out _).Should().BeFalse();
    }
}