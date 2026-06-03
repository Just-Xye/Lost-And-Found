using LostAndFound.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ClaimRequests
{
    [Key]
    public int Id { get; set; }

    public int ItemId { get; set; }

    public string? ItemName { get; set; }
    public string? ImageName { get; set; }

    [Required]
    public string RequesterName { get; set; }

    public string? RequesterEmail { get; set; }
    public string? RequesterPhone { get; set; }

    public DateTime RequestDate { get; set; } = DateTime.Now;
    [ForeignKey("ItemId")]
    public FoundItemList? Item { get; set; }
}