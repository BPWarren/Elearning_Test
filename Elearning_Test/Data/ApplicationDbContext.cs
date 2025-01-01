using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Elearning_Test.Models;

namespace Elearning_Test.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
              : base(options)
        {
        }

        public DbSet<Lecon> Lecons { get; set; }
        public DbSet<Cours> Cours { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Etudiant> Etudiants { get; set; }
        public DbSet<Professeur> Professeurs { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Certification> Certifications { get; set; }
        public DbSet<Categorie> Categories { get; set; } // Ajouter
        public DbSet<Evaluation> Evaluations { get; set; } // Ajouter

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Relation Professeur-Cours
            builder.Entity<Cours>()
                .HasOne(c => c.Professeur)
                .WithMany(p => p.Cours)
                .HasForeignKey(c => c.ProfesseurId);

            // Relation Cours-Leçon
            builder.Entity<Lecon>()
                .HasOne(l => l.Cours)
                .WithMany(c => c.Lecons)
                .HasForeignKey(l => l.CoursId);

            // Relation Étudiant-Cours (via Enrollment)
            builder.Entity<Enrollment>()
                .HasOne(e => e.Etudiant)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.EtudiantId);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Cours)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CoursId);

            // Relation Étudiant-Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Etudiant)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EtudiantId);

            // Relation Cours-Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Cours)
                .WithMany()
                .HasForeignKey(p => p.CoursId);

            // Relation Étudiant-Certification
            builder.Entity<Certification>()
                .HasOne(c => c.Etudiant)
                .WithMany(e => e.Certifications)
                .HasForeignKey(c => c.EtudiantId);

            // Relation Cours-Certification
            builder.Entity<Certification>()
                .HasOne(c => c.Cours)
                .WithMany()
                .HasForeignKey(c => c.CoursId);

            // Relation Categorie-Cours
            builder.Entity<Cours>()
                .HasOne(c => c.Categorie)
                .WithMany(cat => cat.Cours)
                .HasForeignKey(c => c.CategorieId);

            // Relation Cours-Evaluation
            builder.Entity<Evaluation>()
                .HasOne(e => e.Cours)
                .WithMany(c => c.Evaluations)
                .HasForeignKey(e => e.CoursId);

            // Relation Étudiant-Evaluation
            builder.Entity<Evaluation>()
                .HasOne(e => e.Etudiant)
                .WithMany(e => e.Evaluations)
                .HasForeignKey(e => e.EtudiantId);
        }

        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }

        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is IHasTimestamps && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                var entity = (IHasTimestamps)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedAt = DateTime.UtcNow;
                }
                entity.UpdatedAt = DateTime.UtcNow;
            }
        }
    }

    
}
//Les différentes classes sont ici
public class Lecon: IHasTimestamps
{
    public int Id { get; set; }
    public required string Titre { get; set; }
    public required string Contenu { get; set; }
    public int NumeroPage { get; set; } // Numéro unique dans le cours
    public int CoursId { get; set; }
    public required Cours Cours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}
public class Cours: IHasTimestamps
{
    public int Id { get; set; }
    public required string Titre { get; set; }
    public required string Description { get; set; }
    public int ProfesseurId { get; set; }
    public required Professeur Professeur { get; set; }
    public ICollection<Lecon>? Lecons { get; set; }
    public ICollection<Enrollment>? Enrollments { get; set; } // Étudiants enrôlés
    public ICollection<Evaluation>? Evaluations { get; set; }
    public required float Price { get; set; }
    public int CategorieId { get; set; }
    public required Categorie Categorie { get; set; }
    public required string ImageFile { get; set; } // Fichier chargé par l'utilisateur
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}


public class Enrollment: IHasTimestamps
{
    public int Id { get; set; }
    public int EtudiantId { get; set; }
    public required Etudiant Etudiant { get; set; }
    public int CoursId { get; set; }
    public required Cours Cours { get; set; }
    public int Progression { get; set; } // Pourcentage de progression
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}

public class Etudiant: IHasTimestamps
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string Nom { get; set; }
    public ICollection<Enrollment>? Enrollments { get; set; }
    public ICollection<Payment>? Payments { get; set; }
    public ICollection<Certification>? Certifications { get; set; }
    public ICollection<Evaluation>? Evaluations { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}

public class Professeur: IHasTimestamps
{
    public int Id { get; set; }
    public required string UserId { get; set; }
    public required string Nom { get; set; }
    public ICollection<Cours>? Cours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}
//Interface
public interface IHasTimestamps
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
}

public class Payment:IHasTimestamps
{
    public int PaymentId { get; set; }
    public required string OwnerName {get; set;}
    public int EtudiantId { get; set; }
    public required Etudiant Etudiant { get; set; }
    public int CoursId { get; set; }
    public required Cours Cours { get; set; }
    public decimal Amount { get; set; } // Montant du paiement
    public DateTime PaymentDate { get; set; } // Date du paiement
    public required int CVC { get; set; }
    public required string NumeroCarte { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour

}

public class Certification:IHasTimestamps
{
    public int Id { get; set; }
    public int EtudiantId { get; set; }
    public required Etudiant Etudiant { get; set; }
    public int CoursId { get; set; }
    public required Cours Cours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour

}

public class Categorie:IHasTimestamps
{
    public int Id { get; set; }
    public required string  Intitule { get; set; }
    public string? Description {  get; set; }
    public required string ImageFile { get; set; } // Fichier chargé par l'utilisateur
    public ICollection<Cours>? Cours { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}

public class Evaluation : IHasTimestamps
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public int CoursId { get; set; }
    public required Cours Cours { get; set; }
    public int EtudiantId { get; set; }
    public required Etudiant Etudiant { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Date de création
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; // Date de mise à jour
}


