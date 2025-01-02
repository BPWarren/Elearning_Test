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
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var categorie = await _context.Categories.FindAsync(id);
            if (categorie == null)
            {
                return NotFound();
            }
            return View(categorie);
        }

        // POST: Categories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Intitule,Description,ImageFile,CreatedAt,UpdatedAt")] Categorie categorie, IFormFile newImage)
        {
            if (id != categorie.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Gestion de l'image si une nouvelle image est téléchargée
                    if (newImage != null && newImage.Length > 0)
                    {
                        // Vérifier si le fichier est une image
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                        var fileExtension = Path.GetExtension(newImage.FileName).ToLower();

                        if (!Array.Exists(allowedExtensions, ext => ext == fileExtension))
                        {
                            ModelState.AddModelError("newImage", "Le fichier doit être une image (jpg, jpeg, png, gif).");
                            return View(categorie);
                        }

                        // Générer un nom de fichier unique
                        var uniqueFileName = Guid.NewGuid().ToString() + fileExtension;
                        var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");

                        // Créer le répertoire si nécessaire
                        Directory.CreateDirectory(uploadPath);

                        var filePath = Path.Combine(uploadPath, uniqueFileName);

                        // Copier le contenu du fichier dans le chemin spécifié
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await newImage.CopyToAsync(stream);
                        }

                        // Supprimer l'ancienne image si elle existe
                        if (!string.IsNullOrEmpty(categorie.ImageFile))
                        {
                            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", categorie.ImageFile.TrimStart('/'));
                            if (System.IO.File.Exists(oldFilePath))
                            {
                                System.IO.File.Delete(oldFilePath);
                            }
                        }

                        // Mettre à jour le chemin du fichier dans le modèle
                        categorie.ImageFile = "/images/" + uniqueFileName;
                    }

                    // Mettre à jour le champ UpdatedAt
                    categorie.UpdatedAt = DateTime.UtcNow;

                    // Mettre à jour la catégorie dans la base de données
                    _context.Update(categorie);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategorieExists(categorie.Id))
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

            return View(categorie);
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
