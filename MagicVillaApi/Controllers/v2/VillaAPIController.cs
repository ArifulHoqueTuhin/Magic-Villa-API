using AutoMapper;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.Http.HttpResults;

namespace MagicVillaApi.Controllers.v2
{
    [Route("api/v{version:apiversion}/VillaAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;

        private readonly IMapper _mapper;
        private readonly IvillaRepository _dbVilla;
        protected APIResponse _apiResponse;



        public VillaAPIController(ILogger<VillaAPIController> logger, IMapper mapper, IvillaRepository dbVilla)
        {
            _logger = logger;

            _mapper = mapper;
            _dbVilla = dbVilla;
            _apiResponse = new();
        }

        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //[ResponseCache(Duration = 30)]
        [ResponseCache(CacheProfileName = "Default30")]

        public async Task<ActionResult<APIResponse>> GetVilla([FromQuery(Name = "filteroccupancy")] int? occupancy, [FromQuery] string? search, int pageSize = 0, int pageNumber = 1)
        {
            try
            {
                _logger.LogInformation("Fetching villa list...");

                //var villas = await _dbVilla.GetAllAsync();

                var villas = new List<VillaList>();

                if (occupancy > 0)
                {
                    villas = await _dbVilla.GetAllAsync(u => u.Occupancy == occupancy, pageSize: pageSize,
                        pageNumber: pageNumber);
                }

                else
                {
                    villas = await _dbVilla.GetAllAsync(pageSize: pageSize,
                        pageNumber: pageNumber);
                }

                if (!string.IsNullOrEmpty(search))
                {
                    villas = villas.Where(u => u.Name.ToLower().Contains(search.ToLower())).ToList();
                }

                Pagination pagination = new() { PageNumber = pageNumber, PageSize = pageSize };

                //Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagination));



                Response.Headers.Append("X-Pagination", System.Text.Json.JsonSerializer.Serialize(pagination));



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
        //[ResponseCache(Duration = 30)]
        //[ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public async Task<ActionResult<APIResponse>> GetVillaById(int id)
        {
            var villa = await _dbVilla.GetAsync(u => u.Id == id);

            if (villa == null)
            {
                //_logger.LogError($"Villa with ID {id} not found.");

                _apiResponse.StatusCode = HttpStatusCode.NotFound;
                _apiResponse.IsSuccess = false;

                return NotFound(_apiResponse);

            }

            _apiResponse.Result = _mapper.Map<VillaDto>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromForm] VillaCreateDto createDto)
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
      
            await _dbVilla.CreateAsync(NewVilla);

            if (createDto.Image != null)
            {
                string fileName = NewVilla.Id + Path.GetExtension(createDto.Image.FileName);

                string filePath = @"wwwroot/ProductImage/" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                FileInfo file = new FileInfo(directoryLocation);

                if (file.Exists)
                {
                    file.Delete();
                }

                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                   await createDto.Image.CopyToAsync(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                NewVilla.ImageUrl = baseUrl + "/ProductImage/" + fileName;

                NewVilla.ImageLocalPath = filePath;

            }

            else
            {
                NewVilla.ImageUrl = "https://placehold.co/600x400";
            }

            await _dbVilla.UpdateAsync(NewVilla);


            _apiResponse.Result = _mapper.Map<VillaDto>(NewVilla);
            _apiResponse.StatusCode = HttpStatusCode.Created;



            return CreatedAtRoute("GetVillaById", new { id = NewVilla.Id }, _apiResponse);
        }




        [HttpDelete("{id:int}", Name = "DeleteVilla")]
        //[Authorize(Roles = "CUSTOM")]
        [Authorize(Roles = "admin")]
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
  

                if (!string.IsNullOrEmpty(DeleteVilla.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), DeleteVilla.ImageLocalPath);

                    FileInfo File = new FileInfo(oldFilePath);

                    if (File.Exists)
                    {
                        File.Delete();
                    }

                }


             await _dbVilla.DeleteAsync(DeleteVilla);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;



            return Ok(_apiResponse);
        }


        [Authorize(Roles = "admin")]
        [HttpPut("{id:int}", Name = "UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]


        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<APIResponse>> UpdateVilla(int id, [FromForm] VillaUpdateDto updateDto)
        {



            if (updateDto == null || id != updateDto.Id)
            {
                return BadRequest();
            }


            VillaList UpdateVilla = _mapper.Map<VillaList>(updateDto);


            if (updateDto.Image != null)
            {

                if(!string.IsNullOrEmpty(UpdateVilla.ImageLocalPath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), UpdateVilla.ImageLocalPath);
                    
                    FileInfo File = new FileInfo(oldFilePath);

                    if (File.Exists)
                    {
                        File.Delete();
                    }

                }
                string fileName = updateDto.Id + Path.GetExtension(updateDto.Image.FileName);

                string filePath = @"wwwroot/ProductImage/" + fileName;

                var directoryLocation = Path.Combine(Directory.GetCurrentDirectory(), filePath);

                FileInfo file = new FileInfo(directoryLocation);



                using (var fileStream = new FileStream(directoryLocation, FileMode.Create))
                {
                    await updateDto.Image.CopyToAsync(fileStream);
                }

                var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";

                UpdateVilla.ImageUrl = baseUrl + "/ProductImage/" + fileName;

                UpdateVilla.ImageLocalPath = filePath;

            }

            else
            {
                UpdateVilla.ImageUrl = "https://placehold.co/600x400";
            }

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
