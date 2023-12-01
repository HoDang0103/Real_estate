namespace Backend.Models
{
    public class Package
    {
        public int Id { get; set; }
        public decimal PricePerDay { get; set; }
        public int NumberDay { get; set; }
        public DateTime StartDate { get; set; }
        public virtual ICollection<Story>? Stories { get; set; }
    }
}
