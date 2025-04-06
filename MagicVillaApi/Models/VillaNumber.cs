using System;
using System.Collections.Generic;

namespace MagicVillaApi.Models;

public partial class VillaNumber
{
    public int Id { get; set; }

    public int VillaId { get; set; }

    public string VillaDetails { get; set; } = null!;

    public virtual VillaList IdNavigation { get; set; } = null!;
}
