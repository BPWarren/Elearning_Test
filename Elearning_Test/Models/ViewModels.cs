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
}