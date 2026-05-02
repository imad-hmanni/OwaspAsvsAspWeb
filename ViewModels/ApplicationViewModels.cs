using System.ComponentModel.DataAnnotations;

namespace WebApplicationAsp.ViewModels
{
    public class ApplicationCreateViewModel
    {
        [Required(ErrorMessage = "Le nom est requis")]
        [StringLength(200)]
        [Display(Name = "Nom de l'application")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000)]
        [Display(Name = "Description")]
        public string? Description { get; set; }
    }

    public class ApplicationEditViewModel : ApplicationCreateViewModel
    {
        public int Id { get; set; }
    }

    public class ApplicationListItemViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public int AssessedItems { get; set; }
        public double CompliancePercentage { get; set; }
        public string RiskLevel { get; set; } = "N/A";
    }
}
