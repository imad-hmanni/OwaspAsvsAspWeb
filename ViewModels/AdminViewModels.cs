using System.ComponentModel.DataAnnotations;

namespace WebApplicationAsp.ViewModels
{
    public class AdminDashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalApplications { get; set; }
        public int TotalCategories { get; set; }
        public int TotalRequirements { get; set; }
        public int TotalAssessments { get; set; }
        public List<UserListItemViewModel> RecentUsers { get; set; } = new();
    }

    public class UserListItemViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int ApplicationCount { get; set; }
    }

    public class CreateUserViewModel
    {
        [Required]
        [Display(Name = "Nom complet")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Mot de passe")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = "Auditor";
    }

    public class EditUserViewModel
    {
        public string Id { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nom complet")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Rôle")]
        public string Role { get; set; } = string.Empty;
    }

    public class SubCategoryCreateViewModel
    {
        [Required]
        [Display(Name = "Code (ex: V1.1)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Nom")]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Catégorie parente")]
        public int CategoryId { get; set; }
    }

    public class ItemCreateViewModel
    {
        [Required]
        [Display(Name = "Code (ex: V1.1.1)")]
        public string Code { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Niveau (1, 2 ou 3)")]
        public string Level { get; set; } = "1";

        [Required]
        [Display(Name = "Section parente")]
        public int SubCategoryId { get; set; }
    }
}
