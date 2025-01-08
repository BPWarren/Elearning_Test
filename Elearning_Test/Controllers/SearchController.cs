

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Elearning_Test.Controllers
{
    public class SearchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SearchController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Action pour la recherche
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                // Si la requête est vide, retourner une vue vide
                return View(new SearchResultsViewModel { Categories = new List<Categorie>(), Cours = new List<Cours>(), Query = query });
            }

            // Rechercher les catégories correspondantes
            var categories = await _context.Categories
                .Where(c => c.Intitule.Contains(query))
                .ToListAsync();

            // Rechercher les cours correspondants
            var cours = await _context.Cours
                .Where(c => c.Titre.Contains(query) || c.Description.Contains(query))
                .ToListAsync();

            // Passer les résultats à la vue
            var viewModel = new SearchResultsViewModel
            {
                Categories = categories,
                Cours = cours,
                Query = query
            };

            return View(viewModel);
        }

        // Action pour afficher les détails d'une catégorie
        public async Task<IActionResult> CategorieDetails(int id)
        {
            var categorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie == null)
            {
                return NotFound();
            }

            var cours = await _context.Cours
                .Where(c => c.CategorieId == id)
                .ToListAsync();

            var viewModel = new CategorieDetailsViewModel
            {
                Categorie = categorie,
                Cours = cours
            };

            return View(viewModel);
        }

    }
}