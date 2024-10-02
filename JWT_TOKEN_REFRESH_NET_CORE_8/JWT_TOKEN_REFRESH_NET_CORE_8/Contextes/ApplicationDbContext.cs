using JWT_TOKEN_REFRESH_NET_CORE_8.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JWT_TOKEN_REFRESH_NET_CORE_8.Contextes
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    base.OnModelCreating(builder);
        //}
        //public DbSet<ExtendedIdentityUser> ExtendedIdentityUsers { get; set; }
    }
}
