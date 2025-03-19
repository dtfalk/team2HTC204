using System.Runtime.CompilerServices;
using System.Text.Json;
using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Contoso.Api.Data;
using Contoso.Api.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Any;

namespace Contoso.Api.Services;

public class ProductsService : IProductsService
{
    private readonly ContosoDbContext _context;
    private readonly IProductImageService _productImageService;
    private readonly IMapper _mapper;

    private readonly IConfiguration _configuration;

    public ProductsService(ContosoDbContext context, 
                          IMapper mapper, 
                          IProductImageService productImageService,
                          IConfiguration configuration)
    {
        _context = context;
        _mapper = mapper;
        _productImageService = productImageService;
        _configuration = configuration;

    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(QueryParameters queryParameters)
    {
        
        IQueryable<Product> productsQuery = _context.Products;

        if (!string.IsNullOrEmpty(queryParameters.filterText))
        {
            productsQuery = productsQuery.Where(p => p.Category == queryParameters.filterText);
        }

        var products = await productsQuery
                            .Skip(queryParameters.StartIndex) 
                            .Take(queryParameters.PageSize)
                            .ToListAsync();


        foreach (var product in products)
        {

            string targetImageUrl = await GetProductImageUrlBasedOnReleaseDate(product.ImageUrl);

            product.ImageUrl = targetImageUrl;      
        }

        var totalCount = await _context.Products
                                        .Where(p =>  p.Category == queryParameters.filterText || string.IsNullOrEmpty(queryParameters.filterText))
                                        .CountAsync();

        var pagedProducts = new PagedResult<ProductDto>
        {
            Items = _mapper.Map<List<ProductDto>>(products),
            TotalCount = totalCount,
            PageSize = queryParameters.PageSize,
            PageNumber = queryParameters.PageNumber
        };


        return pagedProducts;
    }

    public async Task<ProductDto> GetProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);

        if (product == null)
        {
            return null;
        }

        string targetImageUrl = await GetProductImageUrlBasedOnReleaseDate(product.ImageUrl);

        product.ImageUrl = targetImageUrl;

