using Microsoft.Extensions.Logging;
namespace CatFactFetcher.Core.Share.Storage;


public class LocalStorage(ILogger<LocalStorage> logger) : IFileStorage
{

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

    public async Task<Stream> GetFileStreamAsync(CancellationToken ct)
    {
        await _semaphoreSlim.WaitAsync(ct);
        try
        {

            if (!File.Exists("facts.txt"))
            {
                return Stream.Null;
            }

            return new FileStream("facts.txt", FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize: 4096, useAsync: true);
        }
        finally
        {
            _semaphoreSlim.Release();
        }
    }
}