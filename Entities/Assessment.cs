namespace WebApplicationAsp.Entities
{
    public class Assessment
    {
        public int Id { get; set; }

        public int ApplicationId { get; set; }
        public Application Application { get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;

        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;
        public string? Comment { get; set; }

        public DateTime? AssessedAt { get; set; }
        public string? AssessedById { get; set; }
        public ApplicationUser? AssessedBy { get; set; }
    }
}
