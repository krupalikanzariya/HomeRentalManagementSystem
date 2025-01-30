using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly ImagesRepository _ImagesRepository;

        public ImagesController(ImagesRepository ImagesRepository)
        {
            _ImagesRepository = ImagesRepository;
        }
        [HttpGet]
        public IActionResult GetImages()
        {
            var images = _ImagesRepository.GetImages();
            if (images == null || !images.Any())
            {
                return NotFound(new { Message = "No images found." });
            }
            return Ok(images);
        }
        [HttpGet("{ImageID}")]
        public IActionResult GetImagesByID(int ImageID)
        {
            var images = _ImagesRepository.GetImagesByID(ImageID);
            if (images == null )
            {
                return NotFound(new { Message = "No images found for the given id." });
            }
            return Ok(images);
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteImage(int id)
        {
            var isDeleted = _ImagesRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertImage([FromBody] ImagesModel image)
        {
            if (image == null)
                return BadRequest();

            bool isInserted = _ImagesRepository.Insert(image);
            if (isInserted)
                return Ok(new { Message = "Images inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the image.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateImage(int id, [FromBody] ImagesModel image)
        {
            if (image == null || id != image.ImageID)
            {
                return BadRequest();
            }

            bool isUpdated = _ImagesRepository.Update(image);
            if (isUpdated)
                return Ok(new { Message = "Image updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("ByProperty/{propertyID}")]
        public IActionResult GetImagesByProperty(int propertyID)
        {
            var images = _ImagesRepository.GetImagesByProperty(propertyID);
            if (images == null || !images.Any())
            {
                return NotFound(new { Message = "No images found for the given property." });
            }
            return Ok(images);
        }
        [HttpGet("GetProperties")]
        public IActionResult GetProperties()
        {
            var properties = _ImagesRepository.GetProperties();
            if (!properties.Any())
                return NotFound("No properties found.");
            return Ok(properties);
        }
    }
}
