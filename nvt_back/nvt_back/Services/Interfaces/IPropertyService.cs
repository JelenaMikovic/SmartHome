﻿using nvt_back.DTOs;

namespace nvt_back.Services.Interfaces
{
    public interface IPropertyService
    {
        void AddProperty(AddPropertyDTO dto, int id);
        PageResultDTO<PropertyDTO> GetAllPaginated(int page, int size, int id);
        PageResultDTO<PropertyDTO> GetAllPaginated(int page, int size);
        void AcceptProperty(int id);
        void DenyProperty(int id, ReasonDTO reasonDTO);
        public Task<PropertyDTO> GetDetailsById(int id);
    }
}
