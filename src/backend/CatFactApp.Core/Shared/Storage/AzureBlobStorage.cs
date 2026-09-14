using System.Text;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Specialized;
using CatFactFetcher.Core.Share.Storage;

namespace CatFactApp.Core.Shared.Storage;

public class AzureBlobStorage(BlobServiceClient blobServiceClient) : IFileStorage
{
    private const string ContainerName = "catfacts";
    private const string BlobName = "facts.txt";

    public async Task SaveLineAsync(string line, CancellationToken ct)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        await containerClient.CreateIfNotExistsAsync(cancellationToken: ct);

        // Tworzymy klienta AppendBlob dedykowanego do dopisywania danych
        var appendBlobClient = containerClient.GetAppendBlobClient(BlobName);

        // Jeśli blob nie istnieje, musimy go najpierw utworzyć
        if (!await appendBlobClient.ExistsAsync(ct))
        {
            await appendBlobClient.CreateAsync(cancellationToken: ct);
        }

        // Dodajemy znak nowej linii, aby każdy fakt był w osobnym wierszu
        var contentToAppend = $"{line}{Environment.NewLine}";
        var bytes = Encoding.UTF8.GetBytes(contentToAppend);

        using var memoryStream = new MemoryStream(bytes);

        // Dopisanie danych na koniec pliku w Blob Storage
        await appendBlobClient.AppendBlockAsync(memoryStream, cancellationToken: ct);
    }

    public async Task<Stream> GetFileStreamAsync(CancellationToken ct = default)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);
        var blobClient = containerClient.GetBlobClient(BlobName);

        if (!await blobClient.ExistsAsync(ct))
        {
            // Jeśli plik jeszcze nie istnieje, zwracamy pusty strumień
            return Stream.Null;
        }

        // OpenReadAsync pobiera strumień z Azure Blob Storage do odczytu
        return await blobClient.OpenReadAsync(cancellationToken: ct);
    }
}