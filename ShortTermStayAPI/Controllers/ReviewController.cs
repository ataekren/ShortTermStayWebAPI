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
    public class ReviewController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("CreateReview")]
        public async Task<IActionResult> CreateReview([FromBody] ReviewCreateDto reviewDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User not authenticated");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Role != UserRole.Guest)
            {
                return Unauthorized("Only guests can create reviews");
            }

            var booking = await _context.Bookings
                .Include(b => b.Listing)
                .FirstOrDefaultAsync(b => b.Id == reviewDto.BookingId && b.GuestId == userId);

            if (booking == null)
            {
                return NotFound("Booking not found or doesn't belong to the user");
            }

            var existingReview = await _context.Reviews
                .FirstOrDefaultAsync(r => r.ListingId == booking.ListingId && r.GuestId == userId);

            if (existingReview != null)
            {
                return BadRequest("You have already reviewed this stay");
            }

            var review = new Review
            {
                ListingId = booking.ListingId,
                GuestId = userId,
                Rating = reviewDto.Rating,
                Comment = reviewDto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _context.Reviews.AddAsync(review);
                await _context.SaveChangesAsync();
                return Ok("Review with id " + review.Id + " created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the review");
            }
        }
    }
}