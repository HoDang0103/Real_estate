using Backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Repository.StoryService.Dtos
{
    public class CreateStoryDto
    {
        public int CatalogID { get; set; }
        public int PackageID { get; set; }
        public bool Needs { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Save { get; set; }
        public int Floor { get; set; }
        public string? Address { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? Street { get; set; }
        public string? Project { get; set; }
        public string? Location { get; set; }
        public int Area { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
        public string? Document { get; set; }
        public string? Interior { get; set; }
        public int Bedrooms { get; set; }
        public int WC { get; set; }
        public bool State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }
        public string? UserID { get; set; }
        public List<IFormFile>? ImageFiles { get; set; }
    }
}
