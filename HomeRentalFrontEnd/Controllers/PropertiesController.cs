using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class PropertiesController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public PropertiesController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult AdminPropertiesList()
        {
            List<PropertiesModel> properties = new List<PropertiesModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Properties").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                properties = JsonConvert.DeserializeObject<List<PropertiesModel>>(data); // Deserialize directly
            }
            return View(properties);
        }
        public async Task<IActionResult> PropertiesAddEdit(int? PropertyID)
        {
            await LoadUserList();
            if (PropertyID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Properties/{PropertyID}");
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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(property);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (property.PropertyID == null)
                {
                    property.PropertyID = 0;
                    json = JsonConvert.SerializeObject(property);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Properties", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Properties/{property.PropertyID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("AdminPropertiesList");
            }
            await LoadUserList();
            return View("PropertiesAddEdit", property);
        }
        private async Task LoadUserList()
        {
            var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Properties/GetUsers");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var users = JsonConvert.DeserializeObject<List<UserDropDownModel>>(data);
                ViewBag.UserList = users;
            }
        }
        public async Task<IActionResult> PropertyDelete(int PropertyID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Properties/{PropertyID}");
            return RedirectToAction("AdminPropertiesList");
        }
        public IActionResult PropertiesList()
        {
            return View();
        }
        
    }
}
