using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Data
{
    public class CoffeeShopDbContext : DbContext
    {
        public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options) :
        base(options)
        {
        }
        public DbSet<Product> Products { get; set; }
    }
}

