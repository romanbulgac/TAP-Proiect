namespace DataAccess.Models
{
    /// <summary>
    /// Interface for entities that support soft deletion
    /// </summary>
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
