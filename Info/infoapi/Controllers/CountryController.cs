using infoapi.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace infoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        public readonly IRepository _repository;
        public CountryController(IRepository repository)
        {
            _repository = repository;
        }
        [HttpGet]
        public async Task<IActionResult> GetAllCountries(string? country)
        {
            var countries = await _repository.GetCountries(country);
            if (countries == null || countries.Count == 0)
            {
                return NotFound("No countries found.");
            }

            return Ok(countries);
        }
        [HttpGet("states")]
        public async Task<IActionResult> GetAllStates(int? countryId)
        {
            var states = await _repository.GetStates(countryId);
            if (states == null || states.Count == 0)
            {
                return NotFound("No states found.");
            }

            return Ok(states);
        }
        [HttpGet("AllCities")]
        public async Task<IActionResult> GetAllCities(int? stateid)
        {
            var Cities = await _repository.GetAllCities(stateid);
            if (Cities == null || Cities.Count == 0)
            {
                return NotFound("No states found.");
            }

            return Ok(Cities);
        }
    }
}
