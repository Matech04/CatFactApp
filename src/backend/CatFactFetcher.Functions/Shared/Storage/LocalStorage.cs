
namespace CatFactFetcher.Core.Storages;


public class LocalStorage(ILogger<LocalStorage> logger) : IFileStorage
{

    private static readonly SemaphoreSlim _semaphoreSlim = new(1, 1);

    public async Task SaveLineAsync(string line)
    {
        await _semaphoreSlim.WaitAsync();
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

    public async Task<Stream> GetFileStreamAsync(CancellationToken ct)
    {
        await _semaphoreSlim.WaitAsync(ct);
        try
        {
            return new FileStream("facts.txt", FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}