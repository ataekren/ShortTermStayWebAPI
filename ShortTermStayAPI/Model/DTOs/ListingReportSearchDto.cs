using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.DTOs
{
    public class ListingReportSearchDto
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}