using PeoplesInfo.Models;

namespace infoapi.Interfaces
{
    public interface IRepository
    {
        Task<List<DTOCountry>> GetCountries(string? country);
        Task<List<DTOState>> GetStates(int? countryId);
        Task<List<DTOCity>> GetAllCities(int? stateid);

    }
}
