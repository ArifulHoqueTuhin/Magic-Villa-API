using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueEmail (string email);
        Task<LoginResponseDto> Login (LoginRequestDto loginRequestDTO);
        Task<LocalUser> Registration (RegistrationRequestDto registrationRequestDTO);

    }
}
