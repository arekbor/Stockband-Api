using System.ComponentModel.DataAnnotations;

namespace Stockband.Domain.Enums;

public enum TrackGenre
{
    [Display(Name = "None")]
    None,
    
    [Display(Name = "Alternative Rock")]
    AlternativeRock,
    
    [Display(Name = "Rock")]
    Rock,
    
    [Display(Name = "Pop")]
    Pop,
    
    [Display(Name = "Ambient")]
    Ambient,
    
    [Display(Name = "Disco")]
    Disco,
    
    [Display(Name = "House")]
    House,
    
    [Display(Name = "Electronic")]
    Electronic,
    
    [Display(Name = "Folk")]
    Folk,
    
    [Display(Name = "Indie")]
    Indie,
    
    [Display(Name = "Soundtrack")]
    Soundtrack,
    
    [Display(Name = "Reggae")]
    Reggae,
    
    [Display(Name = "Metal")]
    Metal,
    
    [Display(Name = "Latin")]
    Latin,
    
    [Display(Name = "Jazz")]
    Jazz,
    
    [Display(Name = "Techno")]
    Techno,
    
    [Display(Name = "Dubstep")]
    Dubstep,
    
    [Display(Name = "Trance")]
    Trance,
    
    [Display(Name = "Hip-hop")]
    Hiphop,
    
    [Display(Name = "Rap")]
    Rap,
    
    [Display(Name = "Classical")]
    Classical,
    
    [Display(Name = "Country")]
    Country,

    [Display(Name = "Electronic Dance Music (EDM)")]
    EDM,
    
    [Display(Name = "Other")]
    Other,
}