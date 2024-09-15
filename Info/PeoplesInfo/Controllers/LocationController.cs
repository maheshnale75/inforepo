using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PeoplesInfo.Models;

namespace PeoplesInfoMvc.Controllers
{
    public class LocationController : Controller
    {
        private readonly HttpClient _httpClient;

        public LocationController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Action to load the initial view with the country dropdown
        public IActionResult Index()
        {
            return View();
        }

        // Get all countries
        [HttpGet]
        public async Task<IActionResult> GetCountries()
        {
            var response = await _httpClient.GetStringAsync("https://localhost:7191/api/country"); // Adjust your API endpoint
            var countries = JsonConvert.DeserializeObject<List<DTOCountry>>(response);
            return Json(countries);
        }

        // Get states based on countryId
        [HttpGet]
        public async Task<IActionResult> GetStates(int countryId)
        {
            var response = await _httpClient.GetStringAsync($"https://localhost:7191/api/Country/states?countryId={countryId}"); // Adjust your API endpoint
            var states = JsonConvert.DeserializeObject<List<DTOState>>(response);
            return Json(states);
        }

        // Get cities based on stateId
        [HttpGet]
        public async Task<IActionResult> GetCities(int stateId)
        {
            var response = await _httpClient.GetStringAsync($"https://localhost:7191/api/Country/AllCities?stateid={stateId}"); // Adjust your API endpoint
            var cities = JsonConvert.DeserializeObject<List<DTOCity>>(response);
            return Json(cities);
        }
    }
}
