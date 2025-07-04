namespace MagicVillaApi.Models.DTO
{
    public class VillaNumberDto
    {
        //public int Id { get; set; }
        //public int VillaId { get; set; }

        public int VillaId { get; set; }

        public int VillaNo { get; set; }

        public string? VillaDetails { get; set; }

        public VillaDto? Villa { get; set; }
    }
}
