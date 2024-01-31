﻿using nvt_back.DTOs;
using nvt_back.DTOs.DeviceRegistration;
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
        public Task<IEnumerable<dynamic>> GetPowerConsumption(ReportDTO batteryReportDTO);
        void AddSchedule(ScheduleItemDTO scheduleItem);
        void RemoveSchedule(int scheduleId);
        List<AirConditionerSchedule> GetDeviceSchedule(int deviceId);
        void AddSharedDevice(SharedDeviceDTO dto, int id);
        void UpdateSharedDevice(int id, SharedStatus status);
    }
}
