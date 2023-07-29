namespace Stockband.Domain.Common;

public class MemoryAuditEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}