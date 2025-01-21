using Azure.Storage.Blobs;
using BlazorCosmosStorage.Components;
using BlazorCosmosStorage.Data;
using BlazorCosmosStorage.Models;
using BlazorCosmosStorage.Services;
using BlazorCosmosStorage.Services.Blob;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Services for Identity

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
  .AddCookie(options =>
  {
      options.Cookie.HttpOnly = true;

      options.LoginPath = "/account/login";
      options.LogoutPath = "/account/logout";
      options.AccessDeniedPath = "/account/accessdenied";
  });

builder.Services.AddAuthorization();

builder.Services.AddCascadingAuthenticationState();

builder.Services.Configure<IdentityOptions>(config =>
{
    config.Password.RequireNonAlphanumeric = false;
    config.Password.RequireDigit = false;
});

// Services for Azure Cosmos Db and Azure Storage


builder.Services.AddSingleton<CosmosClient>(sp =>
{
    var featureManagement = sp.GetRequiredService<IFeatureManager>();

    var cosmosConnectionStrings = "";
    var key = "";

    if (featureManagement.IsEnabledAsync("LocalSettings").GetAwaiter().GetResult())
    {
        cosmosConnectionStrings = builder.Configuration["CosmosDb:AccountUri"];
        key = builder.Configuration.GetValue<string>("CosmosDb:Key");
    }
    else
    {
        cosmosConnectionStrings = Environment.GetEnvironmentVariable("CosmosDB__AcountUri");
        key = Environment.GetEnvironmentVariable("CosmosDB__Key");
    }

    CosmosClient cosmosClient = new CosmosClient(cosmosConnectionStrings, key);

    var lifetime = sp.GetRequiredService<IHostApplicationLifetime>();
    lifetime.ApplicationStopping.Register(() => cosmosClient.Dispose());

    return cosmosClient;
});

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddSingleton(sp =>
{
    var featureManagement = sp.GetRequiredService<IFeatureManager>();

    var storageAccount = "";

    if (featureManagement.IsEnabledAsync("LocalSettings").GetAwaiter().GetResult())
    {
        storageAccount = builder.Configuration.GetValue<string>("BlobService:StorageAccount");
    }
    else
    {
        storageAccount = Environment.GetEnvironmentVariable("BlobService__StorageAccount");
    }

    return new BlobServiceClient(storageAccount);
});

builder.Services.AddSingleton<BlobService>();

builder.Services.AddFeatureManagement();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
