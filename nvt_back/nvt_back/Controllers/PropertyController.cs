using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/property")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertyController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }


        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddProperty(AddPropertyDTO dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest("Invalid data");
                }

                this._propertyService.AddProperty(dto);

                return Ok(new MessageDTO("success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
    }
}
