namespace Elearning_Test.Models
{
    public class DashboardViewModel
    {
        // Statistiques principales
        public int TotalEtudiants
        {
            get; set;
        } // Nombre total d'étudiants
        public int TotalProfesseurs
        {
            get; set;
        } // Nombre total de professeurs
        public int TotalCertificatsValides
        {
            get; set;
        } // Nombre de certificats validés
        public int TotalCertificatsEnAttente
        {
            get; set;
        } // Nombre de certificats en attente
        public int TotalCours
        {
            get; set;
        } // Nombre total de cours
        public int TotalInscriptions
        {
            get; set;
        } // Nombre total d'inscriptions

        // Statistiques d'activité
        public int ParticipantsActifs
        {
            get; set;
        } // Nombre d'étudiants actifs (connectés récemment)
        public int FormateursActifs
        {
            get; set;
        } 
        //}
        // Graphiques
        public required Dictionary<string, int> InscriptionsParMois
        {
            get; set;
        }

        public required List<Certification> CertifValide

        {
            get; set;
        }

        public required List<Certification> CertifEnAttente

        {
            get; set;
        }

        public required List<Etudiant> NouveauxEtudiants
        {
            get; set;
        } // Liste des 5 nouveaux étudiants inscrits
        public required List<Cours> NouveauxCours
        {
            get; set;
        } // Liste des 5 nouveaux cours ajoutés

        // Autres éléments essentiels
        public int TotalVisiteurs
        {
            get; set;
        } // Nombre total de visiteurs
        public int TotalPaiements
        {
            get; set;
        } // Nombre total de paiements effectués
        public decimal RevenuTotal
        {
            get; set;
        } // Revenu total généré par les cours

        public required Dictionary<string, int> CoursPopulaires
        {
            get; set;
        } // Nom du cours => Nombre d'étudiants inscrits

        // Utilisateurs en ligne
        public required List<UserInfo> EtudiantsEnLigne
        {
            get; set;
        } // Liste des étudiants actuellement en ligne
        public required List<UserInfo> ProfesseursEnLigne
        {
            get; set;
        }
    }

    public class UserInfo
    {
        public required string Nom
        {
            get; set;
        }

        public required string Prenom
        {
            get; set;
        }
        public required string Email
        {
            get; set;
        }
    }
}