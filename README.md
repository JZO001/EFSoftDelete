# EntityFrameworkCore Soft Delete Implementation with SaveChangesInterceptor

This repository demonstrates, how it is possible to implement soft delete with EntityFrameworkCore using SaveChangesInterceptor.
It is a real scenario, for example in an audit log, where you want to keep the deleted records for a while.
In this example, the deleted records are marked with a flag, which maintaned by the interceptor.
With the configured query filter, the deleted records are not returned by the query.

## The implementation details

This is the implementation of the interceptor, which maintain the IsDeleted flag.

```c#
    public class EntityDeleteInterceptor : SaveChangesInterceptor
    {

        public EntityDeleteInterceptor()
        {
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            HandleDeletedEntities(eventData.Context!);
            return result;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = new CancellationToken())
        {
            HandleDeletedEntities(eventData.Context!);
            return ValueTask.FromResult(result);
        }

        private void HandleDeletedEntities(DbContext context)
        {
            IEnumerable<EntityEntry> deletedEntityEntries = context.ChangeTracker
                .Entries()
                .Where(i => i.State == EntityState.Deleted)
                .Where(i => i.Entity is IEntityBase);

            foreach (EntityEntry entityEntry in deletedEntityEntries)
            {
                IEntityBase entity = (entityEntry.Entity as IEntityBase)!;
                entity.IsDeleted = true;
                entityEntry.State = EntityState.Modified;
            }
        }

    }
``` 

The second important part is the query filter, which is configured in the UserDbConfig.
```c#
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
```


You can create an additional DbContext, something like a "master",
which does not have the query filter, and use it for the audit log.
```c#
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
        // in this sample, the query filter does not configured in the UserDbConfig
		modelBuilder.ApplyConfiguration(new UserDbConfig(false));
	}
```
