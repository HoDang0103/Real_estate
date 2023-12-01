using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Story
    {
        public int Id { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? UserID { get; set; }
        public virtual Catalog? Catalog { get; set; }
        public virtual Package? Package { get; set; }
        [ForeignKey("UserID")]
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Image> Images { get; set; } = new List<Image>();
    }
}
