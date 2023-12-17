using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface IDeviceStateService
    {
        public Task<bool> Toggle(int id, string status, int userId);
        public Task<bool> ChangeRegime(CommandDTO dto, int id);
    }
}
