using CoffeeShop.Data;
using CoffeeShop.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CoffeeShop.Models.Services
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        private readonly CoffeeShopDbContext dbContext;

        public ShoppingCartRepository(CoffeeShopDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public List<ShoppingCartItem>? ShoppingCartItems { get; set; }
        public string? ShoppingCartId { get; set; }

        public static ShoppingCartRepository GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext?.Session;
            var context = services.GetService<CoffeeShopDbContext>() ?? throw new Exception("DB context null");

            string cartId = session?.GetString("CartId") ?? Guid.NewGuid().ToString();
            session?.SetString("CartId", cartId);

            return new ShoppingCartRepository(context) { ShoppingCartId = cartId };
        }

        public void AddToCart(Product product)
        {
            var shoppingCartItem = dbContext.ShoppingCartItems.FirstOrDefault(s =>
            s.Product.Id == product.Id && s.ShoppingCartId == ShoppingCartId);
            if (shoppingCartItem == null)
            {
                shoppingCartItem = new ShoppingCartItem
                {
                    ShoppingCartId = ShoppingCartId,
                    Product = product,
                    Qty = 1,
                };
                dbContext.ShoppingCartItems.Add(shoppingCartItem);
            }
            else
            {
                shoppingCartItem.Qty++;
            }
            dbContext.SaveChanges();
        }

        public int RemoveFromCart(Product product)
        {
            var cartItem = dbContext.ShoppingCartItems
                .FirstOrDefault(i => i.Product.Id == product.Id && i.ShoppingCartId == ShoppingCartId);

            int newQty = 0;

            if (cartItem != null)
            {
                if (cartItem.Qty > 1)
                {
                    cartItem.Qty--;
                    newQty = cartItem.Qty;
                }
                else
                {
                    dbContext.ShoppingCartItems.Remove(cartItem);
                }

                dbContext.SaveChanges();
                UpdateCartCount();
            }

            return newQty;
        }

        public void ClearCart()
        {
            var items = dbContext.ShoppingCartItems
                .Where(i => i.ShoppingCartId == ShoppingCartId);

            dbContext.ShoppingCartItems.RemoveRange(items);
            dbContext.SaveChanges();
            UpdateCartCount();
        }

        public List<ShoppingCartItem> GetAllShoppingCartItems()
        {
            return ShoppingCartItems ??= dbContext.ShoppingCartItems
                .Where(i => i.ShoppingCartId == ShoppingCartId)
                .Include(i => i.Product)
                .ToList();
        }

        public decimal GetShoppingCartTotal()
        {
            return dbContext.ShoppingCartItems
                .Where(i => i.ShoppingCartId == ShoppingCartId)
                .Select(i => i.Product.Price * i.Qty)
                .Sum();
        }

        private void UpdateCartCount()
        {
            int cartCount = dbContext.ShoppingCartItems
                .Where(i => i.ShoppingCartId == ShoppingCartId)
                .Sum(i => i.Qty);

            var httpContextAccessor = new HttpContextAccessor();
            httpContextAccessor.HttpContext?.Session.SetInt32("CartCount", cartCount);
        }
    }
}
