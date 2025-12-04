using AutoMapper;
using MagicVillaApi.Models.DTO;
using MagicVillaApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MagicVillaApi.Repository.IRepository;
using System.Net;
using Asp.Versioning;

namespace MagicVillaApi.Controllers.v2
{
    //[Route("api/[controller]")]
    [Route("api/v{version:apiversion}/VillaNumberAPI")]
    [ApiController]
    [ApiVersion("2.0")]
    public class VillaNunberApiv2 : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IVillaNumberRepositoryv2 _dbVillaNumber1;
        protected ApiResponse _apiResponse;
        public VillaNunberApiv2(IMapper mapper, IVillaNumberRepositoryv2 dbVillaNumber1)
        {
            _mapper = mapper;
            _dbVillaNumber1 = dbVillaNumber1;
            _apiResponse = new();
        }



        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse>> GetVillaNumber()
        {
            var villas = await _dbVillaNumber1.GetAllAsync(includeProperties: "Villa");
            _apiResponse.Result = _mapper.Map<List<VilaNumberDtov2>>(villas);
            _apiResponse.StatusCode = HttpStatusCode.OK;
            return Ok(_apiResponse);
        }


        [HttpGet("{villaNo:int}/{villaId:int}", Name = "GetVillaNumber1ById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<ActionResult<ApiResponse>> GetVillaNumberById(int villaNo, int villaId)
        {
            var villa = await _dbVillaNumber1.GetAsync(u => u.VillaNo == villaNo && u.VillaId == villaId, includeProperties: "Villa");

            if (villa == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.NotFound;

                return NotFound(_apiResponse);
            }

            _apiResponse.Result = _mapper.Map<VilaNumberDtov2>(villa);
            _apiResponse.StatusCode = HttpStatusCode.OK;

            return Ok(_apiResponse);
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ApiResponse>> CreateVillaNumber([FromBody] VillaNumberCreateDtov2 createDto)
        {
            if (createDto == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                return BadRequest(_apiResponse);
            }

            var existingVilla = await _dbVillaNumber1.GetAsync(
             u => u.VillaId == createDto.VillaId && u.VillaNo == createDto.VillaNo);

            if (existingVilla != null)
            {
                ModelState.AddModelError("CustomError", "Villa already exists");
                return BadRequest(ModelState);
            }

            VillaNumberv2 NewVillaNumber = _mapper.Map<VillaNumberv2>(createDto);
            await _dbVillaNumber1.CreateAsync(NewVillaNumber);
            _apiResponse.Result = _mapper.Map<VillaNumberCreateDtov2>(NewVillaNumber);
            _apiResponse.StatusCode = HttpStatusCode.Created;

            return CreatedAtRoute("GetVillaNumberById", new { id = NewVillaNumber.VillaId }, _apiResponse);
            
        }




        [HttpDelete("{villaNo:int}/{villaId:int}", Name = "DeleteUpdateVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse>> DeleteVillaNumber(int villaNo, int villaId)
        {
            if (villaNo == 0 || villaId == 0)
            {
                return BadRequest();
            }

            var deleteVillaNumber = await _dbVillaNumber1.GetAsync(
                u => u.VillaNo == villaNo && u.VillaId == villaId);

            if (deleteVillaNumber == null)
            {
                return NotFound();
            }

            await _dbVillaNumber1.DeleteAsync(deleteVillaNumber);
            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }





        [HttpPut("{villaNo:int}/{villaId:int}", Name = "UpdateNewVillaNumber")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse>> UpdateVillaNumber(int villaNo, int villaId, [FromBody] VillaNumberUpdateDtov2 updateDto)
        {
            if (updateDto == null || villaNo != updateDto.VillaNo || villaId != updateDto.VillaId)
            {
                return BadRequest();
            }



            var existingVillaNumber = await _dbVillaNumber1.GetAsync(
                u => u.VillaNo == villaNo && u.VillaId == villaId);

            if (existingVillaNumber == null)
            {
                return NotFound();
            }

            //Villanumber1 updateVilla = _mapper.Map<Villanumber1>(updateDto);



            existingVillaNumber.SpecialDetails = updateDto.SpecialDetails;





            await _dbVillaNumber1.UpdateAsync(existingVillaNumber);

            var responseDto = _mapper.Map<VillaNumberUpdateDtov2>(existingVillaNumber);

            _apiResponse.Result = responseDto;




            _apiResponse.StatusCode = HttpStatusCode.NoContent;
            _apiResponse.IsSuccess = true;

            return Ok(_apiResponse);
        }


        //[MapToApiVersion("2.0")]
        //[HttpGet]

        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}





    }

}


