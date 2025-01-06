namespace Elearning_Test.Models
{
    public class DashboardViewModel
    {
        public int TotalEtudiants
        {
            get; set;
        }
        public int TotalProfesseurs
        {
            get; set;
        }
        public int TotalCours
        {
            get; set;
        }
        public int TotalCertificats
        {
            get; set;
        }
        public int CoursActifs
        {
            get; set;
        }
        public int CoursTermines
        {
            get; set;
        }
        public int EtudiantsActifs
        {
            get; set;
        }
        public int ProfesseursActifs
        {
            get; set;
        }

        public int NombreVisiteur
        {
            get; set;
        }

        // Pour les graphiques
        public required Dictionary<string, int> InscriptionsParMois
        {
            get; set;
        } // Mois => Nombre d'inscriptions
        public required Dictionary<string, int> CoursPopulaires
        {
            get; set;
        } // Nom du cours => Nombre d'étudiants
    }
}