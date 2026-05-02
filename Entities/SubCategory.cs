namespace WebApplicationAsp.Entities
{
    public class SubCategory
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public ICollection<Item> Items { get; set; } = new List<Item>();
    }
}
