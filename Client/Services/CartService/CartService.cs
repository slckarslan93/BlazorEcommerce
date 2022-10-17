using Blazored.LocalStorage;

namespace BlazorEcommerce.Client.Services.CartService
{
    public class CartService : ICartService
    {
        private readonly ILocalStorageService _localStorage;
        private readonly HttpClient _http;

        public CartService(ILocalStorageService localStorage,HttpClient http)
        {
            _localStorage = localStorage;
            _http = http;
        }

        public event Action OnCahange;

        public async Task AddToCart(CartItem cartItem)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            cart.Add(cartItem);
            await _localStorage.SetItemAsync("cart", cart);
            OnCahange.Invoke();
        }

        public async Task<List<CartItem>> GetCartItems()
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart==null)
            {
                cart = new List<CartItem>();
            }
            return cart;
        }

        public async Task<List<CartProductResponse>> GetCartProducts()
        {
            var cartItems = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            var response = await _http.PostAsJsonAsync("api/cart/products", cartItems);
            var cartProducts = 
                await response.Content.ReadFromJsonAsync<ServiceResponse<List<CartProductResponse>>>();

            return cartProducts.Data;
        }

        public async Task RemoveProductFromCart(int productId, int productTypeId)
        {
            var cart = await _localStorage.GetItemAsync<List<CartItem>>("cart");
            if (cart == null)
            {
                return;
            }

            var cartItem = cart.Find(x => x.ProductId == productId && x.ProductTypeId == productTypeId);
            if (cartItem != null)
            {
                cart.Remove(cartItem);
                await _localStorage.SetItemAsync("cart", cart);
                OnCahange.Invoke();
            }
        }
    }
}
