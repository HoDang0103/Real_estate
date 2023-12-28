namespace Backend.Repository.StoryService.Dtos
{
    public class SearchStoryDto
    {
        public string? District { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? MinArea { get; set; }
        public int? MaxArea { get; set; }
        public string? Title { get; set; }
    }
}
