using System.Net;
using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaApi.Controllers
{
    [Route("api/userAuth")]

    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        protected APIResponse _apiResponse;

        public UserController(IUserRepository userRepo)
        {
            _userRepo = userRepo;
            this._apiResponse = new();


        }

        [HttpPost("login")]

        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var loginResponse = await _userRepo.Login(model);

            if (loginResponse.user == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;

                _apiResponse.ErrorMessage.Add("username or password is incorrect");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            _apiResponse.Result = loginResponse;
            return Ok(_apiResponse);
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {
            bool ifUserEmailUnique = _userRepo.IsUniqueEmail(model.Email);

            if (!ifUserEmailUnique)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;

                _apiResponse.ErrorMessage.Add("email already exist");
                return BadRequest(_apiResponse);

            }

            var user = await _userRepo.Registration(model);

            if (user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;

                _apiResponse.ErrorMessage.Add("registration failed");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }
    }
}
