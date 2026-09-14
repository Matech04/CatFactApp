using CatFactFetcher.Core.Share.Storage;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace CatFactApp.Core.Tests;

public class LocalStorageTests : IDisposable
{
    private const string FilePath = "facts.txt"; // Ścieżka z klasy LocalStorage
    private readonly LocalStorage _storage;

    public LocalStorageTests()
    {
        // Upewniamy się, że środowisko testowe jest czyste przed każdym testem
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }

        var loggerMock = new Mock<ILogger<LocalStorage>>();
        _storage = new LocalStorage(loggerMock.Object);
    }

    [Fact]
    public async Task GetFileStreamAsync_ShouldReturnNull_WhenFileDoesNotExist()
    {
        // Act
        var stream = await _storage.GetFileStreamAsync(CancellationToken.None);

        // Assert
        stream.Should().BeNull();
    }

    [Fact]
    public async Task SaveLineAsync_ShouldCreateFileAndAppendText()
    {
        // Arrange
        var line1 = "2026-03-01 | Fact 1 | 6";
        var line2 = "2026-03-02 | Fact 2 | 6";

        // Act
        await _storage.SaveLineAsync(line1, CancellationToken.None);
        await _storage.SaveLineAsync(line2, CancellationToken.None);

        using var stream = await _storage.GetFileStreamAsync(CancellationToken.None);

        // Assert
        stream.Should().NotBeNull();

        using var reader = new StreamReader(stream!);
        var content = await reader.ReadToEndAsync();

        content.Should().Contain(line1);
        content.Should().Contain(line2);
    }

    public void Dispose()
    {
        // Sprzątanie po wykonaniu testów
        if (File.Exists(FilePath))
        {
            File.Delete(FilePath);
        }
    }
}