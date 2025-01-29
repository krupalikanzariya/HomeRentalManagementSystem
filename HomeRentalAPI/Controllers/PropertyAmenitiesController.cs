using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyAmenitiesController : ControllerBase
    {
        private readonly PropertyAmenitiesRepository _PropertyAmenitiesRepository;
        public PropertyAmenitiesController(PropertyAmenitiesRepository PropertyAmenitiesRepository)
        {
            _PropertyAmenitiesRepository = PropertyAmenitiesRepository;
        }
        [HttpGet]
        public IActionResult GetAllPropertyAmenities()
        {
            var propertyAmenity = _PropertyAmenitiesRepository.GetAll();
            return Ok(propertyAmenity);
        }
        [HttpGet("{id}")]
        public IActionResult GetPropertyAmenityById(int id)
        {
            var propertyAmenity = _PropertyAmenitiesRepository.GetByPK(id);
            if (propertyAmenity == null)
            {
                return NotFound();
            }
            return Ok(propertyAmenity);
        }
        [HttpDelete("{id}")]
        public IActionResult DeletePropertyAmenity(int id)
        {
            var isDeleted = _PropertyAmenitiesRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertProperty([FromBody] PropertyAmenitiesModel propertyAmenity)
        {
            if (propertyAmenity == null)
                return BadRequest();

            bool isInserted = _PropertyAmenitiesRepository.Insert(propertyAmenity);
            if (isInserted)
                return Ok(new { Message = "Property amenity inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the property.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateProperty(int id, [FromBody] PropertyAmenitiesModel propertyAmenity)
        {
            if (propertyAmenity == null || id != propertyAmenity.PropertyAmenityID)
            {
                return BadRequest();
            }

            bool isUpdated = _PropertyAmenitiesRepository.Update(propertyAmenity);
            if (isUpdated)
                return Ok(new { Message = "Property amenity updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("ByProperty/{PropertyID}")]
        public IActionResult GetAmenitiesByProperty(int PropertyID)
        {
            var propertyAmenity = _PropertyAmenitiesRepository.GetAmenitiesByProperty(PropertyID);
            if (propertyAmenity == null || !propertyAmenity.Any())
            {
                return NotFound(new { Message = "No amenity found for the given property." });
            }
            return Ok(propertyAmenity);
        }
        [HttpGet("GetProperties")]
        public IActionResult GetProperties()
        {
            var properties = _PropertyAmenitiesRepository.GetProperties();
            if (!properties.Any())
                return NotFound("No properties found.");
            return Ok(properties);
        }
        [HttpGet("GetAmenities")]
        public IActionResult GetAmenities()
        {
            var amenities = _PropertyAmenitiesRepository.GetAmenities();
            if (!amenities.Any())
                return NotFound("No amenities found.");
            return Ok(amenities);
        }
    }
}
