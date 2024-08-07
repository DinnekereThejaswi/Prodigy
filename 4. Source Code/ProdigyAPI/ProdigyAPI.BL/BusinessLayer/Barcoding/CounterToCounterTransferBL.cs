using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Barcoding;
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

namespace ProdigyAPI.BL.BusinessLayer.Barcoding
{
    public class CounterToCounterTransferBL
    {
        MagnaDbEntities db = null;

        public CounterToCounterTransferBL()
        {
            db = new MagnaDbEntities(true);
        }

        public CounterToCounterTransferBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool GetBarcodeDetailForCTCTransfer(string companyCode, string branchCode, string transferToCounterCode, string barcodeNo, out TransferBarcodeLine ctcBarcodeLine, out string errorMessage)
        {
            errorMessage = string.Empty;
            ctcBarcodeLine = null;
            try {
                var barcodeInfo = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                        && x.barcode_no == barcodeNo).FirstOrDefault();
                if (barcodeInfo == null) {
                    errorMessage = "No barcode details found for the barcode number: " + barcodeNo;
                    return false;
                }
                if (barcodeInfo.isConfirmed != "Y") {
                    errorMessage = string.Format("Barcode number {0} is not confirmed.", barcodeNo);
                    return false;
                }
                if (barcodeInfo.sold_flag == "Y") {
                    errorMessage = string.Format("Barcode number {0} is sold.", barcodeNo);
                    return false;
                }
                if (barcodeInfo.counter_code == transferToCounterCode) {
                    errorMessage = string.Format("Barcode number {0} is already in the same counter {1}.", barcodeNo, transferToCounterCode);
                    return false;
                }
                ctcBarcodeLine = new TransferBarcodeLine
                {
                    BarcodeNo = barcodeInfo.barcode_no,
                    CounterCode = barcodeInfo.counter_code,
                    GSCode = barcodeInfo.gs_code,
                    GrossWt = Convert.ToDecimal(barcodeInfo.gwt),
                    StoneWt = Convert.ToDecimal(barcodeInfo.swt),
                    NetWt = Convert.ToDecimal(barcodeInfo.nwt),
                    ItemCode = barcodeInfo.item_name,
                    Qty = Convert.ToInt32(barcodeInfo.qty),
                    OrderNo = barcodeInfo.order_no
                };
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).description;
                return false;
            }
            return true;
        }

        public bool PostCTCTransfer(CounterToCounterTransferVM ctcTransfer, string userID, out int transferNo, out string errorMessage)
        {
            transferNo = 0;
            errorMessage = string.Empty;
            try {
                #region Validation of the object
                if (ctcTransfer == null) {
                    errorMessage = "No transfer information provided.";
                    return false;
                }
                if (ctcTransfer.BarcodeLines == null || ctcTransfer.BarcodeLines.Count <= 0) {
                    errorMessage = "Transfer barcode lines do not exist.";
                    return false;
                }
                string companyCode = ctcTransfer.CompanyCode;
                string branchCode = ctcTransfer.BranchCode;
                foreach (var bl in ctcTransfer.BarcodeLines) {
                    TransferBarcodeLine bcl = null;
                    bool functinResult = GetBarcodeDetailForCTCTransfer(companyCode, branchCode, ctcTransfer.TransferToCounter, bl.BarcodeNo, out bcl, out errorMessage);
                    if (functinResult == false) {
                        return false;
                    }
                }
                #endregion

                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                DateTime updatedTimestamp = DateTime.Now;

                transferNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "31", true);
                foreach (var bl in ctcTransfer.BarcodeLines) {
                    updatedTimestamp = Globals.GetDateTime();
                    #region Check if the data sent in the Json is correct
                    var bm = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                    && x.barcode_no == bl.BarcodeNo).FirstOrDefault();
                    if (bl.Qty != bm.qty || bl.GrossWt != Convert.ToDecimal(bm.gwt) || bl.StoneWt != Convert.ToDecimal(bm.swt)
                        || bl.NetWt != Convert.ToDecimal(bm.nwt)) {
                        errorMessage = "Either Qty, GrossWt, StoneWt or NetWt requested is not correct for barcode number: " + bl.BarcodeNo;
                        return false;
                    }
                    #endregion

                    #region Insert to KTTU_COUNTER_TRANSFER_LOG
                    string ctcObjId = SIGlobals.Globals.GetMagnaGUID(new string[] { "KTTU_COUNTER_TRANSFER_LOG", transferNo.ToString(), bm.barcode_no }, companyCode, branchCode);
                    KTTU_COUNTER_TRANSFER_LOG ctl = new KTTU_COUNTER_TRANSFER_LOG
                    {
                        obj_id = ctcObjId,
                        company_code = bm.company_code,
                        branch_code = bm.branch_code,
                        barcode_no = bm.barcode_no,
                        from_counter = bm.counter_code,
                        to_counter = ctcTransfer.TransferToCounter,
                        transfer_no = transferNo,
                        transfer_date = applicationDate,
                        transfer_type = "C",
                        operator_code = userID,
                        UpdateOn = Globals.GetDateTime()
                    };
                    db.KTTU_COUNTER_TRANSFER_LOG.Add(ctl);
                    #endregion

                    #region Update Barcode Master Table and Change the counter code
                    bm.counter_code = ctcTransfer.TransferToCounter;
                    bm.order_no = bl.OrderNo;
                    bm.UpdateOn = updatedTimestamp;
                    db.Entry(bm).State = EntityState.Modified;
                    #endregion

                    #region Delete from Stock Taking
                    var stockTkg = db.KTTU_STOCK_TAKING.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                               && x.barcode_no == bl.BarcodeNo).FirstOrDefault();
                    if (stockTkg != null)
                        db.Entry(stockTkg).State = EntityState.Deleted;
                    #endregion
                }

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "31");
                #endregion

                #region Post Counter Stock
                int documentNo = transferNo;
                ErrorVM error = null;
                #region A. Issue Stock
                var issueStockJournal =
                                  from sl in ctcTransfer.BarcodeLines
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
                                      StockTransType = StockTransactionType.Issue,
                                      CompanyCode = g.Key.CompanyCode,
                                      BranchCode = g.Key.BranchCode,
                                      DocumentNo = documentNo,
                                      GS = g.Key.GS,
                                      Counter = g.Key.Counter,
                                      Item = g.Key.Item,
                                      Qty = g.Sum(x => x.Qty),
                                      GrossWt = g.Sum(x => x.GrossWt),
                                      StoneWt = g.Sum(x => x.StoneWt),
                                      NetWt = g.Sum(x => x.NetWt),
                                  };
                if (!UpdateStock(companyCode, branchCode, db, issueStockJournal.ToList(), documentNo, false, out error)) {
                    errorMessage = error.description;
                    return false;
                }
                #endregion

                #region B. Receipt Stock
                var receiptStockJournal =
                                  from sl in ctcTransfer.BarcodeLines
                                  group sl by new
                                  {
                                      CompanyCode = companyCode,
                                      BranchCode = branchCode,
                                      GS = sl.GSCode,
                                      Counter = ctcTransfer.TransferToCounter, //Destn counter
                                  Item = sl.ItemCode
                                  } into g
                                  select new StockJournalVM
                                  {
                                      StockTransType = StockTransactionType.Receipt, //Receipt type
                                  CompanyCode = g.Key.CompanyCode,
                                      BranchCode = g.Key.BranchCode,
                                      DocumentNo = documentNo,
                                      GS = g.Key.GS,
                                      Counter = g.Key.Counter,
                                      Item = g.Key.Item,
                                      Qty = g.Sum(x => x.Qty),
                                      GrossWt = g.Sum(x => x.GrossWt),
                                      StoneWt = g.Sum(x => x.StoneWt),
                                      NetWt = g.Sum(x => x.NetWt),
                                  };
                if (!UpdateStock(companyCode, branchCode, db, receiptStockJournal.ToList(), documentNo, false, out error)) {
                    errorMessage = error.description;
                    return false;
                }
                #endregion
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                var error = new ErrorVM().GetErrorDetails(ex);
                errorMessage = error.customDescription;
                return false;
            }
            return true;
        }

        public bool UpdateStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<StockJournalVM> generalStockJournal, int documentNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post Counter Stock
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

    }
}
