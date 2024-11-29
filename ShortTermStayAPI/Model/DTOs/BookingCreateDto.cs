using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.DTOs
{
    public class BookingCreateDto
    {
        [Required]
        public int ListingId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [MinLength(1)]
        public string NamesOfPeople { get; set; }
    }
} 