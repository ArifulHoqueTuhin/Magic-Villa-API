namespace MagicVillaApi.Models.DTO
{
    public class VillaUpdateDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Details { get; set; }
        public double Rate { get; set; }
        public int Occupancy { get; set; }
        public int SqFt { get; set; }
        public string? ImageUrl { get; set; }
        public string? Amenity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
}
}
