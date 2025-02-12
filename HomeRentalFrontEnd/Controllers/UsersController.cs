using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Text;

namespace HomeRentalFrontEnd.Controllers
{
    public class UsersController : Controller
    {
        private readonly IConfiguration _configuration;
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _httpClient;
        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public IActionResult UsersList()
        {
            List<UsersModel> users = new List<UsersModel>();
            HttpResponseMessage response = _httpClient.GetAsync($"{_httpClient.BaseAddress}/Users").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UsersModel>>(data); // Deserialize directly
            }
            return View(users);
        }
        public async Task<IActionResult> UsersAddEdit(int? UserID)
        {
            if (UserID.HasValue)
            {
                var response = await _httpClient.GetAsync($"{_httpClient.BaseAddress}/Users/{UserID}");
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<UsersModel>(data);
                    return View(user);
                }
            }
            return View(new UsersModel());
        }
        [HttpPost]
        public async Task<IActionResult> Save(UsersModel user)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (user.UserID == null)
                {
                    user.UserID = 0;
                    json = JsonConvert.SerializeObject(user);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Users", content);
                }
                else
                    response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Users/{user.UserID}", content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("UsersList");
            }
            return View("UsersAddEdit", user);
        }
        public async Task<IActionResult> UserDelete(int UserID)
        {
            var response = await _httpClient.DeleteAsync($"{_httpClient.BaseAddress}/Users/{UserID}");
            return RedirectToAction("UsersList");
        }
        public async Task<IActionResult> UserLogin(UserLoginModel userLoginModel)
        {
            if (string.IsNullOrEmpty(userLoginModel.UserName) || string.IsNullOrEmpty(userLoginModel.Password))
            {
                ViewBag.ErrorMessage = "Username and Password are required!";
                return View();
            }

            
            var json = JsonConvert.SerializeObject(userLoginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            
            

            try
            {
                // Make HTTP POST request to the API
                response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Users/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response
                    string data = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<UsersModel>(data); // Deserialize directly

                    HttpContext.Session.SetString("UserID", user.UserID.ToString());
                    HttpContext.Session.SetString("UserName", user.UserName);

                    // Redirect based on role (Admin or User)
                    if (user.RoleID == 1)
                    {
                        return RedirectToAction("Index", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    ViewBag.ErrorMessage = "Invalid username or password!";
                }
                else
                {
                    ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An exception occurred: {ex.Message}";
            }

            return View("Login");
        }
        ////public async Task<IActionResult> UserSignup(UserRegisterModel userSignupModel)
        ////{
        ////    if (string.IsNullOrEmpty(userSignupModel.UserName) || string.IsNullOrEmpty(userSignupModel.Password))
        ////    {
        ////        ViewBag.ErrorMessage = "Username and Password are required!";
        ////        return View();
        ////    }


        ////    var json = JsonConvert.SerializeObject(userLoginModel);
        ////    var content = new StringContent(json, Encoding.UTF8, "application/json");
        ////    HttpResponseMessage response;



        ////    try
        ////    {
        ////        // Make HTTP POST request to the API
        ////        response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Users/Login", content);

        ////        if (response.IsSuccessStatusCode)
        ////        {
        ////            // Deserialize the response
        ////            string data = response.Content.ReadAsStringAsync().Result;
        ////            var user = JsonConvert.DeserializeObject<UsersModel>(data); // Deserialize directly

        ////            HttpContext.Session.SetString("UserID", user.UserID.ToString());
        ////            HttpContext.Session.SetString("UserName", user.UserName);

        ////            // Redirect based on role (Admin or User)
        ////            if (user.RoleID == 1)
        ////            {
        ////                return RedirectToAction("Index", "Admin");
        ////            }
        ////            else
        ////            {
        ////                return RedirectToAction("Index", "Home");
        ////            }
        ////        }
        ////        else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        ////        {
        ////            ViewBag.ErrorMessage = "Invalid username or password!";
        ////        }
        ////        else
        ////        {
        ////            ViewBag.ErrorMessage = "An error occurred during login. Please try again.";
        ////        }
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        ViewBag.ErrorMessage = $"An exception occurred: {ex.Message}";
        ////    }

        ////    return View("Login");
        ////}
        public async Task<IActionResult> Login()
        {
            return View();
        }
        public async Task<IActionResult> Signup()
        {
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Users");
        }

    }
}
