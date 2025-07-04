using System.Net;
using Asp.Versioning;
using MagicVillaApi.Models;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaApi.Controllers
{

    [Route("api/v{version:apiVersion}/userAuth")]
    [ApiController]

    //[ApiVersionNeutral]
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
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]


        public async Task<IActionResult> Login([FromBody] LoginRequestDTO model)
        {
            var tokenDto = await _userRepo.Login(model);

            if (tokenDto == null || string.IsNullOrEmpty(tokenDto.AcessToken))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;

                _apiResponse.ErrorMessage.Add("username or password is incorrect");
                return BadRequest(_apiResponse);
            }

            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            _apiResponse.Result = tokenDto;
            return Ok(_apiResponse);
        }


        [HttpPost("register")]
        [ApiVersion("1.0")]
        [ApiVersion("2.0")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDTO model)
        {

            if (model == null || string.IsNullOrWhiteSpace(model.Email))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage.Add("Invalid registration data");
                return BadRequest(_apiResponse);
            }

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
