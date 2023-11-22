using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.Services;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device-registration")]
    public class DeviceRegistrationController : ControllerBase
    {
        private readonly IDeviceRegistrationService _deviceRegistrationService;

        public DeviceRegistrationController(IDeviceRegistrationService deviceRegistrationController)
        {
            _deviceRegistrationService = deviceRegistrationController;
        }

        [HttpPost]
        public async Task<ActionResult<MessageDTO>> AddLamp(DeviceDTO dto)
        {
            try
            {
                this._deviceRegistrationService.AddLamp(dto);
                return Ok(new MessageDTO("You have successfully added new lamp!"));
            } catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
