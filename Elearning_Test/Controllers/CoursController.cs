using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;

namespace Elearning_Test.Controllers
{
    public class CoursController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CoursController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Cours
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Cours.Include(c => c.Categorie).Include(c => c.Professeur);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Cours/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cours = await _context.Cours
                .Include(c => c.Categorie)
                .Include(c => c.Professeur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cours == null)
            {
                return NotFound();
            }

            return View(cours);
        }

        // GET: Cours/Create
        public IActionResult Create()
        {
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Intitule");
            ViewData["ProfesseurId"] = new SelectList(_context.Professeurs, "Id", "Nom");
            return View();
        }
        // POST: Cours/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Description,ProfesseurId,Price,CategorieId")] Cours cours, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                // Gérer le téléchargement de l'image
                if (imageFile != null && imageFile.Length > 0)
                {
                    // Chemin du dossier imagesImportées
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");

                    // Créer le dossier s'il n'existe pas
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Générer un nom de fichier unique pour éviter les conflits
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageFile.FileName);
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    try
                    {
                        // Enregistrer le fichier
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await imageFile.CopyToAsync(stream);
                        }

                        // Enregistrer le chemin de l'image dans la base de données
                        cours.ImageFile = "/images/" + uniqueFileName;
                    }
                    catch (Exception ex)
                    {
                        // Gérer les exceptions liées au fichier
                        ModelState.AddModelError("", $"Erreur lors de l'importation de l'image : {ex.Message}");
                        ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Intitule", cours.CategorieId);
                        ViewData["ProfesseurId"] = new SelectList(_context.Professeurs, "Id", "Nom", cours.ProfesseurId);
                        return View(cours);
                    }
                }

                // Initialiser les propriétés CreatedAt et UpdatedAt
                cours.CreatedAt = DateTime.UtcNow;
                cours.UpdatedAt = DateTime.UtcNow;

                // Ajouter et enregistrer dans la base de données
                _context.Add(cours);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // En cas de modèle invalide, retourner les listes déroulantes et la vue
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Intitule", cours.CategorieId);
            ViewData["ProfesseurId"] = new SelectList(_context.Professeurs, "Id", "Nom", cours.ProfesseurId);
            return View(cours);
        }



        // GET: Cours/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cours = await _context.Cours.FindAsync(id);
            if (cours == null)
            {
                return NotFound();
            }
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Id", cours.CategorieId);
            ViewData["ProfesseurId"] = new SelectList(_context.Professeurs, "Id", "Id", cours.ProfesseurId);
            return View(cours);
        }

        // POST: Cours/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Description,ProfesseurId,Price,CategorieId,ImageFile,CreatedAt,UpdatedAt")] Cours cours)
        {
            if (id != cours.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cours);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoursExists(cours.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategorieId"] = new SelectList(_context.Categories, "Id", "Id", cours.CategorieId);
            ViewData["ProfesseurId"] = new SelectList(_context.Professeurs, "Id", "Id", cours.ProfesseurId);
            return View(cours);
        }

        // GET: Cours/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cours = await _context.Cours
                .Include(c => c.Categorie)
                .Include(c => c.Professeur)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cours == null)
            {
                return NotFound();
            }

            return View(cours);
        }

        // POST: Cours/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cours = await _context.Cours.FindAsync(id);
            if (cours != null)
            {
                _context.Cours.Remove(cours);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoursExists(int id)
        {
            return _context.Cours.Any(e => e.Id == id);
        }
    }
}
