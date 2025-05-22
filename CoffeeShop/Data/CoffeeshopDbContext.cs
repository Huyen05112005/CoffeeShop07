using Microsoft.AspNetCore.Mvc;
using CoffeeShop.Models;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.Data
{
	public class CoffeeShopDbContext : DbContext
	{
		public CoffeeShopDbContext(DbContextOptions<CoffeeShopDbContext> options) : base(options)
		{

		}
		public DbSet<Product> Products { get; set; }
		//seed data
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<Product>()
				.Property(p => p.Price)
				.HasColumnType("decimal(18,2)");
		}
	}
}

