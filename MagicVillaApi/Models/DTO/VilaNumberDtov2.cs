namespace MagicVillaApi.Models.DTO
{
    public class VilaNumberDtov2
    {
        public int VillaNo { get; set; }
        public int VillaId { get; set; }
        public string? SpecialDetails { get; set; }
        public VillaDto Villa { get; set; } = new();
    }
}
