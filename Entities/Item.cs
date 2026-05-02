namespace WebApplicationAsp.Entities
{
    public class Item
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Level { get; set; } = "1";

        public int SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; } = null!;

        public ICollection<Assessment> Assessments { get; set; } = new List<Assessment>();
    }
}
