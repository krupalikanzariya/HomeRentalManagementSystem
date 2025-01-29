using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public ReviewsController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult ReviewsList()
        {
            List<ReviewsModel> reviews = new List<ReviewsModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Reviews").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                reviews = JsonConvert.DeserializeObject<List<ReviewsModel>>(data); // Deserialize directly
            }
            return View(reviews);
        }
        public async Task<IActionResult> ReviewsAddEdit(int? ReviewID)
        {
            await LoadUserList();
            await LoadPropertyList();

            if (ReviewID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Reviews/{ReviewID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var review = JsonConvert.DeserializeObject<ReviewsModel>(data);
                    return View(review);
                }
            }
            return View(new ReviewsModel());
        }
        [HttpPost]
        public async Task<IActionResult> Save(ReviewsModel review)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(review);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (review.ReviewID == null)
                {
                    review.ReviewID = 0;
                    json = JsonConvert.SerializeObject(review);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Reviews", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Reviews/{review.ReviewID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ReviewsList");
            }
            await LoadUserList();
            await LoadPropertyList();

            return View("ReviewsAddEdit", review);
        }
        private async Task LoadUserList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Reviews/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data);
                ViewBag.UserList = users;
            }
        }
        private async Task LoadPropertyList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Reviews/GetProperties");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }
        public async Task<IActionResult> ReviewsDelete(int ReviewID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Reviews/{ReviewID}");
            return RedirectToAction("ReviewsList");
        }
    }
}
