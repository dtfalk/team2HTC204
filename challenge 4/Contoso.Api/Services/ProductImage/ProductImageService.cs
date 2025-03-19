using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;

namespace Contoso.Api.Services
{
    public class ProductImageService : IProductImageService
    {
        private readonly IConfiguration _configuration;

        private string StorageContainerName = "";
        private string StorageAccountUrl = "";
        private string DefaultImageUrl  = "";
        private string SasToken = "";

        public ProductImageService(IConfiguration configuration)
        {
            _configuration = configuration;

            DefaultImageUrl = _configuration["Azure:StorageAccount:DefaultImageUrl"];
            StorageContainerName = _configuration["Azure:StorageAccount:ContainerName"];
            StorageAccountUrl = _configuration["Azure:StorageAccount:ConnectionString"];
            SasToken =  _configuration["Azure:StorageAccount:SasTokenForContainer"];
        }

        public async Task<string> GetBlobReleaseDateAsync(string blobUrl)
        {
            // Extract the blob name from the provided URL
            var blobName = new Uri(blobUrl).Segments.Last();
            // Build the full URL using the public storage URL, container name, blob name, and SAS token.
            var fullBlobUrl = $"{StorageAccountUrl}/{StorageContainerName}/{blobName}?{SasToken}";
            var blobClient = new BlobClient(new Uri(fullBlobUrl));
            var properties = await blobClient.GetPropertiesAsync();
            var metadata = properties.Value.Metadata;
            metadata.TryGetValue("ReleaseDate", out string releaseDate);
            return releaseDate;
        }

        public string SaveImageToBlobStorage(string blobName, byte[] image)
        {
            // Build the full URL using the public storage URL, container name, blob name, and SAS token.
            var fullBlobUrl = $"{StorageAccountUrl}/{StorageContainerName}/{blobName}?{SasToken}";
            var blobClient = new BlobClient(new Uri(fullBlobUrl));

            var metadata = new Dictionary<string, string> {
                { "ReleaseDate", DateOnly.FromDateTime(DateTime.Now).ToString()}
            };


            try
            {
                blobClient.Upload(BinaryData.FromBytes(image), new BlobUploadOptions { Metadata = metadata });
            }
            catch (System.Exception)
            {
                return "";
            }
            
            return blobClient.Uri.ToString();
            
        }

        public string GetDefaultImageUrl()
        {
            return DefaultImageUrl;
        }
        public string GetContainerName()
        {
            return StorageContainerName;
        }
        public string GetStorageAccountUrl()
        {
            return StorageAccountUrl;
        }
        public string GetSasTokenForContainer()
        {
            return SasToken;
        }
    }
}