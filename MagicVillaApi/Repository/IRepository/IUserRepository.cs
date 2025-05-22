using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueEmail (string email);

        Task<LoginResponseDTO> Login (LoginRequestDTO loginRequestDTO);

        Task<LocalUser> Registration (RegistrationRequestDTO registrationRequestDTO);


    }
}
