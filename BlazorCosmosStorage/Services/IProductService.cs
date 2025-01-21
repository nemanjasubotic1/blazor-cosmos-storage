
using BlazorCosmosStorage.Models;

namespace BlazorCosmosStorage.Services;

public interface IProductService
{
    Task AddItemAsync(Product product);
    Task<IEnumerable<Product>> GetAllAsync(string queryString);
    Task<Product> GetProductAsync(string id, string partitionKey);
    Task UpdateProductAsync(string partitionKey, Product product);
    Task<bool> DeleteProductAsync(string id, string partitionKey);
}
