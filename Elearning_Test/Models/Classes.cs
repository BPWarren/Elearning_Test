using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Elearning_Test.Models
{
    public class Lecon : IHasTimestamps
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public required string Contenu { get; set; }
        public int NumeroPage { get; set; }
        public int CoursId { get; set; }
        public Cours? Cours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Cours : IHasTimestamps
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public required string Description { get; set; }
        public string? ProfesseurId { get; set; }
        public Professeur? Professeur { get; set; }
        public ICollection<Lecon>? Lecons { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Evaluation>? Evaluations { get; set; }
        public required decimal Price { get; set; }
        public int CategorieId { get; set; }
        public Categorie? Categorie { get; set; }
        public required string ImageFile { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Enrollment : IHasTimestamps
    {
        public int Id { get; set; }
        public required string EtudiantId { get; set; }
        public required Etudiant Etudiant { get; set; }
        public int CoursId { get; set; }
        public Cours? Cours { get; set; }
        public int Progression { get; set; }
        public bool IsConnected { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Etudiant : IdentityUser, IHasTimestamps
    {
        public required string Cne { get; set; }
        public required string Nom { get; set; }
        public required string Prenom { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
        public ICollection<Payment>? Payments { get; set; } = new List<Payment>();
        public ICollection<Certification>? Certifications { get; set; }
        public ICollection<Evaluation>? Evaluations { get; set; }
        public bool IsConnected { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Professeur : IdentityUser, IHasTimestamps
    {
        public required string Nom { get; set; }
        public required string Prenom { get; set; }
        public required string Specialite { get; set; }
        public ICollection<Cours>? Cours { get; set; }
        public bool IsConnected { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public interface IHasTimestamps
    {
        DateTime CreatedAt { get; set; }
        DateTime UpdatedAt { get; set; }
    }

    public class Payment : IHasTimestamps
    {
        public int PaymentId { get; set; }
        public required string OwnerName { get; set; }
        public required string EtudiantId { get; set; }
        public Etudiant? Etudiant { get; set; }
        public int CoursId { get; set; }
        public required Cours Cours { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public required int CVC { get; set; }
        public required string NumeroCarte { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Certification : IHasTimestamps
    {
        public int Id { get; set; }
        public required string EtudiantId { get; set; }
        public Etudiant? Etudiant { get; set; }
        public int CoursId { get; set; }
        public required Cours Cours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Categorie : IHasTimestamps
    {
        public int Id { get; set; }
        public required string Intitule { get; set; }
        public string? Description { get; set; }
        public required string ImageFile { get; set; }
        public ICollection<Cours>? Cours { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class Evaluation : IHasTimestamps
    {
        public int Id { get; set; }
        public required string Content { get; set; }
        public int CoursId { get; set; }
        public Cours? Cours { get; set; }
        public required string EtudiantId { get; set; }
        public Etudiant? Etudiant { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }


}
