using Microsoft.Extensions.Logging;
namespace CatFactFetcher.Core.Share.Storage;


public class LocalStorage(ILogger<LocalStorage> logger) : IFileStorage
{
    private const string FilePath = "facts.txt";
    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task SaveLineAsync(string line, CancellationToken ct)
    {
        await _semaphoreSlim.WaitAsync(ct);
        try
        {
            await File.AppendAllTextAsync("facts.txt", line + Environment.NewLine);
            logger.LogInformation("Successfully appended fact in Local File Storage");
        }
        finally
        {
            _semaphoreSlim.Release();
        }

    }

    public Task<Stream?> GetFileStreamAsync(CancellationToken ct)
    {

        if (!File.Exists("facts.txt"))
        {
            return Task.FromResult<Stream?>(null);
        }

        var stream = new FileStream(
        FilePath,
        FileMode.Open,
        FileAccess.Read,
        FileShare.ReadWrite,
        bufferSize: 4096,
        useAsync: true);

        return Task.FromResult<Stream?>(stream);
    }
}
