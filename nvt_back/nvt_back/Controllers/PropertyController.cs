using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/property")]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyController(IPropertyService propertyService, IPropertyRepository propertyRepository)
        {
            _propertyService = propertyService;
            _propertyRepository = propertyRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Property>>> GetProperty()
        {
            try
            {
                return Ok(this._propertyRepository.getAll());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpGet]
        [Route("paginated")]
        public async Task<ActionResult<IEnumerable<Property>>> GetPropertyPaginated(int page, int size)
        {
            try
            {
                PageResultDTO<PropertyDTO> res = this._propertyService.GetAllPaginated(page, size);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddProperty(AddPropertyDTO dto)
        {
            Console.WriteLine(dto);
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
