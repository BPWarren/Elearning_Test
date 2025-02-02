﻿

using Elearning_Test.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace Elearning_Test.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
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
        public DbSet<Categorie> Categories { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configurer le TPT pour Etudiant et Professeur
            builder.Entity<Etudiant>().ToTable("Etudiants"); // Table distincte pour Etudiant
            builder.Entity<Professeur>().ToTable("Professeurs"); // Table distincte pour Professeur

            // Relation Professeur-Cours
            builder.Entity<Cours>()
                .HasOne(c => c.Professeur)
                .WithMany(p => p.Cours)
                .HasForeignKey(c => c.ProfesseurId)
                .OnDelete(DeleteBehavior.Restrict); // Éviter la suppression en cascade

            // Relation Cours-Leçon
            builder.Entity<Lecon>()
                .HasOne(l => l.Cours)
                .WithMany(c => c.Lecons)
                .HasForeignKey(l => l.CoursId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Étudiant-Cours (via Enrollment)
            builder.Entity<Enrollment>()
                .HasOne(e => e.Etudiant)
                .WithMany(e => e.Enrollments)
                .HasForeignKey(e => e.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Enrollment>()
                .HasOne(e => e.Cours)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CoursId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Étudiant-Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Etudiant)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Cours-Payment
            builder.Entity<Payment>()
                .HasOne(p => p.Cours)
                .WithMany()
                .HasForeignKey(p => p.CoursId)
                .OnDelete(DeleteBehavior.Restrict); // Éviter la suppression en cascade

            // Relation Étudiant-Certification
            builder.Entity<Certification>()
                .HasOne(c => c.Etudiant)
                .WithMany(e => e.Certifications)
                .HasForeignKey(c => c.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Cours-Certification
            builder.Entity<Certification>()
                .HasOne(c => c.Cours)
                .WithMany()
                .HasForeignKey(c => c.CoursId)
                .OnDelete(DeleteBehavior.Restrict); // Éviter la suppression en cascade

            // Relation Categorie-Cours
            builder.Entity<Cours>()
                .HasOne(c => c.Categorie)
                .WithMany(cat => cat.Cours)
                .HasForeignKey(c => c.CategorieId)
                .OnDelete(DeleteBehavior.Restrict); // Éviter la suppression en cascade

            // Relation Cours-Evaluation
            builder.Entity<Evaluation>()
                .HasOne(e => e.Cours)
                .WithMany(c => c.Evaluations)
                .HasForeignKey(e => e.CoursId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Étudiant-Evaluation
            builder.Entity<Evaluation>()
                .HasOne(e => e.Etudiant)
                .WithMany(e => e.Evaluations)
                .HasForeignKey(e => e.EtudiantId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurer la précision de Amount dans Payment
            builder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2); // 18 chiffres au total, dont 2 après la virgule

            // Configurer la précision de Price dans Cours
            builder.Entity<Cours>()
                .Property(c => c.Price)
                .HasPrecision(18, 2);
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




