using HomeRentalAPI.Data;
using HomeRentalAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HomeRentalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly BookingsRepository _BookingsRepository;

        public BookingsController(BookingsRepository BookingsRepository)
        {
            _BookingsRepository = BookingsRepository;
        }

        [HttpGet]
        public IActionResult GetAllBookings()
        {
            var bookings = _BookingsRepository.GetAll();
            return Ok(bookings);
        }

        [HttpGet("{id}")]
        public IActionResult GetBookingById(int id)
        {
            var booking = _BookingsRepository.GetByPK(id);
            if (booking == null)
            {
                return NotFound();
            }
            return Ok(booking);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteBooking(int id)
        {
            var isDeleted = _BookingsRepository.Delete(id);
            if (!isDeleted)
            {
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost]
        public IActionResult InsertBooking([FromBody] BookingsModel booking)
        {
            if (booking == null)
                return BadRequest();

            bool isInserted = _BookingsRepository.Insert(booking);
            if (isInserted)
                return Ok(new { Message = "Booking inserted successfully!" });

            return StatusCode(500, "An error occurred while inserting the booking.");
        }

        [HttpPut("{id}")]
        public IActionResult UpdateBooking(int id, [FromBody] BookingsModel booking)
        {
            if (booking == null || id != booking.BookingID)
            {
                return BadRequest();
            }

            bool isUpdated = _BookingsRepository.Update(booking);
            if (isUpdated)
                return Ok(new { Message = "Booking updated successfully!" });

            if (!isUpdated)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("user/{id}")]
        public IActionResult GetBookingsByUser(int id)
        {
            var booking = _BookingsRepository.GetBookingsByUser(id);
            if (booking == null)
            {
                return NotFound(new { Message = "Booking not found." });
            }
            return Ok(booking);
        }
        [HttpGet("GetUsers")]
        public IActionResult GetUsers()
        {
            var users = _BookingsRepository.GetUsers();
            if (!users.Any())
                return NotFound("No users found.");
            return Ok(users);
        }

        [HttpGet("GetProperties")]
        public IActionResult GetProperties()
        {
            var properties = _BookingsRepository.GetProperties();
            if (!properties.Any())
                return NotFound("No properties found.");
            return Ok(properties);
        }
    }
}
