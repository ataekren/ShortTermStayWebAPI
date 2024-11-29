using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.Entities
{
    public class Booking
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int ListingId { get; set; }

        [ForeignKey("ListingId")]
        public Listing Listing { get; set; }

        [Required]
        public int GuestId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string NamesOfPeople { get; set; }

    }
}
