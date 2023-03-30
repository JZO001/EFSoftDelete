using EFSoftDelete.Infrastructure.Persistence.Configurations;
using EFSoftDelete.Models;
using Microsoft.EntityFrameworkCore;

namespace EFSoftDelete.Infrastructure.Persistence
{

    public class AuditReaderDbContext : DbContext
    {

        public AuditReaderDbContext(DbContextOptions<AuditReaderDbContext> option) : base(option)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new UserDbConfig(false));
        }

    }

}
