using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.DTOs
{
    public class ListingCreateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of people must be at least 1.")]
        public int NumberOfPeople { get; set; }

        [Required]
        [MaxLength(100)]
        public string Country { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; }

        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal PricePerNight { get; set; }
    }
}