using Contoso.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class ProductCreateModel : PageModel
{

        public bool IsAdmin { get; set; }

        public string ImageUrl { get; set; }

        [BindProperty]
        public ProductDto Product { get; set; }


        [BindProperty]
        public IFormFile Image { get; set; }


        public string SuccessMessage { get; set; }


        private readonly IContosoAPI _contosoApi;


        public ProductCreateModel(IContosoAPI contosoApi)
        {
            _contosoApi = contosoApi;
        }


        public void OnGet()
        {
            IsAdmin = true;
        }


        public async Task<IActionResult> OnPostCreateProductAsync()
        {
            if (Image != null && Image.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    Image.CopyTo(memoryStream);
                    Product.Image = memoryStream.ToArray();
                }

                var fileName = Path.GetFileName(Image.FileName);
                
                Product.ImageUrl = fileName;
            }

            var response = await _contosoApi.CreateProductAsync(Product);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Product created successfully!";
                return RedirectToPage("/ProductCreate/ProductCreate");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while creating the product.");
                return Page();
            }
        }
}
