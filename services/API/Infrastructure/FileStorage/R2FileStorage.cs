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
