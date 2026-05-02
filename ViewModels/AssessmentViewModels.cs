using WebApplicationAsp.Entities;

namespace WebApplicationAsp.ViewModels
{
    public class ConductViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public List<AssessmentItemDto> Items { get; set; } = new();
    }

    public class AssessmentItemDto
    {
        public int Index { get; set; }
        public int ItemId { get; set; }
        public string ItemCode { get; set; } = string.Empty;
        public string ItemDescription { get; set; } = string.Empty;
        public string ItemLevel { get; set; } = "1";
        public string SubCategoryCode { get; set; } = string.Empty;
        public string SubCategoryName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public AssessmentStatus Status { get; set; } = AssessmentStatus.Pending;
        public string? Comment { get; set; }
    }

    public class AssessmentFormModel
    {
        public int ApplicationId { get; set; }
        public List<AssessmentItemDto> Items { get; set; } = new();
    }

    public class ReviewViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicationName { get; set; } = string.Empty;
        public List<AssessmentItemDto> Items { get; set; } = new();
        public int ValidCount { get; set; }
        public int NotValidCount { get; set; }
        public int NotApplicableCount { get; set; }
        public int PendingCount { get; set; }
        public double CompliancePercentage { get; set; }
    }

    public class AIExplanationResult
    {
        public string Explanation { get; set; } = string.Empty;
        public string Vulnerability { get; set; } = string.Empty;
        public List<string> BestPractices { get; set; } = new();
        public bool Success { get; set; } = true;
        public string? Error { get; set; }
    }
}
