using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Model.Devices;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device-details")]
    public class DeviceDetailsController : ControllerBase
    {
        private readonly IDeviceDetailsService _deviceDetailsService;

        public DeviceDetailsController(IDeviceDetailsService deviceDetailsService)
        {
            _deviceDetailsService = deviceDetailsService;
        }

        [HttpGet]
        [Route("paginated")]
        [Authorize(Roles = "USER, ADMIN, SUPERADMIN")]
        public async Task<ActionResult<PageResultDTO<DeviceDetailsDTO>>> GetPropertyDeviceDetails(
            [FromQuery] int propertyId,
            [FromQuery] int page, [FromQuery] int size)
        {
            try
            {
                return Ok(await _deviceDetailsService.GetPropertyDeviceDetails(propertyId, page, size));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
    }
}
