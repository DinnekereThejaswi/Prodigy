
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Receipts;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.Receipts
{
    public class ReceiptCancelBL
    {
        MagnaDbEntities db = null;

        public ReceiptCancelBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ReceiptCancelBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        public List<ListOfValue> GetReceiptsToCancel(string companyCode, string branchCode, string receiptType, out ErrorVM error, DateTime? receiptDate = null)
        {
            error = null;
            try {
                if (receiptDate == null) {
                    var lov = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                           && x.receipt_type == receiptType && x.cflag != "Y")
                           .Select(y => new { DocumentNo = y.receipt_no, PartyName = y.party_name }).ToList();
                    if (lov != null) {
                        return lov.Select(z => new ListOfValue
                        {
                            Code = z.DocumentNo.ToString(),
                            Name = z.DocumentNo.ToString() + " - " + z.PartyName
                        }).ToList();
                    }
                    else {
                        error = new ErrorVM { description = "There are no receipt numbers for the selected criteria.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                }
                else {
                    //Ah... the problem using anonymous types and var leads to more code but reduces declaration of more classes.
                    //Finally it is a trade-off between less simplicity and less code. Since this is simple, I chose the the former.
                    var lov = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                          && x.receipt_type == receiptType && x.cflag != "Y"
                          && DbFunctions.TruncateTime(x.receipt_date) == DbFunctions.TruncateTime(receiptDate)) //Only difference
                           .Select(y => new { DocumentNo = y.receipt_no, PartyName = y.party_name }).ToList();
                    if (lov != null) {
                        return lov.Select(z => new ListOfValue
                        {
                            Code = z.DocumentNo.ToString(),
                            Name = z.DocumentNo.ToString() + " - " + z.PartyName
                        }).ToList();
                    }
                    else {
                        error = new ErrorVM { description = "There are no receipt numbers for the selected criteria.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return null;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public bool CancelReceipt(string companyCode, string branchCode, string receiptType, int receiptNo, string cancelRemarks, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Issue Master
                var receiptMaster = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                   && x.receipt_type == receiptType && x.receipt_no == receiptNo && x.cflag != "Y")
                                   .FirstOrDefault();
                if (receiptMaster == null) {
                    error = new ErrorVM
                    {
                        description = string.Format("The receipt No. {0} does not exist or it is already cancelled", receiptNo),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                receiptMaster.cflag = "Y";
                receiptMaster.cancelled_by = userID;
                receiptMaster.cancelled_remarks = cancelRemarks;
                receiptMaster.UpdateOn = updatedTimestamp;
                #endregion

                #region Stock Reverse from Issue Lines
                var issueLines = db.KTTU_RECEIPTS_DETAILS.Where(y => y.company_code == companyCode && y.branch_code == branchCode
                                && y.receipt_no == receiptNo).ToList();
                var stockToReverse = from iLine in issueLines
                                     select new BarcodeReceiptDetailVM
                                     {
                                         BarcodeNo = iLine.barcode_no,
                                         GSCode = iLine.gs_code,
                                         CounterCode = iLine.counter_code,
                                         ItemCode = iLine.item_name,
                                         Qty = iLine.item_no,
                                         GrossWt = Convert.ToDecimal(iLine.gwt),
                                         StoneWt = Convert.ToDecimal(iLine.swt),
                                         NetWt = Convert.ToDecimal(iLine.nwt)
                                     };
                #endregion

                #region Reverse Post Item Counter Stock
                if (receiptType.Equals("RH") || receiptType.Equals("RR") || receiptType.Equals("RC")) {// || receiptType.Equals("RM"))  
                    bool counterStockUpdated = UpdateCounterStock(companyCode, branchCode, db, stockToReverse.ToList(), receiptNo, true, out error);
                    if (!counterStockUpdated) {
                        return false;
                    }
                }
                #endregion

                #region Reverse Post GS Stock
                bool gsStockUpdated = UpdateGSStock(companyCode, branchCode, db, stockToReverse.ToList(), receiptNo, true, out error);
                if (!gsStockUpdated) {
                    return false;
                }
                #endregion

                if (receiptType == "RP") {
                    string purificationAccCancelQuery = "UPDATE KSTU_ACC_VOUCHER_TRANSACTIONS \n"
                           + "SET    cflag                = 'Y' \n"
                           + "WHERE  receipt_no           = @p0 \n"
                           + "       AND company_code     = @p1 \n"
                           + "       AND branch_code      = @p2 \n"
                           + "       AND trans_type       = 'PRFR'";
                    List<object> parameterList = new List<object>();
                    parameterList.Add(receiptNo);
                    parameterList.Add(companyCode);
                    parameterList.Add(branchCode);
                    object[] parametersArray = parameterList.ToArray();
                    int recordsAffected = SIGlobals.Globals.ExecuteSQL(purificationAccCancelQuery, db, parametersArray);
                }

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool UpdateCounterStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeReceiptDetailVM> lines, int docNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post Counter Stock
            var generalStockJournal =
                       from sl in lines
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
                           StockTransType = StockTransactionType.BarcodeReceipt,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = docNo,
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

        private bool UpdateGSStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeReceiptDetailVM> lines, int docNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Fill Counter Stock journal
            var generalStockJournal =
                       from sl in lines
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
                           StockTransType = StockTransactionType.BarcodeReceipt,
                           CompanyCode = g.Key.CompanyCode,
                           BranchCode = g.Key.BranchCode,
                           DocumentNo = docNo,
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
                        StockTransType = StockTransactionType.BarcodeReceipt,
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
