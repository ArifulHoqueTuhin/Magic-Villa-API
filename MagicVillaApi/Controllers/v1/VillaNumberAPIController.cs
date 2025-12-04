using AutoMapper;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;

namespace MagicVillaApi.Controllers.v1
{
    [Route("api/v{version:apiversion}/VillaNumberAPI")]

    [ApiController]
    [ApiVersion("1.0")]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNumber;
        protected ApiResponse _apiResponse;

        public VillaNumberAPIController(ILogger<VillaAPIController> logger, IMapper mapper, IVillaNumberRepository dbVillaNumber)
        {
            _logger = logger;
            _mapper = mapper;
            _dbVillaNumber = dbVillaNumber;
            _apiResponse = new();
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber()
        {
            _logger.LogInformation("Fetching villa list...");

            var villas = await _dbVillaNumber.GetAllAsync(includeProperties: "Villa");
            _apiResponse.Result = _mapper.Map<List<VillaNumberDto>>(villas);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }


        [HttpGet("{id:int}", Name = "GetVillaNumberById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse>> GetVillaNumberById(int id)
        {
            var villa = await _dbVillaNumber.GetAsync(u => u.VillaNo == id, includeProperties: "Villa");

            if (villa == null)
            {
                _logger.LogError($"Villa with ID {id} not found.");

                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;

                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VillaNumberDto>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
        {
            if (createDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            VillaNumber NewVillaNumber = _mapper.Map<VillaNumber>(createDto);
            await _dbVillaNumber.CreateAsync(NewVillaNumber);
            _apiResponse.Result = _mapper.Map<VillaNumberDto>(NewVillaNumber);
            _apiResponse.StatusCode = HttpStatusCode.Created;
            return CreatedAtRoute("GetVillaNumberById", new { id = NewVillaNumber.VillaId }, _apiResponse);
        }


        [HttpDelete("{id:int}", Name = "DeleteVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int id)
        {

            if (id == 0)
            {
                return BadRequest();
            }

            var DeleteVillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaNo == id);

            if (DeleteVillaNumber == null)
            {
                return NotFound();
            }

            await _dbVillaNumber.DeleteAsync(DeleteVillaNumber);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }


        [HttpPut("{id:int}", Name = "UpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
        {
            if (updateDto == null)
            {
                return BadRequest();
            }


            VillaNumber UpdateVilla = _mapper.Map<VillaNumber>(updateDto);
            await _dbVillaNumber.UpdateAsync(UpdateVilla);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);

        }
    }

}
