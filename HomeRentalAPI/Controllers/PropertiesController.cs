using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertiesController : ControllerBase
    {
        private readonly PropertiesRepository _PropertiesRepository;

        public PropertiesController(PropertiesRepository PropertiesRepository)
        {
            _PropertiesRepository = PropertiesRepository;
        }

        [HttpGet]
        public IActionResult GetAllProperties()
        {
            var properties = _PropertiesRepository.GetAll();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        public IActionResult GetPropertyById(int id)
        {
            var property = _PropertiesRepository.GetByPK(id);
            if (property == null)
            {
                return NotFound();
            }
            return Ok(property);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteProperty(int id)
        {
            var isDeleted = _PropertiesRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertProperty([FromBody] PropertiesModel property)
        {
            if (property == null)
                return BadRequest();

            bool isInserted = _PropertiesRepository.Insert(property);
            if (isInserted)
                return Ok(new { Message = "Properties inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the property.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProperty(int id, [FromBody] PropertiesModel property)
        {
            if (property == null || id != property.PropertyID)
            {
                return BadRequest();
            }

            bool isUpdated = _PropertiesRepository.Update(property);
            if (isUpdated)
                return Ok(new { Message = "Property updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("ByHost/{hostID}")]
        public IActionResult GetPropertiesByHost(int hostID)
        {
            var properties = _PropertiesRepository.GetPropertiesByHost(hostID);
            if (properties == null || !properties.Any())
            {
                return NotFound(new { Message = "No properties found for the given host." });
            }
            return Ok(properties);
        }

        [HttpGet("Search")]
        public IActionResult SearchProperties([FromQuery] string city, [FromQuery] decimal minPrice, [FromQuery] decimal maxPrice, [FromQuery] int guests)
        {
            if (string.IsNullOrWhiteSpace(city) || minPrice < 0 || maxPrice < 0 || guests <= 0)
            {
                return BadRequest(new { Message = "Invalid search parameters." });
            }

            var properties = _PropertiesRepository.SearchProperties(city, minPrice, maxPrice, guests);
            if (properties == null || !properties.Any())
            {
                return NotFound(new { Message = "No properties found matching the criteria." });
            }
            return Ok(properties);
        }
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _PropertiesRepository.GetUsers();
            if (!users.Any())
                return NotFound("No users found.");
            return Ok(users);
        }
    }
}
