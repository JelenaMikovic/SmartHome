using Microsoft.AspNetCore.Mvc;
using nvt_back.DTOs;
using nvt_back.DTOs.DeviceDetailsDTO;
using nvt_back.Model.Devices;
using nvt_back.Repositories.Interfaces;
using nvt_back.Services.Interfaces;
using static System.Net.Mime.MediaTypeNames;

namespace nvt_back.Services
{
    public class DeviceDetailsService : IDeviceDetailsService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IPropertyRepository _propertyRepository;

        public DeviceDetailsService(IDeviceRepository deviceRepository, IImageService imageService, IUserRepository userRepository, IPropertyRepository propertyRepository)
        {
            _deviceRepository = deviceRepository;
            _imageService = imageService;
            _userRepository = userRepository;
            _propertyRepository = propertyRepository;
        }

        public async Task<object> GetById(int id)
        {
            var device = (DeviceDetailsDTO) await _deviceRepository.GetDetailsById(id);
            device.Image = _imageService.GetBase64StringFromImage(device.Image);
            return device;
        }

        public async Task<PageResultDTO<DeviceDetailsDTO>> GetPropertyDeviceDetails(int propertyId, int page, int size)
        {
            PageResultDTO<DeviceDetailsDTO> result = new PageResultDTO<DeviceDetailsDTO>();

            int count = await _deviceRepository.GetDeviceCountForProperty(propertyId);
            Console.WriteLine();
            if (count == 0)
                return result;

            page = this.getFilteredPage(page, count, size);
            IEnumerable<DeviceDetailsDTO> details = await _deviceRepository.GetPropertyDeviceDetails(propertyId, page, size);

            result.Count = count;
            result.PageIndex = page;
            result.PageSize = size;

            foreach (DeviceDetailsDTO item in details)
            {
                try
                {
                    item.Image = this._imageService.GetBase64StringFromImage(item.Image);
                } catch (Exception ex)
                {   
                    continue;
                }
                result.Items.Add(item);
            }

            return result;
        }


        public async Task<List<SharedDevicesDTO>> GetSharedDevice(int id)
        {
            List<SharedDevicesDTO> result = new List<SharedDevicesDTO>();
            List<SharedDevices> sharedDevices = await _deviceRepository.GetSharedDevicesOwner(id);

            foreach (SharedDevices dev in sharedDevices)
            {
                if(dev.Status == SharedStatus.DENIED)
                {
                    continue;
                }
                User user = await _userRepository.GetById(dev.UserId);
                Device device = dev.DeviceId != null ? await _deviceRepository.GetById(dev.DeviceId.Value) : null;
                Property property = dev.PropertyId != null ? _propertyRepository.GetById(dev.PropertyId.Value) : null;

                result.Add(new SharedDevicesDTO
                {
                    Id = dev.Id,
                    Email = user.Email,
                    PropertyName = property?.Name,
                    DeviceName = device?.Name,
                    SharedType = dev.SharedType.ToString(),
                    SharedStatus = dev.Status.ToString()
                });
            }

            return result;
        }

        public async Task<List<SharedDevicesDTO>> GetSharedDeviceRequests(int id)
        {
            List<SharedDevicesDTO> result = new List<SharedDevicesDTO>();
            List<SharedDevices> sharedDevices = await _deviceRepository.GetSharedDevces(id);

            foreach (SharedDevices dev in sharedDevices)
            {
                if (dev.Status == SharedStatus.PENDING)
                {

                    User user = await _userRepository.GetById(dev.OwnerId);
                    Device device = dev.DeviceId != null ? await _deviceRepository.GetById(dev.DeviceId.Value) : null;
                    Property property = dev.PropertyId != null ? _propertyRepository.GetById(dev.PropertyId.Value) : null;

                    result.Add(new SharedDevicesDTO
                    {
                        Id = dev.Id,
                        Email = user.Email,
                        PropertyName = property?.Name,
                        DeviceName = device?.Name,
                        SharedType = dev.SharedType.ToString(),
                        SharedStatus = dev.Status.ToString()
                    });
                }
            }

            return result;
        }


        public async Task<List<DeviceDetailsDTO>> GetSharedDevicesDetails(int userId)
        {
            List<DeviceDetailsDTO> result = new List<DeviceDetailsDTO>();
            List<SharedDevices> sharedDevices = await _deviceRepository.GetSharedDevces(userId);

            foreach (SharedDevices dev in sharedDevices)
            {
                if (dev.SharedType == SharedType.DEVICE && dev.Status == SharedStatus.ACCEPTED)
                {
                    DeviceDetailsDTO item = await _deviceRepository.GetSharedDeviceDetails((int)dev.DeviceId);

                    try
                    {
                        item.Image = this._imageService.GetBase64StringFromImage(item.Image);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }

                    result.Add(item);
                }
            }

            return result;
        }

        public async Task<List<PropertyDTO>> GetSharedPropertyDetails(int userId)
        {
            List<PropertyDTO> result = new List<PropertyDTO>();
            List<SharedDevices> sharedDevices = await _deviceRepository.GetSharedDevces(userId);
            foreach (SharedDevices dev in sharedDevices)
            {
                if (dev.SharedType == SharedType.PROPERTY && dev.Status == SharedStatus.ACCEPTED)
                {
                    Property property = dev.PropertyId != null ? await _propertyRepository.GetDetailsById(dev.PropertyId.Value) : null;
                    result.Add(new PropertyDTO
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
            }
            return result;
        }



        private int getFilteredPage(int page, int count, int size)
        {
            int maxPages = (int)Math.Ceiling((double)count / size);

            page = page <= 0 ? 1 : page;
            page = page > maxPages ? maxPages : page;

            return page;
        }
    }
}
