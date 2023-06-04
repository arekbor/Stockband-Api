namespace Stockband.Domain.Common;

public class AuditEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime ModifiedAt { get; set; } = DateTime.Now;
    public bool Deleted { get; set; } = false;
}