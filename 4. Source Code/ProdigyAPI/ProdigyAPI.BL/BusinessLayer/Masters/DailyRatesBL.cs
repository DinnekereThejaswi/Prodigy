using ProdigyAPI.BL.BusinessLayer.BatchPosting;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class DailyRatesBL
    {
        MagnaDbEntities db;

        public DailyRatesBL()
        {
            db = new MagnaDbEntities(true);
        }
        public DailyRatesBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        public bool GetDailyRates(string companyCode, string branchCode, out DailyRateVM dailyRates, out ErrorVM error)
        {
            error = null;
            dailyRates = new DailyRateVM();
            try {
                var rateList = (from rm in db.KSTU_RATE_MASTER
                                join gi in db.KSTS_GS_ITEM_ENTRY
                                on new { Company = rm.company_code, Branch = rm.branch_code, GS = rm.gs_code }
                                    equals new { Company = gi.company_code, Branch = gi.branch_code, GS = gi.gs_code }
                                where rm.company_code == companyCode && rm.branch_code == branchCode
                                select new { rm, GSName = gi.item_level1_name }).ToList();
                if (rateList == null || rateList.Count <= 0) {
                    error = new ErrorVM { description = "Rate details not found." };
                    return false;
                }

                DateTime applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                if (applicationDate == null)
                    applicationDate = DateTime.Now.Date;
                else
                    applicationDate = applicationDate.Date;

                
                dailyRates.Date = applicationDate;
                dailyRates.CompanyCode = companyCode;
                dailyRates.BranchCode = branchCode;
                dailyRates.SellingRates = rateList.Where(p => p.rm.bill_type == "S").OrderBy(s => s.rm.gs_code)
                    .Select(r => new SellingRate
                    {
                        GsCode = r.rm.gs_code,
                        GSName = r.GSName,
                        Karat = r.rm.karat,
                        SellingBoardRate = r.rm.rate
                    }).ToList();
                dailyRates.PurchaseRates = rateList.Where(p => p.rm.bill_type == "P").OrderBy(s => s.rm.gs_code)
                    .Select(r => new PurchaseRate
                    {
                        GsCode = r.rm.gs_code,
                        GSName = r.GSName,
                        Karat = r.rm.karat,
                        ExchangeRate = r.rm.exchange_rate,
                        CashRate = r.rm.cash_rate
                    }).ToList();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Post(DailyRateVM dailyRates, string userId, bool performDayend, out ErrorVM error)
        {
            error = null;
            if (dailyRates == null) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Object is null",
                    customDescription = "null object"
                };
                return false;
            }
            if (string.IsNullOrEmpty(dailyRates.BranchCode) || string.IsNullOrEmpty(dailyRates.CompanyCode)) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = "Company or Branch information is required",
                    customDescription = "Company or Branch information is required"
                };
                return false;
            }

            DateTime applicationDate = SIGlobals.Globals.GetApplicationDate(dailyRates.CompanyCode, dailyRates.BranchCode);
            if (dailyRates.Date < applicationDate.Date) {
                error = new ErrorVM
                {
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    description = string.Format($"Date {dailyRates.Date.ToString("dd-MMM-yyyy")} cannot be less than today's date({applicationDate.Date.ToString("dd-MMM-yyyy")}).")
                };
                return false;
            }
            else if (dailyRates.Date > applicationDate.Date) {
                if (!performDayend) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        field = "SET_performDayend_true",
                        description = string.Format($"Day-end is required to change the date. Do you want to perform day-end now?.")
                    };
                    return false;
                }
                else {
                    var dayendBL = new DayEndProcessingBL();
                    bool dayEndSuccess = dayendBL.PostDayEnd(dailyRates.CompanyCode, dailyRates.BranchCode, userId, dailyRates.Date.Date, out error);
                    if (!dayEndSuccess) {
                        return false;
                    }
                }
            }

            try {
                #region Archieve current rate details and move to history
                //Note the use of ToList() here. This will ensure that we will not go to database for every cursor move.
                var currentRates = db.KSTU_RATE_MASTER.Where(r => r.company_code == dailyRates.CompanyCode
                        && r.branch_code == dailyRates.BranchCode).ToList();

                if(currentRates == null) {                    
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is nothing to post." };
                    return false;
                }
                //if (currentRates.FirstOrDefault().isDayEnd != "N") {
                //    error = new ErrorVM
                //    {
                //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                //        description = string.Format("Day-end is already done. Please use Magna and set the date. Date change is not supported in this version of the application.")
                //    };
                //    error.customDescription = error.description;
                //    return false;
                //}

                foreach (var cr in currentRates) {
                    var rateHistory = new KSTU_RATE_MASTER_HIS
                    {
                        obj_id = cr.obj_id,
                        ondate = cr.ondate,
                        bill_type = cr.bill_type,
                        gs_code = cr.gs_code,
                        company_code = cr.company_code,
                        branch_code = cr.branch_code,
                        rate = cr.rate,
                        exchange_rate = cr.exchange_rate,
                        cash_rate = cr.cash_rate,
                        karat = cr.karat,
                        isDayEnd = cr.isDayEnd,
                        operator_code = cr.operator_code,
                        UpdateOn = cr.UpdateOn,
                        UniqRowID = cr.UniqRowID.ToString()

                    };
                    db.KSTU_RATE_MASTER_HIS.Add(rateHistory);
                }
                #endregion
                
                DateTime systemDate = SIGlobals.Globals.GetDateTime(); ;
                var saleGSRates = currentRates.Where(r => r.bill_type == "S");
                var purchaseGSRates = currentRates.Where(r => r.bill_type == "P");

                foreach (var sr in dailyRates.SellingRates) {
                    var rateLine = saleGSRates.Where(r => r.gs_code == sr.GsCode && r.karat == sr.Karat).FirstOrDefault(); //Note that we have already queried for branch and company and there is no need to do so again.
                    if (rateLine != null) {
                        rateLine.ondate = applicationDate;
                        rateLine.rate = sr.SellingBoardRate;
                        rateLine.UpdateOn = systemDate;
                        rateLine.operator_code = userId;
                        db.Entry(rateLine).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                foreach (var pr in dailyRates.PurchaseRates) {
                    var rateLine = purchaseGSRates.Where(r => r.gs_code == pr.GsCode && r.karat == pr.Karat).FirstOrDefault(); //Note that we have already queried for branch and company and there is no need to do so again.
                    if (rateLine != null) {
                        rateLine.ondate = applicationDate;
                        rateLine.exchange_rate = pr.ExchangeRate;
                        rateLine.cash_rate = pr.CashRate;
                        rateLine.UpdateOn = systemDate;
                        rateLine.operator_code = userId;
                        db.Entry(rateLine).State = System.Data.Entity.EntityState.Modified;
                    }
                }

                db.SaveChanges();
                SIGlobals.Globals.WriteTransactionLog(userId, "Rates set for date " + applicationDate.ToString("dd-MM-yyyy"), null, db);
            }
            catch(Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            #region Nicelcy handled exception handler. 
            //catch (DbUpdateException ex) {
            //    error = new ErrorVM
            //    {
            //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
            //        description = "Failed to post rates.",
            //        customDescription = ex.Message,
            //    };
            //    if (ex.InnerException != null)
            //        error.InnerException = ex.InnerException.Message;
            //    error.field = ex.GetType().ToString();
            //    return false;
            //}
            //catch (DbEntityValidationException ex) {
            //    error = new ErrorVM
            //    {
            //        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
            //        description = "Failed to post rates.",
            //        customDescription = ex.Message,
            //    };
            //    error.field = ex.GetType().ToString();
            //    var sb = new StringBuilder();
            //    foreach (var validationErrors in ex.EntityValidationErrors) {
            //        foreach (var validationError in validationErrors.ValidationErrors) {
            //            sb.AppendLine(string.Format("Property: {0} Error: {1}",
            //            validationError.PropertyName, validationError.ErrorMessage));
            //        }
            //    }
            //    error.customDescription = sb.ToString();

            //    return false;
            //}
            //catch (Exception ex) {
            //    error = new ErrorVM {ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
            //        description = "Failed to post rates.", customDescription = ex.Message,  };
            //    if (ex.InnerException != null)
            //        error.InnerException = ex.InnerException.Message;
            //    error.field = ex.GetType().ToString();
            //    return false;
            //} 
            #endregion
            return true;
        }
    }
    
}
