using Newtonsoft.Json;

namespace BlazorCosmosStorage.Models;

public class Product
{
    [JsonProperty(PropertyName = "id")]
    public string Id { get; set; }

    [JsonProperty(PropertyName = "Name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "Description")]
    public string Description { get; set; }

    [JsonProperty(PropertyName = "ImageUrl")]
    public string ImageUrl { get; set; }

    [JsonProperty(PropertyName = "UserId")]
    public string UserId  { get; set; }


    [JsonProperty(PropertyName = "mainkey")] // partition key
    public string MainKey { get; set; }
}
