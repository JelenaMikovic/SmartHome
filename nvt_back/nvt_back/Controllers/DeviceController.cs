using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs.DeviceRegistration;
using nvt_back.DTOs;
using nvt_back.Services;
using nvt_back.Services.Interfaces;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : ControllerBase
    {
        private readonly IDeviceService _deviceService;

        public DeviceController(IDeviceService deviceService)
        {
            _deviceService = deviceService;
        }

        [HttpPost]
        [Route("reports")]
        public async Task<ActionResult<MessageDTO>> GetAmbientSensorReport(AmbientSensorReportDTO ambientSensorReportDTO)
        {
            try
            {
                Console.WriteLine(ambientSensorReportDTO.DeviceId);
                var temperature = this._deviceService.GetEnvironmentalData(ambientSensorReportDTO, "temperature");
                var humidity = this._deviceService.GetEnvironmentalData(ambientSensorReportDTO, "humidity");
                var response = new AmbientSensorReportResponseDTO
                {
                    TemperatureData = temperature.Result.ToList(),
                    HumidityData = humidity.Result.ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}
