using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface ILocationService
    {
        IEnumerable<LocationDTO> GetAll();
    }
}
