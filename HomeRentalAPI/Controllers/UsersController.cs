using Microsoft.AspNetCore.Mvc;
using HomeRentalAPI.Data;
using HomeRentalAPI.Models;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : Controller
    {
        private readonly UsersRepository _UsersRepository;

        public UsersController(UsersRepository UsersRepository)
        {
            _UsersRepository = UsersRepository;
        }

        [HttpPost("Signup")]
        public IActionResult Signup([FromBody] UsersModel user)
        {
            if (user == null)
            {
                return BadRequest(new { Message = "Invalid user data provided." });
            }

            bool isInserted = _UsersRepository.Insert(user);
            if (isInserted)
            {
                return Ok(new { Message = "User registered successfully!" });
            }

            return StatusCode(500, "An error occurred while registering the user.");
        }
        [HttpPost("Login")]
        public IActionResult Login([FromBody] UserLoginModel userLoginModel)
        {
            if (userLoginModel == null || string.IsNullOrEmpty(userLoginModel.UserName) || string.IsNullOrEmpty(userLoginModel.Password))
            {
                return BadRequest(new { Message = "Invalid login data provided." });
            }

            var authenticatedUser = _UsersRepository.Login(userLoginModel);
            if (authenticatedUser != null)
            {
                return Ok(new
                {
                    Message = "Login successful!",
                    UserID = authenticatedUser.UserID,
                    UserName = authenticatedUser.UserName,
                    FirstName = authenticatedUser.FirstName,
                    LastName = authenticatedUser.LastName,
                    Email = authenticatedUser.Email,
                    Password = authenticatedUser.Password,
                    ProfilePictureURL = authenticatedUser.ProfilePictureURL,
                    RoleID = authenticatedUser.RoleID
                });
            }

            return Unauthorized(new { Message = "Invalid username or password." });
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _UsersRepository.GetAll();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetUserById(int id)
        {
            var user = _UsersRepository.GetByPK(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            var isDeleted = _UsersRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertUser([FromBody] UsersModel user)
        {
            if (user == null)
                return BadRequest();

            bool isInserted = _UsersRepository.Insert(user);
            if (isInserted)
                return Ok(new { Message = "User inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the user.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UsersModel user)
        {
            if (user == null || id != user.UserID)
            {
                return BadRequest();
            }

            bool isUpdated = _UsersRepository.Update(user);
            if (isUpdated)
                return Ok(new { Message = "User updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
