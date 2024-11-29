using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShortTermStayAPI.Data;
using ShortTermStayAPI.Model.Entities;
using System.Security.Claims;
using ShortTermStayAPI.Model.DTOs;
using Microsoft.EntityFrameworkCore;

namespace ShortTermStayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ListingController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ListingController(ApplicationDbContext context)
        {
            _context = context;
        }


        [HttpPost("CreateListing")]
        [Authorize]
        public async Task<IActionResult> InsertListing([FromBody] ListingCreateDto listingDto)
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

            var host = await _context.Users.FindAsync(userId);
            if (host == null)
            {
                return NotFound("User not found");
            }

            if (host.Role != UserRole.Host)
            {
                return Unauthorized("Only hosts can create listings");
            }

            var listing = new Listing
            {
                NumberOfPeople = listingDto.NumberOfPeople,
                Country = listingDto.Country,
                City = listingDto.City,
                PricePerNight = listingDto.PricePerNight,
                HostId = userId
            };

            // Basic validation
            if (listing.PricePerNight <= 0)
            {
                return BadRequest("Price must be greater than 0");
            }

            if (string.IsNullOrWhiteSpace(listing.City) || string.IsNullOrWhiteSpace(listing.Country))
            {
                return BadRequest("City and Country are required");
            }

            if (listing.NumberOfPeople <= 0)
            {
                return BadRequest("Number of people must be greater than 0");
            }

            try
            {
                await _context.Listings.AddAsync(listing);
                await _context.SaveChangesAsync();
                //return CreatedAtAction(nameof(InsertListing), new { id = listing.Id }, listing);
                return Ok("Listing with id " + listing.Id + " created successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while creating the listing");
            }
        }

        [HttpGet("QueryListings")]
        public async Task<ActionResult<PagedResult<ListingResponseDto>>> QueryListings([FromQuery] ListingSearchDto searchParams)
        {
            if (searchParams.PageNumber < 1) searchParams.PageNumber = 1;
            if (searchParams.PageSize < 1) searchParams.PageSize = 10;
            if (searchParams.PageSize > 50) searchParams.PageSize = 50;

            var query = _context.Listings
                .Include(l => l.Reviews)
                .AsQueryable();

            // Apply filters
            if (searchParams.NumberOfPeople.HasValue)
            {
                query = query.Where(l => l.NumberOfPeople >= searchParams.NumberOfPeople.Value);
            }

            if (!string.IsNullOrEmpty(searchParams.Country))
            {
                query = query.Where(l => l.Country.ToLower() == searchParams.Country.ToLower());
            }

            if (!string.IsNullOrEmpty(searchParams.City))
            {
                query = query.Where(l => l.City.ToLower() == searchParams.City.ToLower());
            }

            if (searchParams.StartDate.HasValue && searchParams.EndDate.HasValue)
            {
                var unavailableListingIds = await _context.Bookings
                    .Where(b => 
                        (searchParams.StartDate < b.EndDate && searchParams.StartDate >= b.StartDate) ||
                        (searchParams.EndDate > b.StartDate && searchParams.EndDate <= b.EndDate) ||
                        (searchParams.StartDate <= b.StartDate && searchParams.EndDate >= b.EndDate) ||
                        (searchParams.StartDate >= b.StartDate && searchParams.EndDate <= b.EndDate))
                    .Select(b => b.ListingId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(l => !unavailableListingIds.Contains(l.Id));
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

            var listings = await query
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .Select(l => new ListingResponseDto
                {
                    Id = l.Id,
                    NumberOfPeople = l.NumberOfPeople,
                    Country = l.Country,
                    City = l.City,
                    PricePerNight = l.PricePerNight,
                    HostId = l.HostId,
                    AverageRating = l.Reviews.Any() ? l.Reviews.Average(r => (double)r.Rating) : null,
                    NumberOfReviews = l.Reviews.Count
                })
                .ToListAsync();

            var result = new PagedResult<ListingResponseDto>
            {
                Items = listings,
                PageNumber = searchParams.PageNumber,
                PageSize = searchParams.PageSize,
                TotalPages = totalPages,
                TotalCount = totalCount
            };

            return Ok(result);
        }
    }
}