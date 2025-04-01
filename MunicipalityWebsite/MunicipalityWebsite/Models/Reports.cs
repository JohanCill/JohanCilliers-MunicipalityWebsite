using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalityWebsite.Models
{
    public class Reports
    {
        [Key]
        public int ReportID { get; set; }
        [Required]
        [ForeignKey("Citizen")]
        public int CitizenID { get; set; }
        [Required]
        [StringLength(100)]
        public string ReportType { get; set; }
        [Required]
        [StringLength(1000)]
        public string Details { get; set; }

        public DateTime SubmissionDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50)]
        public string Status { get; set; }
    }
}
