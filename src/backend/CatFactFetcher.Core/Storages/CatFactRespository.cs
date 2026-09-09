using System.Xml;

namespace CatFactFetcher.Core.Storages;

public interface ICatFactRepository
{
    Task SaveAsync(string fact, int length);
}
public class CatFactRepository: ICatFactRepository
{
    public Task SaveAsync(string fact, int length)
    {
        
        return System.IO.File.AppendAllTextAsync("facts.txt", $"{fact} \n");;
    }
}