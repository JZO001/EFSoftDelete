using EFSoftDelete.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFSoftDelete.Infrastructure.Persistence.Configurations
{

    public class UserDbConfig : IEntityTypeConfiguration<User>
    {

        private readonly bool _useAuditQueryFilter = true;

        public UserDbConfig()
        {
        }

        public UserDbConfig(bool useAuditQueryFilter)
        {
            _useAuditQueryFilter = useAuditQueryFilter;
        }

        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.Property(x => x.IsDeleted).IsRequired();
            builder.Property(x => x.UserName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Age).IsRequired();

            if (_useAuditQueryFilter) builder.HasQueryFilter(x => !x.IsDeleted);
        }

    }

}
