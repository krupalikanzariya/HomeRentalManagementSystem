using HomeRentalFrontEnd.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Data;
using System.Diagnostics.Metrics;
using System.Net.Http;
using System.Net.Http.Headers;
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

            // Retrieve JWT token from session
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users"); // Redirect if no token found
            }

            var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Users");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token); // Add token to header

            HttpResponseMessage response = _httpClient.SendAsync(request).Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                users = JsonConvert.DeserializeObject<List<UsersModel>>(data);
            }
            return View(users);
        }

        public async Task<IActionResult> UsersAddEdit(int? UserID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users"); // Redirect if no token found
            }

            if (UserID.HasValue)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"{_httpClient.BaseAddress}/Users/{UserID}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
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
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage();

                if (user.UserID == null)
                {
                    user.UserID = 0;
                    json = JsonConvert.SerializeObject(user);
                    content = new StringContent(json, Encoding.UTF8, "application/json");
                    request.Method = HttpMethod.Post;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Users");
                }
                else
                {
                    request.Method = HttpMethod.Put;
                    request.RequestUri = new Uri($"{_httpClient.BaseAddress}/Users/{user.UserID}");
                }

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                request.Content = content;
                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("UsersList");
                }
            }
            return View("UsersAddEdit", user);
        }

        public async Task<IActionResult> UserDelete(int UserID)
        {
            var token = HttpContext.Session.GetString("Token");
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Users");
            }

            var request = new HttpRequestMessage(HttpMethod.Delete, $"{_httpClient.BaseAddress}/Users/{UserID}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            await _httpClient.SendAsync(request);
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
                response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Users/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();

                    // Deserialize correctly
                    var responseObject = JsonConvert.DeserializeObject<dynamic>(data);

                    if (responseObject != null && responseObject.user != null)
                    {
                        var user = responseObject.user;

                        // Store UserID and UserName safely
                        // Cast dynamic object to correct types
                        int userID = (int)responseObject.user.userID;
                        string userName = (string)responseObject.user.userName;

                        var token = (string)responseObject.token; // Extract JWT token

                        HttpContext.Session.SetString("Token", token);

                        // Now safely store values in session
                        HttpContext.Session.SetString("UserID", userID.ToString());
                        HttpContext.Session.SetString("UserName", userName);


                        // Redirect based on role
                        if (user.roleID == 1)
                        {
                            return RedirectToAction("Index", "Dashboard");
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
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

        public async Task<IActionResult> UserSignup(UserSignupModel userSignupModel)
        {
            if (!ModelState.IsValid)
            {
                return View("Signup", userSignupModel);
            }

            var json = JsonConvert.SerializeObject(userSignupModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            try
            {
                response = await _httpClient.PostAsync($"{_httpClient.BaseAddress}/Users/Signup", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Login", "Users");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ViewBag.ErrorMessage = $"Registration failed: {errorMessage}";
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"An exception occurred: {ex.Message}";
            }

            return View("Signup", userSignupModel);
        }

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
