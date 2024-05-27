using AutoMapper;
using Dtos.Cart;
using Microsoft.EntityFrameworkCore;
public class CartService
{
    private readonly AppDBContext _dbContext;
    private readonly ILogger<UserService> _logger;

    private readonly IMapper _mapper;

    public CartService(AppDBContext dbContext, ILogger<UserService> logger, IMapper mapper)
    {
        _dbContext = dbContext;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<Cart>> GetCartItemsAsync(Guid userId)
    {
        return await _dbContext.Carts.Include(c => c.Products).Where(c => c.UserID == userId).ToListAsync();
    }

    public async Task<bool> AddToCartAsync(Guid productId, Guid userId)
    {
        var existingCart = await _dbContext.Carts
            .FirstOrDefaultAsync(c => c.UserID == userId);

        var newCart = new Cart
        {
            // ProductId = productId,
            UserID = userId
        };
        if (existingCart == null)
        {
            _dbContext.Carts.Add(newCart);
            await _dbContext.SaveChangesAsync();
        }

        var product = await _dbContext.Products.FindAsync(productId);

        if (existingCart != null && product != null)
        {

            existingCart.Products.Add(product);
            await _dbContext.SaveChangesAsync();
            return true;
        }
        return false;
    }

    // public async Task<bool> RemoveFromCartAsync(Guid productId, Guid userId)
    // {
    //     var cartItem = await _dbContext.Carts
    //         .Where(c => c.Products.Any(p => p.ProductID == productId && c.UserID == userId))
    //         .FirstOrDefaultAsync();

    //     if (cartItem == null)
    //     {
    //         // Product not found in the user's cart
    //         return false;
    //     }

    //     _dbContext.Carts.Remove(cartItem);
    //     await _dbContext.SaveChangesAsync();
    //     return true;
    // }

    public async Task<bool> RemoveProductFromCart(Guid userId, Guid productId)
    {
        // Find the cart that belongs to the given userId
        var cart = await _dbContext.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.UserID == userId);

        // If the cart is not found
        if (cart == null)
        {
            return false;
        }

        // Find the product-cart relationship that matches the given productId and userId
        var productCartRelationship = cart.Products.FirstOrDefault(p => p.ProductID == productId);

        // If the product-cart relationship is not found
        if (productCartRelationship == null)
        {
            return false;
        }

        // Remove the product-cart relationship
        cart.Products.Remove(productCartRelationship);

        // Save the changes to the database
        await _dbContext.SaveChangesAsync();

        return true;
    }
}
