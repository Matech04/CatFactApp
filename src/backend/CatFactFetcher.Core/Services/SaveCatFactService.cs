using CatFactFetcher.Core.Dto;
using CatFactFetcher.Core.Storages;
using FluentResults;

namespace CatFactFetcher.Core.Services;

public interface ISaveCatFactService
{
    public Task<Result<CatFactDto>> SaveAsync();
}

public class SaveCatFactService : ISaveCatFactService
{

    private readonly IFetchDataService _fetchDataService;
    private readonly ICatFactRepository _catFactRepository;
    private readonly ILogger<SaveCatFactService> _logger;


    public SaveCatFactService(IFetchDataService fetchDataService, ICatFactRepository catFactRepository, ILogger<SaveCatFactService> logger)
    {
        _fetchDataService = fetchDataService;
        _catFactRepository = catFactRepository;
        _logger = logger;
    }
    public async Task<Result<CatFactDto>> SaveAsync()
    {

        var factResult = await _fetchDataService.FetchAsync<CatFactDto>("https://catfact.ninja/fact");

        if (!factResult.IsSuccess)
        {
            _logger.LogError("Failed to fetch fact from external server");
            return Result.Fail<CatFactDto>("Failed to fetch fact from external server");
        }

        var fact = factResult.Value;

        try{
        await _catFactRepository.SaveAsync(fact.fact, fact.length);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex.Message);
            return Result.Fail<CatFactDto>(ex.Message);
        }

        _logger.LogInformation("Successfuly Saved Cat Fact: " + fact.fact);
        return Result.Ok<CatFactDto>(fact);

    }
}