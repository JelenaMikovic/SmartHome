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
        private readonly IMailService _mailService;

        public PropertyService(IPropertyRepository propertyRepository, IImageService imageService, IAddressRepository addressRepository, IMailService mailService)
        {
            this._propertyRepository = propertyRepository;
            this._imageService = imageService;
            this._addressRepository = addressRepository;
            this._mailService = mailService;
        }
        public void AddProperty(AddPropertyDTO dto, int id)
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
                        UserId = id,
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

            page = page <= 0 ? 1 : page;
            page = page > maxPages ? maxPages : page;

            return page;
        }

        public PageResultDTO<PropertyDTO> GetAllPaginated(int page, int size, int id)
        {
            int count = this._propertyRepository.GetCountForOwner(id);
            PageResultDTO<PropertyDTO> result = new PageResultDTO<PropertyDTO>();

            if (count == 0)
            {
                return result;
            }

            page = this.getFilteredPage(page, count, size);

            IEnumerable<Property> properties = this._propertyRepository.GetAllPaginatedForOwner(page, size, id);

            
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
                        Country = property.Address.City.Country.Name,
                    },
                    Name = property.Name,
                    NumOfFloors = property.NumOfFloors,
                    Status = property.Status.ToString(),
                    Owner = null
                });
            }

            return result;
        }

        public PageResultDTO<PropertyDTO> GetAllPaginated(int page, int size)
        {
            int count = this._propertyRepository.GetCount();
            PageResultDTO<PropertyDTO> result = new PageResultDTO<PropertyDTO>();

            if (count == 0)
            {
                return result;
            }

            page = this.getFilteredPage(page, count, size);

            IEnumerable<Property> properties = this._propertyRepository.GetAllPaginated(page, size);


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
                    Status = property.Status.ToString(),
                    Owner = new UserDTO
                    {
                        Id = property.Owner.Id,
                        Name = property.Owner.Name,
                        Surname = property.Owner.Surname,
                        Email = property.Owner.Email,
                        IsActivated = property.Owner.IsActivated,
                        Role = property.Owner.Role.ToString()
                    }
                });
            }

            return result;
        }

        public void AcceptProperty(int id)
        {
            Property property = this._propertyRepository.GetById(id);

            if (property != null)
            {
                if (property.Status != PropertyStatus.PENDING)
                {
                    throw new Exception("You can't update processed requests.");
                }

                property.Status = PropertyStatus.ACCEPTED;
                this._propertyRepository.Update(property);

                this._mailService.SendPropertyApprovedEmail(property.Owner.Email, property.Owner.Name, property.Name);

            }
            else
            {
                throw new Exception("Property with the given id doesn't exist");
            }
        }

        public void DenyProperty(int id, ReasonDTO reasonDTO)
        {
            Property property = this._propertyRepository.GetById(id);

            if (property != null)
            {
                if (property.Status != PropertyStatus.PENDING)
                {
                    throw new Exception("You can't update processed requests.");
                }

                property.Status = PropertyStatus.DENIED;
                property.RejectionReason = reasonDTO.Reason;
                this._propertyRepository.Update(property);

                this._mailService.SendPropertyDeniedEmail(property.Owner.Email, property.Owner.Name, property.Name, reasonDTO.Reason);
            }

            else
            {
                throw new Exception("Property with the given id doesn't exist");
            }
        }

        public async Task<PropertyDTO> GetDetailsById(int id)
        {
            Property property = await _propertyRepository.GetDetailsById(id);
            return new PropertyDTO
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
                Status = property.Status.ToString(),
                Owner = null
            };
        }


    }
}
