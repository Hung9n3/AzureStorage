using AzureStorage.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureStorage.Core.Database
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }
        public DbSet<BlobContainer> BlobContainers { get; set; } = null!;
        public DbSet<BlobItem> BlobItems { get; set; } = null!;
    }
}
