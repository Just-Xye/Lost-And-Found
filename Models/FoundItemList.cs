using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace LostAndFound.Models
{
    public class FoundItemList
    {
        public bool IsApproved { get; set; } = false;
        [Key]
        public int Id { get; set; }
        public string? ItemName { get; set; }
        [Required]
        public string? PickupLocation { get; set; }
        [Required]
        public string? FoundAt { get; set; }
        public string? Name { get; set; }
        [Required]
        public DateTime DateFound { get; set; }
        [Required]
        public string? Description { get; set; }
        [Required]
        public string? ImageName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}
