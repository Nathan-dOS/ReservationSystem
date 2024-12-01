using Reservation.Data.Enum;
using System.ComponentModel.DataAnnotations;

namespace Reservation.ViewModel
{
    public class RegisterViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email address is required")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Confirm Password address is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password must match")]
        public string ConfirmPassword { get; set; }


        [Required(ErrorMessage ="Name required")]
        [StringLength(60)]
        public string Name { get; set; }

        [Required(ErrorMessage ="PhoneNumber is required")]
        [StringLength(15)]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "CPF is required")]
        [StringLength(11)]
        public string CPF { get; set; }

        [Required(ErrorMessage ="Address is required")]
        [StringLength(100)]
        public string Address { get; set; }

        [StringLength(10)]
        public string? CRMNumber { get; set; }
        [StringLength(10)]
        public string? OABNumber { get; set; }

        [Required]
        public EnumUserType UserType { get; set; }

    }
}
