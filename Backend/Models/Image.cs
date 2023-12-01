namespace Backend.Models
{
    public class Image
    {
        public int Id { get; set; }
        public int StoryID { get; set; }
        public string? ImageLink { get; set; }
        public bool IsPrimary { get; set; }
        public virtual Story? Story { get; set; }
    }
}
