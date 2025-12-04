
namespace MagicVillaApi.Models;

public partial class VillaNumber
{
    public int VillaId { get; set; }
    public int VillaNo { get; set; }
    public string VillaDetails { get; set; } = null!;
    public virtual VillaList Villa { get; set; } = null!;
}
