
using TypicalTechTools.Models.Data;
using Microsoft.EntityFrameworkCore;
using TypicalTechTools.Models;
using Ganss.Xss;

namespace TypicalTechTools.Models.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly TypicalTechToolsDBContext _context;
      
        // Constructor that requests the TypicalTechToolsDBContext from dependency injection
        public ProductRepository(TypicalTechToolsDBContext context)
        {
      
        
            _context = context;
        }

        public void CreateProduct(Product product)
        {
           
           
            _context.Products.Add(product);
            // Save all DbSet changes to the database
            _context.SaveChanges();
        }

     

        public List<Product> GetAllProducts()
        {
            // Access the Products DbSet and put it into a list
            // The DbSet is associated with the Products database
            return _context.Products.ToList();
        }
        public void UpdateProductPrice(string productId, decimal newPrice)
        {
            var product = _context.Products.FirstOrDefault(p => p.ProductCode == productId);
            if (product != null)
            {
                product.ProductPrice = newPrice;
                product.UpdatedDate = DateTime.Now; // Set the updated date
                _context.Products.Update(product);
                _context.SaveChanges();
            }
        }
       

        public Product GetProduct(string productCode)
        {
            // Ask the context class to access the Products DbSet and find any entries where the productCode
            // is equal to the provided value. Then get the first one (There should only technically be 1)
            return _context.Products.Where(p => p.ProductCode == productCode).FirstOrDefault();
        }

        public void UpdateProduct(Product product)
        {
            // Pass the product to the DbSet to have its details updated
            _context.Products.Update(product);
            // Apply the changes to the database
            _context.SaveChanges();
        }
       

    }
}
