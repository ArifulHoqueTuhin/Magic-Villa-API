
namespace MagicVillaApi.Models;

public partial class LocalUser
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Roles { get; set; } = null!;
}
