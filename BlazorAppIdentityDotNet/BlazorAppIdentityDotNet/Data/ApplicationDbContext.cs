using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlazorAppIdentityDotNet.Data.Models;

namespace BlazorAppIdentityDotNet.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<BlazorAppIdentityDotNet.Data.Models.Cerere> Cerere { get; set; } = default!;
    }
}
