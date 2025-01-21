using Microsoft.AspNetCore.Identity;

namespace BlazorCosmosStorage.Models;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; }
}
