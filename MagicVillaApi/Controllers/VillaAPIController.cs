using AutoMapper;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Azure;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaAPIController : ControllerBase
    {

        private readonly ILogger<VillaAPIController> _logger;
        //private readonly MagicVillaContext _dbData;
        private readonly IMapper _mapper;
        private readonly IvillaRepository _dbVilla;
        protected APIResponse _apiResponse;



        public VillaAPIController(ILogger<VillaAPIController> logger, IMapper mapper, IvillaRepository dbVilla)
        {
            _logger = logger;

            _mapper = mapper;
            _dbVilla = dbVilla;
            this._apiResponse = new();
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVilla()
        {
            try
            {
                _logger.LogInformation("Fetching villa list...");

                var villas = await _dbVilla.GetAllAsync();

                _apiResponse.Result = _mapper.Map<List<VillaDto>>(villas);
                _apiResponse.StatusCode = HttpStatusCode.OK;

                return Ok(_apiResponse);
            }

            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessage = new List<string>() { ex.ToString() };
            }
            return _apiResponse;
        }



        [HttpGet("{id:int}", Name = "GetVillaById")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVillaById(int id)
        {
            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                _logger.LogError($"Villa with ID {id} not found.");

                _apiResponse.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VillaDto>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody] VillaCreateDto createDto)
        {
            if (createDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var existingVilla = await _dbVilla.GetAsync(u => u.Name.ToLower() == createDto.Name.ToLower());

            if (existingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }


            VillaList NewVilla = _mapper.Map<VillaList>(createDto);


            //Villa model = new()
            //{
            //    Amenity = createDTO.Amenity,
            //    Details = createDTO.Details,
            //    ImageUrl = createDTO.ImageUrl,
            //    Name = createDTO.Name,
            //    Occupancy = createDTO.Occupancy,
            //    Rate = createDTO.Rate,
            //    Sqft = createDTO.Sqft
            //};




            await _dbVilla.CreateAsync(NewVilla);

            _apiResponse.Result = _mapper.Map<VillaDto>(NewVilla);
            _apiResponse.StatusCode = HttpStatusCode.Created;



            return CreatedAtRoute("GetVillaById", new { id = NewVilla.Id }, _apiResponse);
        }




        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        [Authorize(Roles = "CUSTOM")]

        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {


            if (id == 0)
            {
                return BadRequest();
            }


            var DeleteVilla = await _dbVilla.GetAsync(u => u.Id == id);

            if (DeleteVilla == null)
            {
                return NotFound();
            }


            await _dbVilla.DeleteAsync(DeleteVilla);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;



            return Ok(_apiResponse);
        }


        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromBody] VillaUpdateDto updateDto)
        {



            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }


            VillaList UpdateVilla = _mapper.Map<VillaList>(updateDto);

            await _dbVilla.UpdateAsync(UpdateVilla);

            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;



            return Ok(_apiResponse);


        }


        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaUpdateDto> patchDto)
        {
            if (patchDto == null || id == 0)
            {
                return BadRequest();
            }
            var villa = await _dbVilla.GetAsync(u => u.Id == id, tracked: false);

            VillaUpdateDto villaDTO = _mapper.Map<VillaUpdateDto>(villa);


            if (villa == null)
            {
                return BadRequest();
            }

            patchDto.ApplyTo(villaDTO, ModelState);
            VillaList model = _mapper.Map<VillaList>(villaDTO);

            await _dbVilla.UpdateAsync(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }

    }

}
