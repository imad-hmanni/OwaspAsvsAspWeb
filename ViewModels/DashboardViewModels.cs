namespace WebApplicationAsp.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalApplications { get; set; }
        public int TotalRequirements { get; set; }
        public int TotalAssessments { get; set; }
        public double OverallCompliance { get; set; }
        public string RiskLevel { get; set; } = "N/A";
        public List<CategoryComplianceViewModel> CategoryCompliance { get; set; } = new();
        public List<ApplicationSummaryViewModel> RecentApplications { get; set; } = new();
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public int NotApplicableCount { get; set; }
        public int PendingCount { get; set; }
    }

    public class CategoryComplianceViewModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public double CompliancePercentage { get; set; }
    }

    public class ApplicationSummaryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double CompliancePercentage { get; set; }
        public string RiskLevel { get; set; } = "N/A";
        public DateTime CreatedAt { get; set; }
    }
}
