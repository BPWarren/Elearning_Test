using System.ComponentModel.DataAnnotations;

namespace Elearning_Test.Models
{
    public class RegisterAdminViewModel
    {
        [Required(ErrorMessage = "Le champ Nom est obligatoire.")]
        public required string Nom
        {
            get; set;
        }

        [Required(ErrorMessage = "Le champ Email est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse email n'est pas valide.")]
        public required string Email
        {
            get; set;
        }

        [Required(ErrorMessage = "Le champ Mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        public required string MotDePasse
        {
            get; set;
        }

        [Required(ErrorMessage = "La confirmation du mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public required string ConfirmationMotDePasse
        {
            get; set;
        }
    }

}
