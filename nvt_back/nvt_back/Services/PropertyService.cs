using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using nvt_back.DTOs;

namespace nvt_back.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IPropertyRepository _propertyRepository;
        private readonly IImageService _imageService;

        public PropertyService(IPropertyRepository propertyRepository, IImageService imageService)
        {
            this._propertyRepository = propertyRepository;
            this._imageService = imageService;
        }
        public void AddProperty(AddPropertyDTO dto)
        {
            string filePath = this._imageService.SaveImage(dto.Image);

            Property newProperty = new Property {
                Name = dto.Name,
                Area = dto.Area,
                NumOfFloors = dto.NumOfFloors,
                ImagePath = filePath,
                UserId = 3,
                Status = PropertyStatus.PENDING
            };

            this._propertyRepository.Add(newProperty);
        }

    }
}
