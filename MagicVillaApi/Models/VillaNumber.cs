using System;
using System.Collections.Generic;

namespace MagicVillaApi.Models;

//public partial class VillaNumber
//{

//    public int VillaNo { get; set; }
//    public int VillaId { get; set; }

//    public string VillaDetails { get; set; } = null!;

//    public virtual VillaList Villa { get; set; } = null!;
//}


public class VillaNumber
{
    public int Id { get; set; } 
    public int VillaNo { get; set; }
    public int VillaId { get; set; }
    public string VillaDetails { get; set; } 

    public VillaList Villa { get; set; }

    //public  DateTime CreateDate { get; set; } = DateTime.Now;
    //public DateTime? UpdateDate { get; set; } = null;
}
