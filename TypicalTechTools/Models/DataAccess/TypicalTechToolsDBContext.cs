using TypicalTechTools.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TypicalTechTools.Models.Data
{
    public class TypicalTechToolsDBContext : DbContext
    {
        public TypicalTechToolsDBContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Product> Products { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Seeding initial data
            modelBuilder.Entity<Product>().HasData(
                new Product { ProductCode = "12345", ProductName = "Generic Headphones", ProductPrice = 84.99m, ProductDescription = "bluetooth headphones with fair battery life and a 1 month warranty", UpdatedDate = DateTime.Now },
                new Product { ProductCode = "12346", ProductName = "Expensive Headphones", ProductPrice = 149.99m, ProductDescription = "bluetooth headphones with good battery life and a 6 month warranty", UpdatedDate = DateTime.Now },
                new Product { ProductCode = "12347", ProductName = "Name Brand Headphones", ProductPrice = 199.99m, ProductDescription = "bluetooth headphones with good battery life and a 12 month warranty", UpdatedDate = DateTime.Now },
                new Product { ProductCode = "12348", ProductName = "Generic Wireless Mouse", ProductPrice = 39.99m, ProductDescription = "simple bluetooth pointing device", UpdatedDate = DateTime.Now },
                new Product { ProductCode = "12349", ProductName = "Logitech Mouse and Keyboard", ProductPrice = 73.99m, ProductDescription = "mouse and keyboard wired combination", UpdatedDate = DateTime.Now },
                new Product { ProductCode = "12350", ProductName = "Logitech Wireless Mouse", ProductPrice = 149.99m, ProductDescription = "quality wireless mouse", UpdatedDate = DateTime.Now }
            );

            modelBuilder.Entity<Comment>().HasData(
               new Comment
               {
                   commentId = 1,
                   comment_text = "This is a great product. Highly Recommended.",
                   product_code = "12345",
                   created_date = DateTime.Now,
                   UserId = 2 // User1's Id
               },
                new Comment
                {
                    commentId = 2,
                    comment_text = "Not worth the excessive price. Stick with a cheaper generic one.",
                    product_code = "12350",
                    created_date = DateTime.Now,
                    UserId = 3 // User2's Id
                },
                new Comment
                {
                    commentId = 3,
                    comment_text = "A great budget buy. As good as some of the expensive alternatives.",
                    product_code = "12345",
                    created_date = DateTime.Now,
                    UserId = 4 // User3's Id
                },
                new Comment
                {
                    commentId = 4,
                    comment_text = "Total garbage. Never buying this brand again.",
                    product_code = "12347",
                    created_date = DateTime.Now,
                    UserId = 2 // User1's Id
                }
            );
       
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    UserName = "Admin",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_1"),
                    Role = "ADMIN"
                },
               new AppUser
                {
                    Id = 2,
                    UserName = "User1",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_2"),
                    Role = "USER"
                },
                new AppUser
                {
                    Id = 3,
                    UserName = "User2",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_3"),
                    Role = "USER"
                },
                new AppUser
                {
                    Id = 4,
                    UserName = "User3",
                    Password = BCrypt.Net.BCrypt.EnhancedHashPassword("Password_4"),
                    Role = "USER"
                });
        }

    }
}
