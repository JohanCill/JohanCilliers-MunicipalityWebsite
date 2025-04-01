using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MunicipalityWebsite.Models
{
    public class Staff
    {
        [Key]
        public int StaffID { get; set; }
        
        
		[Required]
		[StringLength(100)]
		public string FullName { get; set; }
        [Required]
        [StringLength(100)]
        public string Position { get; set; }
        [Required]
        [StringLength(100)]
        public string Department { get; set; }
        [Required]
        [StringLength(100)]
        public string Email { get; set; }
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
        [Required]
        public DateTime HireDate { get; set; }
    }
}
