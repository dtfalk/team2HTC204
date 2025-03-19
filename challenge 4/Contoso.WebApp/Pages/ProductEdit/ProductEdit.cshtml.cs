using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProductEditModel : PageModel
{
        private readonly IContosoAPI _contosoAPI;
        
        public bool isAdmin { get; set; } = true;

        [BindProperty]
        public ProductDto Product { get; set; }

        [BindProperty]
        public IFormFile Image { get; set; } 

        public string ErrorMessage { get; set; }

        public ProductEditModel(IContosoAPI contosoAPI)
        {
            _contosoAPI = contosoAPI;
        }

        public async Task OnGetAsync(int id)
        {

            var productResponse = await _contosoAPI.GetProductAsync(id);

            if (!productResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "Failed to retrieve product";
                return;
            }

            Product = productResponse.Content;
        }


        // Implementiraj logiku da se provjerava da li je korisnik promijenio sliku
        public async Task<IActionResult>  OnPostEditProductAsync(int productId,string initialProductUrl)
        {
            // Set URL to initial URL
            Product.ImageUrl = initialProductUrl;

            if (Image != null && Image.Length > 0)
            {
                string fileName = Image.FileName;

                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);
                // Save image to wwwroot/images
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }


                using (var memoryStream = new MemoryStream())
                {
                    await Image.CopyToAsync(memoryStream);
                    Product.Image = memoryStream.ToArray();
                }

                Product.ImageUrl = fileName;
            }

            Product.Id = productId;
            Product.Name = Product.Name;
            Product.Price = Product.Price;

            var response = await _contosoAPI.UpdateProductAsync(Product);

            if (!response.IsSuccessStatusCode)
            {
                ErrorMessage = "Failed to update product";
                return Page();
            }

            TempData["SuccessMessage"] = "Product updated successfully";

            return RedirectToPage("/ProductEdit/ProductEdit", new { id = productId });
        }

}

