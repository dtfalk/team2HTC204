namespace Contoso.Api.Services
{
    public interface IProductImageService
    {
        Task<string> GetBlobReleaseDateAsync(string blobUrl);

        string SaveImageToBlobStorage(string blobName, byte[] image);

        string GetSasTokenForContainer();

        string GetDefaultImageUrl();

        string GetContainerName();

        string GetStorageAccountUrl();
    }
}