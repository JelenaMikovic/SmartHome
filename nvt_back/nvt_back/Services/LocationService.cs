using nvt_back.DTOs;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;

namespace nvt_back.Services
{
    public class LocationService : ILocationService
    {
        private ICityRepository _cityRepository;

        public LocationService(ICityRepository cityRepository)
        {
            this._cityRepository = cityRepository;
        }

        public IEnumerable<LocationDTO> GetAll()
        {
            List<LocationDTO> locations = new List<LocationDTO>();   
            foreach (City city in this._cityRepository.GetAll()) 
            {
                locations.Add(new LocationDTO
                {
                    CityId = city.Id,
                    Location = city.Name + ", " + city.Country.Name
                });
            }

            return locations;
        }
    }
}
