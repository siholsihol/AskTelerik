using Bogus;
using TestTelerikGrid.DTOs;

namespace TestTelerikGrid.Services
{
    public class ProductService
    {
        private static List<ProductDTO> _products = new();

        public List<ProductDTO> GenerateProduct(int count)
        {
            var faker = new Faker<ProductDTO>()
            .RuleFor(u => u.Id, f => f.Random.Number(0, 200))
            .RuleFor(u => u.Name, f => f.Commerce.Product())
            .RuleFor(u => u.Price, f => Convert.ToDecimal(f.Commerce.Price(100, 10000)))
            .RuleFor(u => u.ReleaseDate, f => f.Date.Recent(10))
            .RuleFor(u => u.Active, f => Convert.ToBoolean(f.Random.Number(0, 1)));

            _products = faker.Generate(count);

            return GetProducts();
        }

        public List<ProductDTO> GetProducts()
        {
            return _products;
        }

        public void CreateProduct(ProductDTO itemToAdd)
        {
            _products.Add(itemToAdd);
        }

        public void UpdateProduct(ProductDTO itemToUpdate)
        {
            var index = _products.FindIndex(x => x.Id == itemToUpdate.Id);

            if (index != -1)
                _products[index] = itemToUpdate;
        }

        public void DeleteProduct(ProductDTO itemToDelete)
        {
            var index = _products.FindIndex(x => x.Id == itemToDelete.Id);

            if (index != -1)
                _products.Remove(_products[index]);
        }
    }
}
