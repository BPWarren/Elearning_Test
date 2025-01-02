using Microsoft.AspNetCore.Identity;

namespace Elearning_Test.Models
{
    public class Admin : IdentityUser, IHasTimestamps
    {
        public required string Nom
        {
            get; set;
        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
