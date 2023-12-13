namespace nvt_back.Services.Interfaces
{
    public interface IDeviceStateService
    {
        public Task<bool> Toggle(int id, string status);
    }
}
