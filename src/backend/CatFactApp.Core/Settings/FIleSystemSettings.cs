public enum FileSystemType
{
    Local,
    AzureBlob
}

public class FileSystemSettings
{
    public FileSystemType Provider {get; set;}
}