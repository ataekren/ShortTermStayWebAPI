using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using ShortTermStayAPI.Data;
using ShortTermStayAPI.Model.Entities;
using ShortTermStayAPI.Model.DTOs;
using System.Security.Claims;

namespace ShortTermStayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BookingController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingCreateDto bookingDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized("User not authenticated");
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return BadRequest("Invalid user ID");
            }

            var listing = await _context.Listings.FindAsync(bookingDto.ListingId);
            if (listing == null)
            {
                return NotFound("Listing not found");
            }

            if (bookingDto.StartDate >= bookingDto.EndDate)
            {
                return BadRequest("Start date must be before end date");
            }

            if (bookingDto.StartDate < DateTime.UtcNow.Date)
            {
                return BadRequest("Cannot book dates in the past");
            }

            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.ListingId == bookingDto.ListingId &&
                              ((bookingDto.StartDate >= b.StartDate && bookingDto.StartDate < b.EndDate) ||
                               (bookingDto.EndDate > b.StartDate && bookingDto.EndDate <= b.EndDate) ||
                               (bookingDto.StartDate <= b.StartDate && bookingDto.EndDate >= b.EndDate)));

            if (existingBooking)
            {
                return BadRequest("Listing is already booked for these dates");
            }

            var booking = new Booking
            {
                ListingId = bookingDto.ListingId,
                GuestId = userId,
                StartDate = bookingDto.StartDate,
                EndDate = bookingDto.EndDate,
                NamesOfPeople = bookingDto.NamesOfPeople
            };

            try
            {
                await _context.Bookings.AddAsync(booking);
                await _context.SaveChangesAsync();
                //return CreatedAtAction(nameof(CreateBooking), new { id = booking.Id }, booking);
                return Ok("Booking with id " + booking.Id + " created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the booking");
            }
        }
    }
}
