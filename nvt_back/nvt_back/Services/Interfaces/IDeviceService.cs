using nvt_back.DTOs.DeviceRegistration;
using nvt_back.Model.Devices;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceService 
    {
        public Task<List<Device>> GetAll();
        public Task<IEnumerable<dynamic>> GetBrightnessLevelData(ReportDTO lampReportDTO);
        public Task<IEnumerable<dynamic>> GetEnvironmentalData(ReportDTO dto, string field);
    }
}
