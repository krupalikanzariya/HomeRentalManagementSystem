using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            List<AmenitiesModel> amenities = new List<AmenitiesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Amenities");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                amenities = JsonConvert.DeserializeObject<List<AmenitiesModel>>(data);
            }
            return View(amenities);
        }

        public async Task<IActionResult> AmenitiesAddEdit(int? AmenityID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (AmenityID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Amenities/{AmenityID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
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
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(amenity);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage();

                if (amenity.AmenityID == null)
                {
                    amenity.AmenityID = 0;
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Amenities");
                }
                else
                {
                    request.Method = HttpMethod.Put;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Amenities/{amenity.AmenityID}");
                }

                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("AmenitiesList");
                }
            }

            return View("AmenitiesAddEdit", amenity);
        }


        public async Task<IActionResult> AmenitiesDelete(int AmenityID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/Amenities/{AmenityID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.SendAsync(request);
            return RedirectToAction("AmenitiesList");
        }

    }
}
