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
        [Required]
        public decimal Price { get; set; }
        public int CategorieId { get; set; } // Pour lier l'ID de la catégorie sélectionnée
        public List<Categorie>? Categories { get; set; } // Liste des catégories disponibles
    }

    public class AddLeconViewModel
    {
        public int CoursId { get; set; }
        public required string Titre { get; set; }
        public required string Contenu { get; set; }
        public int NumeroPage { get; set; }
    }
}