namespace WebApplicationAsp.ViewModels
{
    public class ReportViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public string? ApplicationDescription { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int TotalRequirements { get; set; }
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public int NotApplicableCount { get; set; }
        public int PendingCount { get; set; }
        public double CompliancePercentage { get; set; }
        public string RiskLevel { get; set; } = "N/A";
        public string RiskColor { get; set; } = "secondary";
        public List<CategoryReportViewModel> CategoryBreakdown { get; set; } = new();
        public List<string> Recommendations { get; set; } = new();
    }

    public class CategoryReportViewModel
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public int NotApplicableCount { get; set; }
        public int PendingCount { get; set; }
        public double CompliancePercentage { get; set; }
        public string RiskColor { get; set; } = "secondary";
    }
}
