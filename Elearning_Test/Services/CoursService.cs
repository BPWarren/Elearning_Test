

using Elearning_Test.Models;
using Elearning_Test.Data;
using Microsoft.EntityFrameworkCore;

namespace Elearning_Test.Services
{

    public class CoursService : ICoursService
    {
        private readonly ApplicationDbContext _context;

        public CoursService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer les cours d'un professeur
        public async Task<IEnumerable<Cours>> GetCoursByProfesseurAsync(string professeurId)
        {
            return await _context.Cours
                .Where(c => c.ProfesseurId == professeurId)
                .ToListAsync();
        }

        // Récupérer un cours par son Id
        public async Task<Cours?> GetCoursByIdAsync(int id)
        {
            return await _context.Cours.FindAsync(id);
        }

        // Créer un cours
        public async Task CreateCoursAsync(Cours cours)
        {
            if (cours == null)
                throw new ArgumentNullException(nameof(cours));

            _context.Cours.Add(cours);
            await _context.SaveChangesAsync();
        }

        // Mettre à jour un cours
        public async Task UpdateCoursAsync(Cours cours)
        {
            if (cours == null)
                throw new ArgumentNullException(nameof(cours));

            var existingCours = await _context.Cours.FindAsync(cours.Id);
            if (existingCours != null)
            {
                existingCours.Titre = cours.Titre;
                existingCours.Description = cours.Description;
                existingCours.Price = cours.Price;
                existingCours.ImageFile = cours.ImageFile; // Mise à jour de l'image
                existingCours.ProfesseurId = cours.ProfesseurId;

                _context.Cours.Update(existingCours);
                await _context.SaveChangesAsync();
            }
        }

        // Supprimer un cours
        public async Task DeleteCoursAsync(int id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours != null)
            {
                _context.Cours.Remove(cours);
                await _context.SaveChangesAsync();
            }
        }
    }
    public interface ICoursService
    {
        Task<IEnumerable<Cours>> GetCoursByProfesseurAsync(string professeurId);
        Task<Cours?> GetCoursByIdAsync(int id);
        Task CreateCoursAsync(Cours cours);
        Task UpdateCoursAsync(Cours cours);
        Task DeleteCoursAsync(int id);
    }


}