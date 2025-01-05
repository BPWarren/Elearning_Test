using Elearning_Test.Models;
using Elearning_Test.Data;
using Microsoft.EntityFrameworkCore;
/*using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
*/

namespace Elearning_Test.Services
{
    

    public class CategorieService : ICategorieService
    {
        private readonly ApplicationDbContext _context;

        public CategorieService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Récupérer toutes les catégories
        public async Task<IEnumerable<Categorie>> GetAllCategoriesAsync(int? limit = null)
        {
            var query = _context.Categories.AsQueryable();

            // Appliquer la limite si elle est spécifiée
            if (limit.HasValue && limit.Value > 0)
            {
                query = query.Take(limit.Value);
            }

            return await query.ToListAsync();
        }

        // Récupérer une catégorie par son ID
        public async Task<Categorie?> GetCategorieByIdAsync(int id)
        {
            return await _context.Categories.FindAsync(id);
        }

        // Créer une nouvelle catégorie
        public async Task CreateCategorieAsync(Categorie categorie)
        {
            if (categorie == null)
                throw new ArgumentNullException(nameof(categorie));

            _context.Categories.Add(categorie);
            await _context.SaveChangesAsync();
        }

        // Mettre à jour une catégorie existante
        public async Task UpdateCategorieAsync(Categorie categorie)
        {
            if (categorie == null)
                throw new ArgumentNullException(nameof(categorie));

            var existingCategorie = await _context.Categories.FindAsync(categorie.Id);
            if (existingCategorie != null)
            {
                existingCategorie.Intitule = categorie.Intitule;
                existingCategorie.Description = categorie.Description;
                existingCategorie.ImageFile = categorie.ImageFile;
                existingCategorie.UpdatedAt = DateTime.UtcNow;

                _context.Categories.Update(existingCategorie);
                await _context.SaveChangesAsync();
            }
        }

        // Supprimer une catégorie
        public async Task DeleteCategorieAsync(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie != null)
            {
                _context.Categories.Remove(categorie);
                await _context.SaveChangesAsync();
            }
        }
    }

    public interface ICategorieService
    {
        Task<IEnumerable<Categorie>> GetAllCategoriesAsync(int? limit = null); // Ajout du paramètre limit
        Task<Categorie?> GetCategorieByIdAsync(int id);
        Task CreateCategorieAsync(Categorie categorie);
        Task UpdateCategorieAsync(Categorie categorie);
        Task DeleteCategorieAsync(int id);
    }
}

