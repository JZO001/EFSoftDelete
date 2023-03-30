using EFSoftDelete.Interfaces;

namespace EFSoftDelete.Models
{

    public abstract class EntityBase : IEntityBase
    {

        protected EntityBase()
        {
        }

        public Guid Id { get; init; } = Guid.NewGuid();

        public bool IsDeleted { get; set; }

    }

}
