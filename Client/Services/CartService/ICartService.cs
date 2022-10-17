namespace BlazorEcommerce.Client.Services.CartService
{
    public interface ICartService
    {
        event Action OnCahange;
        Task AddToCart(CartItem cartItem);
        Task<List<CartItem>> GetCartItems();
    }
}
