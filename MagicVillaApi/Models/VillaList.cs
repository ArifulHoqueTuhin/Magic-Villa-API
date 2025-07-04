using System;
using System.Collections.Generic;

namespace MagicVillaApi.Models;

public partial class VillaList
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Details { get; set; } = null!;

    public double Rate { get; set; }

    public int SqFt { get; set; }

    public int Occupancy { get; set; }

    public string? ImageUrl { get; set; } 

    public string? ImageLocalPath { get; set; }

    public string Amenity { get; set; } = null!;

    public DateTime CreateDate { get; set; }

    public DateTime? UpdateDate { get; set; }

    public virtual ICollection<VillaNumber> VillaNumbers { get; set; } = new List<VillaNumber>();

   
}
