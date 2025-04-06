﻿using AutoMapper;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Models;
using MagicVillaApi.Repository.IRepository;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaNumberAPIController : ControllerBase
    {
        private readonly ILogger<VillaAPIController> _logger;
        //private readonly MagicVillaContext _dbData;
        private readonly IMapper _mapper;
        private readonly IVillaNumberRepository _dbVillaNumber;
        protected APIResponse _apiResponse;



        public VillaNumberAPIController(ILogger<VillaAPIController> logger, IMapper mapper, IVillaNumberRepository dbVillaNumber)
        {
            _logger = logger;

            _mapper = mapper;
            _dbVillaNumber = dbVillaNumber;
            this._apiResponse = new();
        }

        [HttpGet]

        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<APIResponse>> GetVillaNumber()
        {
            _logger.LogInformation("Fetching villa list...");

            var villas = await _dbVillaNumber.GetAllAsync();

            _apiResponse.Result = _mapper.Map<List<VillaNumberDto>>(villas);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }



        [HttpGet("{id:int}", Name = "GetVillaNumberById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<APIResponse>> GetVillaNumberById(int id)
        {
            var villa = await _dbVillaNumber.GetAsync(u => u.VillaId == id);

            if (villa == null)
            {
                _logger.LogError($"Villa with ID {id} not found.");

                _apiResponse.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VillaNumberDto>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<APIResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDto createDto)
        {
            if (createDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var existingVilla = await _dbVillaNumber.GetAsync(u => u.VillaId == createDto.VillaId);

            if (existingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
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
        public async Task<ActionResult<APIResponse>> DeleteVillaNumber(int id)
        {


            if (id == 0)
            {
                return BadRequest();
            }


            var DeleteVillaNumber = await _dbVillaNumber.GetAsync(u => u.VillaId == id);

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
        public async Task<ActionResult<APIResponse>> UpdateVillaNumber(int id, [FromBody] VillaNumberUpdateDto updateDto)
        {



            if (updateDto == null || id != updateDto.VillaId)
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
