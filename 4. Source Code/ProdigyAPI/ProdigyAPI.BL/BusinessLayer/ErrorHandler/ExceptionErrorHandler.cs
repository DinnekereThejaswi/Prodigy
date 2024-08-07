using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;


namespace ProdigyAPI.BL.BusinessLayer.ErrorHandler
{
    public class ExceptionErrorHandler
    { 
        public ExceptionHandlerVM ManageException(Exception ex)
        {
            ExceptionHandlerVM appError = new ExceptionHandlerVM();
            try {
                var exceptionType = ex.GetType();
                appError.ExceptionType = exceptionType.ToString();
                appError.ErrorMessage = ex.Message;
                appError.StackTrace = ex.StackTrace;
                appError.ErrorCode = ex.HResult;

                if (exceptionType == typeof(DbEntityValidationException)) {
                    appError.EntityValidationErrs = new List<EntityValidationErrVM>();
                    DbEntityValidationException dbEVEx = (DbEntityValidationException)ex;
                    foreach (var validationErrors in dbEVEx.EntityValidationErrors) {
                        foreach (var validationError in validationErrors.ValidationErrors) {
                            EntityValidationErrVM eve = new EntityValidationErrVM { PropertyName = validationError.PropertyName, ErrorMessage = validationError.ErrorMessage };
                            appError.EntityValidationErrs.Add(eve);
                        }
                    }
                }
                else if (exceptionType == typeof(DbUpdateException)) {
                    DbUpdateException dbUpdEx = (DbUpdateException)ex;
                    if (dbUpdEx.InnerException != null) {
                        if (dbUpdEx.InnerException.InnerException != null) {
                            var ie = dbUpdEx.InnerException.InnerException;
                            appError.InnerExceptionMessage = ie.Message;
                        }
                        else {
                            appError.InnerExceptionMessage = dbUpdEx.InnerException.Message;
                        }

                    }
                }
                else {
                    if (ex.InnerException != null) {
                        if (ex.InnerException.InnerException != null) {
                            appError.InnerExceptionMessage = ex.InnerException.InnerException.Message;
                        }
                        else {
                            appError.InnerExceptionMessage = ex.InnerException.Message;
                        }

                    }
                }
                logException(appError);
            }
            catch (Exception) {
                //Do nothing, exception in exception, out of my control!
            }
            return appError;
        }

        private void logException(ExceptionHandlerVM eh)
        {
            try {
                using (MagnaDbEntities db = new MagnaDbEntities(true)) {
                    ApplicationErrorLog ael = new ApplicationErrorLog
                    {
                        AbsolutePathUrl = "",
                        ErrorMessage = SIGlobals.Globals.LeftN(eh.ErrorMessage, 1999),
                        ErrorSource = null,
                        HelpLink = null,
                        HResult = eh.ErrorCode.ToString(),
                        InnerExpMsg = SIGlobals.Globals.LeftN(eh.InnerExceptionMessage, 1999),
                        InnerExpSource = null,
                        InsertedBy = "SYS",
                        OccurredOn = SIGlobals.Globals.GetDateTime(),
                        StackTrace1 = SIGlobals.Globals.LeftN(eh.StackTrace, 1999),
                        StackTrace2 = SIGlobals.Globals.LeftN(eh.GetEntityValidationError(), 1999),
                        IPAddress = null
                    };
                    db.ApplicationErrorLogs.Add(ael);
                    db.SaveChanges();
                }
            }
            catch (Exception) {
                //Do nothing.
            }
        }
    }
}