        return _mapper.Map<ProductDto>(product);
    }

    public async Task<ProductDto> CreateProductAsync(ProductDto product)
    {
        var productModel = _mapper.Map<Product>(product);

        string imageRemoteUrl = _productImageService.SaveImageToBlobStorage(productModel.ImageUrl, product.Image);

        var cur_url = new Uri(imageRemoteUrl);
        productModel.ImageUrl = cur_url.Segments.Last();


        productModel.Id =  new Random().Next(1, 100000);
        _context.Products.Add(productModel);

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(productModel);
    }

    public async Task<ProductDto> UpdateProductAsync(ProductDto product)
    {
        var existingProduct = await _context.Products.AsNoTracking().FirstAsync(x => x.Id == product.Id);

        if  (existingProduct == null)
        {
            return null;
        }

        existingProduct.Name = product.Name;
        existingProduct.Description = product.Description;
        existingProduct.Price = product.Price;

      
        if (existingProduct.ImageUrl != product.ImageUrl)
        {
            if (!Uri.IsWellFormedUriString(existingProduct.ImageUrl, UriKind.Absolute))
            {

                string imageRemoteUrl = _productImageService.SaveImageToBlobStorage(product.ImageUrl, product.Image);
                var cur_url = new Uri(imageRemoteUrl);
                existingProduct.ImageUrl = cur_url.Segments.Last();
            }
        }


        _context.Entry(existingProduct).State = EntityState.Modified;

        await _context.SaveChangesAsync();

        return _mapper.Map<ProductDto>(existingProduct);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.AsNoTracking().FirstAsync(x => x.Id == id);

        _context.Products.Remove(product);

        await _context.SaveChangesAsync();
    }

    public async Task UploadProductsImagesToBlobAsync(List<ProductImageDto> productImages)
    {

        List<Product> productsToUpdate = new List<Product>();

        foreach (var productImage in productImages)
        {
            string imageUrl = _productImageService.SaveImageToBlobStorage(productImage.ImageUrl, productImage.Image);

            if (imageUrl == "") {
                continue;
            }

            string imageName = imageUrl.Split('/')
                                        .Last()
                                        .Split('.')
                                        .First();

            var product = _context.Products
                                  .FirstOrDefault(x => x.ImageUrl.Contains(imageName));

            if (product != null){
                product.ImageUrl = imageUrl;
                productsToUpdate.Add(product);
            }
        }

        if (productsToUpdate.Count > 0)
        {
            _context.Products.UpdateRange(productsToUpdate);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetProductCategories()
    {
        return await _context.Products.Select(x => x.Category).Distinct().ToListAsync();
    }


     private async Task<string> GetProductImageUrlBasedOnReleaseDate(string imageUrl)
{
    Console.WriteLine($"🔍 Debug: Received ImageUrl = '{imageUrl}'");

    // Fetch environment variables
    var StorageAccountUrl = Environment.GetEnvironmentVariable("Azure__StorageAccount__ConnectionString");
    var storageContainerName = Environment.GetEnvironmentVariable("Azure__StorageAccount__ContainerName");
    var sasTokenForContainer = Environment.GetEnvironmentVariable("Azure__StorageAccount__SasTokenForContainer");
    var defaultImageUrl = Environment.GetEnvironmentVariable("Azure__StorageAccount__DefaultImageUrl");

    // ✅ 1) If it's already a full URL, extract the last segment
    if (!string.IsNullOrEmpty(imageUrl) && 
        imageUrl.StartsWith("http", StringComparison.OrdinalIgnoreCase))
    {
        // Extract last part of the URL (e.g., "image_name.jpeg")
        imageUrl = new Uri(imageUrl).Segments[^1];
        Console.WriteLine($"Segmented Url {imageUrl}");
    }

    // Validate required environment variables
    if (string.IsNullOrEmpty(storageContainerName) || string.IsNullOrEmpty(sasTokenForContainer))
    {
        Console.WriteLine($"❌ Error: Missing Storage container name or SAS token. Returning default image.");
        return defaultImageUrl;
    }

    imageUrl = $"{StorageAccountUrl}/{storageContainerName}/{imageUrl}?{sasTokenForContainer}";

    string releaseDate;
    try
    {
        // ✅ Attempt to get the release date
        releaseDate = await _productImageService.GetBlobReleaseDateAsync(imageUrl);
        
        if (releaseDate == null)
        {
            Console.WriteLine($"❌ Error: Blob not found. Returning default image.");
            return defaultImageUrl;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error: Exception occurred while retrieving blob metadata: {ex.Message}");
        return defaultImageUrl;
    }

    // Validate and parse the release date safely
    if (!DateTime.TryParse(releaseDate, out var releaseDate_new))
    {
        Console.WriteLine($"❌ Error: Failed to parse release date '{releaseDate}'. Using default image.");
        return defaultImageUrl;
    }

    Console.WriteLine($"🔍 Debug: Constructed full blob URL = '{imageUrl}'");

    // Check if the product is released (Use UTC for accurate comparisons)
    if (releaseDate_new.Date > DateTime.UtcNow.Date)
    {
        Console.WriteLine($"⚠️ Warning: Product not released yet. Using default image: {defaultImageUrl}");
        return defaultImageUrl;
    }

    Console.WriteLine($"✅ Final Image URL = '{imageUrl}'");
    return imageUrl;
}

    public async Task CreateProductsAsync(string folderPath)
    {
        var filePath = Path.Combine(folderPath, "products.json");
        var imageFolderPath = Path.Combine(folderPath, "images");
  
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"The file {folderPath} does not exist.");
        }

        var jsonData = await File.ReadAllTextAsync(filePath);

        // Make sure that ProductDto construcotr is called so that Id is generated

        var products = JsonSerializer.Deserialize<List<ProductDto>>(jsonData);

        if (products != null)
        {
            foreach (var product in products)
            {
                var imageFileName = Path.GetFileName(product.ImageUrl);
                var imagePath = Path.Combine(imageFolderPath, imageFileName);

                if (File.Exists(imagePath))
                {
                    product.Image = await File.ReadAllBytesAsync(imagePath);
                }
                else
                {
                    product.Image = null;
                }

                // Update the image url to the remote url after saving the image to blob storage
                product.ImageUrl = _productImageService.SaveImageToBlobStorage(product.ImageUrl, product.Image);
            }
        }

      
        var productModels = _mapper.Map<List<Product>>(products);

        // // LOG PRODUCTS TO FILE

        // var logFilePath = Path.Combine(folderPath, "products_log.txt");
        // using (var writer = new StreamWriter(logFilePath, append: true))
        // {
        //     foreach (var product in productModels)
        //     {
        //         var productJson = JsonSerializer.Serialize(product);
        //         await writer.WriteLineAsync(productJson);
        //         await writer.WriteLineAsync("-------------------------------------------------");
        //     }
        // }


        // // MAKE BULK UPDATE WITH COSMOS API

        var cosmosClientOptions = new CosmosClientOptions() { AllowBulkExecution = true };
        var cosmosClient = new CosmosClient(_configuration["Azure:CosmosDB:ConnectionString"], cosmosClientOptions);
        var container = cosmosClient.GetContainer(_configuration["Azure:CosmosDB:DatabaseName"], "Products");
       

        List<Task> tasks = new List<Task>();

        foreach (var productModel in productModels)
        {
            // Cosmos DB needs id property to be a string, ef core appends that as default but manually needs to be done like this
            var product = new 
            { 
                id = productModel.Id.ToString(), 
                productModel.Id,
                productModel.Name, 
                productModel.Description, 
                productModel.Price, 
                productModel.ImageUrl, 
                productModel.Category 
            };
            
            tasks.Add(container.CreateItemAsync(product, new PartitionKey(product.Category))
                .ContinueWith(itemResponse =>
                {
                    if (!itemResponse.IsCompletedSuccessfully)
                    {
                        AggregateException innerExceptions = itemResponse.Exception.Flatten();
                        if (innerExceptions.InnerExceptions.FirstOrDefault(innerEx => innerEx is CosmosException) is CosmosException cosmosException)
                        {
                            Console.WriteLine($"Received {cosmosException.StatusCode} ({cosmosException.Message}).");
                        }
                        else
                        {
                            Console.WriteLine($"Exception {innerExceptions.InnerExceptions.FirstOrDefault()}.");
                        }
                    }              
                })
            );
        }

        await Task.WhenAll(tasks);
    }
}