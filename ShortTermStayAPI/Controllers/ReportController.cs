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
    public class ReportController : ApiControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ListingsWithRatings")]
        public async Task<ActionResult<PagedResult<ListingReportDto>>> GetListingsWithRatings([FromQuery] ListingReportSearchDto searchParams)
        {
            // Verify if the user is an admin
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized("User not authenticated");
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null || user.Role != UserRole.Admin)
            {
                return Unauthorized("Only administrators can access this report");
            }

            // Validate page parameters
            if (searchParams.PageNumber < 1) searchParams.PageNumber = 1;
            if (searchParams.PageSize < 1) searchParams.PageSize = 10;
            if (searchParams.PageSize > 50) searchParams.PageSize = 50;

            var query = from l in _context.Listings
                       join u in _context.Users on l.HostId equals u.Id
                       select new { Listing = l, Host = u };

            // Apply filters
            if (!string.IsNullOrEmpty(searchParams.Country))
            {
                query = query.Where(x => x.Listing.Country.ToLower() == searchParams.Country.ToLower());
            }

            if (!string.IsNullOrEmpty(searchParams.City))
            {
                query = query.Where(x => x.Listing.City.ToLower() == searchParams.City.ToLower());
            }

            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)searchParams.PageSize);

            var listings = await query
                .Skip((searchParams.PageNumber - 1) * searchParams.PageSize)
                .Take(searchParams.PageSize)
                .Select(x => new ListingReportDto
                {
                    Id = x.Listing.Id,
                    Country = x.Listing.Country,
                    City = x.Listing.City,
                    NumberOfPeople = x.Listing.NumberOfPeople,
                    PricePerNight = x.Listing.PricePerNight,
                    AverageRating = x.Listing.Reviews.Any() ? x.Listing.Reviews.Average(r => r.Rating) : null,
                    NumberOfReviews = x.Listing.Reviews.Count,
                    HostId = x.Listing.HostId,
                    HostEmail = x.Host.Email
                })
                .OrderByDescending(l => l.AverageRating)
                .ThenByDescending(l => l.NumberOfReviews)
                .ToListAsync();

            var result = new PagedResult<ListingReportDto>
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