

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
        public async Task<IActionResult> VoirLecons(int id)
        {
            var leconsA = await _leconService.GetLeconsByCoursIdAsync(id);
            VoirLeconsViewModel vlvm = new()
            {
                Lecons = leconsA,
                CoursId = id
            };
            return View(vlvm);
        }
        //Ajout d'une nouvelle lecon à un cours
        [HttpGet]
        public IActionResult AjouterLecon(int id)
        {
            LeconViewModel lvm = new ()
            {
                CoursId = id,
            };
            return View(lvm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterLecon(LeconViewModel lvm)
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
                return RedirectToAction("VoirLecons", new { id = lvm.CoursId });
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Une erreur s'est produite lors de la création de la leçon.");
            }
            return View(lvm);
        }

        //Modification de lecon
        [HttpGet]
        public async Task<IActionResult> ModifierLecon(int id)
        {
            var lecon = await _leconService.GetLeconByIdAsync(id);
            try
            {
                // Récupérer la leçon existante par son ID
                if (lecon == null)
                {
                    return NotFound(); // Retourne une erreur 404 si la leçon n'existe pas
                }

                // Créer un ViewModel pour la modification
                var lvm = new LeconViewModel
                {
                    Id = lecon.Id,
                    CoursId = lecon.CoursId,
                    Titre = lecon.Titre,
                    Contenu = lecon.Contenu,
                    NumeroPage = lecon.NumeroPage,
                };

                return View(lvm);
            }
            catch (Exception)
            {
                // Gérer les erreurs et rediriger vers une page d'erreur
                ModelState.AddModelError("", "Une erreur s'est produite lors de la récupération de la leçon.");
                return RedirectToAction("VoirLecons", new { id = lecon.CoursId });
            }
        }
        //Sppression de lecon
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupprimerLecon(int id)
        {
            var lecon = await _leconService.GetLeconByIdAsync(id);
            try
            {
                // Récupérer la leçon à supprimer
                if (lecon == null)
                {
                    return NotFound(); // Retourne une erreur 404 si la leçon n'existe pas
                }

                // Supprimer la leçon
                await _leconService.DeleteLeconAsync(id);

                // Rediriger vers la liste des leçons du cours
                return RedirectToAction("VoirLecons", new { id = lecon.CoursId });
            }
            catch (Exception)
            {
                // Gérer les erreurs et afficher un message d'erreur
                ModelState.AddModelError("", "Une erreur s'est produite lors de la suppression de la leçon.");
                return RedirectToAction("VoirLecons", new { id = lecon.CoursId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ModifierLecon(LeconViewModel lvm)
        {
            try
            {
                // Récupérer la leçon existante par son ID
                var lecon = await _leconService.GetLeconByIdAsync(lvm.Id);
                if (lecon == null)
                {
                    return NotFound(); // Retourne une erreur 404 si la leçon n'existe pas
                }

                // Mettre à jour les propriétés de la leçon
                lecon.Titre = lvm.Titre;
                lecon.Contenu = lvm.Contenu;
                lecon.NumeroPage = lvm.NumeroPage;

                // Enregistrer les modifications dans la base de données
                await _leconService.UpdateLeconAsync(lecon);

                // Rediriger vers la liste des leçons du cours
                return RedirectToAction("VoirLecons", new { id = lecon.CoursId });
            }
            catch (Exception)
            {
                // Gérer les erreurs et afficher un message d'erreur
                ModelState.AddModelError("", "Une erreur s'est produite lors de la mise à jour de la leçon.");
            }

            // Si le modèle n'est pas valide ou s'il y a une erreur, réafficher le formulaire de modification
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
