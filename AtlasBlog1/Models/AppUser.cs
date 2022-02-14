using Microsoft.AspNetCore.Identity;

namespace AtlasBlog1.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string FullName { get { return $"{FirstName} {LastName}"; } }
        public string? DisplayName { get; set; }

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();
    }
}
