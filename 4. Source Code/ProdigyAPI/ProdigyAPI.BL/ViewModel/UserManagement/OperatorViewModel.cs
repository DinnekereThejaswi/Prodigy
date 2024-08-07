using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ProdigyAPI.BL.ViewModel.Master;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ProdigyAPI.BL.ViewModel.UserManagement
{
    public class OperatorViewModel
    {
        [Required(ErrorMessage = "OperatorCode is required")]
        [DataType(DataType.Text), MaxLength(5), MinLength(2)]
        public string OperatorCode { get; set; }

        [Required(ErrorMessage = "OperatorName is required")]
        [DataType(DataType.Text), MaxLength(30), MinLength(2)]
        public string OperatorName { get; set; }

        [Required(ErrorMessage = "OperatorType is required")]
        public string OperatorType { get; set; }

        [Required(ErrorMessage = "MobileNo is required")]
        [DataType(DataType.Text), MaxLength(10), MinLength(10)]
        public string MobileNo { get; set; }

        public string BranchCode { get; set; }
        public string CompanyCode { get; set; }
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Text), MaxLength(20), MinLength(3)]
        public string Password { get; set; }
        public int RoleID { get; set; }

        //[JsonConverter(typeof(StringEnumConverter))]
        public string Status { get; set; }

        [Required(ErrorMessage = "MaxDiscountPercentAllowed is required and should be between 0.00 and 99.00")]
        [Range(0.0, 99)]
        public decimal MaxDiscountPercentAllowed { get; set; }

        [Required(ErrorMessage = "CounterCode is required")]
        [DataType(DataType.Text), MaxLength(5), MinLength(1)]
        public string CounterCode { get; set; }
        public string DefaultStore { get; set; }
        public List<ListOfValue> MappedStores { get; set; }
    }

    public class OperatorPasswordViewModel
    {
        public string BranchCode { get; set; }
        public string CompanyCode { get; set; }
        [Required(ErrorMessage = "Old Password is required")]
        [DataType(DataType.Password), MaxLength(20), MinLength(3)]
        public string OldPassword { get; set; }
        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password), MaxLength(20), MinLength(3)]
        public string NewPassword { get; set; }

    }

    public enum OperatorType
    {
        Admin,
        Others
    }

    public enum OperatorStutus
    {
        Active,
        Closed
    }
}
