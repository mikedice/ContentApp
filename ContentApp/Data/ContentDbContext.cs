using Microsoft.EntityFrameworkCore;
using ContentApp.Models;

namespace ContentApp.Data
{
    public class ContentAppDbContext : DbContext
    {
        public ContentAppDbContext (DbContextOptions options) : base(options)
        {
        }

        public DbSet<Asset> Assets { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>().ToTable("Asset");
        }
    }
}