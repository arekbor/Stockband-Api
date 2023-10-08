using System.ComponentModel.DataAnnotations;

namespace Stockband.Domain.Enums;

public enum TrackType
{
    [Display(Name = "Demo")]
    Demo,
    [Display(Name = "Release")]
    Release,
    [Display(Name = "Rehearsal")]
    Rehearsal,
    [Display(Name = "Jam")]
    Jam,
    [Display(Name = "Remix")]
    Remix,
    [Display(Name = "Other")]
    Other,
}