using Microsoft.AspNetCore.Mvc;

namespace BlazorEcommerce.Server.Services.ProductService
{
    public class ProductService : IProductService
    {
        private readonly DataContext _context;

        public ProductService(DataContext context)
        {
            _context = context;
        }

        public async Task<ServiceResponse<Product>> GetProductAsync(int productId)
        {
            var response = new ServiceResponse<Product>();
            var product = await _context.Products.Include(p=>p.Variants).ThenInclude(v=>v.ProductType).FirstOrDefaultAsync(p=>p.Id == productId);
            if (product == null)
            {
                response.Success = false;
                response.Message = "Sorry,but this product does not exist";
            }
            else
            {
                response.Data = product;
            }
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsAsync()
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.Include(p=>p.Variants).ToListAsync()
            };
            return response;
        }

        public async Task<ServiceResponse<List<Product>>> GetProductsByCategory(string categoryUrl)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await _context.Products.Where(p => p.Category.Url.ToLower().Equals(categoryUrl.ToLower())).Include(p=>p.Variants).ToListAsync()
            };
            return response;

        }

        private Task<List<Product>> FindProductsBySearchText(string searchText)
        {
            return _context.Products.Where(p => p.Title.ToLower().Contains(searchText.ToLower()) || p.Description.ToLower().Contains(searchText.ToLower())).Include(p => p.Variants).ToListAsync();
        }

        public async Task<ServiceResponse<List<Product>>> SearchPeoducts(string searchText)
        {
            var response = new ServiceResponse<List<Product>>
            {
                Data = await FindProductsBySearchText(searchText)
            };
            return response;
        }


        public async Task<ServiceResponse<List<string>>> GetProductsSearchSuggestions(string searchText)
        {
            var products =await FindProductsBySearchText(searchText);
            List<string> result = new List<string>();

            foreach (var product in products)
            {
                if (product.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase)) 
                {
                    result.Add(product.Title);
                }

                if (product.Description !=null)
                {
                    var punchtuation = product.Description.Where(char.IsPunctuation).Distinct().ToArray();

                    var words = product.Description.Split().Select(s => s.Trim(punchtuation));

                    foreach (var word in words)
                    {
                        if (word.Contains(searchText,StringComparison.OrdinalIgnoreCase)&& !result.Contains(word))
                        {
                            result.Add(word);
                        }
                    }
                }

            }

            return new ServiceResponse<List<string>> { Data = result};
        }
    }
}
