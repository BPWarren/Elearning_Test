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
    public class CategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategorieCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string imagePath = string.Empty;

                if (model.ImageFile != null)
                {
                    // Définir le chemin du dossier où enregistrer l'image
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder);

                    // Générer un nom unique pour le fichier
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(model.ImageFile.FileName);
                    imagePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copier le fichier sur le serveur
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // Relatif à la racine du site
                    imagePath = "/images/" + uniqueFileName;
                }

                // Créer une instance du modèle principal
                var categorie = new Categorie
                {
                    Intitule = model.Intitule,
                    Description = model.Description,
                    ImageFile = imagePath,
                };

                // Ajouter à la base de données
                _context.Add(categorie);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Categories/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories.FindAsync(id);
            CategorieEditViewModel cevm = new()
            {
                Id = categorie!.Id,
                Intitule = categorie!.Intitule,
                Description = categorie!.Description,
                ImageTexte = categorie!.ImageFile
            };
            if (categorie == null)
            {
                return NotFound();
            }
            return View(cevm);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategorieEditViewModel cevm)
        {
            var categorie = await _context.Categories.FindAsync(cevm.Id);
            try
            {
                string imagePath = "none";
                if (cevm.ImageFile != null)
                {
                    // Définir le chemin du dossier où enregistrer l'image
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                    Directory.CreateDirectory(uploadsFolder);

                    // Générer un nom unique pour le fichier
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(cevm.ImageFile.FileName);
                    imagePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Copier le fichier sur le serveur
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await cevm.ImageFile.CopyToAsync(stream);
                    }

                    // Relatif à la racine du site
                    imagePath = "/images/" + uniqueFileName;
                }

                // Mettre à jour le champ UpdatedAt
                if (imagePath != "none")
                {
                    categorie!.ImageFile = imagePath;
                }
                else
                {
                    categorie!.ImageFile = cevm.ImageTexte!;
                }
                categorie.Intitule = cevm.Intitule;
                categorie.Description = cevm.Intitule;
                categorie!.UpdatedAt = DateTime.UtcNow;

                // Mettre à jour la catégorie dans la base de données
                _context.Update(categorie);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {
                return View(cevm);
            }
        }




        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (categorie == null)
            {
                return NotFound();
            }

            return View(categorie);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var categorie = await _context.Categories.FindAsync(id);
            if (categorie != null)
            {
                _context.Categories.Remove(categorie);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CategorieExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
