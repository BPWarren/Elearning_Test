namespace Elearning_Test.Models
{
    public class CertifViewModel
    {
        public required List<Certification> Certificationsvalide { get; set; }
        public required List<Certification> CertificationsEnAttente
        {
            get; set;
        }
    }
}
