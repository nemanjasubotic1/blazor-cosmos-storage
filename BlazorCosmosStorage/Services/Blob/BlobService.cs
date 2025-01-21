using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Azure.Storage;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.FeatureManagement;

namespace BlazorCosmosStorage.Services.Blob;

public class BlobService
{
    private readonly BlobContainerClient _containerClient;
    private readonly IConfiguration _configuration;
    private readonly IFeatureManager _featureManager;

    public BlobService(BlobServiceClient blobServiceClient, IConfiguration configuration, IFeatureManager featureManager)
    {
        _configuration = configuration;
        _featureManager = featureManager;


        if (featureManager.IsEnabledAsync("LocalSettings").GetAwaiter().GetResult())
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(configuration["BlobService:ContainerName"]);
        }
        else
        {
            _containerClient = blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("BlobService__ContainerName"));
        }
    }

    public async Task UploadFileAsync(string fullPath, IBrowserFile file)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(fullPath);

        try
        {
            using var stream = file.OpenReadStream();
            await blobClient.UploadAsync(stream, true);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error uploading file: " + ex.Message);
            throw;
        }
    }

    public string GetFileWithSas(string fullPath)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(fullPath);

        BlobSasBuilder sasBuilder = new()
        {
            BlobContainerName = _containerClient.Name,
            BlobName = fullPath,
            Resource = "b",
            StartsOn = DateTimeOffset.UtcNow.AddHours(0),
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
        };

        sasBuilder.SetPermissions(BlobAccountSasPermissions.All);

        var accountName = "";
        var accountKey = "";

        if (_featureManager.IsEnabledAsync("LocalSettings").GetAwaiter().GetResult())
        {
            accountName = _configuration["BlobService:AccountName"];
            accountKey =_configuration["BlobService:AccountPrimaryKey"];
        }
        else
        {
           accountName = Environment.GetEnvironmentVariable("BlobService__AccountName");
           accountKey = Environment.GetEnvironmentVariable("BlobService__AccountPrimaryKey");
        }


        var sasToken = sasBuilder.ToSasQueryParameters(new StorageSharedKeyCredential(
            accountName, accountKey)).ToString();

        var fullUrlWithSasToken = $"{blobClient.Uri}?{sasToken}";

        return fullUrlWithSasToken;
    }


    public async Task DeleteFileAsync(string fullPath)
    {
        BlobClient blobClient = _containerClient.GetBlobClient(fullPath);

        try
        {
            await blobClient.DeleteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting blob: {ex.Message}");
            throw;
        }
    }
}
