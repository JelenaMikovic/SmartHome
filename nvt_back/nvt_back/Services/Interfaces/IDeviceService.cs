﻿using nvt_back.DTOs.DeviceRegistration;
using nvt_back.Model.Devices;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceService 
    {
        public Task<List<Device>> GetAll();
        public Task<IEnumerable<dynamic>> GetBrightnessLevelData(ReportDTO lampReportDTO);
        public Task<IEnumerable<dynamic>> GetEnvironmentalData(ReportDTO dto, string field);
        Task<List<int>> GetPropertyIdsWithHomeBatteries();
        public Task<List<HomeBattery>> GetAllBatteriesForPropertyId(int propertyId);

        public Task<List<Device>> GetConsumingPowerDevicesForProperty(int propertyId);
        public Task<IEnumerable<dynamic>> GetActionTableData(int id);
    }
}
