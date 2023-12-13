namespace nvt_back.Services.Interfaces
{
    public interface IDeviceSimulatorInitializationService
    {
        public Task<object> Initialize(int deviceId);
    }
}
