namespace ShortTermStayAPI.Model.DTOs
{
    public class ListingResponseDto
    {
        public int Id { get; set; }
        public int NumberOfPeople { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public decimal PricePerNight { get; set; }
        public double? AverageRating { get; set; }
        public int NumberOfReviews { get; set; }
        public int HostId { get; set; }
    }
}