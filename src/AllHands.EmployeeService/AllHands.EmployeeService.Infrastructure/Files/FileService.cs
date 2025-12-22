using System.Net;
using System.Text;
using AllHands.EmployeeService.Application.Abstractions;
using AllHands.EmployeeService.Application.Dto;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;

namespace AllHands.EmployeeService.Infrastructure.Files;

public sealed class FileService(IAmazonS3 s3Client, IOptionsMonitor<S3Options> options) : IFileService
{
    private const string OriginalFileNameHeader = "original-filename";
    
    public async Task SaveAvatarAsync(AllHandsFile file, CancellationToken cancellationToken)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.AvatarsPrefix}/{file.Name}",
            InputStream = file.Stream,
            ContentType = file.ContentType
        };
        
        putRequest.Metadata.Add(OriginalFileNameHeader, Convert.ToBase64String(Encoding.UTF8.GetBytes(file.OriginalFileName ?? string.Empty)));

        await s3Client.PutObjectAsync(putRequest, cancellationToken);
    }

    public async Task<AllHandsFile> GetAvatarAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = options.CurrentValue.BucketName,
                Key = $"{options.CurrentValue.AvatarsPrefix}/{id}"
            };

            var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);

            return new AllHandsFile(response.ResponseStream, id, response.Headers.ContentType)
            {
                OriginalFileName = !string.IsNullOrEmpty(response.Metadata[OriginalFileNameHeader])
                    ? Encoding.UTF8.GetString(Convert.FromBase64String(response.Metadata[OriginalFileNameHeader])) 
                    : Path.GetFileName(response.Key)
            };
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return await GetDefaultAvatarAsync(id, cancellationToken);
        }
    }

    public async Task DeleteAvatarAsync(string id, CancellationToken cancellationToken)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.AvatarsPrefix}/{id}"
        };
        
        _ = await s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }

    private async Task<AllHandsFile> GetDefaultAvatarAsync(string id, CancellationToken cancellationToken)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.AvatarsPrefix}/{options.CurrentValue.DefaultAvatarName}"
        };
        var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);
        
        return new AllHandsFile(response.ResponseStream, id, response.Headers.ContentType)
        {
            OriginalFileName = Path.GetFileName(response.Key)
        };
    }

    public async Task SaveCompanyLogoAsync(AllHandsFile file, CancellationToken cancellationToken)
    {
        var putRequest = new PutObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.CompanyLogosPrefix}/{file.Name}",
            InputStream = file.Stream,
            ContentType = file.ContentType
        };
        
        putRequest.Metadata.Add(OriginalFileNameHeader, Convert.ToBase64String(Encoding.UTF8.GetBytes(file.OriginalFileName ?? string.Empty)));

        await s3Client.PutObjectAsync(putRequest, cancellationToken);
    }

    public async Task<AllHandsFile> GetCompanyLogoAsync(string id, CancellationToken cancellationToken)
    {
        try
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = options.CurrentValue.BucketName,
                Key = $"{options.CurrentValue.CompanyLogosPrefix}/{id}"
            };

            var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);

            return new AllHandsFile(response.ResponseStream, id, response.Headers.ContentType)
            {
                OriginalFileName = !string.IsNullOrEmpty(response.Metadata[OriginalFileNameHeader]) 
                    ? Encoding.UTF8.GetString(Convert.FromBase64String(response.Metadata[OriginalFileNameHeader])) 
                    : Path.GetFileName(response.Key)
            };
        }
        catch (AmazonS3Exception ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return await GetDefaultCompanyLogoAsync(id, cancellationToken);
        }
    }
    
    private async Task<AllHandsFile> GetDefaultCompanyLogoAsync(string id, CancellationToken cancellationToken)
    {
        var getRequest = new GetObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.CompanyLogosPrefix}/{options.CurrentValue.DefaultCompanyLogoName}"
        };
        var response = await s3Client.GetObjectAsync(getRequest, cancellationToken);
        
        return new AllHandsFile(response.ResponseStream, id, response.Headers.ContentType)
        {
            OriginalFileName = Path.GetFileName(response.Key)
        };
    }
    
    public async Task DeleteCompanyLogoAsync(string id, CancellationToken cancellationToken)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = options.CurrentValue.BucketName,
            Key = $"{options.CurrentValue.CompanyLogosPrefix}/{id}"
        };
        
        _ = await s3Client.DeleteObjectAsync(deleteRequest, cancellationToken);
    }
}
