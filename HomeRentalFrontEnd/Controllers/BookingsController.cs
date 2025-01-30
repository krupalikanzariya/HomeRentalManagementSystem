using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class BookingsController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public BookingsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult BookingsList()
        {
            List<BookingsModel> bookings = new List<BookingsModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Bookings").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                bookings = JsonConvert.DeserializeObject<List<BookingsModel>>(data); // Deserialize directly
            }
            return View(bookings);
        }
        public async Task<IActionResult> BookingsAddEdit(int? BookingID)
        {
            await LoadUserList();
            await LoadPropertyList();

            if (BookingID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Bookings/{BookingID}");
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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(booking);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (booking.BookingID == null)
                {
                    booking.BookingID = 0;
                    json = JsonConvert.SerializeObject(booking);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Bookings", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Bookings/{booking.BookingID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("BookingsList");

            }
            await LoadUserList();
            await LoadPropertyList();

            return View("BookingsAddEdit", booking);
        }
        private async Task LoadUserList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Bookings/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data);
                ViewBag.UserList = users;
            }
        }
        private async Task LoadPropertyList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Bookings/GetProperties");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }
        public async Task<IActionResult> BookingsDelete(int BookingID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Bookings/{BookingID}");
            return RedirectToAction("BookingsList");
        }
    }
}
