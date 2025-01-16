using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AmenitiesController : ControllerBase
    {
        private readonly AmenitiesRepository _AmenitiesRepository;

        public AmenitiesController(AmenitiesRepository AmenitiesRepository)
        {
            _AmenitiesRepository = AmenitiesRepository;
        }

        [HttpGet]
        public IActionResult GetAllAmenities()
        {
            var amenities = _AmenitiesRepository.GetAll();
            return Ok(amenities);
        }

        [HttpGet("{id}")]
        public IActionResult GetAmenityById(int id)
        {
            var amenity = _AmenitiesRepository.GetByPK(id);
            if (amenity == null)
            {
                return NotFound();
            }
            return Ok(amenity);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAmenity(int id)
        {
            var isDeleted = _AmenitiesRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertAmenity([FromBody] AmenitiesModel amenity)
        {
            if (amenity == null)
                return BadRequest();

            bool isInserted = _AmenitiesRepository.Insert(amenity);
            if (isInserted)
                return Ok(new { Message = "Amenity inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the amenity.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAmenity(int id, [FromBody] AmenitiesModel amenity)
        {
            if (amenity == null || id != amenity.AmenityID)
            {
                return BadRequest();
            }

            bool isUpdated = _AmenitiesRepository.Update(amenity);
            if (isUpdated)
                return Ok(new { Message = "Amenity updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
