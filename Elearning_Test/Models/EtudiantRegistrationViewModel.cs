using System.ComponentModel.DataAnnotations;

namespace Elearning_Test.Models
{
    public class EtudiantRegistrationViewModel
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Nom { get; set; }
        [Required]
        public required string Prenom { get; set; }
        [Required]
        public required string Cne { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required, DataType(DataType.Password)]
        public required string MotDePasse { get; set; }

        [Required, Compare("MotDePasse")]
        public required string ConfirmationMotDePasse { get; set; }
    }

}
