using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace LostAndFound.Models
{
    public class LostItemList
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string? ItemName { get; set; }
        public string? ImageName { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public string? Description { get; set; }
        [Required]
        public string? LocationLost { get; set; }
        [Required]
        public DateTime DateLost { get; set; }
        [Required]
        public string? OwnerName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        public string? PhoneNumber { get; set; }
        public string? Section { get; set; }

    }
}
