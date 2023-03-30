namespace EFSoftDelete.Interfaces
{

    public interface IEntityBase
    {

        Guid Id { get; }

        bool IsDeleted { get; set; }

    }

}
