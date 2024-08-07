using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel
{
    public class CustomModelLevelValidator
    {
    }



    /// <summary>
    /// Custom Level Validation for Company
    /// </summary>
    public class CompanyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string accCode = (string)value;
            if (SIGlobals.Globals.IsValidCompany(accCode)) {
                return ValidationResult.Success;
            }
            else {
                return new ValidationResult("Invalid Company Code");
            }
        }
    }

    /// <summary>
    /// Custom Model Level Validation for Branch
    /// </summary>
    public class BranchAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            string branch = (string)value;
            if (SIGlobals.Globals.IsValidBranch(branch)) {
                return ValidationResult.Success;
            }
            else {
                return new ValidationResult("Invalid Branch Code");
            }
        }
    }
}
