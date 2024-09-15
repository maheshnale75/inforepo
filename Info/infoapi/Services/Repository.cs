using infoapi.Interfaces;
using Microsoft.EntityFrameworkCore;
using PeoplesInfo;
using PeoplesInfo.Models;

namespace infoapi.Services
{
    public class Repository : IRepository
    {
        private readonly infoContext _dbcontext;
        public Repository(infoContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task<List<DTOCountry>> GetCountries(string? country)
        {



            var query = _dbcontext.Countries

                                  .AsQueryable();

            // If the country parameter is provided, filter the countries that start with the provided value
            if (!string.IsNullOrEmpty(country))
            {
                query = query.Where(c => c.CountryName.StartsWith(country));
            }

            // Execute the query and get the list of countries along with their states
            var countries = await query.ToListAsync();
            return countries;

        }
        public async Task<List<DTOState>> GetStates(int? countryId)
        {
            if (countryId.HasValue)
            {
                // Fetch states related to the provided countryId
                var states = await _dbcontext.States
                                             .Where(s => s.CountryId == countryId.Value)
                                             // Include related cities
                                             .AsNoTracking() // Avoid tracking to prevent duplication
                                             .ToListAsync();
                return states;
            }
            else
            {
                // Return all states if no countryId is provided
                var allStates = await _dbcontext.States
                                                .Include(s => s.Cities) // Include related cities
                                                .AsNoTracking() // Avoid tracking to prevent duplication
                                                .ToListAsync();
                return allStates;
            }
        }
        public async Task<List<DTOCity>> GetAllCities(int? stateid)
        {
            var cities = await _dbcontext.Cities.Where(c => c.StateId == stateid).ToListAsync();
            return cities.ToList();
        }

    }
}
