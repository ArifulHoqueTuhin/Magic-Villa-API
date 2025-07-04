using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;

namespace MagicVillaApi.Repository.IRepository
{
    public interface IUserRepository
    {
        bool IsUniqueEmail (string email);

        Task<TokenDTO> Login (LoginRequestDTO loginRequestDTO);

        Task<RegistrationResponseDto> Registration (RegistrationRequestDTO registrationRequestDTO);


    }
}
