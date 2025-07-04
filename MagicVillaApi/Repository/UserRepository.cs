using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace MagicVillaApi.Repository
{
    public class UserRepository : IUserRepository
    {

        private readonly MagicVillaContext _dbData;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private string SecretKey;

        public UserRepository(MagicVillaContext dbData, IConfiguration configuration, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _dbData = dbData;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secrets");
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public bool IsUniqueEmail(string email)
        {
            var User = _dbData.ApplicationUsers.FirstOrDefault(u => u.Email == email);

            if (User == null)
            {
                return true;
            }

            return false;
        }


        public async Task<TokenDTO> Login(LoginRequestDTO loginRequestDTO)
        {
            var user = await _dbData.ApplicationUsers.FirstOrDefaultAsync(u => u.Email.ToLower() == loginRequestDTO.Email.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);


            if (user == null || isValid == false)
            {
                return new TokenDTO()
                {
                    AcessToken = "",
                    //Message = "User Login failed"

                };

            }



            
            var accessToken = await GetAcessToken(user);

            TokenDTO tokenDto = new TokenDTO()

            {
                AcessToken = accessToken
                //UserId = user.Id,
                //Name = user.Name,
                //Email = user.Email
            
            };

            return tokenDto;
        }



        public async Task<RegistrationResponseDto> Registration(RegistrationRequestDTO registrationRequestDTO)
        {
            var newUser = new ApplicationUser
            {
                Name = registrationRequestDTO.Name,
                UserName = registrationRequestDTO.Email,
                NormalizedUserName = registrationRequestDTO.Email.ToUpper(),
                Email = registrationRequestDTO.Email,
                NormalizedEmail = registrationRequestDTO.Email.ToUpper()
            };

            try
            {
                var result = await _userManager.CreateAsync(newUser, registrationRequestDTO.Password);

                if (result.Succeeded)
                {

                    if (!_roleManager.RoleExistsAsync("admin").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("admin"));
                    }
                    if (!_roleManager.RoleExistsAsync("customer").GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole("customer"));
                    }


                    string roleToAssign = string.IsNullOrWhiteSpace(registrationRequestDTO.Roles) ? "customer" : registrationRequestDTO.Roles.ToLower();

                    if (roleToAssign != "admin" && roleToAssign != "customer")
                    {
                        roleToAssign = "customer";
                    }

                    await _userManager.AddToRoleAsync(newUser, roleToAssign);





                    //var UserToReturn = await _dbData.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == registrationRequestDTO.Email);

                    var UserToReturn = await _dbData.ApplicationUsers.FirstOrDefaultAsync(u => u.Name == registrationRequestDTO.Name);

                    return new RegistrationResponseDto()
                    {
                        IsSuccess = true,
                        Message = "User registration successful"

                    };

                    //return _mapper.Map<UserDto>(UserToReturn); JOKHON mAPPER USE HOBE


                };

            }
            catch (Exception ex)
            {
                throw new Exception("Error during registration: " + ex.Message);
            }

            return null;
        }

        private async Task<string> GetAcessToken(ApplicationUser user)
        {
           
            var roles = await _userManager.GetRolesAsync(user);

            var TokenHandler = new JwtSecurityTokenHandler();

            var Key = Encoding.ASCII.GetBytes(SecretKey);


            var TokenDescriptor = new SecurityTokenDescriptor

            {
                Subject = new ClaimsIdentity(new Claim[]

                {

                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Role, roles.FirstOrDefault())
                }),

                Expires = DateTime.UtcNow.AddDays(7),

                SigningCredentials = new(new SymmetricSecurityKey(Key), SecurityAlgorithms.HmacSha256Signature)

            };


            var Token = TokenHandler.CreateToken(TokenDescriptor);

            var tokenString = TokenHandler.WriteToken(Token);

            return tokenString;

        }
    }

}
