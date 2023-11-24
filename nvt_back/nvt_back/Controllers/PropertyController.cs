using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/property")]
    public class PropertyController : Controller
    {
        private readonly IPropertyService _propertyService;
        private readonly IPropertyRepository _propertyRepository;

        public PropertyController(IPropertyService propertyService, IPropertyRepository propertyRepository)
        {
            _propertyService = propertyService;
            _propertyRepository = propertyRepository;
        }

        [HttpGet]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
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
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public async Task<ActionResult<IEnumerable<Property>>> GetPropertyPaginated(int page, int size)
        {
            Console.WriteLine(_user.Id);

            try
            {
                PageResultDTO<PropertyDTO> res;
                if (_user.Role == UserRole.USER)
                {
                    res = this._propertyService.GetAllPaginated(page, size, _user.Id);
                }
                else
                {
                    res = this._propertyService.GetAllPaginated(page, size);

                }
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public async Task<ActionResult<MessageDTO>> AddProperty(AddPropertyDTO dto)
        {
            Console.WriteLine(dto);
            try
            {
                if (dto == null)
                {
                    return BadRequest("Invalid data");
                }

                this._propertyService.AddProperty(dto, _user.Id);

                return Ok(new MessageDTO("success"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpPut]
        [Route("accept/{id}")]
        [Authorize(Roles = "ADMIN, SUPERADMIN")]
        public async Task<ActionResult<MessageDTO>> AcceptProperty(int id)
        {
            try
            {
                this._propertyService.AcceptProperty(id);
                return Ok(new MessageDTO("Property accepted successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
    }
}
