using TypicalTechTools.Models;

namespace TypicalTechTools.Models.Repository
{
    public interface IProductRepository
    {
        List<Product> GetAllProducts();
        Product GetProduct(string productCode);
        void CreateProduct(Product product);
        void UpdateProduct(Product product);
        
        void UpdateProductPrice(string productId, decimal newPrice);
    }
}
