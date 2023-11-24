using System;
using System.Threading.Tasks;
using Coravel.Invocable;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using InfluxDB.Client.Flux;
using InfluxDB.Client.Writes;
using Microsoft.AspNetCore.Mvc;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.InfluxDB.Invocables
{
    public class DeviceActivityCheckInvocable : IInvocable
    {
        private readonly InfluxDBService _service;
        private readonly IDeviceRepository _deviceRepository;

        public DeviceActivityCheckInvocable(InfluxDBService service, IDeviceRepository deviceRepository)
        {
            _service = service;
            _deviceRepository = deviceRepository;
        }


        public async Task Invoke()
        {
            Console.WriteLine("\nScheduled method started...");

            /*using var client = new FluxClient(
                new FluxConnectionOptions("http://localhost:8086/", "client", "password".ToCharArray(),
                FluxConnectionOptions.AuthenticationType.BasicAuthentication));*/

            var fluxQuery = "from(bucket: \"measurements\")\n" +
                            "  |> range(start: -8s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"mesonline\")\n" +
                            "  |> filter(fn: (r) => r[\"_field\"] == \"status\" and r[\"_value\"] == 0)" +
                            "  |> distinct()";

            /*client.QueryAsync(fluxQuery, record =>
            {
                // process the flux query records
                Console.WriteLine(record.GetTime() + ": " + record.GetValue());
            },
                            (error) =>
                            {
                                // error handling while processing result
                                Console.WriteLine(error.ToString());

                            }, () =>
                            {
                                // on complete
                                Console.WriteLine("Query completed");
                            }).GetAwaiter().GetResult();*/


            /* using var client = new InfluxDBClient("http://localhost:8086", "lcAh7AldxWNn_Yctss3wyFB_iNTnA32rFVJ47qfmIf2cDQ1WRjPHJTPWCDuxMkkbluR5fjx1DBosdwrjLLux9g==");
             List<Device> onlineDevices = _deviceRepository.GetOnlineDevices();
             *//*var newOfflineDevices = onlineDevices
                     .Where(device => !devicesIdAndStatus.Any(das => das.Id == device.Id.ToString()))
                     .ToList();*//*

             var fluxTables = await client.GetQueryApi().QueryAsync(fluxQuery, "nwt");
             fluxTables.ForEach(fluxTable =>
             {
                 var fluxRecords = fluxTable.Records;
                 fluxRecords.ForEach(fluxRecord =>
                 {
                     string id = fluxRecord.GetValueByKey("device_id").ToString().Substring(2);
                     int status = (int)fluxRecord.GetValueByKey("_value");
                     Console.WriteLine(id);
                 });
             }).Wait();*/

            var devicesIdAndStatus = await _service.QueryAsync(async query =>
            {
                var flux = "from(bucket: \"measurements\")\n" +
                            "  |> range(start: -8s)\n" +
                            "  |> filter(fn: (r) => r[\"_measurement\"] == \"mesonline\")\n" +
                            "  |> filter(fn: (r) => r[\"_field\"] == \"status\" and r[\"_value\"] == 0)" +
                            "  |> distinct()";

                var tables = await query.QueryAsync(flux, "nwt");
                return tables.SelectMany(table => table.Records)
                    .Select(record => new
                    {
                        Id = record.GetValueByKey("device_id").ToString().Substring(2),
                        Status = (int)(record.GetValueByKey("_field"))
                    }).ToList();
            });

            List<Device> onlineDevices = _deviceRepository.GetOnlineDevices();

            var newOfflineDevices = onlineDevices
                .Where(device => !devicesIdAndStatus.Any(das => das.Id == device.Id.ToString()))
                .ToList();

            foreach (Device newOfflineDevice in newOfflineDevices)
                _deviceRepository.ChangeOnlineStatus(newOfflineDevice.Id, false);

            Console.WriteLine("\nScheduled method finished...");
        }
    }
}