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
        Uri baseAddress = new Uri("http://localhost:5283/api");
        private readonly HttpClient _client;
        public UsersController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Login(string username, string password, string role)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Username and Password are required!";
                return View();
            }

            var userLoginModel = new
            {
                UserName = username,
                Password = password
            };
            var json = JsonConvert.SerializeObject(userLoginModel);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            
            

            try
            {
                // Make HTTP POST request to the API
                response = await _client.PostAsync($"{_client.BaseAddress}/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    // Deserialize the response
                    string data = response.Content.ReadAsStringAsync().Result;
                    var user = JsonConvert.DeserializeObject<UsersModel>(data); // Deserialize directly

                    // Redirect based on role (Admin or User)
                    if (role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", "Admin");
                    }
                    else
                    {
                        return RedirectToAction("UserDashboard", "User");
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

            return View();
        }
    }
}
