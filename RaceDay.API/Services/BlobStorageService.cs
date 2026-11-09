using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace RaceDay.API.Services
{
    public class BlobStorageService
    {
        private readonly IConfiguration _configuration;

        public BlobStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(
            IFormFile file,
            string containerName)
        {
            var connectionString =
                _configuration["AzureStorage:ConnectionString"];

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Azure Storage is not configured.");
            }

            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file was supplied.");
            }

            const long maxFileSize = 5 * 1024 * 1024;

            if (file.Length > maxFileSize)
            {
                throw new ArgumentException(
                    "The image must be 5 MB or smaller.");
            }

            var allowedTypes = new[]
            {
                "image/jpeg",
                "image/png",
                "image/webp"
            };

            if (!allowedTypes.Contains(file.ContentType))
            {
                throw new ArgumentException(
                    "Only JPG, PNG and WebP images are allowed.");
            }

            var extension = Path.GetExtension(file.FileName)
                .ToLowerInvariant();

            var allowedExtensions = new[]
            {
                ".jpg", ".jpeg", ".png", ".webp"
            };

            if (!allowedExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    "Invalid image file extension.");
            }

            var serviceClient =
                new BlobServiceClient(connectionString);

            var container =
                serviceClient.GetBlobContainerClient(containerName);

            await container.CreateIfNotExistsAsync(
                PublicAccessType.None);

            var blobName =
                $"{Guid.NewGuid()}{extension}";

            var blob =
                container.GetBlobClient(blobName);

            await using var stream = file.OpenReadStream();

            await blob.UploadAsync(
                stream,
                new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders
                    {
                        ContentType = file.ContentType
                    }
                });

            return blobName;
        }
    }
}