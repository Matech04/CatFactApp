namespace CatFactFetcher.Core.Storages;
public class AzureBlobRepository: IFileSystemRepository
{

    private readonly ILogger<AzureBlobRepository> _logger;

    public AzureBlobRepository(ILogger<AzureBlobRepository> logger)
    {
        _logger = logger;
    }

    public async Task SaveAsync(string fact, int length)
    {
        _logger.LogInformation("SAVED FILE IN AZURE BLOB");
    }
}