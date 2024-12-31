using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

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
        }
    }

    //Les différentes classes sont ici
    public class Lecon
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public required string Contenu { get; set; }
        public int NumeroPage { get; set; } // Numéro unique dans le cours
        public int CoursId { get; set; }
        public required Cours Cours { get; set; }
    }

    public class Cours
    {
        public int Id { get; set; }
        public required string Titre { get; set; }
        public required string Description { get; set; }
        public int ProfesseurId { get; set; }
        public required Professeur Professeur { get; set; }
        public ICollection<Lecon>? Lecons { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; } // Étudiants enrôlés
    }

    public class Enrollment
    {
        public int Id { get; set; }
        public int EtudiantId { get; set; }
        public required Etudiant Etudiant { get; set; }
        public int CoursId { get; set; }
        public required Cours Cours { get; set; }
        public int Progression { get; set; } // Pourcentage de progression
    }

    public class Etudiant
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public required string Nom { get; set; }
        public ICollection<Enrollment>? Enrollments { get; set; }
    }

    public class Professeur
    {
        public int Id { get; set; }
        public required string UserId { get; set; } // Lié à l'utilisateur Identity
        public required string Nom { get; set; }
        public ICollection<Cours>? Cours { get; set; } // Les cours créés par ce professeur
    }
}
