using nvt_back.Model.Devices;

namespace nvt_back.Repositories.Interfaces
{
    public interface ILampRepository
    {
        public Task<Lamp> GetById(int id);
    }
}
