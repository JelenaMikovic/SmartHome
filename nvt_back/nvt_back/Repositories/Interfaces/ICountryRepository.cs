namespace nvt_back.Repositories.Interfaces
{
    public interface ICountryRepository
    {
        IEnumerable<Country> GetAll();
    }
}
