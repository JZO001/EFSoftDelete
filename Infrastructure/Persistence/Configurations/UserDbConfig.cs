using EFSoftDelete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFSoftDelete.Infrastructure.Persistence.Configurations
{

    public class UserDbConfig : IEntityTypeConfiguration<User>
    {

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.UserName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Age).IsRequired();

            builder.HasQueryFilter(x => !x.IsDeleted);
        }

    }

}
