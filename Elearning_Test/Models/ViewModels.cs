using System.ComponentModel.DataAnnotations;

namespace Elearning_Test.Models
{
    public class CoursViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Titre { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public IFormFile ImageFile { get; set; }
        public string ImageStr { get; set; }
        [Required]
        public decimal Price { get; set; }
        public DateTime LastUpdate { get; set; }
        public int CategorieId { get; set; } // Pour lier l'ID de la catégorie sélectionnée
        public List<Categorie>? Categories { get; set; } // Liste des catégories disponibles
    }

    public class LeconViewModel
    {
        public int Id { get; set; }
        public int CoursId { get; set; }
        [Required]
        public string Titre { get; set; }
        [Required]
        public string Contenu { get; set; }
        [Required]
        public int NumeroPage { get; set; }
    }

    public class VoirLeconsViewModel
    {
        public int CoursId { get; set; }
        public List<Lecon> Lecons { get; set; }
    }

    public class HomePageViewModel
    {
        public IEnumerable<Categorie> categories { get; set; }
        public IEnumerable<Cours> Cours { get; set; }
    }

    public class InscriptionPageViewModel
    {
        public string OwnerName {  get; set; }
        public int Cvc { get; set; }
        public string NumeroCarte {  get; set; }
        public Etudiant Etudiant { get; set; }
        public Cours Cours { get; set; }
    }

    public class CategorieEditViewModel
    {
        public int Id{ get; set; }
        [Required]
        public string Intitule { get; set; } = string.Empty;
        [Required]
        public string? Description { get; set; }
        public IFormFile? ImageFile { get; set; }
        public string? ImageTexte {  get; set; }

    }


    public class EtudiantsParCoursViewModel
    {
        public int CoursId { get; set; }
        public string? CoursTitre { get; set; }
        public List<EtudiantProgressionViewModel>? Etudiants { get; set; }
    }
    public class EtudiantProgressionViewModel
    {
        public string? EtudiantId { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public int Progression { get; set; }
        public bool IsCompleted { get; set; }
    }
    public class StatistiquesCoursViewModel
    {
        public int CoursId { get; set; }
        public string? CoursTitre { get; set; }
        public int NombreEtudiants { get; set; }
        public int ProgressionMoyenne { get; set; }
        public int NombreTermines { get; set; }
        public int NombreEnCours { get; set; }
    }

    public class CoursViewModel2
    {
        public int Id { get; set; }
        public string Titre { get; set; }
        public string Description { get; set; }
        public int Progression { get; set; }
    }
    public class StatistiquesEtudiantViewModel
    {
        public int NombreCoursInscrits { get; set; }
        public int NombreCoursTermines { get; set; }
        public int ProgressionMoyenne { get; set; }
    }
}