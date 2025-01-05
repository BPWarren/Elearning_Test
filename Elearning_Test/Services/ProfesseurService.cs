using Elearning_Test.Models;
using Elearning_Test.Data;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Elearning_Test.Services
{
    public class ProfesseurService : IProfesseurService
    {
        private readonly ApplicationDbContext _context;

        public ProfesseurService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer tous les professeurs
        public async Task<IEnumerable<Professeur>> GetAllProfesseursAsync()
        {
            return await _context.Professeurs.ToListAsync();
        }

        // Récupérer un professeur par son Id
        public async Task<Professeur?> GetProfesseurByIdAsync(string id)
        {
            return await _context.Professeurs.FindAsync(id);
        }

        // Créer un professeur
        public async Task CreateProfesseurAsync(Professeur professeur)
        {
            if (professeur == null)
                throw new ArgumentNullException(nameof(professeur));

            _context.Professeurs.Add(professeur);
            await _context.SaveChangesAsync();
        }

        // Mettre à jour un professeur
        public async Task UpdateProfesseurAsync(Professeur professeur)
        {
            if (professeur == null)
                throw new ArgumentNullException(nameof(professeur));

            var existingProfesseur = await _context.Professeurs.FindAsync(professeur.Id);
            if (existingProfesseur != null)
            {
                existingProfesseur.Nom = professeur.Nom;
                existingProfesseur.Prenom = professeur.Prenom;
                existingProfesseur.Email = professeur.Email;
                existingProfesseur.Specialite = professeur.Specialite;

                _context.Professeurs.Update(existingProfesseur);
                await _context.SaveChangesAsync();
            }
        }

        // Supprimer un professeur
        public async Task DeleteProfesseurAsync(string id)
        {
            var professeur = await _context.Professeurs.FindAsync(id);
            if (professeur != null)
            {
                _context.Professeurs.Remove(professeur);
                await _context.SaveChangesAsync();
            }
        }

        // Récupérer les cours d'un professeur
        public async Task<IEnumerable<Cours>> GetCoursByProfesseurIdAsync(string professeurId)
        {
            return await _context.Cours
                .Where(c => c.ProfesseurId == professeurId)
                .ToListAsync();
        }
    }

    public interface IProfesseurService
    {
        Task<IEnumerable<Professeur>> GetAllProfesseursAsync();
        Task<Professeur?> GetProfesseurByIdAsync(string id);
        Task CreateProfesseurAsync(Professeur professeur);
        Task UpdateProfesseurAsync(Professeur professeur);
        Task DeleteProfesseurAsync(string id);
        Task<IEnumerable<Cours>> GetCoursByProfesseurIdAsync(string professeurId);
    }
}