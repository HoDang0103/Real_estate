namespace Backend.Repository.StoryService.Dtos
{
    public class StoryDto
    {
        public int Id { get; set; }
        public bool Needs { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool Save { get; set; }
        public int Floor { get; set; }
        public string? Address { get; set; }
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
        public DateTime CreatedAt { get; set; }
        public DateTime EndDate { get; set; }
        public CatalogDto? Catalog { get; set; }
        public List<ImageDto>? Images { get; set; }
        public ApplicationUserDto? User { get; set; }
    }

    public class ImageDto
    {
        public int Id { get; set; }
        public string? ImageLink { get; set; }
    }

    public class ApplicationUserDto 
    {
        public string? Id { get; set; }
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
    }

    public class CatalogDto
    {
        public int Id { get; set; }
        public string? CatalogName { get; set; }
    }
}
