
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

        // Action pour la recherche avec autocomplétion
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Json(new { categories = new List<Categorie>(), cours = new List<Cours>() });
            }

            // Rechercher les catégories correspondantes
            var categories = await _context.Categories
                .Where(c => c.Intitule.Contains(query))
                .Select(c => new { c.Id, c.Intitule })
                .ToListAsync();

            // Rechercher les cours correspondants
            var cours = await _context.Cours
                .Where(c => c.Titre.Contains(query) || c.Description.Contains(query))
                .Select(c => new { c.Id, c.Titre, c.Description })
                .ToListAsync();

            // Retourner les résultats au format JSON
            return Json(new { categories, cours });
        }

        public async Task<IActionResult> CategorieDetails(int id)
        {
            // Récupérer la catégorie
            var categorie = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (categorie == null)
            {
                return NotFound();
            }

            // Récupérer les cours associés à cette catégorie
            var cours = await _context.Cours
                .Where(c => c.CategorieId == id)
                .ToListAsync();

            // Passer les données à la vue
            var viewModel = new CategorieDetailsViewModel
            {
                Categorie = categorie,
                Cours = cours
            };

            return View(viewModel);
        }

        public async Task<IActionResult> CoursDetails(int id)
        {
            // Récupérer le cours
            var cours = await _context.Cours
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cours == null)
            {
                return NotFound();
            }

            // Récupérer les leçons associées à ce cours
            var lecons = await _context.Lecons
                .Where(l => l.CoursId == id)
                .ToListAsync();

            // Passer les données à la vue
            var viewModel = new CoursDetailsViewModel
            {
                Cours = cours,
                Lecons = lecons
            };

            return View(viewModel);
        }
    }
}