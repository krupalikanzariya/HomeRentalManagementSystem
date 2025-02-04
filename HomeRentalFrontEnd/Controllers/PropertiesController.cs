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
        public async Task<IActionResult> PropertiesList()
        {
            List<PropertiesModel> properties = new List<PropertiesModel>();
            HttpResponseMessage response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Properties");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                properties = JsonConvert.DeserializeObject<List<PropertiesModel>>(data);
            }
            return View(properties);
        }


    }
}
//public async Task<IActionResult> PropertyDetails(int propertyId)
//{
//    // Fetch property details
//    var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Properties/{propertyId}");
//    var property = new PropertiesModel();
//    if (response.IsSuccessStatusCode)
//    {
//        var data = await response.Content.ReadAsStringAsync();
//        property = JsonConvert.DeserializeObject<PropertiesModel>(data);
//    }

//    // Fetch images for this property
//    var imagesResponse = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Images/GetImagesByProperty/{propertyId}");
//    var images = new List<ImagesModel>();
//    if (imagesResponse.IsSuccessStatusCode)
//    {
//        var imagesData = await imagesResponse.Content.ReadAsStringAsync();
//        images = JsonConvert.DeserializeObject<List<ImagesModel>>(imagesData);
//    }

//    ViewBag.Images = images; // Pass images to the view
//    return View(property);
//}
//@model HomeRentalFrontEnd.Models.PropertiesModel
//@{
//    var images = ViewBag.Images as List<HomeRentalFrontEnd.Models.ImagesModel>;
//}

//< div class= "container" >
//    < h2 > @Model.Title </ h2 >
//    < p > @Model.Description </ p >
//    < p >< strong > Location:</ strong > @Model.City, @Model.State, @Model.Country </ p >
//    < p >< strong > Price per Night:</ strong > $@Model.PricePerNight </ p >

//    < h3 > Property Images </ h3 >
//    < div class= "row" >
//        @if(images != null && images.Any())
//        {
//    @foreach(var image in images)
//            {
//                < div class= "col-md-4" >
//                    < img src = "@image.ImageURL" class= "img-fluid img-thumbnail" alt = "Property Image" >
//                </ div >
//            }
//        }
//        else
//{
//            < p > No images available for this property.</ p >
//        }
//    </ div >
//</ div >


