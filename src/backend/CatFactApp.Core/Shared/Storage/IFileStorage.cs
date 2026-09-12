namespace CatFactFetcher.Core.Share.Storage;

public interface IFileStorage
{
    Task SaveLineAsync(string line, CancellationToken ct);
    Task<Stream> GetFileStreamAsync(CancellationToken ct);
}