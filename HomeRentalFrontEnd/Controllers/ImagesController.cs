using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public ImagesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5283/api")
            };
        }

        [HttpGet]
        public IActionResult ImagesList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            List<ImagesModel> images = new List<ImagesModel>();
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Images");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = _httpClient.SendAsync(request).Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                images = JsonConvert.DeserializeObject<List<ImagesModel>>(data);
            }
            return View(images);
        }

        public async Task<IActionResult> ImagesAddEdit(int? ImageID)
        {
            await LoadPropertyList();

            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ImageID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Images/{ImageID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var image = JsonConvert.DeserializeObject<ImagesModel>(data);
                    return View(image);
                }
            }
            return View(new ImagesModel());
        }

        [HttpPost]
        public async Task<IActionResult> Save(ImagesModel image)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(image);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(image.ImageID == null ? HttpMethod.Post : HttpMethod.Put,
                    $"{_httpClient.BaseAddress}/Images" + (image.ImageID != null ? $"/{image.ImageID}" : ""))
                {
                    Content = content
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ImagesList");
            }

            await LoadPropertyList();
            return View("ImagesAddEdit", image);
        }

        public async Task<IActionResult> ImagesDelete(int ImageID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/Images/{ImageID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.SendAsync(request);
            return RedirectToAction("ImagesList");
        }

        private async Task LoadPropertyList()
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return;
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Images/GetProperties");
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
