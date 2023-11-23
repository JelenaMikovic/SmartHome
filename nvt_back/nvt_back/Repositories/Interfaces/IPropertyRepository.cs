using nvt_back.DTOs;

namespace nvt_back.Repositories.Interfaces
{
    public interface IPropertyRepository
    {
        void Add(Property property);
        IEnumerable<Property> getAll();
        IEnumerable<Property> GetAllPaginated(int page, int size);
        int GetCount();
        int GetCountForOwner(int id);
        IEnumerable<Property> GetAllPaginatedForOwner(int page, int size, int id);
    }
}
