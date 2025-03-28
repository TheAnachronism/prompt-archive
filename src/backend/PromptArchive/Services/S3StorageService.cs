using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using PromptArchive.Configuration;

namespace PromptArchive.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _baseUrl;

    public S3StorageService(IOptions<S3StorageSettings> s3Settings)
    {
        ArgumentNullException.ThrowIfNull(s3Settings.Value.BucketName);
        ArgumentNullException.ThrowIfNull(s3Settings.Value.AccessKey);
        ArgumentNullException.ThrowIfNull(s3Settings.Value.SecretKey);

        _bucketName = s3Settings.Value.BucketName;
        _baseUrl = s3Settings.Value.BaseUrl ?? $"https://{_bucketName}.s3.amazonaws.com";

        _s3Client = new AmazonS3Client(
            s3Settings.Value.AccessKey,
            s3Settings.Value.SecretKey,
            new AmazonS3Config
            {
                RegionEndpoint = Amazon.RegionEndpoint.GetBySystemName(
                    s3Settings.Value.Region ?? "us-east-1"
                )
            }
        );
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var key = $"images/{uniqueFileName}";

        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.PublicRead
        };

        await _s3Client.PutObjectAsync(putRequest);

        return key;
    }

    public async Task DeleteImageAsyncTask(string imagePath)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = imagePath
        };

        await _s3Client.DeleteObjectAsync(deleteRequest);
    }

    public string GetImageUrl(string imagePath)
    {
        return string.IsNullOrEmpty(imagePath) ? string.Empty : $"{_baseUrl}/{imagePath}";
    }
}