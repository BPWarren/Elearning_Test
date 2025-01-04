using System.ComponentModel.DataAnnotations;

namespace Elearning_Test.Models
{
    public class ProfesseurRegistrationViewModel
    {
        [Required(ErrorMessage = "Le nom d'utilisateur est obligatoire.")]
        public required string UserName
        {
            get; set;
        }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        public required string Nom
        {
            get; set;
        }

        [Required(ErrorMessage = "Le prénom est obligatoire.")]
        public required string Prenom
        {
            get; set;
        }

        [Required(ErrorMessage = "La spécialité est obligatoire.")]
        public required string Specialite
        {
            get; set;
        }

        [Required(ErrorMessage = "L'adresse e-mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "Veuillez fournir une adresse e-mail valide.")]
        public required string Email
        {
            get; set;
        }

        [Required(ErrorMessage = "Le mot de passe est obligatoire.")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public required string MotDePasse
        {
            get; set;
        }

        [Required(ErrorMessage = "Veuillez confirmer votre mot de passe.")]
        [Compare("MotDePasse", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        [DataType(DataType.Password)]
        public required string ConfirmationMotDePasse
        {
            get; set;
        }
    }
}
