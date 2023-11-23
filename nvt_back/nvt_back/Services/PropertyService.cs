using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.DTOs;
using System.Transactions;

namespace nvt_back.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IImageService _imageService;
        private readonly IAddressRepository _addressRepository;

        public PropertyService(IPropertyRepository propertyRepository, IImageService imageService, IAddressRepository addressRepository)
        {
            this._propertyRepository = propertyRepository;
            this._imageService = imageService;
            this._addressRepository = addressRepository;
        }
        public void AddProperty(AddPropertyDTO dto)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    string filePath = this._imageService.SaveImage(dto.Image);

                    Property newProperty = new Property
                    {
                        Name = dto.Name,
                        Area = dto.Area,
                        NumOfFloors = dto.NumOfFloors,
                        ImagePath = filePath,
                        Address = new Address
                        {
                            Lat = dto.Address.Lat,
                            Lng = dto.Address.Lng,
                            Name = dto.Address.Name,
                            CityId = dto.Address.CityId
                        },
                        UserId = 3,
                        Status = PropertyStatus.PENDING
                    };

                    this._propertyRepository.Add(newProperty);

                    transaction.Complete();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            
        }

        private int getFilteredPage(int page, int count, int size)
        {
            int maxPages = (int)Math.Ceiling((double)count / size);

            page = page > 0 ? page : 0;
            page = page > maxPages ? maxPages : page;

            return page;
        }

        public PageResultDTO<PropertyDTO> GetAllPaginated(int page, int size)
        {
            int id = 3;
            int count = this._propertyRepository.GetCountForOwner(id);
            page = this.getFilteredPage(page, count, size);

            IEnumerable<Property> properties = this._propertyRepository.GetAllPaginatedForOwner(page, size, id);

            PageResultDTO<PropertyDTO> result = new PageResultDTO<PropertyDTO>();
            result.Count = count;
            result.PageIndex = page;
            result.PageSize = size;
            foreach (Property property in properties)
            {
                result.Items.Add(new PropertyDTO
                {
                    Id = property.Id,
                    Area = property.Area,
                    Image = this._imageService.GetBase64StringFromImage(property.ImagePath),
                    Address = new ReturnedAddressDTO
                    {
                        Id = property.Address.Id,
                        Lat = property.Address.Lat,
                        Lng = property.Address.Lng,
                        Name = property.Address.Name,
                        City = property.Address.City.Name, 
                        Country = property.Address.City.Country.Name
                    },
                    Name = property.Name,
                    NumOfFloors = property.NumOfFloors,
                    Status = property.Status.ToString()
                });
            }

            return result;
        }
    }
}
