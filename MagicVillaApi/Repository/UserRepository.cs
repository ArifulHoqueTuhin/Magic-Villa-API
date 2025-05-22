using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
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
            SecretKey = configuration.GetValue<string>("ApiSettings:Secrets");
        }

        public bool IsUniqueEmail(string email)
        {
            //var User = _dbData.Users.FirstOrDefault(u => u.Email == email);

            var User = _dbData.LocalUsers.FirstOrDefault(u => u.Email == email);

            if (User == null)
            {
                return true;
            }

            return false;
        }


        public async Task<LoginResponseDTO> Login (LoginRequestDTO loginRequestDTO)
        {
            var user = await _dbData.LocalUsers.FirstOrDefaultAsync(u => u.Email == loginRequestDTO.Email && u.Password == loginRequestDTO.Password);

            if (user == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    user = null
                };

            }

                // if user was found generate JWT Token

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

                LoginResponseDTO loginResponseDTO = new LoginResponseDTO()

                {
                    Token = TokenHandler.WriteToken(Token),
                    user = user,
                };

                return loginResponseDTO;
            }
        


        public async Task<LocalUser> Registration(RegistrationRequestDTO registrationRequestDTO)

        {
            LocalUser NewUser = new()
            {
                UserName = registrationRequestDTO.UserName,

                Email = registrationRequestDTO.Email,

                Password = registrationRequestDTO.Password,

                Roles = registrationRequestDTO.Roles
            };

            _dbData.LocalUsers.Add(NewUser);

            await _dbData.SaveChangesAsync();

            NewUser.Email = "";

            NewUser.Password = "";

            return NewUser;



        }
    }

}
