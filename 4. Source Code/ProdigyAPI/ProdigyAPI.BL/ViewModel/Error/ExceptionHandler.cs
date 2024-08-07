using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Error
{
    public class ExceptionHandlerVM
    {
        public string ExceptionType { get; set; }
        public string ErrorMessage { get; set; }
        public string InnerExceptionMessage { get; set; }
        public string StackTrace { get; set; }
        public List<EntityValidationErrVM> EntityValidationErrs { get; set; }
        public int ErrorCode { get; set; }
        public override string ToString()
        {
            string validationErrors = string.Empty;
            if (EntityValidationErrs != null) {
                foreach (var x in EntityValidationErrs) {
                    validationErrors = validationErrors + "Property- " + x.PropertyName + ". Msg- " + x.ErrorMessage + Environment.NewLine;
                }
            }
            string innerExceptionMsg = string.Empty;
            if (!string.IsNullOrEmpty(InnerExceptionMessage)) {
                innerExceptionMsg = "-Detailed Error: " + InnerExceptionMessage + Environment.NewLine;
            }
            return "Error: " + ErrorMessage + Environment.NewLine +
                (innerExceptionMsg.Length > 0 ? innerExceptionMsg : string.Empty) +
                (validationErrors.Length > 0 ? "-Validation Errors: " + validationErrors : "");

        }
        public string FullErrorDetail()
        {
            return this.ToString() + Environment.NewLine +
                (!string.IsNullOrEmpty(StackTrace) ? "-Stack Trace: " + StackTrace : string.Empty);
        }

        public string GetEntityValidationError()
        {
            string validationErrors = string.Empty;
            if (EntityValidationErrs != null) {
                foreach (var x in EntityValidationErrs) {
                    validationErrors = validationErrors + "Property- " + x.PropertyName + ". Msg- " + x.ErrorMessage + Environment.NewLine;
                }
            }
            if (!string.IsNullOrEmpty(validationErrors))
                return "Validation Errors: " + validationErrors;
            else
                return null;
        }
        
    }
    public class EntityValidationErrVM
    {
        public string PropertyName { get; set; }
        public string ErrorMessage { get; set; }
    }


}
