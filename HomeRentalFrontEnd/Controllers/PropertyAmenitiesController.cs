using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class PropertyAmenitiesController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public PropertyAmenitiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult PropertyAmenitiesList()
        {
            List<PropertyAmenitiesModel> propertyAmenities = new List<PropertyAmenitiesModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/PropertyAmenities").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                propertyAmenities = JsonConvert.DeserializeObject<List<PropertyAmenitiesModel>>(data); // Deserialize directly
            }
            return View(propertyAmenities);
        }
        public async Task<IActionResult> PropertyAmenitiesAddEdit(int? PropertyAmenityID)
        {
            await LoadAmenityList();
            await LoadPropertyList();
            if (PropertyAmenityID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/PropertyAmenities/{PropertyAmenityID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var propertyAmenity = JsonConvert.DeserializeObject<PropertyAmenitiesModel>(data);
                    return View(propertyAmenity);
                }
            }
            return View(new PropertyAmenitiesModel());
        }
        [HttpPost]
        public async Task<IActionResult> Save(PropertyAmenitiesModel propertyAmenity)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(propertyAmenity);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (propertyAmenity.PropertyAmenityID == null)
                {
                    propertyAmenity.PropertyAmenityID = 0;
                    json = JsonConvert.SerializeObject(propertyAmenity);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/PropertyAmenities", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/PropertyAmenities/{propertyAmenity.PropertyAmenityID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("PropertyAmenitiesList");
            }
            await LoadAmenityList();
            await LoadPropertyList();
            return View("PropertyAmenitiesAddEdit", propertyAmenity);
        }
        public async Task<IActionResult> PropertyAmenitiesDelete(int PropertyAmenityID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/PropertyAmenities/{PropertyAmenityID}");
            return RedirectToAction("PropertyAmenitiesList");
        }
        private async Task LoadAmenityList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/PropertyAmenities/GetAmenities");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var amenities = JsonConvert.DeserializeObject<List<AmenitiesDropDownModel>>(data);
                ViewBag.AmenityList = amenities;
            }
        }
        private async Task LoadPropertyList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/PropertyAmenities/GetProperties");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }
    }
}
