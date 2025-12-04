namespace MagicVillaApi.Models.DTO
{
    public class LoginResponseDto
    {
        public LocalUser? user { get; set; }
        public string?  Token { get; set; }
    }
}
