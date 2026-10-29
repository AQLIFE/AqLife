using Amazon.S3;
using Amazon.S3.Model;
using AqLife.Application.Abstractions.FileStorage;
using AqLife.Shared.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Duende.IdentityModel.ClaimComparer;

namespace AqLife.Infrastructure.FileStorage
{
    public class R2FileStorage(IAmazonS3 client, IOptions<R2Options> options) : IFileStorage
    {
        private readonly string bucketName = options.Value.BucketName;

        public async Task SaveAsync(
            Stream content,
            string key,
            CancellationToken ct = default)
        {
            await client.PutObjectAsync(
                new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    InputStream = content,
                    DisablePayloadSigning = true,
                    DisableDefaultChecksumValidation = true
                },
                ct);
        }

        public async Task<bool> DeleteAsync(
            string key,
            CancellationToken ct = default)
        {
            await client.DeleteObjectAsync(
                new DeleteObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                },
                ct);

            return true;
        }


        public async Task<FileStorageRangeReadResult> OpenReadRangeAsync(
            string key,
            long start,
            long end,
            CancellationToken ct = default)
        {
            var metadata = await client.GetObjectMetadataAsync(
                new GetObjectMetadataRequest
                {
                    BucketName = bucketName,
                    Key = key
                },
                ct);

            var totalLength = metadata.ContentLength;
            if (totalLength <= 0 || start < 0 || end < start || start >= totalLength)
                throw new ArgumentOutOfRangeException(nameof(start), "请求的文件字节范围无效。");

            end = Math.Min(end, totalLength - 1);

            var response = await client.GetObjectAsync(
                new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key,
                    ByteRange = new ByteRange(start, end)
                },
                ct);

            return new FileStorageRangeReadResult(response.ResponseStream, totalLength, start, end);
        }

        //public async Task<bool> ExitsAsync(string key,CancellationToken ct = default)
        //{
        //    var response = await client.e
        //}

        public async Task<Stream> OpenReadAsync(
            string key,
            CancellationToken ct = default)
        {
            var response = await client.GetObjectAsync(
                new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = key
                },
                ct);

            return response.ResponseStream;
        }
    }
}
