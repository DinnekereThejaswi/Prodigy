using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Stock
{
    class StockPostBL
    {
        public bool CounterStockPost(MagnaDbEntities dbContext, List<StockJournalVM> counterStockJournal, bool reverseEntries, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (reverseEntries) {
                foreach(var x in counterStockJournal) {
                    x.Qty = x.Qty * -1;
                    x.GrossWt = x.GrossWt * -1;
                    x.StoneWt = x.StoneWt * -1;
                    x.NetWt = x.NetWt * -1;
                }
            }
            foreach (StockJournalVM sj in counterStockJournal) {
                KTTU_COUNTER_STOCK cs =
                    dbContext.KTTU_COUNTER_STOCK.Where(x => x.company_code == sj.CompanyCode &&
                    x.branch_code == sj.BranchCode && x.gs_code == sj.GS &&
                    x.counter_code == sj.Counter && x.item_name == sj.Item).FirstOrDefault();
                if (cs == null) {
                    errorMsg = "Counter Stock line is not found for item: " + sj.Item + " @ counter " + sj.Counter;
                    return false;
                }
                switch (sj.StockTransType) {
                    case StockTransactionType.Sales:
                        cs.sales_units += sj.Qty;
                        cs.sales_gwt += sj.GrossWt;
                        cs.sales_swt += sj.StoneWt;
                        cs.sales_nwt += sj.NetWt;
                        cs.closing_units -= sj.Qty;
                        cs.closing_gwt -= sj.GrossWt;
                        cs.closing_swt -= sj.StoneWt;
                        cs.closing_nwt -= sj.NetWt;
                        break;
                    case StockTransactionType.SalesReturn:
                    case StockTransactionType.Receipt:
                    case StockTransactionType.BarcodeReceipt:
                        cs.receipt_units += sj.Qty;
                        cs.receipt_gwt += sj.GrossWt;
                        cs.receipt_swt += sj.StoneWt;
                        cs.receipt_nwt += sj.NetWt;
                        cs.closing_units += sj.Qty;
                        cs.closing_gwt += sj.GrossWt;
                        cs.closing_swt += sj.StoneWt;
                        cs.closing_nwt += sj.NetWt;
                        break;
                    case StockTransactionType.Issue:
                    case StockTransactionType.BarcodeIssue:
                        cs.issues_units += sj.Qty;
                        cs.issues_gwt += sj.GrossWt;
                        cs.issues_swt += sj.StoneWt;
                        cs.issues_nwt += sj.NetWt;
                        cs.closing_units -= sj.Qty;
                        cs.closing_gwt -= sj.GrossWt;
                        cs.closing_swt -= sj.StoneWt;
                        cs.closing_nwt -= sj.NetWt;
                        break;
                    default:
                        break;
                }
                cs.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(cs).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

        public bool GSStockPost(MagnaDbEntities dbContext, List<StockJournalVM> gsStockJournal, bool reverseEntries, out string errorMsg)
        {
            errorMsg = string.Empty;
            if (reverseEntries) {
                gsStockJournal.Select(gsj => {
                    gsj.Qty = gsj.Qty * -1;
                    gsj.GrossWt = gsj.GrossWt * -1;
                    gsj.StoneWt = gsj.StoneWt * -1;
                    gsj.NetWt = gsj.NetWt * -1;
                    return gsj;
                });
            }
            foreach (StockJournalVM sj in gsStockJournal) {
                KTTU_GS_SALES_STOCK gs =
                            dbContext.KTTU_GS_SALES_STOCK.Where(x => x.company_code == sj.CompanyCode &&
                            x.branch_code == sj.BranchCode && x.gs_code == sj.GS).FirstOrDefault();
                if (gs == null) {
                    errorMsg = "GS Stock line is not found for GS: " + sj.GS;
                    return false;
                }
                switch (sj.StockTransType) {
                    case StockTransactionType.Sales:
                    case StockTransactionType.Issue:
                    case StockTransactionType.BarcodeIssue:
                        gs.issue_units += sj.Qty;
                        gs.issue_gwt += sj.GrossWt;
                        gs.issue_nwt += sj.NetWt;
                        gs.closing_units -= sj.Qty;
                        gs.closing_gwt -= sj.GrossWt;
                        gs.closing_nwt -= sj.NetWt;
                        break;
                    case StockTransactionType.SalesReturn:
                    case StockTransactionType.Receipt:
                    case StockTransactionType.BarcodeReceipt:
                        gs.receipt_units += sj.Qty;
                        gs.receipt_gwt += sj.GrossWt;
                        gs.receipt_nwt += sj.NetWt;
                        gs.closing_units += sj.Qty;
                        gs.closing_gwt += sj.GrossWt;
                        gs.closing_nwt += sj.NetWt;
                        break;
                    default:
                        break;
                }
                gs.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(gs).State = System.Data.Entity.EntityState.Modified;
            }
            return true;
        }

    }
}
