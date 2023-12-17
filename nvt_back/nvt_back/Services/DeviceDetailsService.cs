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

        public DeviceDetailsService(IDeviceRepository deviceRepository, IImageService imageService)
        {
            _deviceRepository = deviceRepository;
            _imageService = imageService;
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

        private int getFilteredPage(int page, int count, int size)
        {
            int maxPages = (int)Math.Ceiling((double)count / size);

            page = page <= 0 ? 1 : page;
            page = page > maxPages ? maxPages : page;

            return page;
        }
    }
}
