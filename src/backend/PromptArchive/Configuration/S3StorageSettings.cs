namespace PromptArchive.Configuration;

public class S3StorageSettings
{
    public string? BucketName { get; set; }
    public string? Region { get; set; }
    public string? AccessKey { get; set; }
    public string? SecretKey { get; set; }
    public string? BaseUrl { get; set; }
}