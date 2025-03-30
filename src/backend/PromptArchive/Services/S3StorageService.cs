using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using FastEndpoints;
using FluentResults;
using Microsoft.Extensions.Options;
using PromptArchive.Configuration;

namespace PromptArchive.Services;

public class S3StorageService : IStorageService
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _baseUrl;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public S3StorageService(IOptions<S3StorageSettings> s3Settings, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
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

    public async Task<string> UploadImageAsync(Stream fileStream, string fileName, string contentType, CancellationToken cancellationToken = default)
    {
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var key = $"images/{uniqueFileName}";

        var putRequest = new TransferUtilityUploadRequest
        {
            BucketName = _bucketName,
            Key = key,
            InputStream = fileStream,
            ContentType = contentType,
            CannedACL = S3CannedACL.Private
        };

        var transferUtility = new TransferUtility(_s3Client);
        await transferUtility.UploadAsync(putRequest, cancellationToken);

        return uniqueFileName;
    }

    public async Task DeleteImageAsyncTask(string imagePath, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(imagePath))
            return;

        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = $"images/{imagePath}"
        };

        await _s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }

    public string GetImageUrl(string imagePath)
    {
        return $"prompts/versions/images/{Uri.EscapeDataString(imagePath)}";
    }

    public async Task<Result<(Stream Stream, string ContentType)>> GetImageStreamAsync(string imagePath, CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = $"images/{imagePath}"
            };

            var response = await _s3Client.GetObjectAsync(request, cancellationToken);
            return (response.ResponseStream, response.Headers.ContentType);
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
         return Result.Fail("Image not found");
        }
    }
}