using Contoso.Api.Data;
using Contoso.Api.Models;

namespace Contoso.Api.Services
{
    public interface IProductsService
    {
        Task<PagedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters);
        Task<List<string>> GetProductCategories();
        Task<ProductDto> GetProductAsync(int id);
        Task<ProductDto> CreateProductAsync(ProductDto product);

        Task CreateProductsAsync(string folderPath);

        Task UploadProductsImagesToBlobAsync(List<ProductImageDto> productImages);
        
        Task<ProductDto> UpdateProductAsync(ProductDto product);
        Task DeleteProductAsync(int id);
    }
}