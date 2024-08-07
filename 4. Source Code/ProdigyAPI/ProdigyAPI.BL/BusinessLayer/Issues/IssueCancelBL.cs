
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Issues;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Issues
{
    public class IssueCancelBL
    {
        MagnaDbEntities db = null;

        public IssueCancelBL()
        {
            db = new MagnaDbEntities(true);
        }

        public IssueCancelBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        public List<ListOfValue> GetIssueToCancel(string companyCode, string branchCode, string issueType, out ErrorVM error, DateTime? issueDate = null)
        {
            error = null;
            try {
                var id = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        && x.cflag != "Y" && x.issue_no > 0);
                

                if (issueDate == null) {
                    var lov = (from im in db.KTTU_ISSUE_MASTER
                                    join sm in db.KSTU_SUPPLIER_MASTER
                                    on new { CC = im.company_code, BC = im.branch_code, PC = im.party_name }
                                       equals new { CC = sm.company_code, BC = sm.branch_code, PC = sm.party_code }
                                    where im.company_code == companyCode && im.branch_code == branchCode
                                        && im.issue_type == issueType && im.cflag != "Y"
                                        && !id.Select(m => m.issue_no).Contains((im.issue_no))
                                    select new { DocumentNo = im.issue_no, PartyName = sm.party_name })
                                    .OrderByDescending(od => od.DocumentNo).ToList();
                    
                    if (lov != null) {
                        return lov.Select(z => new ListOfValue
                        {
                            Code = z.DocumentNo.ToString(),
                            Name = z.DocumentNo.ToString() + " - " + z.PartyName
                        }).ToList();
                    }
                    else {
                        error = new ErrorVM { description = "There are no issues numbers for the selected criteria.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                }
                else {
                    //Ah... the problem using anonymous types and var leads to more code but reduces declaration of more classes.
                    //Finally it is a trade-off between less simplicity and less code. Since this is simple, I chose the the former.
                    //var lov = db.KTTU_ISSUE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    //       && x.issue_type == issueType && x.cflag != "Y"
                    //       && DbFunctions.TruncateTime(x.issue_date) == DbFunctions.TruncateTime(issueDate)) //Only difference
                    //       .Select(y => new { DocumentNo = y.issue_no, PartyName = y.party_name }).ToList();
                    var lov = (from im in db.KTTU_ISSUE_MASTER
                               join sm in db.KSTU_SUPPLIER_MASTER
                               on new { CC = im.company_code, BC = im.branch_code, PC = im.party_name }
                                  equals new { CC = sm.company_code, BC = sm.branch_code, PC = sm.party_code }
                               where im.company_code == companyCode && im.branch_code == branchCode
                                   && im.issue_type == issueType && im.cflag != "Y"
                                   && !id.Select(m => m.issue_no).Contains((im.issue_no))
                                   && DbFunctions.TruncateTime(im.issue_date) == DbFunctions.TruncateTime(issueDate) //Only difference
                               orderby im.issue_no
                               select new { DocumentNo = im.issue_no, PartyName = sm.party_name }).ToList()
                               .OrderByDescending(od => od.DocumentNo).ToList();

                    if (lov != null) {
                        return lov.Select(z => new ListOfValue
                        {
                            Code = z.DocumentNo.ToString(),
                            Name = z.DocumentNo.ToString() + " - " + z.PartyName
                        }).ToList();
                    }
                    else {
                        error = new ErrorVM { description = "There are no issues numbers for the selected criteria.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }
        
        public bool CancelIssue(string companyCode, string branchCode, string issueType, int issueNo, string cancelRemarks, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Issue No. Validation
                var rm = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                && x.cflag != "Y" && x.issue_no > 0);
                var issue = (from im in db.KTTU_ISSUE_MASTER
                             where im.company_code == companyCode && im.branch_code == branchCode
                                 && im.issue_type == issueType && im.cflag != "Y"
                                 && im.issue_no == issueNo
                                 && !rm.Select(m => m.issue_no).Contains((im.issue_no))
                             select new { DocumentNo = im.issue_no }).FirstOrDefault();
                if (issue == null) {
                    error = new ErrorVM { description = "There is no issue detail found.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                } 
                #endregion

                #region Issue Master
                var issueMaster = db.KTTU_ISSUE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                   && x.issue_type == issueType && x.issue_no == issueNo && x.cflag != "Y")
                                   .FirstOrDefault();
                if (issueMaster == null) {
                    error = new ErrorVM
                    {
                        description = string.Format("The issue No. {0} does not exist or it is already cancelled", issueNo),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                issueMaster.cflag = "Y";
                issueMaster.cancelled_by = userID;
                issueMaster.cancelled_remarks = cancelRemarks;
                issueMaster.UpdateOn = updatedTimestamp; 
                #endregion

                #region Stock Reverse from Issue Lines
                var issueLines = db.KTTU_ISSUE_DETAILS.Where(y => y.company_code == companyCode && y.branch_code == branchCode
                                && y.issue_no == issueNo).ToList();
                var stockToReverse = from iLine in issueLines
                                     select new BarcodeIssueLineVM
                                     {
                                         BarcodeNo = iLine.barcode_no,
                                         GSCode = iLine.gs_code,
                                         CounterCode = iLine.counter_code,
                                         ItemCode = iLine.item_name,
                                         Qty = iLine.units,
                                         GrossWt = Convert.ToDecimal(iLine.gwt),
                                         StoneWt = Convert.ToDecimal(iLine.swt),
                                         NetWt = Convert.ToDecimal(iLine.nwt)
                                     }; 
                #endregion

                #region Reverse Post Item Counter Stock
                if (issueType.Equals("IH") || issueType.Equals("IR") || issueType.Equals("IM") || issueType.Equals("IC")) {
                    bool counterStockUpdated = UpdateCounterStock(companyCode, branchCode, db, stockToReverse.ToList(), issueNo, true, out error);
                    if (!counterStockUpdated) {
                        return false;
                    }
                }
                #endregion

                #region Reverse Post GS Stock
                bool gsStockUpdated = UpdateGSStock(companyCode, branchCode, db, stockToReverse.ToList(), issueNo, true, out error);
                if (!gsStockUpdated) {
                    return false;
                }
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool UpdateCounterStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeIssueLineVM> issueLines, int issueNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post Counter Stock
            var generalStockJournal =
                       from sl in issueLines
                       group sl by new
                       {
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = sl.GSCode,
                           Counter = sl.CounterCode,
                           Item = sl.ItemCode
                       } into g
                       select new StockJournalVM
                       {
                           StockTransType = StockTransactionType.BarcodeIssue,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = issueNo,
                           GS = g.Key.GS,
                           Counter = g.Key.Counter,
                           Item = g.Key.Item,
                           Qty = g.Sum(x => x.Qty),
                           GrossWt = g.Sum(x => x.GrossWt),
                           StoneWt = g.Sum(x => x.StoneWt),
                           NetWt = g.Sum(x => x.NetWt),
                       };
            StockPostBL stockPost = new StockPostBL();
            string errorMessage = string.Empty;
            bool counterStockPostSuccess = stockPost.CounterStockPost(dbContext, generalStockJournal.ToList(), postReverse, out errorMessage);
            if (!counterStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion
        
            return true;
        }

        private bool UpdateGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeIssueLineVM> issueLines, int issueNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Fill Counter Stock journal
            var generalStockJournal =
                       from sl in issueLines
                       group sl by new
                       {
                           CompanyCode = companyCode,
                           BranchCode = branchCode,
                           GS = sl.GSCode,
                           Counter = sl.CounterCode,
                           Item = sl.ItemCode
                       } into g
                       select new StockJournalVM
                       {
                           StockTransType = StockTransactionType.BarcodeIssue,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = issueNo,
                           GS = g.Key.GS,
                           Counter = g.Key.Counter,
                           Item = g.Key.Item,
                           Qty = g.Sum(x => x.Qty),
                           GrossWt = g.Sum(x => x.GrossWt),
                           StoneWt = g.Sum(x => x.StoneWt),
                           NetWt = g.Sum(x => x.NetWt),
                       };
            #endregion

            #region Post GS Stock
            var summarizedGSStockJournal =
                    from cj in generalStockJournal
                    group cj by new { Company = cj.CompanyCode, Branch = cj.BranchCode, GS = cj.GS, DocumentNo = cj.DocumentNo } into g
                    select new StockJournalVM
                    {
                        StockTransType = StockTransactionType.BarcodeIssue,
                        CompanyCode = g.Key.Company,
                        BranchCode = g.Key.Branch,
                        DocumentNo = g.Key.DocumentNo,
                        GS = g.Key.GS,
                        Counter = "",
                        Item = "",
                        Qty = g.Sum(x => x.Qty),
                        GrossWt = g.Sum(x => x.GrossWt),
                        StoneWt = g.Sum(x => x.StoneWt),
                        NetWt = g.Sum(x => x.NetWt)
                    };
            string errorMessage = string.Empty;
            bool gsStockPostSuccess = new StockPostBL().GSStockPost(dbContext, summarizedGSStockJournal.ToList(), postReverse, out errorMessage);
            if (!gsStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion

            return true;
        }
    }
}
