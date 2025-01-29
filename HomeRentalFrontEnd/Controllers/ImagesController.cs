using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public ImagesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult ImagesList()
        {
            List<ImagesModel> images = new List<ImagesModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Images").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                images = JsonConvert.DeserializeObject<List<ImagesModel>>(data); // Deserialize directly
            }
            return View(images);
        }
        public async Task<IActionResult> ImagesAddEdit(int? ImageID)
        {
            await LoadPropertyList();
            if (ImageID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Images/{ImageID}");
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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(image);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (image.ImageID == null)
                {
                    image.ImageID = 0;
                    json = JsonConvert.SerializeObject(image);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Images", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Images/{image.ImageID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("ImagesList");
            }
            await LoadPropertyList();
            return View("ImagesAddEdit", image);
        }
        public async Task<IActionResult> ImagesDelete(int ImageID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Images/{ImageID}");
            return RedirectToAction("ImagesList");
        }
        
        private async Task LoadPropertyList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Images/GetProperties");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var properties = JsonConvert.DeserializeObject<List<PropertiesDropDownModel>>(data);
                ViewBag.PropertyList = properties;
            }
        }
    }
}
