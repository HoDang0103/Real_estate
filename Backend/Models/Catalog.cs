namespace Backend.Models
{
    public class Catalog
    {
        public int Id { get; set; }
        public string? CatalogName { get; set; }
        public virtual ICollection<Story>? Stories { get; set; }
    }
}
