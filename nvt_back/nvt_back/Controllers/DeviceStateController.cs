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
    public class DeviceStateController : Controller
    {
        private readonly IDeviceStateService _deviceStateService;

        public DeviceStateController(IDeviceStateService deviceStateService)
        {
            _deviceStateService = deviceStateService;
        }

        [HttpGet]
        [Route("on/{id}")]
        //[Authorize(Roles = "USER")]
        public async Task<ActionResult<MessageDTO>> ToggleOn(int id)
        {
            Console.WriteLine("TU");
            var ID = _user.Id;
            Console.WriteLine("TU");
            try
            {
                bool hasStatusChanged = await this._deviceStateService.Toggle(id, "ON", _user.Id);
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

        [HttpGet]
        [Route("off/{id}")]
        //[Authorize(Roles = "USER")]
        public async Task<ActionResult<MessageDTO>> ToggleOff(int id)
        {
            try
            {
                bool hasStatusChanged = await this._deviceStateService.Toggle(id, "OFF", _user.Id);
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

        [HttpPut]
        [Route("regime")]
        public async Task<ActionResult<MessageDTO>> ChangeRegime(CommandDTO dto)
        {
            try
            {
                bool hasStatusChanged = await this._deviceStateService.ChangeRegime(dto, _user.Id);
                if (hasStatusChanged)
                {
                    return Ok(new MessageDTO("You changed device option '" + dto.Action + "' to: " + dto.Value));
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
