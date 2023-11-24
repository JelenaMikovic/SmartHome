using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.DTOs.DeviceCommunication;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device-activation")]
    public class DeviceActivationController : ControllerBase
    {
        private readonly IDeviceActivationService _deviceActivationService;

        public DeviceActivationController(IDeviceActivationService deviceActivationService)
        {
            _deviceActivationService = deviceActivationService;
        }

        [HttpPut]
        [Route("{deviceId}/on")]
        public async Task<ActionResult<MessageDTO>> ActivateDevice(int deviceId)
        {
            try
            {
                this._deviceActivationService.ActivateDevice(deviceId);
                return Ok(new MessageDTO("You have successfully activated device with id: " + deviceId.ToString()));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPut]
        [Route("{deviceId}/off")]
        public async Task<ActionResult<MessageDTO>> DeactivateDevice(int deviceId)
        {
            try
            {
                this._deviceActivationService.DeactivateDevice(deviceId);
                return Ok(new MessageDTO("You have successfully deactivated device with id: " + deviceId.ToString()));
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
