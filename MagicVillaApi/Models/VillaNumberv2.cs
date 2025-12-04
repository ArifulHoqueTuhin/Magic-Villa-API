
namespace MagicVillaApi.Models;

public partial class VillaNumberv2
{
    public int VillaNo { get; set; }
    public int VillaId { get; set; }
    public string? SpecialDetails { get; set; }
    public virtual VillaList Villa { get; set; } = null!;
}
