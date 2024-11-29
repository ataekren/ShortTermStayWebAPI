using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ShortTermStayAPI.Model.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string PasswordHash { get; set; }

        public UserRole Role { get; set; }

        public ICollection<Listing> Listings { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }

    /// <summary>
    /// Roles 
    /// </summary>
    public enum UserRole
    {
        /// <summary>
        /// Regular user who can book listings
        /// </summary>
        Guest = 0,
        
        /// <summary>
        /// User who can create and manage listings
        /// </summary>
        Host = 1,
        
        /// <summary>
        /// Web Site Admin
        /// </summary>
        Admin = 2
    }
}
