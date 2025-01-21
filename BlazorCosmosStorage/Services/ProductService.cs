using BlazorCosmosStorage.Models;
using Microsoft.Azure.Cosmos;
using Microsoft.FeatureManagement;

namespace BlazorCosmosStorage.Services;


public class ProductService : IProductService
{
    private readonly CosmosClient _cosmosClient;

    private readonly Database _database;
    private readonly Container _container;

    public ProductService(CosmosClient cosmosClient, IConfiguration configuration, IFeatureManager featureManager)
    {
        _cosmosClient = cosmosClient;

        var databaseName = "";
        var containerName = "";

        if (featureManager.IsEnabledAsync("LocalSettings").GetAwaiter().GetResult())
        {
            databaseName = configuration["CosmosDb:DatabaseName"];
            containerName = configuration["CosmosDb:ContainerName"];
        }
        else
        {
            databaseName = Environment.GetEnvironmentVariable("CosmosDb__DatabaseName");
            containerName = Environment.GetEnvironmentVariable("CosmosDb__ContainerName");
        }

        _database = _cosmosClient.GetDatabase(databaseName);
        _container = _database.GetContainer(containerName);
    }

    public async Task AddItemAsync(Product product)
    {
        try
        {
            await _container.CreateItemAsync(product, new PartitionKey(product.MainKey));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
        }
    }

    public async Task<IEnumerable<Product>> GetAllAsync(string queryString)
    {
        try
        {
            var query = _container.GetItemQueryIterator<Product>(new QueryDefinition(queryString));
            var results = new List<Product>();

            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();
                results.AddRange([.. response]);
            }

            return results;
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");

            return Enumerable.Empty<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");

            return Enumerable.Empty<Product>();
        }

    }

    public async Task<Product> GetProductAsync(string id, string partitionKey)
    {
        try
        {
            ItemResponse<Product> response = await _container.ReadItemAsync<Product>(id, new PartitionKey(partitionKey));

            return response.Resource;
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
            return new Product();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
            return new Product();
        }
    }

    public async Task UpdateProductAsync(string partitionKey, Product product)
    {
        try
        {
            await _container.UpsertItemAsync(product, new PartitionKey(partitionKey));
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
        }
    }

    public async Task<bool> DeleteProductAsync(string id, string partitionKey)
    {
        try
        {
            await _container.DeleteItemStreamAsync(id, new PartitionKey(partitionKey));
            return true;
        }
        catch (CosmosException ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error message: {ex.Message}");
            return false;
        }
    }
}
