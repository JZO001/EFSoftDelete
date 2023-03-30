using EFSoftDelete.Models;
using Microsoft.EntityFrameworkCore;

namespace EFSoftDelete.Infrastructure.Persistence
{

    public sealed class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions option) : base(option)
        {
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

    }

}
