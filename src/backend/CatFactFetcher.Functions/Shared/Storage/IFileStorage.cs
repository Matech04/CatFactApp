public interface IFileStorage
{
    Task SaveLineAsync(string line);
    Task<Stream> GetFileStreamAsync(CancellationToken ct);
}