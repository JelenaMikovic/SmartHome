using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Flux;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using nvt_back.Model.Devices;
using nvt_back.Mqtt;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services;
using nvt_back.Services.Interfaces;

namespace nvt_back.InfluxDB.Invocables
{
    public class DeviceActivityCheckInvocable : IInvocable
    {
        private readonly InfluxDBService _influxDBService;
        private readonly IDeviceOnlineStatusService _deviceOnlineStatusService;
        private readonly IDeviceService _deviceService;
        private readonly IMqttClientService _mqttClientService;

        public DeviceActivityCheckInvocable(InfluxDBService influxDBService, 
            IDeviceOnlineStatusService deviceOnlineStatusService,
            IDeviceService deviceService,
            IMqttClientService mqttClientService)
        {
            _influxDBService = influxDBService;
            _deviceOnlineStatusService = deviceOnlineStatusService;
            _deviceService = deviceService;
            _mqttClientService = mqttClientService;
        }


        public async Task Invoke()
        {
            try
            {
                Console.WriteLine("\nScheduled method started...");

                var devices = await _deviceService.GetAll();
                foreach (Device device in devices)
                {
                    if ((DateTime.UtcNow - device.LastHeartbeatTime).TotalSeconds > 30 && device.IsOnline)
                    {
                        await _deviceOnlineStatusService.UpdateOnlineStatus(device.Id, false);
                        await _influxDBService.WriteHeartbeatToInfluxDBForDevice(device.Id, 0);
                        await _mqttClientService.PublishDeactivatedStatus(device.Id);
                    }
                }

                Console.WriteLine("\nScheduled method finished...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}