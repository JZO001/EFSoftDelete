using EFSoftDelete.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace EFSoftDelete.Infrastructure.Interceptors
{

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

}
