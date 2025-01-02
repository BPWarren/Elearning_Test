using System.ComponentModel.DataAnnotations;

namespace Elearning_Test.Models
{
    public class CategorieCreateViewModel
    {
        public string Intitule { get; set; } = string.Empty;
        public required string Description { get; set; }

        [Required(ErrorMessage = "Veuillez sélectionner une image.")]
        public required IFormFile ImageFile { get; set; }

    }

}
