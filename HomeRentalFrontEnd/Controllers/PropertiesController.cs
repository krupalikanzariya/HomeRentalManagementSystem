using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public PropertiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5283/api")
            };
        }

        [HttpGet]
        public IActionResult AdminPropertiesList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            List<PropertiesModel> properties = new List<PropertiesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                properties = JsonConvert.DeserializeObject<List<PropertiesModel>>(data);
            }
            return View(properties);
        }

        public async Task<IActionResult> PropertiesAddEdit(int? PropertyID)
        {
            await LoadUserList();

            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (PropertyID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/{PropertyID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var property = JsonConvert.DeserializeObject<PropertiesModel>(data);
                    return View(property);
                }
            }
            return View(new PropertiesModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save(PropertiesModel property)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(property);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(property.PropertyID == null ? HttpMethod.Post : HttpMethod.Put,
                    $"{_httpClient.BaseAddress}/Properties" + (property.PropertyID != null ? $"/{property.PropertyID}" : ""))
                {
                    Content = content
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("AdminPropertiesList");
            }

            await LoadUserList();
            return View("PropertiesAddEdit", property);
        }

        public async Task<IActionResult> PropertyDelete(int PropertyID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/Properties/{PropertyID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.SendAsync(request);
            return RedirectToAction("AdminPropertiesList");
        }
        
        public async Task<IActionResult> PropertiesList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.LoginMesssage = "Please, Login To View Properties";
                return RedirectToAction("Login", "Users");
            }

            List<PropertiesModel> properties = new List<PropertiesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                properties = JsonConvert.DeserializeObject<List<PropertiesModel>>(data);
            }
            return View(properties);
        }
        [HttpGet]
        public async Task<IActionResult> PropertiesDetails(int PropertyID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            // Fetch property details
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/{PropertyID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);

            PropertiesModel property = null;
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                property = JsonConvert.DeserializeObject<PropertiesModel>(data);
            }

            // Fetch reviews
            var reviewsRequest = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Reviews/GetReviewsByProperty/{PropertyID}");
            reviewsRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var reviewsResponse = await _httpClient.SendAsync(reviewsRequest);

            List<ReviewsModel> reviews = new List<ReviewsModel>();
            if (reviewsResponse.IsSuccessStatusCode)
            {
                var reviewsData = await reviewsResponse.Content.ReadAsStringAsync();
                reviews = JsonConvert.DeserializeObject<List<ReviewsModel>>(reviewsData);
            }

            // Pass data to the view
            var model = new PropertyDetailsViewModel
            {
                Property = property,
                Reviews = reviews
            };

            return View(model);
        }

        private async Task LoadUserList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/GetUsers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data);
                ViewBag.UserList = users;
            }
        }
    }
}
