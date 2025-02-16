using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class PropertyAmenitiesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PropertyAmenitiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5283/api")
            };
        }

        [HttpGet]
        public IActionResult PropertyAmenitiesList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            List<PropertyAmenitiesModel> propertyAmenities = new List<PropertyAmenitiesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/PropertyAmenities");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                propertyAmenities = JsonConvert.DeserializeObject<List<PropertyAmenitiesModel>>(data);
            }
            return View(propertyAmenities);
        }

        public async Task<IActionResult> PropertyAmenitiesAddEdit(int? PropertyAmenityID)
        {
            await LoadAmenityList();
            await LoadPropertyList();

            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (PropertyAmenityID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/PropertyAmenities/{PropertyAmenityID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
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
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(propertyAmenity);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(propertyAmenity.PropertyAmenityID == null ? HttpMethod.Post : HttpMethod.Put,
                    $"{_httpClient.BaseAddress}/PropertyAmenities" + (propertyAmenity.PropertyAmenityID != null ? $"/{propertyAmenity.PropertyAmenityID}" : ""))
                {
                    Content = content
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("PropertyAmenitiesList");
            }

            await LoadAmenityList();
            await LoadPropertyList();
            return View("PropertyAmenitiesAddEdit", propertyAmenity);
        }

        public async Task<IActionResult> PropertyAmenitiesDelete(int PropertyAmenityID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/PropertyAmenities/{PropertyAmenityID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.SendAsync(request);
            return RedirectToAction("PropertyAmenitiesList");
        }

        private async Task LoadAmenityList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/PropertyAmenities/GetAmenities");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var amenities = JsonConvert.DeserializeObject<List<AmenitiesDropDownModel>>(data);
                ViewBag.AmenityList = amenities;
            }
        }

        private async Task LoadPropertyList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/PropertyAmenities/GetProperties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }
    }
}
