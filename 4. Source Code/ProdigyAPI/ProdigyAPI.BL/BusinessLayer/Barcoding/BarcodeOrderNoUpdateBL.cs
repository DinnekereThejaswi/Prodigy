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
    public class BarcodeOrderNoUpdateBL
    {
        MagnaDbEntities db = null;

        public BarcodeOrderNoUpdateBL()
        {
            db = new MagnaDbEntities(true);
        }

        public BarcodeOrderNoUpdateBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public bool GetBarcodeDetail(string companyCode, string branchCode, string barcodeNo, out TransferBarcodeLine ctcBarcodeLine, out string errorMessage)
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

        public bool SaveBarcode(string companyCode, string branchCode, TransferBarcodeLine barcodeItem, string userID, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                #region Validation of the object
                if (barcodeItem == null) {
                    errorMessage = "No barcode information provided.";
                    return false;
                }

                TransferBarcodeLine ctcBarcodeLine = null;
                if (!GetBarcodeDetail(companyCode, branchCode, barcodeItem.BarcodeNo, out ctcBarcodeLine, out errorMessage)) {
                    return false;
                }
                #endregion
                    
                #region Update Barcode
                var bm = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                                && x.barcode_no == barcodeItem.BarcodeNo).FirstOrDefault();
                if(bm == null) {
                    errorMessage = "Barcode number is not found.";
                    return false;
                }
                if (barcodeItem.Qty != bm.qty || barcodeItem.GrossWt != Convert.ToDecimal(bm.gwt) || barcodeItem.StoneWt != Convert.ToDecimal(bm.swt)
                    || barcodeItem.NetWt != Convert.ToDecimal(bm.nwt)) {
                    errorMessage = "Either Qty, GrossWt, StoneWt or NetWt requested is not correct for barcode number: " + barcodeItem.BarcodeNo;
                    return false;
                }
                bm.order_no = barcodeItem.OrderNo;
                bm.UpdateOn = Globals.GetDateTime();
                db.Entry(bm).State = EntityState.Modified;
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
