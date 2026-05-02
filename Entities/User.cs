using static System.Net.Mime.MediaTypeNames;

namespace WebApplicationAsp.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }

        public ICollection<Application> Applications { get; set; }
    }
}
