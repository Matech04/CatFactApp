using Azure.Storage.Blobs;
using CatFactApp.Core.Shared.Storage;
using FluentAssertions;
using Testcontainers.Azurite;
using Xunit;

namespace CatFactApp.Core.Tests;

public class AzureBlobStorageTests : IAsyncLifetime
{
    private readonly AzuriteContainer _azuriteContainer = new AzuriteBuilder("mcr.microsoft.com/azure-storage/azurite:latest")
        .WithCommand("--skipApiVersionCheck")
        .Build();

    public async Task InitializeAsync() => await _azuriteContainer.StartAsync();

    public async Task DisposeAsync() => await _azuriteContainer.StopAsync();

    [Fact]
    public async Task SaveLineAsync_And_GetFileStreamAsync_ShouldIntegrateWithBlobStorage()
    {
        // Arrange
        var blobServiceClient = new BlobServiceClient(_azuriteContainer.GetConnectionString());
        var storage = new AzureBlobStorage(blobServiceClient);
        var testLine = "2026-03-01 | Test Azure Fact | 15";

        // Act
        await storage.SaveLineAsync(testLine, CancellationToken.None);
        using var stream = await storage.GetFileStreamAsync(CancellationToken.None);

        // Assert
        stream.Should().NotBeNull();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();

        content.Should().Contain(testLine);
    }
}