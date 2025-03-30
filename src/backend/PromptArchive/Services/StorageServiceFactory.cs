using PromptArchive.Configuration;

namespace PromptArchive.Services;

public static class StorageServiceFactory
{
    public static IStorageService CreateStorageService(IServiceProvider serviceProvider)
    {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();
        var storageType = configuration.GetValue<string>("Storage:Type");

        return storageType?.ToLower() switch
        {
            "s3" => serviceProvider.GetRequiredService<S3StorageService>(),
            "local" => serviceProvider.GetRequiredService<LocalStorageService>(),
            _ => throw new ArgumentException($"Unsupported storage type: {storageType}")
        };
    }
}