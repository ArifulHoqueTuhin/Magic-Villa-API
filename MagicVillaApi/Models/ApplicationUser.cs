using Microsoft.AspNetCore.Identity;

namespace MagicVillaApi.Models
{
    public class ApplicationUser :IdentityUser
    {
        public string UserName { get; set; }
    }
}
