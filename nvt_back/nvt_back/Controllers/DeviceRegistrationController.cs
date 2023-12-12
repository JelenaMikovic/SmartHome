using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.DTOs.DeviceRegistration;
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
        [Route("ac")]
        public async Task<ActionResult<MessageDTO>> Add(AirConditionerRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new AC!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("ambient-sensor")]
        public async Task<ActionResult<MessageDTO>> Add(AmbientSensorRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new ambient sensor!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("evcharger")]
        public async Task<ActionResult<MessageDTO>> Add(EVChargerRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new EV charger!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("home-battery")]
        public async Task<ActionResult<MessageDTO>> Add(HomeBatteryRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new home battery!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("irrigation-system")]
        public async Task<ActionResult<MessageDTO>> Add(IrrigationSystemRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new irrigation system!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("solar-panel")]
        public async Task<ActionResult<MessageDTO>> Add(SolarPanelRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new solar panel!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("vehicle-gate")]
        public async Task<ActionResult<MessageDTO>> Add(VehicleGateRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new vehicle gate!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("washing-machine")]
        public async Task<ActionResult<MessageDTO>> Add(WashingMachineRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new washing machine!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("lamp")]
        public async Task<ActionResult<MessageDTO>> Add(LampRegistrationDTO dto)
        {
            try
            {
                await this._deviceRegistrationService.Add(dto);
                return Ok(new MessageDTO("You have successfully added new lamp!"));
            } catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
