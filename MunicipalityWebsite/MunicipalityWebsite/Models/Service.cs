using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalityWebsite.Models
{
    public class Service
    {
        [Key]
        public int RequestID { get; set; }
        [Required]
        [ForeignKey("Citizen")]
        public int CitizenID { get; set; }
        [Required]
        [StringLength(100)]
        public string ServiceType { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
    }
}
