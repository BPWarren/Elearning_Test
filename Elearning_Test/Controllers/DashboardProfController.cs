

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

// ... autres using
namespace Elearning_Test.Controllers
{
    [Authorize(Roles = "Professeur")]
    public class DashboardProfController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICoursService _coursService;
        private readonly ILeconService _leconService;
        private readonly IWebHostEnvironment _environment;
        private readonly UserManager<IdentityUser> _userManager;

        public DashboardProfController(
            ICoursService coursService,
            ILeconService leconService,
            IWebHostEnvironment environment,
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _coursService = coursService;
            _leconService = leconService;
            _environment = environment;
            _userManager = userManager;
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);  // Récupère l'utilisateur actuellement connecté

            var userId = user!.Id;
            string professeurId = userId;
            var cours = await _coursService.GetCoursByProfesseurAsync(professeurId!);
            return View(cours);
        }

        // Ajouter un cours

        [HttpGet]
        public IActionResult AjouterCours()
        {
            var categories = _context.Categories.ToList();
            var viewModel = new CoursViewModel
            {
                Categories = categories,
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> AjouterCours(CoursViewModel cvm)
        {
            string photoPath = "example.jpg";
            var Photo = cvm.ImageFile;
            if (Photo != null && Photo.Length > 0)
            {
                string uploadDir = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{Photo.FileName}";
                string filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await Photo.CopyToAsync(fileStream);
                }

                photoPath = $"/images/{uniqueFileName}";

                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    return Unauthorized();
                }

                Cours newCours = new()
                {
                    Titre = cvm.Titre,
                    Description = cvm.Description,
                    Price = cvm.Price,
                    ImageFile = photoPath,
                    ProfesseurId = user.Id,
                    CategorieId = cvm.CategorieId
                };

                await _coursService.CreateCoursAsync(newCours);
                return RedirectToAction("Index", "DashboardProf");
            }

                

            cvm.Categories = _context.Categories.ToList();
            return View(cvm);
        }


        // Modifier un cours
        [HttpGet]
        public async Task<IActionResult> EditCours(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            var categories = _context.Categories.ToList();
            var user = await _userManager.GetUserAsync(User);  // Récupère l'utilisateur actuellement connecté
            var userId = user!.Id;
            if (cours == null || cours.ProfesseurId != userId)
                return NotFound();
            CoursViewModel cvm = new()
            {
                Id = cours.Id,
                Titre = cours.Titre,
                Description = cours.Description,
                Price = cours.Price,
                CategorieId = cours.CategorieId,
                Categories = categories,
                ImageStr = cours.ImageFile
            };
            return View(cvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCours(CoursViewModel cvm)
        {
            IFormFile imageImport = cvm.ImageFile;
            string photoPath = cvm.ImageStr;
            if (imageImport != null && imageImport.Length > 0)
            {
                string uploadDir = Path.Combine(_environment.WebRootPath, "images");
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                string uniqueFileName = $"{Guid.NewGuid()}_{imageImport.FileName}";
                string filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await imageImport.CopyToAsync(fileStream);
                }
                photoPath = $"/images/{uniqueFileName}";

            }
            var user = await _userManager.GetUserAsync(User);  // Récupère l'utilisateur actuellement connecté
            var userId = user!.Id;
            string ProfesseurId = userId;
            var newCours = await _coursService.GetCoursByIdAsync(cvm.Id);
            if (newCours != null)
            {
                newCours.Titre = cvm.Titre;
                newCours.Description = cvm.Description;
                newCours.Price = cvm.Price;
                newCours.ImageFile = photoPath;
                await _coursService.UpdateCoursAsync(newCours);
                return RedirectToAction("Index");
            }
            cvm.Categories = _context.Categories.ToList();
            return View(cvm);
        }


        // Supprimer un cours
        // GET: Confirm deletion of a course
        [HttpGet]
        public async Task<IActionResult> SupprimerCours(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            if (cours == null)
            {
                return NotFound();
            }

            return View(cours);
        }

        // POST: Delete a course
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerCours(Cours cours)
        {
            int id = cours.Id;
            try
            {

                await _coursService.DeleteCoursAsync(id);
                return RedirectToAction("Index"); // Replace with the appropriate action
            }

            catch (Exception)
            {
                // Log the error and notify the user
                ModelState.AddModelError("", "An error occurred while deleting the course.");
                return View(await _coursService.GetCoursByIdAsync(id));
            }
        }

        //Voir les leçons d'un cours
        [HttpGet]
        public async Task<IActionResult> VoirLecons(int IdCours)
        {
            var leconsA = await _leconService.GetLeconsByCoursIdAsync(IdCours);
            VoirLeconsViewModel vlvm = new()
            {
                Lecons = leconsA,
                CoursId = IdCours
            };
            return View(vlvm);
        }
        //Ajout d'une nouvelle lecon à un cours
        [HttpGet]
        public IActionResult AjouterLecon(int IdCours)
        {
            LeconViewModel lvm = new ()
            {
                CoursId = IdCours,
            };
            return View(lvm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterLecon(LeconViewModel lvm)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    Lecon newLecon = new ()
                    {
                        CoursId = lvm.CoursId,
                        Titre = lvm.Titre,
                        Contenu = lvm.Contenu,
                        NumeroPage = lvm.NumeroPage,
                    };
                    await _leconService.CreateLeconAsync(newLecon);
                    return RedirectToAction("VoirLecons", new { IdCours = lvm.CoursId });
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "Une erreur s'est produite lors de la création de la leçon.");
                }
            }
            return View(lvm);
        }


        [HttpPost]
        public async Task<JsonResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { error = "Le fichier est invalide ou vide." });
            }

            try
            {
                // Définir le dossier où enregistrer les fichiers
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "UploadedImages");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Générer un nom de fichier unique
                var fileName = Path.GetFileName(file.FileName);
                var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(fileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Sauvegarder le fichier
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Retourner l'URL où le fichier est accessible
                var fileUrl = Url.Content($"~/UploadedImages/{uniqueFileName}");
                return Json(new { location = fileUrl });
            }
            catch (Exception ex)
            {
                // Gérer les erreurs et retourner une réponse JSON
                return Json(new { error = "Une erreur s'est produite lors de l'upload de l'image." });
            }
        }



    }
}
