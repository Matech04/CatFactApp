using CatFactFetcher.Core.Storages;
using FluentResults;

namespace CatFactFetcher.Core.Services;

public interface ISaveCatFactService
{
    public Task SaveAsync(string fact, int length);
}

public class SaveCatFactService : ISaveCatFactService
{

    private readonly ICatFactRepository _catFactRepository;

    public SaveCatFactService(ICatFactRepository catFactRepository)
    {
        _catFactRepository = catFactRepository;
    }
    public Task SaveAsync(string fact, int length)
    {
        return _catFactRepository.SaveAsync(fact, length);
    }
}