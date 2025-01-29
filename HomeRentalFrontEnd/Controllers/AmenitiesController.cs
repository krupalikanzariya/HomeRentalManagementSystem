using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class AmenitiesController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public AmenitiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult AmenitiesList()
        {
            List<AmenitiesModel> amenities = new List<AmenitiesModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Amenities").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                amenities = JsonConvert.DeserializeObject<List<AmenitiesModel>>(data); // Deserialize directly
            }
            return View(amenities);
        }
        public async Task<IActionResult> AmenitiesAddEdit(int? AmenityID)
        {
            

            if (AmenityID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Amenities/{AmenityID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var amenity = JsonConvert.DeserializeObject<AmenitiesModel>(data);
                    return View(amenity);
                }
            }
            return View(new AmenitiesModel());
        }
        [HttpPost]
        public async Task<IActionResult> Save(AmenitiesModel amenity)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(amenity);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (amenity.AmenityID == null)
                {
                    amenity.AmenityID = 0;
                    json = JsonConvert.SerializeObject(amenity);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Amenities", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Amenities/{amenity.AmenityID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("AmenitiesList");
            }
           
            return View("AmenitiesAddEdit", amenity);
        }
        public async Task<IActionResult> AmenitiesDelete(int AmenityID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Amenities/{AmenityID}");
            return RedirectToAction("AmenitiesList");
        }
    }
}
