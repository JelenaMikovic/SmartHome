using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs.DeviceRegistration;
using nvt_back.DTOs;
using nvt_back.Services.Interfaces;
using System;
using System.Threading.Tasks;
using Quartz;
using static Quartz.Logging.OperationName;
using nvt_back.Model.Devices;
using System.Security.Cryptography.X509Certificates;
using nvt_back.DTOs.DeviceDetailsDTO;
using nvt_back.Services;
using System.Collections.Generic;

namespace nvt_back.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : Controller
    {
        private readonly IDeviceService _deviceService;
        private readonly IDeviceDetailsService _deviceDetailsService;
        //private readonly IScheduler _scheduler;

        public DeviceController(IDeviceService deviceService, IDeviceDetailsService deviceDetailsService) //IScheduler scheduler)
        {
            // _scheduler = scheduler;
            _deviceService = deviceService;
            _deviceDetailsService = deviceDetailsService;
        }

        [HttpPost]
        [Route("ambient-reports")]
        public async Task<ActionResult<MessageDTO>> GetAmbientSensorReport(ReportDTO ambientSensorReportDTO)
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

        [HttpPost]
        [Route("lamp-reports")]
        public async Task<ActionResult<MessageDTO>> GetLampReport(ReportDTO lampReportDTO)
        {
            try
            {
                Console.WriteLine(lampReportDTO.DeviceId);
                var brightness = this._deviceService.GetBrightnessLevelData(lampReportDTO);
                var response = new
                {
                    BrightnessData = brightness.Result.ToList(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost]
        [Route("power-consumption")]
        public async Task<ActionResult<MessageDTO>> GetPowerConsumptionReport(ReportDTO batteryReportDTO)
        {
            try
            {
                var powerConsumption = this._deviceService.GetPowerConsumption(batteryReportDTO);
                var response = new
                {
                    ConsumptionData = powerConsumption.Result.ToList(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet]
        [Route("action-table/{id}")]
        public async Task<ActionResult<MessageDTO>> GetActionTableData(int id)
        {
            try
            {
                Console.WriteLine(id);
                var table = this._deviceService.GetActionTableData(id);
                var response = new
                {
                    TableData = table.Result.ToList(),
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSchedule([FromBody] ScheduleItemDTO scheduleItem)
        {
            _deviceService.AddSchedule(scheduleItem);
            try
            {
                //var jobStart = JobBuilder.Create<Job>()
                //    .UsingJobData("ScheduleId", schedule.Id)
                //    .UsingJobData("Action", "On")
                //    .UsingJobData("Mode", scheduleItem.Mode)
                //    .UsingJobData("Temperature", scheduleItem.Temperature)
                //    .Build();

                //System.TimeOnly systemStartTime = System.TimeOnly.Parse(scheduleItem.StartTime);
                //Quartz.TimeOfDay quartzStartTime = new Quartz.TimeOfDay(systemStartTime.Hour, systemStartTime.Minute, systemStartTime.Second);

                //var trigger = TriggerBuilder.Create()
                //    .WithDailyTimeIntervalSchedule(x => x.OnEveryDay().StartingDailyAt(quartzStartTime))
                //    .Build();

                //await _scheduler.ScheduleJob(jobStart, trigger);

                //var jobEnd = JobBuilder.Create<Job>()
                //    .UsingJobData("ScheduleId", scheduleItem.ScheduleId)
                //    .UsingJobData("Action", "Off")
                //    .Build();
                //
                //System.TimeOnly systemEndTime = System.TimeOnly.Parse(scheduleItem.EndTime);
                //Quartz.TimeOfDay quartzEndTime = new Quartz.TimeOfDay(systemEndTime.Hour, systemEndTime.Minute, systemEndTime.Second);

                //var triggerEnd = TriggerBuilder.Create()
                //    .WithDailyTimeIntervalSchedule(x => x.OnEveryDay().StartingDailyAt(quartzEndTime))
                //    .Build();
                //
                //await _scheduler.ScheduleJob(jobEnd, triggerEnd);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpDelete("remove/{scheduleId}")]
        public async Task<IActionResult> RemoveSchedule(int scheduleId)
        {
            try
            {
                _deviceService.RemoveSchedule(scheduleId);
                //var jobKeyStart = new JobKey($"Job-{scheduleId}-On");
                //var jobKeyEnd = new JobKey($"Job-{scheduleId}-Off");

                //await _scheduler.DeleteJob(jobKeyStart);
                //await _scheduler.DeleteJob(jobKeyEnd);

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("schedules/{deviceId}")]
        public async Task<ActionResult<MessageDTO>> GetSchedules(int deviceId)
        {
            try
            {
                List<AirConditionerSchedule> data = _deviceService.GetDeviceSchedule(deviceId);
                var response = new
                {
                    Schedules = data,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }

        }

        [HttpPost("shared")]
        public async Task<ActionResult<MessageDTO>> AddSharedDevice(SharedDeviceDTO dto)
        {
            try
            {
                _deviceService.AddSharedDevice(dto, _user.Id);
                var response = new
                {

                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPut("shared/accept/{id}")]
        public async Task<ActionResult<MessageDTO>> AcceptSharedDevice(int id)
        {
            try
            {
                _deviceService.UpdateSharedDevice(id, SharedStatus.ACCEPTED);
                return Ok(id);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPut("shared/deny/{id}")]
        public async Task<ActionResult<MessageDTO>> DenySharedDevice(int id)
        {
            try
            {
                _deviceService.UpdateSharedDevice(id, SharedStatus.DENIED);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("shared/properties")]
        public async Task<ActionResult<List<PropertyDTO>>> GetUserSharedProperties()
        {
            try
            {
                List<PropertyDTO> res = await _deviceDetailsService.GetSharedPropertyDetails(_user.Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpGet("shared/devices")]
        public async Task<ActionResult<List<DeviceDetailsDTO>>> GetUserSharedDevices()
        {
            try
            {
                List<DeviceDetailsDTO> res = await _deviceDetailsService.GetSharedDevicesDetails(_user.Id);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpGet("shared")]
        public async Task<ActionResult<List<SharedDevicesDTO>>> GetSharedDevice()
        {
            try
            {
                return Ok(await _deviceDetailsService.GetSharedDevice(_user.Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }

        [HttpGet("shared/requests")]
        public async Task<ActionResult<List<SharedDevicesDTO>>> GetSharedRequests()
        {
            try
            {
                //TO DO:
                return Ok(await _deviceDetailsService.GetSharedDeviceRequests(_user.Id));
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error. " + ex.Message);
            }
        }
    }
}
