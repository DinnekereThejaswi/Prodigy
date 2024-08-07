using System;
using System.ComponentModel.DataAnnotations;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class UserLoginViewModel
    {
        [Key]
        public int ID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [DataType(DataType.Text), MaxLength(75), MinLength(3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Text), MaxLength(15), MinLength(8)]
        public string Password { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please Specify Employee ID")]
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        [DataType(DataType.Text), MaxLength(75), MinLength(3)]
        public string FullName { get; set; }

        [DataType(DataType.Date), Range(typeof(DateTime), "01-JAN-1950", "31-JAN-3000",
        ErrorMessage = "Value for {0} must be between {1} and {2}")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [Range(1, 2, ErrorMessage = "Gender is required")]
        public int Gender { get; set; }

        [Required(ErrorMessage = "Address is required")]
        [DataType(DataType.Text), MaxLength(2000), MinLength(3)]
        public string Address { get; set; }

        public string Landmark { get; set; }

        [Required(ErrorMessage = "City is required")]
        [DataType(DataType.Text), MaxLength(75), MinLength(3)]
        public string City { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Gender is required")]
        public int DistrictID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "State is required")]
        public int StateID { get; set; }

        [Required(ErrorMessage = "Pincode is required")]
        [DataType(DataType.Text), MaxLength(6), MinLength(6)]
        public string Pincode { get; set; }

        [Required(ErrorMessage = "Mobile Number is required")]
        [DataType(DataType.Text), MaxLength(15), MinLength(10)]
        public string MobileNo { get; set; }

        public string AltMobileNo { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.Text), MaxLength(75), MinLength(3)]
        public string Email { get; set; }

        public string ImagePath { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please input a valid value for Role ID")]
        public int RoleID { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Please input a valid value for Theme ID")]
        public int ThemeID { get; set; }

        public bool IsActive { get; set; }
        public bool IsBlocked { get; set; }
        public bool IsClosed { get; set; }
        public string PasswordSalt { get; set; }
        public DateTime PasswordTimestamp { get; set; }
        public int InsertedBy { get; set; }
        public DateTime InsertedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
    }
}
