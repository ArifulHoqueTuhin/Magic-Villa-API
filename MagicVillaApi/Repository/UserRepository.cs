using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MagicVillaApi.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly MagicVillaContext _dbData;
        private string SecretKey;
        public UserRepository(MagicVillaContext dbData, IConfiguration configuration)
        {
            _dbData = dbData;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secrets") ?? throw new InvalidOperationException("ApiSettings:Secrets is not configured!");
        }

        public bool IsUniqueEmail(string email)
        {

            var User = _dbData.LocalUsers.FirstOrDefault(u => u.Email == email);

            if (User == null)
            {
                return true;
            }

            return false;
        }


        public async Task<LoginResponseDto> Login(LoginRequestDto loginRequestDTO)
        {
            var user = await _dbData.LocalUsers.FirstOrDefaultAsync(u => u.Email == loginRequestDTO.Email && u.Password == loginRequestDTO.Password);

            if (user == null)
            {
                return new LoginResponseDto()
                {
                    Token = "",
                    user = null
                };

            }

            var TokenHandler = new JwtSecurityTokenHandler();
            var Key = Encoding.ASCII.GetBytes(SecretKey);
            var TokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, user.Roles)
                }),

                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            };


            var Token = TokenHandler.CreateToken(TokenDescriptor);
            LoginResponseDto loginResponseDTO = new LoginResponseDto()
            {
                Token = TokenHandler.WriteToken(Token),
                user = user,
            };

            return loginResponseDTO;
        }

        public async Task<LocalUser> Registration(RegistrationRequestDto registrationRequestDto)

        {
            LocalUser NewUser = new()
            {
                UserName = registrationRequestDto.UserName!,
                Email = registrationRequestDto.Email!,
                Password = registrationRequestDto.Password!,
                Roles = registrationRequestDto.Roles!
            };

            _dbData.LocalUsers.Add(NewUser);
            await _dbData.SaveChangesAsync();

            NewUser.Email = "";
            NewUser.Password = "";
            return NewUser;

        }
    }

}
