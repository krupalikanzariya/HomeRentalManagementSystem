using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public BookingsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5283/api")
            };
        }

        [HttpGet]
        public IActionResult BookingsList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            List<BookingsModel> bookings = new List<BookingsModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Bookings");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                bookings = JsonConvert.DeserializeObject<List<BookingsModel>>(data);
            }
            return View(bookings);
        }

        public async Task<IActionResult> UserBookings(int PropertyID)
        {
            await LoadUserList();
            await LoadPropertyList();

            int? userId = CommonVariable.UserID();
            if (userId == null)
            {
                return RedirectToAction("Login", "Users");
            }
            var token = HttpContext.Session.GetString("Token");

            // Fetch property details from API
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Properties/{PropertyID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.SendAsync(request);

            PropertiesModel property = null;
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                property = JsonConvert.DeserializeObject<PropertiesModel>(data);
            }

            var model = new BookingsModel
            {
                UserID = userId.Value,
                PropertyID = PropertyID,
                PropertyTitle = property?.Title, // Store the property title
                PropertyImage = property?.Images?.FirstOrDefault()?.ImageURL, // Store the first image URL
                PricePerNight = property?.PricePerNight ?? 0 // Pass property price per night
            };

            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> UserBookings(BookingsModel booking)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(booking);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}/Bookings")
                {
                    Content = content
                };
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("BookingsList");
                }
            }

            await LoadUserList();
            await LoadPropertyList();
            return View(booking);
        }

        public async Task<IActionResult> BookingsAddEdit(int? BookingID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            await LoadUserList();
            await LoadPropertyList();

            if (BookingID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Bookings/{BookingID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var booking = JsonConvert.DeserializeObject<BookingsModel>(data);
                    return View(booking);
                }
            }
            return View(new BookingsModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save(BookingsModel booking)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(booking);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage();

                if (booking.BookingID == null)
                {
                    booking.BookingID = 0;
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Bookings");
                }
                else
                {
                    request.Method = HttpMethod.Put;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Bookings/{booking.BookingID}");
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = content;

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("BookingsList");
                }
            }

            await LoadUserList();
            await LoadPropertyList();
            return View("BookingsAddEdit", booking);
        }

        private async Task LoadUserList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Bookings/GetUsers");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data);
                ViewBag.UserList = users;
            }
        }

        private async Task LoadPropertyList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Bookings/GetProperties");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }

        public async Task<IActionResult> BookingsDelete(int BookingID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/Bookings/{BookingID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.SendAsync(request);
            return RedirectToAction("BookingsList");
        }
    }
}
