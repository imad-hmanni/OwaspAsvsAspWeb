using Microsoft.AspNetCore.Identity;

namespace WebApplicationAsp.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
