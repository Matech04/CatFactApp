public interface IFileSystemRepository
{
    Task SaveAsync(string fact, int length);
}