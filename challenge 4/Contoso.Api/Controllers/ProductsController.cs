using System.Threading.Tasks;
using Contoso.Api.Data;
using Contoso.Api.Models;
using Contoso.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Contoso.Api.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductsService _productService;

    public ProductsController(IProductsService productService)
    {
        _productService = productService;
    }

    [HttpPost]
    public async Task<PagedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters)
    {
        return await _productService.GetProductsAsync(queryParameters);
    }

    [HttpGet("categories")]
    public async Task<List<string>> GetProductCategories()
    {
        return await _productService.GetProductCategories();
    }
    
    [HttpGet("{id}")]
    public async Task<ProductDto> GetProductAsync(int id)
    {
        return await _productService.GetProductAsync(id);
    }

    [HttpPost("upload/images")]
    [Authorize]
    public async Task<IActionResult> GetUploadBlobUrl([FromBody] List<ProductImageDto> productImage)
    {
        Console.WriteLine("Upload images to blob");
        await _productService.UploadProductsImagesToBlobAsync(productImage);
        return  Ok();
    }

    [HttpPost("create")]
    [Authorize]
    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        return await _productService.CreateProductAsync(product);
    }

    [HttpPost("create/bulk")]
    [Authorize]
    public async Task CreateProductsAsync()
    {
        var productsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "products");
        
        await _productService.CreateProductsAsync(productsFolderPath);   
    }


    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateProductAsync(ProductDto product)
    {
        var updatedProduct = await _productService.UpdateProductAsync(product);

        if (updatedProduct == null)
        {
            return BadRequest("Product not found");
        }

        return Ok(updatedProduct);
    }


    [HttpDelete("{id}")]
    [Authorize]
    public async Task DeleteProductAsync(int id)
    {
        await _productService.DeleteProductAsync(id);
    }
}