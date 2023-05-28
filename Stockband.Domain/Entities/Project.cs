
using Stockband.Domain.Common;

namespace Stockband.Domain.Entities;

public class Project : AuditEntity
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public virtual User Owner { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}