

using Elearning_Test.Models;
using Elearning_Test.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elearning_Test.Services
{
    public class EtudiantService : IEtudiantService
    {
        private readonly ApplicationDbContext _context;

        public EtudiantService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les étudiants
        public async Task<IEnumerable<Etudiant>> GetAllEtudiantsAsync()
        {
            return await _context.Etudiants.ToListAsync();
        }

        // Récupérer un étudiant par son Id
        public async Task<Etudiant?> GetEtudiantByIdAsync(string id)
        {
            return await _context.Etudiants.FindAsync(id);
        }

        // Créer un étudiant
        public async Task CreateEtudiantAsync(Etudiant etudiant)
        {
            if (etudiant == null)
                throw new ArgumentNullException(nameof(etudiant));

            _context.Etudiants.Add(etudiant);
            await _context.SaveChangesAsync();
        }

        // Mettre à jour un étudiant
        public async Task UpdateEtudiantAsync(Etudiant etudiant)
        {
            if (etudiant == null)
                throw new ArgumentNullException(nameof(etudiant));

            var existingEtudiant = await _context.Etudiants.FindAsync(etudiant.Id);
            if (existingEtudiant != null)
            {
                existingEtudiant.Cne = etudiant.Cne;
                existingEtudiant.Nom = etudiant.Nom;
                existingEtudiant.Prenom = etudiant.Prenom;
                existingEtudiant.Email = etudiant.Email;
                existingEtudiant.IsConnected = etudiant.IsConnected;

                _context.Etudiants.Update(existingEtudiant);
                await _context.SaveChangesAsync();
            }
        }

        // Supprimer un étudiant
        public async Task DeleteEtudiantAsync(string id)
        {
            var etudiant = await _context.Etudiants.FindAsync(id);
            if (etudiant != null)
            {
                _context.Etudiants.Remove(etudiant);
                await _context.SaveChangesAsync();
            }
        }

        // Récupérer les inscriptions d'un étudiant
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByEtudiantIdAsync(string etudiantId)
        {
            return await _context.Enrollments
                .Where(e => e.EtudiantId == etudiantId)
                .ToListAsync();
        }

        // Récupérer les paiements d'un étudiant
        public async Task<IEnumerable<Payment>> GetPaymentsByEtudiantIdAsync(string etudiantId)
        {
            return await _context.Payments
                .Where(p => p.EtudiantId == etudiantId)
                .ToListAsync();
        }

        // Récupérer les certifications d'un étudiant
        public async Task<IEnumerable<Certification>> GetCertificationsByEtudiantIdAsync(string etudiantId)
        {
            return await _context.Certifications
                .Where(c => c.EtudiantId == etudiantId)
                .ToListAsync();
        }

        // Récupérer les évaluations d'un étudiant
        public async Task<IEnumerable<Evaluation>> GetEvaluationsByEtudiantIdAsync(string etudiantId)
        {
            return await _context.Evaluations
                .Where(e => e.EtudiantId == etudiantId)
                .ToListAsync();
        }
    }

    public interface IEtudiantService
    {
        Task<IEnumerable<Etudiant>> GetAllEtudiantsAsync();
        Task<Etudiant?> GetEtudiantByIdAsync(string id);
        Task CreateEtudiantAsync(Etudiant etudiant);
        Task UpdateEtudiantAsync(Etudiant etudiant);
        Task DeleteEtudiantAsync(string id);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByEtudiantIdAsync(string etudiantId);
        Task<IEnumerable<Payment>> GetPaymentsByEtudiantIdAsync(string etudiantId);
        Task<IEnumerable<Certification>> GetCertificationsByEtudiantIdAsync(string etudiantId);
        Task<IEnumerable<Evaluation>> GetEvaluationsByEtudiantIdAsync(string etudiantId);
    }
}
