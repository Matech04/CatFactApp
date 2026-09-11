namespace CatFactFetcher.Core.Storages;


public class LocalFileRepository: IFileSystemRepository
{

    private readonly SemaphoreSlim _semaphoreSlim = new(1,1);

    public async Task SaveAsync(string fact, int length)
    {
        await _semaphoreSlim.WaitAsync();
        try
        {
            await System.IO.File.AppendAllTextAsync("facts.txt", $"{DateTime.UtcNow:dd.MM.yyyy HH:mm} | {fact} \n");
        }
        finally
        {
            _semaphoreSlim.Release();
        }
        
    }
}