

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Elearning_Test.Models;
using Elearning_Test.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Elearning_Test.Data;

// ... autres using
namespace Elearning_Test.Controllers
{
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
            var categories = _context.Categories.ToList(); // Charge les catégories depuis la base de données
            var viewModel = new CoursViewModel
            {
                Categories = categories,
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AjouterCours(CoursViewModel cvm)
        {
            if (ModelState.IsValid)
            {
                IFormFile imageImport = cvm.ImageFile;
                string photoPath = "AucuneImage?";

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
                Cours newCours = new Cours
                {
                    Titre = cvm.Titre,
                    Description = cvm.Description,
                    Price = cvm.Price,
                    ImageFile = photoPath,
                    ProfesseurId = ProfesseurId,
                    CategorieId = 3
                };
                await _coursService.CreateCoursAsync(newCours);
                return RedirectToAction("Index");
            }
            cvm.Categories = _context.Categories.ToList();
            return View();
        }

        // Modifier un cours
        [HttpGet]
        public async Task<IActionResult> EditCours(int id)
        {
            var cours = await _coursService.GetCoursByIdAsync(id);
            if (cours == null || cours.ProfesseurId != User.Identity!.Name)
                return NotFound();

            return View(cours);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCours(Cours cours, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
                    var uploadPath = Path.Combine(_environment.WebRootPath, "images/cours");
                    Directory.CreateDirectory(uploadPath);

                    var filePath = Path.Combine(uploadPath, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(stream);
                    }

                    cours.ImageFile = $"/images/cours/{fileName}";
                }

                await _coursService.UpdateCoursAsync(cours);
                return RedirectToAction(nameof(Index));
            }
            return View(cours);
        }
    }
}
