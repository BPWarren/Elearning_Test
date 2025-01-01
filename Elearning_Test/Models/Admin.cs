namespace Elearning_Test.Models
{
    public class Admin : IHasTimestamps
    {
        public int Id
        {
            get; set;
        }
        public required string UserId
        {
            get; set;
        }
        public required string Nom
        {
            get; set;
        }
        public ICollection<Enrollment>? Enrollments
        {
            get; set;
        }
        public ICollection<Payment>? Payments
        {
            get; set;
        }
        public ICollection<Certification>? Certifications
        {
            get; set;
        }
        public ICollection<Evaluation>? Evaluations
        {
            get; set;
        }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
    }
}
