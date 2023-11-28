using Bogus;
using TestTelerikGrid.DTOs;

namespace TestTelerikGrid.Services
{
    public class ProductService
    {
        private static List<ProductDTO> _products = new();

        public List<ProductDTO> GenerateProduct(int count, List<CategoryDTO> categories = null)
        {
            var categoryList = categories;
            if (categories == null)
                categoryList = GetCategories();

            var faker = new Faker<ProductDTO>()
            .RuleFor(u => u.Id, f => f.Random.Number(0, 200))
            .RuleFor(u => u.Name, f => f.Commerce.Product())
            .RuleFor(u => u.Price, f => Convert.ToDecimal(f.Commerce.Price(100, 10000)))
            .RuleFor(u => u.ReleaseDate, f => f.Date.Recent(10))
            .RuleFor(u => u.Active, f => Convert.ToBoolean(f.Random.Number(0, 1)))
            .RuleFor(u => u.CategoryId, f => f.PickRandom(categoryList).Id);

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

        public static List<CategoryDTO> GetCategories()
        {
            var startId = 1;

            var faker = new Faker<CategoryDTO>()
                    .RuleFor(u => u.Id, f => startId++)
                    .RuleFor(u => u.Name, f => f.Commerce.Department())
                    .RuleFor(u => u.Description, f => f.Commerce.ProductDescription());

            return faker.Generate(3);
        }
    }
}
