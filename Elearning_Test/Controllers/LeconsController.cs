using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Elearning_Test.Models;

namespace Elearning_Test.Controllers
{
    public class LeconsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeconsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Lecons
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Lecons.Include(l => l.Cours);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Lecons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecon = await _context.Lecons
                .Include(l => l.Cours)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecon == null)
            {
                return NotFound();
            }

            return View(lecon);
        }

        // GET: Lecons/Create
        public IActionResult Create()
        {
            // Afficher une liste des cours avec leur titre au lieu de leur ID
            ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre");
            return View();
        }

        // POST: Lecons/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Titre,Contenu,NumeroPage,CoursId")] Lecon lecon)
        {
            if (ModelState.IsValid)
            {
                // Récupérer l'objet Cours correspondant à CoursId
                var cours = await _context.Cours.FindAsync(lecon.CoursId);
                if (cours == null)
                {
                    ModelState.AddModelError("CoursId", "Le cours sélectionné est invalide.");
                    ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre", lecon.CoursId);
                    return View(lecon);
                }

                // Initialiser les propriétés CreatedAt, UpdatedAt et Cours
                lecon.CreatedAt = DateTime.UtcNow;
                lecon.UpdatedAt = DateTime.UtcNow;
                lecon.Cours = cours;

                _context.Add(lecon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            // Recharger la liste des cours en cas d'erreur
            ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre", lecon.CoursId);
            return View(lecon);
        }

        // GET: Lecons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecon = await _context.Lecons
                .Include(l => l.Cours)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecon == null)
            {
                return NotFound();
            }
            // Afficher une liste des cours avec leur titre au lieu de leur ID
            ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre", lecon.CoursId);
            return View(lecon);
        }

        // POST: Lecons/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Titre,Contenu,NumeroPage,CoursId,CreatedAt,UpdatedAt")] Lecon lecon)
        {
            if (id != lecon.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Récupérer l'objet Cours correspondant à CoursId
                    var cours = await _context.Cours.FindAsync(lecon.CoursId);
                    if (cours == null)
                    {
                        ModelState.AddModelError("CoursId", "Le cours sélectionné est invalide.");
                        ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre", lecon.CoursId);
                        return View(lecon);
                    }

                    // Mettre à jour la propriété UpdatedAt et Cours
                    lecon.UpdatedAt = DateTime.UtcNow;
                    lecon.Cours = cours;

                    _context.Update(lecon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeconExists(lecon.Id))
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
            // Recharger la liste des cours en cas d'erreur
            ViewData["CoursId"] = new SelectList(_context.Cours, "Id", "Titre", lecon.CoursId);
            return View(lecon);
        }

        // GET: Lecons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lecon = await _context.Lecons
                .Include(l => l.Cours)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (lecon == null)
            {
                return NotFound();
            }

            return View(lecon);
        }

        // POST: Lecons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lecon = await _context.Lecons.FindAsync(id);
            if (lecon != null)
            {
                _context.Lecons.Remove(lecon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeconExists(int id)
        {
            return _context.Lecons.Any(e => e.Id == id);
        }
    }
}