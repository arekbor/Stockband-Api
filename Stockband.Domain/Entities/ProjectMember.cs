

using Stockband.Domain.Common;

namespace Stockband.Domain.Entities;

public class ProjectMember: AuditEntity
{
    public int MemberId { get; set; }
    public virtual User Member { get; set; }
    public int ProjectId { get; set; }
    public virtual Project Project { get; set; }
}