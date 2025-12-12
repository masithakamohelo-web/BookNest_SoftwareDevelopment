using System.ComponentModel.DataAnnotations;

namespace ASPNETCore_DB.Models
{
    public class Consumer
    {
        [Key]
        [Display(Name = "Consumer ID")]
        [Required(ErrorMessage = "Consumer ID is required.")]
        [StringLength(10, MinimumLength = 3, ErrorMessage = "Consumer ID must be 3-10 characters")]
        public string? ConsumerId { get; set; }

        [Display(Name = "Full Name")]
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be 2-100 characters")]
        public string? Name { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string? Email { get; set; }

        [Display(Name = "Address")]
        [Required(ErrorMessage = "Address is required")]
        [StringLength(200, ErrorMessage = "Address cannot exceed 200 characters")]
        public string? Address { get; set; }

        [Display(Name = "Phone")]
        [Required(ErrorMessage = "Phone is required")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [Display(Name = "Registration Date")]
        [Required(ErrorMessage = "Registration Date is required")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Profile Photo")]
        public string? Photo { get; set; }
    }
}