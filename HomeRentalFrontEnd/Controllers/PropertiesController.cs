using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Claims;
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

                    // 🔹 Debugging Output: Check if image URLs are populated
                    Console.WriteLine($"Fetched Property: {JsonConvert.SerializeObject(property)}");

                    return View(property);
                }
            }
            return View(new PropertiesModel());
        }
        [HttpPost]
        public async Task<IActionResult> Save(PropertiesModel property, List<string> ImageURLs)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var hostIdString = HttpContext.Session.GetString("HostID");
            if (string.IsNullOrEmpty(hostIdString) || !int.TryParse(hostIdString, out int hostId) || hostId <= 0)
            {
                return Unauthorized("User ID not found or invalid in session.");
            }

            // Remove ModelState error for HostID before assigning
            ModelState.Remove(nameof(property.HostID));
            property.HostID = hostId;

            if (ModelState.IsValid)
            {
                // Assign Images if provided
                property.Images = ImageURLs?
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .Select(url => new ImagesModel { ImageURL = url })
                    .ToList() ?? new List<ImagesModel>();

                var json = JsonConvert.SerializeObject(property);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage();

                if (property.PropertyID == null || property.PropertyID == 0)
                {
                    // If PropertyID is null or 0, it's a new property (Create)
                    property.PropertyID = 0;
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Properties");
                }
                else
                {
                    // If PropertyID exists, it's an update (Edit)
                    request.Method = HttpMethod.Put;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Properties/{property.PropertyID}");
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("AdminPropertiesList");
                }
            }

            return View("PropertiesAddEdit", property);
        }
        public async Task<IActionResult> HostProperty(int? PropertyID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            PropertiesModel property = new PropertiesModel();

            if (PropertyID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/{PropertyID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    property = JsonConvert.DeserializeObject<PropertiesModel>(responseData);
                }
            }

            return View(property);
        }

        [HttpPost]
        public async Task<IActionResult> HostProperty(PropertiesModel property)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var jsonContent = new StringContent(JsonConvert.SerializeObject(property), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(property.PropertyID == 0 ? HttpMethod.Post : HttpMethod.Put, $"{_httpClient.BaseAddress}/Properties")
            {
                Headers = { Authorization = new AuthenticationHeaderValue("Bearer", token) },
                Content = jsonContent
            };

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Property hosted successfully!";
                return RedirectToAction("MyProperties");
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to host property.";
                return View(property);
            }
        }

        public async Task<IActionResult> MyProperties()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/HostProperties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            List<PropertiesModel> properties = new List<PropertiesModel>();

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                properties = JsonConvert.DeserializeObject<List<PropertiesModel>>(responseData);
            }

            return View(properties);
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

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                // Get error message from response
                var errorMessage = await response.Content.ReadAsStringAsync();

                // Log the error
                Console.WriteLine($"Delete failed: {response.StatusCode} - {errorMessage}");

                // Store the error message to display in the view
                TempData["ErrorMessage"] = $"Delete failed: {response.StatusCode} - {errorMessage}";
            }
            else
            {
                TempData["SuccessMessage"] = "Property deleted successfully!";
            }

            return RedirectToAction("AdminPropertiesList");
        }


        public async Task<IActionResult> PropertiesList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.LoginMessage = "Please, Login To View Properties";
                return RedirectToAction("Login", "Users");
            }

            List<PropertiesModel> properties = new List<PropertiesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);
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
