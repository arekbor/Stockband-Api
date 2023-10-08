using Stockband.Domain.Common;
using Stockband.Domain.Enums;

namespace Stockband.Domain.Entities;

public class Track:AuditEntity
{
    public string Title { get; set; }
    public string Artist { get; set; }
    public DateTime Release { get; set; }
    public string Description { get; set; }

    public TrackGenre TrackGenre { get; set; }
    public TrackType TrackType { get; set; }

    public bool Privacy { get; set; } //false = Public, true = Private

    public string TrackUrlPath { get; set; }
    public string CoverUrlPath { get; set; }

    public virtual User User { get; set; }
    public int UserId { get; set; }
}