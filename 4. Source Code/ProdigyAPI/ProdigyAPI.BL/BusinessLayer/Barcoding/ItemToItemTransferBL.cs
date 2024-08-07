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
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.Barcoding
{
    public class ItemToItemTransferBL
    {
        MagnaDbEntities db = null;

        public ItemToItemTransferBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ItemToItemTransferBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool GetBarcodeDetailForItemToItemTransfer(string companyCode, string branchCode, string transferToGsCode, string transferToItemCode, string barcodeNo, out TransferBarcodeLine ctcBarcodeLine, out string errorMessage)
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
                if (barcodeInfo.gs_code != transferToGsCode) {
                    errorMessage = string.Format("GS code {0} for barcode number {1} should be same as transfer GS Code {2}", barcodeInfo.gs_code, barcodeNo, transferToGsCode);
                    return false;
                }
                if (barcodeInfo.item_name == transferToItemCode) {
                    errorMessage = string.Format("Barcode number {0} is already mapped to item {1}.", barcodeNo, transferToItemCode);
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

        public bool PostTransfer(ItemToItemTransferVM itemTransfer, string userID, out int transferNo, out string errorMessage)
        {
            transferNo = 0;
            errorMessage = string.Empty;
            try {
                #region Validation of the object
                if (itemTransfer == null) {
                    errorMessage = "No transfer information provided.";
                    return false;
                }
                if (itemTransfer.BarcodeLines == null || itemTransfer.BarcodeLines.Count <= 0) {
                    errorMessage = "Transfer barcode lines do not exist.";
                    return false;
                }
                string companyCode = itemTransfer.CompanyCode;
                string branchCode = itemTransfer.BranchCode;
                foreach (var bl in itemTransfer.BarcodeLines) {
                    TransferBarcodeLine bcl = null;
                    bool functinResult = GetBarcodeDetailForItemToItemTransfer(companyCode, branchCode, itemTransfer.TransferToGSCode, 
                        itemTransfer.TransferToItemCode, bl.BarcodeNo, out bcl, out errorMessage);
                    if (functinResult == false) {
                        return false;
                    }
                }
                #endregion

                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                DateTime updatedTimestamp = DateTime.Now;

                transferNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "31", true);
                foreach (var bl in itemTransfer.BarcodeLines) {
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
                        from_counter = bm.item_name,
                        to_counter = itemTransfer.TransferToItemCode,
                        transfer_no = transferNo,
                        transfer_date = applicationDate,
                        transfer_type = "I",
                        operator_code = userID,
                        UpdateOn = Globals.GetDateTime()
                    };
                    db.KTTU_COUNTER_TRANSFER_LOG.Add(ctl);
                    #endregion

                    #region Update Barcode Master Table and Change the counter code
                    bm.item_name = itemTransfer.TransferToItemCode;
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
                var ctcTransObj = new CounterToCounterTransferBL();
                #region A. Issue Stock
                var issueStockJournal =
                                  from sl in itemTransfer.BarcodeLines
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
                if (!ctcTransObj.UpdateStock(companyCode, branchCode, db, issueStockJournal.ToList(), documentNo, false, out error)) {
                    errorMessage = error.description;
                    return false;
                }
                #endregion

                #region B. Receipt Stock
                var receiptStockJournal =
                                  from sl in itemTransfer.BarcodeLines
                                  group sl by new
                                  {
                                      CompanyCode = companyCode,
                                      BranchCode = branchCode,
                                      GS = sl.GSCode,
                                      Counter = sl.CounterCode,
                                      Item = itemTransfer.TransferToItemCode //only change
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
                if (!ctcTransObj.UpdateStock(companyCode, branchCode, db, receiptStockJournal.ToList(), documentNo, false, out error)) {
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

    }
}
