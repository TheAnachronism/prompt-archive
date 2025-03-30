using FluentResults;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using PromptArchive.Configuration;

namespace PromptArchive.Services;

public class S3StorageService : IStorageService
{
    private readonly IMinioClient _s3Client;
    private readonly string _bucketName;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<S3StorageService> _logger;

    public S3StorageService(IOptions<S3StorageSettings> s3Settings, IHttpContextAccessor httpContextAccessor,
        ILogger<S3StorageService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        ArgumentNullException.ThrowIfNull(s3Settings.Value.BucketName);
        ArgumentNullException.ThrowIfNull(s3Settings.Value.AccessKey);
        ArgumentNullException.ThrowIfNull(s3Settings.Value.SecretKey);

        _bucketName = s3Settings.Value.BucketName;
        var baseUrl = s3Settings.Value.BaseUrl ?? $"https://{_bucketName}.s3.amazonaws.com";

        var builder = new MinioClient()
            .WithEndpoint(baseUrl)
            .WithCredentials(s3Settings.Value.AccessKey, s3Settings.Value.SecretKey)
            .WithSSL(s3Settings.Value.Secure ?? true);

        if (!string.IsNullOrWhiteSpace(s3Settings.Value.Region))
            builder = builder.WithRegion(s3Settings.Value.Region);

        _s3Client = builder.Build();

        EnsureBucketExistsAsync().GetAwaiter().GetResult();
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            var bucketExistsArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);

            var found = await _s3Client.BucketExistsAsync(bucketExistsArgs);
            if (!found)
            {
                var makeBucketArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);

                await _s3Client.MakeBucketAsync(makeBucketArgs);

                _logger.LogInformation("Created bucket: {BucketName}", _bucketName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to ensure bucket exists: {BucketName}", _bucketName);
            throw;
        }
    }

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType,
        CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var key = $"images/{uniqueFileName}";

        var length = fileStream.Length;

        var putRequest = new PutObjectArgs()
            .WithBucket(_bucketName)
            .WithObject(key)
            .WithStreamData(fileStream)
            .WithObjectSize(length)
            .WithContentType(contentType);

        try
        {
            await _s3Client.PutObjectAsync(putRequest, cancellationToken);
            _logger.LogInformation("Successfully uploaded {FileName} to {BucketName}", uniqueFileName, _bucketName);

            return uniqueFileName;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file {FileName} to {BucketName}", uniqueFileName, _bucketName);
            throw;
        }
    }

    public async Task DeleteImageAsyncTask(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        var deleteRequest = new RemoveObjectArgs()
            .WithBucket(_bucketName)
            .WithObject($"images/{imagePath}");

        try
        {
            await _s3Client.RemoveObjectAsync(deleteRequest, cancellationToken);
            _logger.LogInformation("Successfully deleted {ImagePath} from {BucketName}", imagePath, _bucketName);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file {ImagePath} from {BucketName}", imagePath, _bucketName);
            throw;
        }
    }

    public string GetImageUrl(string imagePath)
    {
        return $"prompts/versions/images/{Uri.EscapeDataString(imagePath)}";
    }

    public async Task<Result<(Stream Stream, string ContentType)>> GetImageStreamAsync(string imagePath,
        CancellationToken cancellationToken = default)
    {
        var path = $"images/{imagePath}";
        try
        {
            var statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(path);

            // Get object metadata to check if it exists and get content type
            var objectStat = await _s3Client.StatObjectAsync(statObjectArgs, cancellationToken);

            // Create a memory stream to hold the object data
            var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(path)
                .WithCallbackStream(stream =>
                {
                    stream.CopyTo(memoryStream);
                    memoryStream.Position = 0;
                });

            await _s3Client.GetObjectAsync(getObjectArgs, cancellationToken);

            return (memoryStream, objectStat.ContentType);
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning("Image not found: {ImagePath} in bucket {BucketName}", imagePath, _bucketName);
            return Result.Fail("Image not found");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving image {ImagePath} from {BucketName}", imagePath, _bucketName);
            return Result.Fail(ex.Message);
        }
    }
}