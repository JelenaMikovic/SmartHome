using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs.DeviceRegistration;
using nvt_back.DTOs;
using nvt_back.Services;
using nvt_back.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device-toggle")]
    public class DeviceStateController : ControllerBase
    {
        private readonly IDeviceStateService _deviceStateService;

        public DeviceStateController(IDeviceStateService deviceStateService)
        {
            _deviceStateService = deviceStateService;
        }

        [HttpPut]
        [Route("on/{id}")]
        [Authorize(Roles = "USER")]
        public async Task<ActionResult<MessageDTO>> ToggleOn(int id)
        {
            try
            {
                Console.WriteLine("tu");
                return Ok();
                bool hasStatusChanged = await this._deviceStateService.Toggle(id, "ON");
                if (hasStatusChanged)
                {
                    return Ok(new MessageDTO("You changed device status to: ON"));
                }
                else
                {
                    return BadRequest(new MessageDTO("Status hasn't changed"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("off/{id}")]
        [Authorize(Roles = "USER")]
        public async Task<ActionResult<MessageDTO>> ToggleOff(int id)
        {
            try
            {
                bool hasStatusChanged = await this._deviceStateService.Toggle(id, "OFF");
                if (hasStatusChanged)
                {
                    return Ok(new MessageDTO("You changed device status to: OFF"));
                }
                else
                {
                    return BadRequest(new MessageDTO("Status hasn't changed"));
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
