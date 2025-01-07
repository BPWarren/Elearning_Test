using Elearning_Test.Data;
using Elearning_Test.Models;
using Microsoft.EntityFrameworkCore;

namespace Elearning_Test.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vérifie si un étudiant est déjà inscrit à un cours
        public async Task<bool> EstEtudiantInscritAsync(string etudiantId, int coursId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.EtudiantId == etudiantId && e.CoursId == coursId);
        }

        // Récupère une inscription spécifique
        public async Task<Enrollment?> GetEnrollmentAsync(string etudiantId, int coursId)
        {
            return await _context.Enrollments
                .FirstOrDefaultAsync(e => e.EtudiantId == etudiantId && e.CoursId == coursId);
        }

        // Ajoute une nouvelle inscription
        public async Task AjouterEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
        }

        // Met à jour une inscription existante
        public async Task MettreAJourEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
        }

        // Récupère toutes les inscriptions d'un étudiant
        public async Task<List<Enrollment>> GetEnrollmentsParEtudiantAsync(string etudiantId)
        {
            return await _context.Enrollments
                .Where(e => e.EtudiantId == etudiantId)
                .Include(e => e.Cours)
                .Include(e => e.CurrentLecon)
                .ToListAsync();
        }

        // Récupère toutes les inscriptions pour un cours
        public async Task<List<Enrollment>> GetEnrollmentsParCoursAsync(int coursId)
        {
            return await _context.Enrollments
                .Where(e => e.CoursId == coursId)
                .Include(e => e.Etudiant)
                .ToListAsync();
        }
    }

    public interface IEnrollmentService
    {
        Task<bool> EstEtudiantInscritAsync(string etudiantId, int coursId);
        Task<Enrollment> GetEnrollmentAsync(string etudiantId, int coursId);
        Task AjouterEnrollmentAsync(Enrollment enrollment);
        Task MettreAJourEnrollmentAsync(Enrollment enrollment);
        Task<List<Enrollment>> GetEnrollmentsParEtudiantAsync(string etudiantId);
        Task<List<Enrollment>> GetEnrollmentsParCoursAsync(int coursId);
    }
}
