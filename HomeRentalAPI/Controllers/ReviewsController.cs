using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ReviewsRepository _ReviewsRepository;

        public ReviewsController(ReviewsRepository ReviewsRepository)
        {
            _ReviewsRepository = ReviewsRepository;
        }
        [Authorize]
        [HttpGet]
        public IActionResult GetAllReviews()
        {
            var reviews = _ReviewsRepository.GetAll();
            return Ok(reviews);
        }
        [Authorize]
        [HttpGet("{id}")]
        public IActionResult GetReviewById(int id)
        {
            var review = _ReviewsRepository.GetByPK(id);
            if (review == null)
            {
                return NotFound();
            }
            return Ok(review);
        }
        [Authorize]
        [HttpDelete("{id}")]
        public IActionResult DeleteReview(int id)
        {
            var isDeleted = _ReviewsRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }
        [Authorize]
        [HttpPost]
        public IActionResult InsertReview([FromBody] ReviewsModel review)
        {
            if (review == null)
                return BadRequest();

            bool isInserted = _ReviewsRepository.Insert(review);
            if (isInserted)
                return Ok(new { Message = "Review inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the review.");
        }
        [Authorize]
        [HttpPut("{id}")]
        public IActionResult UpdateReview(int id, [FromBody] ReviewsModel review)
        {
            if (review == null || id != review.ReviewID)
            {
                return BadRequest();
            }

            bool isUpdated = _ReviewsRepository.Update(review);
            if (isUpdated)
                return Ok(new { Message = "Review updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [Authorize]
        [HttpGet("property/{id}")]
        public IActionResult GetReviewsByProperty(int id)
        {
            var review = _ReviewsRepository.GetReviewsByProperty(id);
            if (review == null)
            {
                return NotFound(new { Message = "Reviews not found." });
            }
            return Ok(review);
        }
        [Authorize]
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _ReviewsRepository.GetUsers();
            if (!users.Any())
                return NotFound("No users found.");
            return Ok(users);
        }
        [Authorize]
        [HttpGet("GetProperties")]
        public IActionResult GetProperties()
        {
            var properties = _ReviewsRepository.GetProperties();
            if (!properties.Any())
                return NotFound("No properties found.");
            return Ok(properties);
        }
    }
}
