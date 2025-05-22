namespace MagicVillaApi.Models.DTO
{
    public class LoginResponseDTO
    {
        public LocalUser user { get; set; }

        public string  Token { get; set; }
    }
}
