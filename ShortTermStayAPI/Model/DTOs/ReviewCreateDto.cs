using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.DTOs
{
    public class ReviewCreateDto
    {
        [Required]
        public int BookingId { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Comment { get; set; }
    }
} 