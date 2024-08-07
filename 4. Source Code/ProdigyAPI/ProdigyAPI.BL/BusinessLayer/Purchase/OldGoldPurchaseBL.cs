using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.OldPurchase;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Purchase
{
    public class OldGoldPurchaseBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities(true);
        private const string MODULE_SEQ_NO = "3";
        private const string TABLE_NAME = "KTTU_PURCHASE_EST_MASTER";
        private CultureInfo indianDefaultCulture = new CultureInfo("hi-IN");
        #endregion

        #region Purchase Methods
        public string GetKaratDetailsFromItemCode(string companyCode, string branchCode, string itemCode, string gsCode)
        {
            //DataTable tblData = Common.ExecuteQuery("SELECT * FROM ITEM_MASTER im WHERE im.gs_code='" + gsCode + "' AND im.obj_status='O'");
            //DataRow[] rowData = tblData.Select("ITEM_CODE='" + itemCode + "'");
            //if (rowData != null)
            //    return Ok(new { karat = rowData[0]["Karat"].ToString() });
            //else
            //    return Content(HttpStatusCode.BadRequest, "Invlid GSCode and Item Code");

            //List<ITEM_MASTER> im = db.ITEM_MASTER.Where(it => it.gs_code == gsCode && it.obj_status == "O" && it.company_code == companyCode && it.branch_code == branchCode).ToList();
            //ITEM_MASTER item = im.Where(itm => itm.Item_code == itemCode).FirstOrDefault();
            //return item.Karat;

            string karat = db.ITEM_MASTER.Where(it => it.gs_code == gsCode
                                                && it.obj_status == "O"
                                                && it.company_code == companyCode
                                                && it.branch_code == branchCode
                                                && it.Item_code == itemCode).FirstOrDefault().Karat;
            return karat;
        }

        public dynamic GetGS(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var result = db.KSTS_GS_ITEM_ENTRY.Where(k => k.company_code == companyCode
                                                            && k.branch_code == branchCode && k.bill_type == "P"
                                                            && k.object_status != "C"
                                                            && (k.measure_type == "P" || k.measure_type == "W")).OrderBy(r => r.item_level1_id).ToList();
                return result;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public PurchaseEstMasterVM GetPurchaseEstimationDetails(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            List<KTTU_PURCHASE_EST_DETAILS> kped = new List<KTTU_PURCHASE_EST_DETAILS>();
            List<KTTU_PURCHASE_STONE_DETAILS> kpsd = new List<KTTU_PURCHASE_STONE_DETAILS>();
            List<PurchaseEstDetailsVM> lstOfPed = new List<PurchaseEstDetailsVM>();

            try {
                kpem = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == estNo
                                                            && est.company_code == companyCode
                                                            && est.branch_code == branchCode).FirstOrDefault();
                kped = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == estNo
                                                            && est.company_code == companyCode
                                                            && est.branch_code == branchCode).ToList();
                if (kpem == null) {
                    error = new ErrorVM()
                    {
                        field = "",
                        index = 0,
                        description = "Estimation No Does Not Exist",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return null;
                }

                if (kpem.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        field = "",
                        index = 0,
                        description = "Purchase Est No: " + kpem.est_no + " already Billed. Bill No:" + kpem.bill_no,
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return null;
                }

                PurchaseEstMasterVM pem = new PurchaseEstMasterVM();
                pem.ObjID = kpem.obj_id;
                pem.CompanyCode = kpem.company_code;
                pem.BranchCode = kpem.branch_code;
                pem.EstNo = kpem.est_no;
                pem.BillNo = kpem.bill_no;
                pem.PurItem = kpem.pur_item;
                pem.CustID = kpem.cust_id;
                pem.CustName = kpem.cust_name;
                pem.PDate = kpem.p_date;
                pem.Tax = kpem.tax;
                pem.TodayRate = kpem.today_rate;
                pem.OperatorCode = kpem.operator_code;
                pem.GrandTotal = kpem.grand_total;
                pem.PType = kpem.p_type;
                pem.UpdateOn = kpem.UpdateOn;
                pem.SchemeDuration = kpem.scheme_duratation;
                pem.TotalTaxAmount = kpem.total_tax_amount;
                pem.TotalPurchaseAmount = kpem.total_purchase_amount;
                pem.InvoiceType = kpem.invoice_type;
                pem.Salutation = kpem.salutation;
                pem.CFlag = kpem.cflag;
                pem.RateDeduct = kpem.ratededuct;
                pem.IsPAN = kpem.isPAN;
                pem.Address1 = kpem.address1;
                pem.Address2 = kpem.address2;
                pem.Address3 = kpem.address3;
                pem.City = kpem.city;
                pem.PinCode = kpem.pin_code;
                pem.MobileNo = kpem.mobile_no;
                pem.State = kpem.state;
                pem.StateCode = kpem.state_code;
                pem.TIN = kpem.tin;
                pem.PANNo = kpem.pan_no;
                pem.IDType = kpem.id_type;
                pem.IDDetails = kpem.id_details;
                pem.BookingType = kpem.booking_type;
                pem.PhoneNo = kpem.phone_no;
                pem.EmailID = kpem.Email_ID;

                foreach (KTTU_PURCHASE_EST_DETAILS ped in kped) {
                    PurchaseEstDetailsVM pedv = new PurchaseEstDetailsVM();
                    pedv.ObjID = ped.obj_id;
                    pedv.CompanyCode = ped.company_code;
                    pedv.BranchCode = ped.branch_code;
                    pedv.BillNo = ped.bill_no;
                    pedv.EstNo = ped.est_no;
                    pedv.SlNo = ped.sl_no;
                    pedv.ItemName = ped.item_name;
                    pedv.ItemNo = ped.item_no;
                    pedv.GrossWt = ped.gwt;
                    pedv.StneWt = ped.swt;
                    pedv.NetWt = ped.nwt;
                    pedv.MeltingPercent = ped.melting_percent;
                    pedv.MeltingLoss = ped.melting_loss;
                    pedv.PurchaseRate = ped.purchase_rate;
                    pedv.DiamondAmount = ped.diamond_amount;
                    pedv.GoldAmount = ped.gold_amount;
                    pedv.ItemAmount = ped.item_amount;
                    pedv.SalCode = ped.sal_code;
                    pedv.GSCode = ped.gs_code;
                    pedv.UpdateOn = ped.UpdateOn;
                    pedv.PurityPercent = ped.purity_per;
                    pedv.ConvertionWt = ped.convertion_wt;
                    pedv.FinYear = ped.Fin_Year;
                    pedv.ItemDescription = ped.item_description;
                    pedv.ItemwiseTaxPercentage = ped.itemwise_tax_percentage;
                    pedv.ItemwiseTaxAmount = ped.itemwise_tax_amount;
                    pedv.ItemwisePurchaseAmount = ped.itemwise_purchase_amount;
                    pedv.InvoiceType = ped.invoice_type;
                    pedv.RateDeduct = ped.ratededuct;
                    pedv.GSTGroupCode = ped.GSTGroupCode;
                    pedv.SGSTPercent = ped.SGST_Percent;
                    pedv.SGSTAmount = ped.SGST_Amount;
                    pedv.CGSTPercent = ped.CGST_Percent;
                    pedv.CGSTAmount = ped.CGST_Amount;
                    pedv.IGSTPercent = ped.IGST_Percent;
                    pedv.IGSTAmount = ped.IGST_Amount;
                    pedv.HSN = ped.HSN;
                    pedv.Dcts = ped.dcts;
                    //pedv.RefNo = ped.RefNo;

                    List<PurchaseEstStoneDetailsVM> lstOfPesd = new List<PurchaseEstStoneDetailsVM>();
                    //kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.name == "Old Stone" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                    kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.gs_code == "OST" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                    foreach (KTTU_PURCHASE_STONE_DETAILS psd in kpsd) {
                        PurchaseEstStoneDetailsVM pesd = new PurchaseEstStoneDetailsVM();
                        pesd.ObjID = psd.obj_id;
                        pesd.CompanyCode = psd.company_code;
                        pesd.BranchCode = psd.branch_code;
                        pesd.BillNO = psd.bill_no;
                        pesd.SlNo = psd.sno;
                        pesd.EstNo = psd.est_no;
                        pesd.ItemSlNo = psd.item_sno;
                        pesd.Type = psd.type;
                        pesd.Name = psd.name;
                        pesd.Qty = psd.qty;
                        pesd.Carrat = psd.carrat;
                        pesd.Rate = psd.rate;
                        pesd.Amount = psd.amount;
                        pesd.PurchaseDealerBillNo = psd.pur_dealer_bill_no;
                        pesd.GSCode = psd.gs_code;
                        pesd.PurReturnNo = psd.pur_return_no;
                        pesd.UpdateOn = psd.UpdateOn;
                        pesd.FinYear = psd.Fin_Year;
                        lstOfPesd.Add(pesd);
                    }
                    pedv.lstPurchaseEstStoneDetailsVM = lstOfPesd;
                    List<PurchaseEstStoneDetailsVM> lstOfPD = new List<PurchaseEstStoneDetailsVM>();
                    //kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.name == "Old Diamond" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                    kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.gs_code == "OD" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                    foreach (KTTU_PURCHASE_STONE_DETAILS psd in kpsd) {
                        PurchaseEstStoneDetailsVM pesd = new PurchaseEstStoneDetailsVM();
                        pesd.ObjID = psd.obj_id;
                        pesd.CompanyCode = psd.company_code;
                        pesd.BranchCode = psd.branch_code;
                        pesd.BillNO = psd.bill_no;
                        pesd.SlNo = psd.sno;
                        pesd.EstNo = psd.est_no;
                        pesd.ItemSlNo = psd.item_sno;
                        pesd.Type = psd.type;
                        pesd.Name = psd.name;
                        pesd.Qty = psd.qty;
                        pesd.Carrat = psd.carrat;
                        pesd.Rate = psd.rate;
                        pesd.Amount = psd.amount;
                        pesd.PurchaseDealerBillNo = psd.pur_dealer_bill_no;
                        pesd.GSCode = psd.gs_code;
                        pesd.PurReturnNo = psd.pur_return_no;
                        pesd.UpdateOn = psd.UpdateOn;
                        pesd.FinYear = psd.Fin_Year;
                        lstOfPD.Add(pesd);
                    }
                    pedv.lstPurchaseEstDiamondDetailsVM = lstOfPD;
                    lstOfPed.Add(pedv);
                }
                pem.lstPurchaseEstDetailsVM = lstOfPed;
                return pem;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public PurchaseEstMasterVM GetAttachedPurchaseEstimationDetailsForPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            List<KTTU_PURCHASE_EST_DETAILS> kped = new List<KTTU_PURCHASE_EST_DETAILS>();
            List<KTTU_PURCHASE_STONE_DETAILS> kpsd = new List<KTTU_PURCHASE_STONE_DETAILS>();
            List<PurchaseEstDetailsVM> lstOfPed = new List<PurchaseEstDetailsVM>();
            PurchaseEstMasterVM pem = new PurchaseEstMasterVM();
            try {
                List<KTTU_PAYMENT_DETAILS> lstOfAttachedEst = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo && pay.company_code == companyCode && pay.branch_code == branchCode && pay.pay_mode == "PE").ToList();
                foreach (KTTU_PAYMENT_DETAILS kpd in lstOfAttachedEst) {
                    int attachedEstNo = Convert.ToInt32(kpd.Ref_BillNo);
                    kpem = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == attachedEstNo && est.company_code == companyCode && est.branch_code == branchCode).FirstOrDefault();
                    kped = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == attachedEstNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();

                    if (kpem == null) {
                        error = new ErrorVM() { field = "", index = 0, description = "Invalid estimation number" };
                        return null;
                    }

                    pem.ObjID = kpem.obj_id;
                    pem.CompanyCode = kpem.company_code;
                    pem.BranchCode = kpem.branch_code;
                    pem.EstNo = kpem.est_no;
                    pem.BillNo = kpem.bill_no;
                    pem.PurItem = kpem.pur_item;
                    pem.CustID = kpem.cust_id;
                    pem.CustName = kpem.cust_name;
                    pem.PDate = kpem.p_date;
                    pem.Tax = kpem.tax;
                    pem.TodayRate = kpem.today_rate;
                    pem.OperatorCode = kpem.operator_code;
                    pem.GrandTotal = kpem.grand_total;
                    pem.PType = kpem.p_type;
                    pem.UpdateOn = kpem.UpdateOn;
                    pem.SchemeDuration = kpem.scheme_duratation;
                    pem.TotalTaxAmount = kpem.total_tax_amount;
                    pem.TotalPurchaseAmount = kpem.total_purchase_amount;
                    pem.InvoiceType = kpem.invoice_type;
                    pem.Salutation = kpem.salutation;
                    pem.CFlag = kpem.cflag;
                    pem.RateDeduct = kpem.ratededuct;
                    pem.IsPAN = kpem.isPAN;
                    pem.Address1 = kpem.address1;
                    pem.Address2 = kpem.address2;
                    pem.Address3 = kpem.address3;
                    pem.City = kpem.city;
                    pem.PinCode = kpem.pin_code;
                    pem.MobileNo = kpem.mobile_no;
                    pem.State = kpem.state;
                    pem.StateCode = kpem.state_code;
                    pem.TIN = kpem.tin;
                    pem.PANNo = kpem.pan_no;
                    pem.IDType = kpem.id_type;
                    pem.IDDetails = kpem.id_details;
                    pem.BookingType = kpem.booking_type;
                    pem.PhoneNo = kpem.phone_no;
                    pem.EmailID = kpem.Email_ID;

                    foreach (KTTU_PURCHASE_EST_DETAILS ped in kped) {
                        PurchaseEstDetailsVM pedv = new PurchaseEstDetailsVM();
                        pedv.ObjID = ped.obj_id;
                        pedv.CompanyCode = ped.company_code;
                        pedv.BranchCode = ped.branch_code;
                        pedv.BillNo = ped.bill_no;
                        pedv.EstNo = ped.est_no;
                        pedv.SlNo = ped.sl_no;
                        pedv.ItemName = ped.item_name;
                        pedv.ItemNo = ped.item_no;
                        pedv.GrossWt = ped.gwt;
                        pedv.StneWt = ped.swt;
                        pedv.NetWt = ped.nwt;
                        pedv.MeltingPercent = ped.melting_percent;
                        pedv.MeltingLoss = ped.melting_loss;
                        pedv.PurchaseRate = ped.purchase_rate;
                        pedv.DiamondAmount = ped.diamond_amount;
                        pedv.GoldAmount = ped.gold_amount;
                        pedv.ItemAmount = ped.item_amount;
                        pedv.SalCode = ped.sal_code;
                        pedv.GSCode = ped.gs_code;
                        pedv.UpdateOn = ped.UpdateOn;
                        pedv.PurityPercent = ped.purity_per;
                        pedv.ConvertionWt = ped.convertion_wt;
                        pedv.FinYear = ped.Fin_Year;
                        pedv.ItemDescription = ped.item_description;
                        pedv.ItemwiseTaxPercentage = ped.itemwise_tax_percentage;
                        pedv.ItemwiseTaxAmount = ped.itemwise_tax_amount;
                        pedv.ItemwisePurchaseAmount = ped.itemwise_purchase_amount;
                        pedv.InvoiceType = ped.invoice_type;
                        pedv.RateDeduct = ped.ratededuct;
                        pedv.GSTGroupCode = ped.GSTGroupCode;
                        pedv.SGSTPercent = ped.SGST_Percent;
                        pedv.SGSTAmount = ped.SGST_Amount;
                        pedv.CGSTPercent = ped.CGST_Percent;
                        pedv.CGSTAmount = ped.CGST_Amount;
                        pedv.IGSTPercent = ped.IGST_Percent;
                        pedv.IGSTAmount = ped.IGST_Amount;
                        pedv.HSN = ped.HSN;
                        pedv.Dcts = ped.dcts;
                        //pedv.RefNo = ped.RefNo;

                        List<PurchaseEstStoneDetailsVM> lstOfPesd = new List<PurchaseEstStoneDetailsVM>();
                        kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.name == "Old Stone" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                        foreach (KTTU_PURCHASE_STONE_DETAILS psd in kpsd) {
                            PurchaseEstStoneDetailsVM pesd = new PurchaseEstStoneDetailsVM();
                            pesd.ObjID = psd.obj_id;
                            pesd.CompanyCode = psd.company_code;
                            pesd.BranchCode = psd.branch_code;
                            pesd.BillNO = psd.bill_no;
                            pesd.SlNo = psd.sno;
                            pesd.EstNo = psd.est_no;
                            pesd.ItemSlNo = psd.item_sno;
                            pesd.Type = psd.type;
                            pesd.Name = psd.name;
                            pesd.Qty = psd.qty;
                            pesd.Carrat = psd.carrat;
                            pesd.Rate = psd.rate;
                            pesd.Amount = psd.amount;
                            pesd.PurchaseDealerBillNo = psd.pur_dealer_bill_no;
                            pesd.GSCode = psd.gs_code;
                            pesd.PurReturnNo = psd.pur_return_no;
                            pesd.UpdateOn = psd.UpdateOn;
                            pesd.FinYear = psd.Fin_Year;
                            lstOfPesd.Add(pesd);
                        }
                        pedv.lstPurchaseEstStoneDetailsVM = lstOfPesd;
                        List<PurchaseEstStoneDetailsVM> lstOfPD = new List<PurchaseEstStoneDetailsVM>();
                        kpsd = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == estNo && est.item_sno == pedv.SlNo && est.name == "Old Diamond" && est.company_code == companyCode && est.branch_code == branchCode).ToList();
                        foreach (KTTU_PURCHASE_STONE_DETAILS psd in kpsd) {
                            PurchaseEstStoneDetailsVM pesd = new PurchaseEstStoneDetailsVM();
                            pesd.ObjID = psd.obj_id;
                            pesd.CompanyCode = psd.company_code;
                            pesd.BranchCode = psd.branch_code;
                            pesd.BillNO = psd.bill_no;
                            pesd.SlNo = psd.sno;
                            pesd.EstNo = psd.est_no;
                            pesd.ItemSlNo = psd.item_sno;
                            pesd.Type = psd.type;
                            pesd.Name = psd.name;
                            pesd.Qty = psd.qty;
                            pesd.Carrat = psd.carrat;
                            pesd.Rate = psd.rate;
                            pesd.Amount = psd.amount;
                            pesd.PurchaseDealerBillNo = psd.pur_dealer_bill_no;
                            pesd.GSCode = psd.gs_code;
                            pesd.PurReturnNo = psd.pur_return_no;
                            pesd.UpdateOn = psd.UpdateOn;
                            pesd.FinYear = psd.Fin_Year;
                            lstOfPD.Add(pesd);
                        }
                        pedv.lstPurchaseEstDiamondDetailsVM = lstOfPD;
                        lstOfPed.Add(pedv);
                    }
                    pem.lstPurchaseEstDetailsVM = lstOfPed;
                }
                return pem;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public PurchaseEstMasterVM GetPurchaseEstimationDetailsForTotal(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            PurchaseEstDetailsVM totalPurchaseDet = new PurchaseEstDetailsVM();
            List<KTTU_PURCHASE_EST_DETAILS> kped = new List<KTTU_PURCHASE_EST_DETAILS>();
            List<KTTU_PURCHASE_STONE_DETAILS> kpsd = new List<KTTU_PURCHASE_STONE_DETAILS>();
            List<PurchaseEstDetailsVM> lstOfPed = new List<PurchaseEstDetailsVM>();

            try {
                kpem = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).FirstOrDefault();
                kped = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == estNo && est.company_code == companyCode && est.branch_code == branchCode).ToList();

                if (kpem == null) {
                    error = new ErrorVM() { field = "", index = 0, description = "Invalid estimation number" };
                    return null;
                }

                PurchaseEstMasterVM pem = new PurchaseEstMasterVM();
                pem.ObjID = kpem.obj_id;
                pem.CompanyCode = kpem.company_code;
                pem.BranchCode = kpem.branch_code;
                pem.EstNo = kpem.est_no;
                pem.BillNo = kpem.bill_no;
                pem.PurItem = kpem.pur_item;
                pem.CustID = kpem.cust_id;
                pem.CustName = kpem.cust_name;
                pem.PDate = kpem.p_date;
                pem.Tax = kpem.tax;
                pem.TodayRate = kpem.today_rate;
                pem.OperatorCode = kpem.operator_code;
                pem.GrandTotal = kpem.grand_total;
                pem.PType = kpem.p_type;
                pem.UpdateOn = kpem.UpdateOn;
                pem.SchemeDuration = kpem.scheme_duratation;
                pem.TotalTaxAmount = kpem.total_tax_amount;
                pem.TotalPurchaseAmount = kpem.total_purchase_amount;
                pem.InvoiceType = kpem.invoice_type;
                pem.Salutation = kpem.salutation;
                pem.CFlag = kpem.cflag;
                pem.RateDeduct = kpem.ratededuct;
                pem.IsPAN = kpem.isPAN;
                pem.Address1 = kpem.address1;
                pem.Address2 = kpem.address2;
                pem.Address3 = kpem.address3;
                pem.City = kpem.city;
                pem.PinCode = kpem.pin_code;
                pem.MobileNo = kpem.mobile_no;
                pem.State = kpem.state;
                pem.StateCode = kpem.state_code;
                pem.TIN = kpem.tin;
                pem.PANNo = kpem.pan_no;
                pem.IDType = kpem.id_type;
                pem.IDDetails = kpem.id_details;
                pem.BookingType = kpem.booking_type;
                pem.PhoneNo = kpem.phone_no;
                pem.EmailID = kpem.Email_ID;
                foreach (KTTU_PURCHASE_EST_DETAILS ped in kped) {
                    totalPurchaseDet.GrossWt = totalPurchaseDet.GrossWt + ped.gwt;
                    totalPurchaseDet.StneWt = totalPurchaseDet.StneWt + ped.swt;
                    totalPurchaseDet.NetWt = totalPurchaseDet.NetWt + ped.nwt;
                    totalPurchaseDet.GoldAmount = totalPurchaseDet.GoldAmount + ped.gold_amount;
                    totalPurchaseDet.DiamondAmount = totalPurchaseDet.DiamondAmount + ped.diamond_amount;
                    totalPurchaseDet.ItemAmount = totalPurchaseDet.ItemAmount + ped.item_amount;
                }
                totalPurchaseDet.ItemAmount = Math.Round(totalPurchaseDet.ItemAmount, 0, MidpointRounding.ToEven);
                lstOfPed.Add(totalPurchaseDet);
                pem.lstPurchaseEstDetailsVM = lstOfPed;
                return pem;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public PurchaseEstMasterVM GetAttachedPurchaseEstimationDetailsForPrintTotal(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            PurchaseEstDetailsVM totalPurchaseDet = new PurchaseEstDetailsVM();
            List<KTTU_PURCHASE_EST_DETAILS> kped = new List<KTTU_PURCHASE_EST_DETAILS>();
            List<KTTU_PURCHASE_STONE_DETAILS> kpsd = new List<KTTU_PURCHASE_STONE_DETAILS>();
            List<PurchaseEstDetailsVM> lstOfPed = new List<PurchaseEstDetailsVM>();
            PurchaseEstMasterVM pem = new PurchaseEstMasterVM();

            try {
                // Taking attached Old gold purchase estimation for the selected sales Estimation No.
                List<KTTU_PAYMENT_DETAILS> lstOfAttachedEst = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                && pay.company_code == companyCode
                                                                && pay.branch_code == branchCode
                                                                && pay.pay_mode == "PE"
                                                                && pay.cflag != "Y").ToList();
                foreach (KTTU_PAYMENT_DETAILS kpd in lstOfAttachedEst) {

                    int attachedEstNo = Convert.ToInt32(kpd.Ref_BillNo);
                    kpem = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == attachedEstNo
                                                            && est.company_code == companyCode
                                                            && est.branch_code == branchCode).FirstOrDefault();
                    kped = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == attachedEstNo
                                                                && est.company_code == companyCode
                                                                && est.branch_code == branchCode).ToList();

                    if (kpem == null) {
                        error = new ErrorVM() { field = "", index = 0, description = "Invalid estimation number" };
                        return null;
                    }
                    pem.ObjID = kpem.obj_id;
                    pem.CompanyCode = kpem.company_code;
                    pem.BranchCode = kpem.branch_code;
                    pem.EstNo = kpem.est_no;
                    pem.BillNo = kpem.bill_no;
                    pem.PurItem = kpem.pur_item;
                    pem.CustID = kpem.cust_id;
                    pem.CustName = kpem.cust_name;
                    pem.PDate = kpem.p_date;
                    pem.Tax = kpem.tax;
                    pem.TodayRate = kpem.today_rate;
                    pem.OperatorCode = kpem.operator_code;
                    pem.GrandTotal = kpem.grand_total;
                    pem.PType = kpem.p_type;
                    pem.UpdateOn = kpem.UpdateOn;
                    pem.SchemeDuration = kpem.scheme_duratation;
                    pem.TotalTaxAmount = kpem.total_tax_amount;
                    pem.TotalPurchaseAmount = kpem.total_purchase_amount;
                    pem.InvoiceType = kpem.invoice_type;
                    pem.Salutation = kpem.salutation;
                    pem.CFlag = kpem.cflag;
                    pem.RateDeduct = kpem.ratededuct;
                    pem.IsPAN = kpem.isPAN;
                    pem.Address1 = kpem.address1;
                    pem.Address2 = kpem.address2;
                    pem.Address3 = kpem.address3;
                    pem.City = kpem.city;
                    pem.PinCode = kpem.pin_code;
                    pem.MobileNo = kpem.mobile_no;
                    pem.State = kpem.state;
                    pem.StateCode = kpem.state_code;
                    pem.TIN = kpem.tin;
                    pem.PANNo = kpem.pan_no;
                    pem.IDType = kpem.id_type;
                    pem.IDDetails = kpem.id_details;
                    pem.BookingType = kpem.booking_type;
                    pem.PhoneNo = kpem.phone_no;
                    pem.EmailID = kpem.Email_ID;

                    foreach (KTTU_PURCHASE_EST_DETAILS ped in kped) {
                        totalPurchaseDet.GrossWt = totalPurchaseDet.GrossWt + ped.gwt;
                        totalPurchaseDet.StneWt = totalPurchaseDet.StneWt + ped.swt;
                        totalPurchaseDet.NetWt = totalPurchaseDet.NetWt + ped.nwt;
                        totalPurchaseDet.GoldAmount = totalPurchaseDet.GoldAmount + ped.gold_amount;
                        totalPurchaseDet.DiamondAmount = totalPurchaseDet.DiamondAmount + ped.diamond_amount;
                        totalPurchaseDet.ItemAmount = totalPurchaseDet.ItemAmount + ped.item_amount;
                    }
                }
                totalPurchaseDet.ItemAmount = Math.Round(totalPurchaseDet.ItemAmount, 0, MidpointRounding.ToEven);
                lstOfPed.Add(totalPurchaseDet);
                pem.lstPurchaseEstDetailsVM = lstOfPed;
                return pem;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public int SavePurchaseEstimation(PurchaseEstMasterVM purchase, out ErrorVM error)
        {
            error = null;
            if (purchase == null) {
                error = new ErrorVM()
                {
                    field = "",
                    index = 0,
                    description = "Invalid Purchase Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return 0;
            }

            if (purchase.BillNo != 0) {
                error = new ErrorVM()
                {
                    field = "",
                    index = 0,
                    description = "Purchase Est No: " + purchase.EstNo + " already Billed. Bill No:" + purchase.BillNo,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return 0;
            }

            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            KSTU_CUSTOMER_MASTER customer = new KSTU_CUSTOMER_MASTER();
            try {

                if (purchase.EstNo != 0) {
                    #region Deleting Existing Estimation
                    KTTU_PURCHASE_EST_MASTER delEstMaster = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == purchase.EstNo
                                                                                                && est.company_code == purchase.CompanyCode
                                                                                                && est.branch_code == purchase.BranchCode).FirstOrDefault();
                    if (delEstMaster != null) {
                        db.KTTU_PURCHASE_EST_MASTER.Remove(delEstMaster);
                    }

                    List<KTTU_PURCHASE_EST_DETAILS> delLstEstDet = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == purchase.EstNo
                                                                                                        && est.company_code == purchase.CompanyCode
                                                                                                        && est.branch_code == purchase.BranchCode).ToList();
                    if (delLstEstDet != null) {
                        foreach (KTTU_PURCHASE_EST_DETAILS delEst in delLstEstDet) {
                            db.KTTU_PURCHASE_EST_DETAILS.Remove(delEst);
                        }
                    }

                    List<KTTU_PURCHASE_STONE_DETAILS> delLstOfPurStoneDet = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == purchase.EstNo
                                                                                                                    && est.company_code == purchase.CompanyCode
                                                                                                                    && est.branch_code == purchase.BranchCode).ToList();
                    if (delLstOfPurStoneDet != null) {
                        foreach (KTTU_PURCHASE_STONE_DETAILS delEstStone in delLstOfPurStoneDet) {
                            db.KTTU_PURCHASE_STONE_DETAILS.Remove(delEstStone);
                        }
                    }
                    #endregion
                }

                if (purchase.CustID != null && purchase.CustID != 0) {
                    customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(purchase.CustID), purchase.MobileNo, purchase.CompanyCode, purchase.BranchCode);
                }

                #region Saving of  Old Gold Purchase Master
                int estNo = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == MODULE_SEQ_NO
                                                    && sq.company_code == purchase.CompanyCode
                                                    && sq.branch_code == purchase.BranchCode).FirstOrDefault().nextno;
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, estNo, purchase.CompanyCode, purchase.BranchCode);
                kpem.obj_id = objectID;
                kpem.company_code = purchase.CompanyCode;
                kpem.branch_code = purchase.BranchCode;
                kpem.est_no = purchase.EstNo == 0 ? estNo : purchase.EstNo;
                kpem.bill_no = purchase.BillNo;
                kpem.pur_item = purchase.PurItem;
                kpem.p_date = SIGlobals.Globals.GetApplicationDate(purchase.CompanyCode, purchase.BranchCode);
                kpem.tax = purchase.Tax;
                kpem.today_rate = purchase.TodayRate;
                kpem.operator_code = purchase.OperatorCode;
                kpem.grand_total = purchase.GrandTotal;
                kpem.p_type = purchase.EstNo == 0 ? "PE" : "P";
                kpem.UpdateOn = SIGlobals.Globals.GetDateTime();
                kpem.scheme_duratation = 0;
                kpem.total_tax_amount = 0;
                kpem.total_purchase_amount = purchase.TotalPurchaseAmount;
                kpem.invoice_type = purchase.InvoiceType == null ? 0 : purchase.InvoiceType;
                kpem.cflag = "N";
                kpem.ratededuct = purchase.RateDeduct == null ? 0 : purchase.RateDeduct;
                kpem.booking_type = purchase.BookingType;
                //ToDo - Thease two fields are hotcoded- Need input from magna team.
                kpem.diamondRateCalculation_type = 0;
                kpem.additional_amount = 0;
                if (customer.cust_id != 0) {
                    kpem.cust_id = customer.cust_id;
                    kpem.cust_name = customer.cust_name;
                    kpem.salutation = customer.salutation;
                    kpem.isPAN = customer.pan_no == null ? "N" : "Y";
                    kpem.address1 = customer.address1;
                    kpem.address2 = customer.address2;
                    kpem.address3 = customer.address3;
                    kpem.city = customer.city;
                    kpem.pin_code = customer.pin_code;
                    kpem.mobile_no = customer.mobile_no;
                    kpem.state = customer.state;
                    kpem.state_code = customer.state_code;
                    kpem.tin = customer.tin;
                    kpem.pan_no = customer.pan_no;
                    kpem.id_type = customer.id_type;
                    kpem.id_details = customer.id_details;
                    kpem.phone_no = customer.phone_no;
                    kpem.Email_ID = customer.Email_ID;
                }
                else {
                    kpem.cust_name = "";
                }
                db.KTTU_PURCHASE_EST_MASTER.Add(kpem);

                // If Sales Details Exist updating the purchase sales Estimation master as well.
                KTTU_SALES_EST_MASTER salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(sal => sal.company_code == purchase.CompanyCode && sal.branch_code == purchase.BranchCode && sal.est_no == estNo).FirstOrDefault();
                if (salesEstMaster != null) {
                    salesEstMaster.purchase_amount = Convert.ToDecimal(purchase.TotalPurchaseAmount);
                    db.Entry(salesEstMaster).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #region Saving of Old Gold Purchase Details
                int SlNo = 1;
                string obj_id = objectID;
                foreach (PurchaseEstDetailsVM ped in purchase.lstPurchaseEstDetailsVM) {
                    KTTU_PURCHASE_EST_DETAILS kped = new KTTU_PURCHASE_EST_DETAILS();
                    kped.obj_id = obj_id;
                    kped.company_code = ped.CompanyCode;
                    kped.branch_code = ped.BranchCode;
                    kped.bill_no = ped.BillNo;
                    kped.est_no = kpem.est_no;
                    kped.sl_no = SlNo;
                    kped.item_name = ped.ItemName;
                    kped.item_no = ped.ItemNo;
                    kped.gwt = ped.GrossWt;
                    kped.swt = ped.StneWt;
                    kped.nwt = ped.NetWt;
                    kped.melting_percent = ped.MeltingPercent;
                    kped.melting_loss = ped.MeltingLoss;
                    kped.purchase_rate = ped.PurchaseRate;
                    kped.diamond_amount = ped.DiamondAmount;
                    kped.gold_amount = ped.ItemAmount - ped.DiamondAmount;//ped.GoldAmount;
                    kped.item_amount = ped.ItemAmount;
                    kped.sal_code = ped.SalCode;
                    kped.gs_code = ped.GSCode;
                    kped.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kped.purity_per = ped.PurityPercent;
                    kped.convertion_wt = ped.ConvertionWt;
                    kped.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, purchase.CompanyCode, purchase.BranchCode);
                    kped.item_description = ped.ItemName;//ped.ItemDescription;
                    kped.itemwise_tax_percentage = 0;
                    kped.itemwise_tax_amount = 0;
                    kped.itemwise_purchase_amount = purchase.TotalPurchaseAmount;// ped.ItemwisePurchaseAmount;
                    kped.invoice_type = ped.InvoiceType;
                    kped.ratededuct = 0;
                    kped.GSTGroupCode = db.KSTS_GS_ITEM_ENTRY.Where(kgie => kgie.gs_code == ped.GSCode && kgie.company_code == ped.CompanyCode && kgie.branch_code == ped.BranchCode).FirstOrDefault().GSTGoodsGroupCode;//ped.GSTGroupCode;
                    kped.SGST_Percent = 0;
                    kped.SGST_Amount = 0;
                    kped.CGST_Percent = 0;
                    kped.CGST_Amount = 0;
                    kped.IGST_Percent = 0;
                    kped.IGST_Amount = 0;
                    kped.HSN = Convert.ToString(db.usp_GetGSTHSNCode(ped.CompanyCode, ped.BranchCode, ped.GSCode, ped.ItemName).FirstOrDefault());
                    kped.dcts = ped.Dcts == null ? 0 : ped.Dcts;
                    // Column is not there in the db.
                    //kped.RefNo = ped.RefNo;
                    db.KTTU_PURCHASE_EST_DETAILS.Add(kped);

                    #region Saving of Old Gold Purchase Stone Details
                    if (ped.lstPurchaseEstStoneDetailsVM != null) {
                        int stoneSlNo = 1;
                        foreach (PurchaseEstStoneDetailsVM pesd in ped.lstPurchaseEstStoneDetailsVM) {
                            KTTU_PURCHASE_STONE_DETAILS kpsd = new KTTU_PURCHASE_STONE_DETAILS();
                            kpsd.obj_id = objectID;
                            kpsd.company_code = pesd.CompanyCode;
                            kpsd.branch_code = pesd.BranchCode;
                            kpsd.bill_no = pesd.BillNO;
                            kpsd.sno = stoneSlNo;
                            kpsd.est_no = kped.est_no;
                            kpsd.item_sno = SlNo;
                            kpsd.type = purchase.EstNo == 0 ? "PE" : "P";
                            kpsd.name = pesd.Name;
                            kpsd.qty = pesd.Qty;
                            kpsd.carrat = pesd.Carrat;
                            kpsd.rate = pesd.Rate;
                            kpsd.amount = pesd.Amount;
                            kpsd.pur_dealer_bill_no = pesd.PurchaseDealerBillNo;
                            kpsd.gs_code = pesd.GSCode == null ? "OST" : pesd.GSCode;
                            kpsd.pur_return_no = pesd.PurReturnNo == null ? 0 : pesd.PurReturnNo;
                            kpsd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpsd.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, pesd.CompanyCode, pesd.BranchCode);
                            kpsd.UniqRowID = Guid.NewGuid();
                            db.KTTU_PURCHASE_STONE_DETAILS.Add(kpsd);
                            stoneSlNo++;
                        }
                        if (ped.lstPurchaseEstDiamondDetailsVM != null) {
                            foreach (PurchaseEstStoneDetailsVM pesd in ped.lstPurchaseEstDiamondDetailsVM) {
                                KTTU_PURCHASE_STONE_DETAILS kpsd = new KTTU_PURCHASE_STONE_DETAILS();
                                kpsd.obj_id = objectID;
                                kpsd.company_code = pesd.CompanyCode;
                                kpsd.branch_code = pesd.BranchCode;
                                kpsd.bill_no = pesd.BillNO;
                                kpsd.sno = stoneSlNo;
                                kpsd.est_no = kped.est_no;
                                kpsd.item_sno = SlNo;
                                kpsd.type = purchase.EstNo == 0 ? "PE" : "P";
                                kpsd.name = pesd.Name;
                                kpsd.qty = pesd.Qty;
                                kpsd.carrat = pesd.Carrat;
                                kpsd.rate = pesd.Rate;
                                kpsd.amount = pesd.Amount;
                                kpsd.pur_dealer_bill_no = pesd.PurchaseDealerBillNo;
                                kpsd.gs_code = pesd.GSCode == null ? "OD" : pesd.GSCode;
                                kpsd.pur_return_no = pesd.PurReturnNo == null ? 0 : pesd.PurReturnNo;
                                kpsd.UpdateOn = SIGlobals.Globals.GetDateTime();
                                kpsd.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, pesd.CompanyCode, pesd.BranchCode);
                                kpsd.UniqRowID = Guid.NewGuid();
                                db.KTTU_PURCHASE_STONE_DETAILS.Add(kpsd);
                                stoneSlNo++;
                            }
                        }
                        #endregion
                    }
                    SlNo++;
                }
                #endregion

                if (kpem.p_type == "PE") {
                    SIGlobals.Globals.UpdateSeqenceNumber(db, MODULE_SEQ_NO, purchase.CompanyCode, purchase.BranchCode);
                }

                #region Update to Estimation Master
                // Need to update total Purchase Amount to Estimation Master if exist.
                KTTU_SALES_EST_MASTER estMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == purchase.EstNo && est.company_code == purchase.CompanyCode && est.branch_code == purchase.BranchCode).FirstOrDefault();
                if (estMaster != null) {
                    estMaster.purchase_amount = Convert.ToDecimal(purchase.TotalPurchaseAmount);
                    db.Entry(estMaster).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                db.SaveChanges();
                return kpem.est_no;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return 0;
            }
        }

        public bool UpdatePurchaseEstimation(int EstNo, PurchaseEstMasterVM purchase, out ErrorVM error)
        {
            error = null;

            if (EstNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Estimation Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            KTTU_PURCHASE_EST_MASTER kpem = new KTTU_PURCHASE_EST_MASTER();
            KSTU_CUSTOMER_MASTER customer = new CustomerBL().GetActualCustomerDetails(Convert.ToInt32(purchase.CustID), purchase.MobileNo, purchase.CompanyCode, purchase.BranchCode);
            try {
                #region Deleting Existing Estimation
                if (EstNo != 0) {
                    KTTU_PURCHASE_EST_MASTER delEstMaster = db.KTTU_PURCHASE_EST_MASTER.Where(est => est.est_no == EstNo
                                                                                           && est.company_code == purchase.CompanyCode
                                                                                           && est.branch_code == purchase.BranchCode).FirstOrDefault();
                    db.KTTU_PURCHASE_EST_MASTER.Remove(delEstMaster);

                    List<KTTU_PURCHASE_EST_DETAILS> delLstEstDet = db.KTTU_PURCHASE_EST_DETAILS.Where(est => est.est_no == EstNo
                                                                                                        && est.company_code == purchase.CompanyCode
                                                                                                        && est.branch_code == purchase.BranchCode).ToList();
                    foreach (KTTU_PURCHASE_EST_DETAILS delEst in delLstEstDet) {
                        db.KTTU_PURCHASE_EST_DETAILS.Remove(delEst);
                    }
                    List<KTTU_PURCHASE_STONE_DETAILS> delLstOfPurStoneDet = db.KTTU_PURCHASE_STONE_DETAILS.Where(est => est.est_no == EstNo
                                                                                                                    && est.company_code == purchase.CompanyCode
                                                                                                                    && est.branch_code == purchase.BranchCode).ToList();
                    foreach (KTTU_PURCHASE_STONE_DETAILS delEstStone in delLstOfPurStoneDet) {
                        db.KTTU_PURCHASE_STONE_DETAILS.Remove(delEstStone);
                    }
                }
                #endregion

                #region Saving of  Old Gold Purchase Master
                string objectID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, purchase.EstNo, purchase.CompanyCode, purchase.BranchCode);
                kpem.obj_id = objectID;
                kpem.company_code = purchase.CompanyCode;
                kpem.branch_code = purchase.BranchCode;
                kpem.est_no = EstNo;
                kpem.bill_no = purchase.BillNo;
                kpem.pur_item = purchase.PurItem;
                kpem.cust_id = customer.cust_id;
                kpem.cust_name = customer.cust_name;
                kpem.p_date = purchase.PDate;
                kpem.tax = purchase.Tax;
                kpem.today_rate = purchase.TodayRate;
                kpem.operator_code = purchase.OperatorCode;
                kpem.grand_total = purchase.GrandTotal;
                kpem.p_type = purchase.PType;
                kpem.UpdateOn = purchase.UpdateOn;
                kpem.scheme_duratation = purchase.SchemeDuration;
                kpem.total_tax_amount = purchase.TotalTaxAmount;
                kpem.total_purchase_amount = purchase.TotalPurchaseAmount;
                kpem.invoice_type = purchase.InvoiceType;
                kpem.salutation = purchase.Salutation;
                kpem.cflag = purchase.CFlag;
                kpem.ratededuct = purchase.RateDeduct;
                kpem.isPAN = customer.pan_no == null ? "N" : "Y";
                kpem.address1 = customer.address1;
                kpem.address2 = customer.address2;
                kpem.address3 = customer.address3;
                kpem.city = customer.city;
                kpem.pin_code = customer.pin_code;
                kpem.mobile_no = customer.mobile_no;
                kpem.state = customer.state;
                kpem.state_code = customer.state_code;
                kpem.tin = customer.tin;
                kpem.pan_no = customer.pan_no;
                kpem.id_type = customer.id_type;
                kpem.id_details = customer.id_details;
                kpem.booking_type = purchase.BookingType;
                kpem.phone_no = customer.phone_no;
                kpem.Email_ID = customer.Email_ID;
                db.KTTU_PURCHASE_EST_MASTER.Add(kpem);

                // If Sales Details Exist updating the purchase sales Estimation master as well.
                KTTU_SALES_EST_MASTER salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(sal => sal.company_code == purchase.CompanyCode && sal.branch_code == purchase.BranchCode && sal.est_no == purchase.EstNo).FirstOrDefault();
                if (salesEstMaster != null) {
                    salesEstMaster.purchase_amount = Convert.ToDecimal(purchase.TotalPurchaseAmount);
                    db.Entry(salesEstMaster).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                #region Saving of Old Gold Purchase Details
                int SlNo = 1;
                foreach (PurchaseEstDetailsVM ped in purchase.lstPurchaseEstDetailsVM) {
                    KTTU_PURCHASE_EST_DETAILS kped = new KTTU_PURCHASE_EST_DETAILS();
                    kped.obj_id = objectID;
                    kped.company_code = ped.CompanyCode;
                    kped.branch_code = ped.BranchCode;
                    kped.bill_no = ped.BillNo;
                    kped.est_no = kpem.est_no;
                    kped.sl_no = SlNo;
                    kped.item_name = ped.ItemName;
                    kped.item_no = ped.ItemNo;
                    kped.gwt = ped.GrossWt;
                    kped.swt = ped.StneWt;
                    kped.nwt = ped.NetWt;
                    kped.melting_percent = ped.MeltingPercent;
                    kped.melting_loss = ped.MeltingLoss;
                    kped.purchase_rate = ped.PurchaseRate;
                    kped.diamond_amount = ped.DiamondAmount;
                    kped.gold_amount = ped.GoldAmount;
                    kped.item_amount = ped.ItemAmount;
                    kped.sal_code = ped.SalCode;
                    kped.gs_code = ped.GSCode;
                    kped.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kped.purity_per = ped.PurityPercent;
                    kped.convertion_wt = ped.ConvertionWt;
                    kped.Fin_Year = SIGlobals.Globals.GetFinancialYear(db, purchase.CompanyCode, purchase.BranchCode);
                    kped.item_description = ped.ItemDescription;
                    kped.itemwise_tax_percentage = 0;
                    kped.itemwise_tax_amount = 0;
                    kped.itemwise_purchase_amount = purchase.TotalPurchaseAmount;// ped.ItemwisePurchaseAmount;
                    kped.invoice_type = ped.InvoiceType;
                    kped.ratededuct = 0;
                    kped.GSTGroupCode = ped.GSTGroupCode;
                    kped.SGST_Percent = 0;
                    kped.SGST_Amount = 0;
                    kped.CGST_Percent = 0;
                    kped.CGST_Amount = 0;
                    kped.IGST_Percent = 0;
                    kped.IGST_Amount = 0;
                    kped.HSN = Convert.ToString(db.usp_GetGSTHSNCode(ped.CompanyCode, ped.BranchCode, ped.GSCode, ped.ItemName).FirstOrDefault());
                    kped.dcts = ped.Dcts;
                    //kped.RefNo = ped.RefNo;
                    db.KTTU_PURCHASE_EST_DETAILS.Add(kped);

                    #region Saving of Old Gold Purchase Stone Details
                    if (ped.lstPurchaseEstStoneDetailsVM != null) {
                        string obj_id = objectID;
                        foreach (PurchaseEstStoneDetailsVM pesd in ped.lstPurchaseEstStoneDetailsVM) {
                            KTTU_PURCHASE_STONE_DETAILS kpsd = new KTTU_PURCHASE_STONE_DETAILS();
                            kpsd.obj_id = obj_id;
                            kpsd.company_code = pesd.CompanyCode;
                            kpsd.branch_code = pesd.BranchCode;
                            kpsd.bill_no = pesd.BillNO;
                            kpsd.sno = pesd.SlNo;
                            kpsd.est_no = kped.est_no;
                            kpsd.item_sno = pesd.ItemSlNo;
                            kpsd.type = pesd.Type;
                            kpsd.name = pesd.Name;
                            kpsd.qty = pesd.Qty;
                            kpsd.carrat = pesd.Carrat;
                            kpsd.rate = pesd.Rate;
                            kpsd.amount = pesd.Amount;
                            kpsd.pur_dealer_bill_no = pesd.PurchaseDealerBillNo;
                            kpsd.gs_code = pesd.GSCode;
                            kpsd.pur_return_no = pesd.PurReturnNo;
                            kpsd.UpdateOn = SIGlobals.Globals.GetDateTime();
                            kpsd.Fin_Year = pesd.FinYear;
                            db.KTTU_PURCHASE_STONE_DETAILS.Add(kpsd);
                        }
                    }
                    #endregion

                    SlNo = SlNo + 1;
                }
                #endregion

                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public List<AllPurchaseVM> GetAttachedEstimation(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo
                                                                                    && pay.pay_mode == "PE"
                                                                                    && pay.company_code == companyCode
                                                                                    && pay.branch_code == branchCode).ToList();
                if (payment == null || payment.Count == 0) {
                    //error = new ErrorVM() { field = "", index = 0, description = "There is no Purchase Attahement For this Estimation ", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return null;
                }


                //int purEstNo = Convert.ToInt32(payment[0].Ref_BillNo);
                //List<AllPurchaseVM> lstAllPurchse = (from kpem in db.KTTU_PURCHASE_EST_MASTER
                //                                     join kped in db.KTTU_PURCHASE_EST_DETAILS
                //                                     on kpem.est_no equals kped.est_no
                //                                     where kpem.est_no == purEstNo && kpem.company_code == companyCode && kpem.branch_code == branchCode
                //                                     group kped by new
                //                                     {
                //                                         kpem.est_no,
                //                                         kpem.total_tax_amount,
                //                                         kpem.p_date,
                //                                         kpem.cust_id,
                //                                         kpem.total_purchase_amount,
                //                                         kpem.cust_name
                //                                     } into g
                //                                     select new AllPurchaseVM
                //                                     {
                //                                         GrossWt = g.Sum(x => x.gwt),
                //                                         EstNo = g.Key.est_no,
                //                                         GSTAmount = g.Key.total_tax_amount.ToString(),
                //                                         Date = g.Key.p_date.ToString(),
                //                                         CustID = g.Key.cust_id,
                //                                         Amount = g.Key.total_purchase_amount.ToString(),
                //                                         Customer = g.Key.cust_name
                //                                     }).ToList();

                List<AllPurchaseVM> lstAllPurchse = new List<AllPurchaseVM>();
                foreach (KTTU_PAYMENT_DETAILS pay in payment) {
                    int purEstNo = Convert.ToInt32(pay.Ref_BillNo);
                    AllPurchaseVM purchase = (from kpem in db.KTTU_PURCHASE_EST_MASTER
                                              join kped in db.KTTU_PURCHASE_EST_DETAILS
                                              on kpem.est_no equals kped.est_no
                                              where kpem.est_no == purEstNo && kpem.company_code == companyCode && kpem.branch_code == branchCode
                                              group kped by new
                                              {
                                                  kpem.est_no,
                                                  kpem.total_tax_amount,
                                                  kpem.p_date,
                                                  kpem.cust_id,
                                                  kpem.total_purchase_amount,
                                                  kpem.cust_name
                                              } into g
                                              select new AllPurchaseVM
                                              {
                                                  GrossWt = g.Sum(x => x.gwt),
                                                  EstNo = g.Key.est_no,
                                                  GSTAmount = g.Key.total_tax_amount,
                                                  Date = g.Key.p_date,
                                                  CustID = g.Key.cust_id,
                                                  Amount = g.Key.total_purchase_amount,
                                                  Customer = g.Key.cust_name
                                              }).FirstOrDefault();
                    lstAllPurchse.Add(purchase);
                }
                return lstAllPurchse;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<OrderItemVM> GetOldStonePurchaseItem(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                List<KSTS_GS_ITEM_ENTRY> stone = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode && gs.branch_code == branchCode && gs.metal_type == "ST" && gs.bill_type == "P" && gs.measure_type == "C").ToList();
                List<OrderItemVM> data = (from gs in db.KSTS_GS_ITEM_ENTRY
                                          where gs.company_code == companyCode && gs.branch_code == branchCode && gs.metal_type == "ST" && gs.bill_type == "P" && gs.measure_type == "C"
                                          select new OrderItemVM()
                                          {
                                              ItemName = gs.item_level1_name,
                                              GSCode = gs.gs_code
                                          }).ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public List<OrderItemVM> GetOldDiamondPurchaseItem(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                List<KSTS_GS_ITEM_ENTRY> stone = db.KSTS_GS_ITEM_ENTRY.Where(gs => gs.company_code == companyCode && gs.branch_code == branchCode && gs.metal_type == "ST" && gs.bill_type == "P" && gs.measure_type == "C").ToList();
                List<OrderItemVM> data = (from gs in db.KSTS_GS_ITEM_ENTRY
                                          where gs.company_code == companyCode && gs.branch_code == branchCode && gs.metal_type == "DM" && gs.bill_type == "P" && gs.measure_type == "C"
                                          select new OrderItemVM()
                                          {
                                              ItemName = gs.item_level1_name,
                                              GSCode = gs.gs_code
                                          }).ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic StoneDiamondName(string companyCode, string branchCode, string type, out ErrorVM error)
        {
            error = null;
            try {
                var data = from d in db.KSTU_STONE_DIAMOND_MASTER
                           where d.company_code == companyCode && d.branch_code == branchCode && d.type == type
                           select new
                           {
                               StoneType = d.type,
                               StoneName = d.stone_name
                           };
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic StoneDiamondRate(string companyCode, string branchCode, string type, decimal karat, out ErrorVM error)
        {
            error = null;
            try {
                List<KSTU_DIAMOND_RATE_MASTER> diamondRate = db.KSTU_DIAMOND_RATE_MASTER.Where(d => d.company_code == companyCode
                                                                        && d.branch_code == branchCode
                                                                        && d.dm_name == type
                                                                        && d.object_status == "O"
                                                                        && d.karat_from < karat).ToList();
                // Logic is taken by Magna.
                if (diamondRate.Count > 0) {
                    foreach (KSTU_DIAMOND_RATE_MASTER dm in diamondRate) {
                        if (dm.karat_to >= karat) {
                            return new { OrgRate = dm.rate };
                        }
                    }
                }
                return new { OrgRate = 0 };
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public string PurchaseEstimationDotMatrixPrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(pur => pur.company_code == companyCode && pur.branch_code == branchCode && pur.est_no == estNo).FirstOrDefault();
            if (purchase == null) return "";
            List<KTTU_PURCHASE_EST_DETAILS> purchaseDet = db.KTTU_PURCHASE_EST_DETAILS.Where(pur => pur.company_code == companyCode && pur.branch_code == branchCode && pur.est_no == estNo).ToList();
            List<KTTU_PURCHASE_STONE_DETAILS> purchaseStone = db.KTTU_PURCHASE_STONE_DETAILS.Where(pur => pur.company_code == companyCode && pur.branch_code == branchCode && pur.est_no == estNo).ToList();
            try {
                StringBuilder sb = new StringBuilder();
                int width = 90;
                StringBuilder strdoubleTransLine = new StringBuilder();
                strdoubleTransLine.Append('=', width);
                StringBuilder strTransLine = new StringBuilder();
                strTransLine.Append('-', width);
                decimal Spaces = 0M;
                StringBuilder strSpaces = new StringBuilder();
                Spaces = ((width - strTransLine.Length) / 3);
                strSpaces.Append(' ', Convert.ToInt32(Spaces));
                string esc = Convert.ToChar(27).ToString();
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(64).ToString());
                sb.Append(esc + Convert.ToChar(77).ToString());
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(69).ToString());
                sb.AppendLine();

                sb.AppendLine(string.Format("{0,0}{1,30}{2,30}", purchase.cust_name.ToString(), "Purchase Estimate : " + purchase.est_no.ToString(), ""));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));
                sb.AppendLine(string.Format("{0,-20}{1,10}{2,10}{3,10}{4,11}{5,15}{6,14}", "Description", "Rate", "Gr.Wt", "St.Wt", "Net Wt", "St.Amt", "Amount"));
                sb.AppendLine(string.Format("{0,-19}{1,12}{2,10}{3,10}{4,11}{5,14}{6,14}", "", "(Rs.Ps)", "(Grams)", "(Grams)", "(Grams)", "(Rs.Ps)", "(Rs.Ps)"));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strdoubleTransLine));

                decimal Grosswt = 0;
                decimal Stonewt = 0;
                decimal Netwt = 0;
                decimal StoneAmt = 0;
                decimal Amount = 0;

                for (int i = 0; i < purchaseDet.Count; i++) {
                    string gsCode = purchaseDet[i].gs_code;
                    string itemName = purchaseDet[i].item_name;
                    ITEM_MASTER itemMaster = db.ITEM_MASTER.Where(item => item.gs_code == gsCode && item.Item_code == itemName && item.company_code == companyCode && item.branch_code == branchCode).FirstOrDefault();

                    sb.AppendLine(string.Format("{0,-18}{1,12}{2,11}{3,10}{4,12}{5,13}{6,14}", itemMaster.Item_Name.ToString(), purchaseDet[i].purchase_rate.ToString(), purchaseDet[i].gwt.ToString(), purchaseDet[i].swt.ToString(), purchaseDet[i].nwt.ToString(), purchaseDet[i].diamond_amount.ToString(), purchaseDet[i].item_amount.ToString()));
                    Grosswt += Convert.ToDecimal(purchaseDet[i].gwt);
                    Stonewt += Convert.ToDecimal(purchaseDet[i].swt);
                    Netwt += Convert.ToDecimal(purchaseDet[i].nwt);
                    StoneAmt += Convert.ToDecimal(purchaseDet[i].diamond_amount);
                    Amount += Convert.ToDecimal(purchaseDet[i].item_amount);
                }

                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                Amount = Math.Round(Amount, 0, MidpointRounding.ToEven);
                sb.AppendLine(string.Format("{0,-20}{1,-10}{2,11}{3,10}{4,12}{5,13}{6,14}", "Grand Total:", "", Grosswt, Stonewt, Netwt, StoneAmt, Amount.ToString("n", indianDefaultCulture)));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

                sb.Append(string.Format("{0,10}{1}", "Date:", purchase.p_date.ToString()));
                sb.Append(string.Format("{0,5}{1}", "#", purchase.operator_code.ToString()));
                sb.AppendLine(string.Format("{0,10}{1}", ":", purchaseDet[0].sal_code.ToString()));

                if (purchaseStone != null && purchaseStone.Count > 0) {
                    for (int j = 0; j < purchaseStone.Count; j++)
                        sb.AppendLine(string.Format("{0,-90}", j + 1 + ")" + purchaseStone[j].name + "\\" + purchaseStone[j].rate + "*" + purchaseStone[j].carrat));
                }

                sb.Append(Convert.ToChar(27).ToString() + Convert.ToChar(70).ToString());
                sb.Append(Convert.ToChar(18).ToString());
                sb.Append(esc + Convert.ToChar(64).ToString());
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetThermalPrint40Column(string companyCode, string branchCode, int estNo, bool fromSales, out ErrorVM error)
        {
            error = null;
            StringBuilder sbStart = new StringBuilder();
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', 60);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((60 - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));
            string esc = Convert.ToChar(27).ToString();

            try {
                KTTU_PURCHASE_EST_MASTER purchaseMaster = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.company_code == companyCode
                                                                                        && p.branch_code == branchCode
                                                                                        && p.est_no == estNo).FirstOrDefault();
                if (purchaseMaster == null) {
                    if (fromSales) {
                        return "";
                    }
                    else {
                        error = new ErrorVM()
                        {
                            description = "Invalid Purchase Estimaion Number",
                            ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                        };
                        return "";
                    }
                }
                List<KTTU_PURCHASE_EST_DETAILS> purchaseDet = db.KTTU_PURCHASE_EST_DETAILS.Where(p => p.company_code == companyCode
                                                                                                && p.branch_code == branchCode
                                                                                                && p.est_no == estNo).ToList();


                sbStart.AppendLine("<Table  font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse; width: 366px;\" >");  //FRAME=BOX RULES=NONE
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD  style=\"border-right:thin; border-bottom:thin; border-left:thin\" colspan=7 ALIGN = \"CENTER\"><b>PURCHASE ESTIMATION <br></b></TD>"));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=7 ALIGN = \"LEFT\"><b>Est. No.: {0}<br></b></TD>", purchaseMaster.est_no));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TH  style=\"border-top:0; border-right:thin; border-bottom:thin solid; border-left:thin\" colspan=7 ALIGN = \"LEFT\"><b>Name: {0}<br></b></TD>", purchaseMaster.cust_name));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor= WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Description</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Rate</b></TH>");
                sbStart.AppendLine("<TH colspan = 3 ALIGN = \"CENTER\" width=\"18%\" ><b>[Weight in Grams]</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b>Stone</b></TH>");
                sbStart.AppendLine("<TH style=\"border-bottom:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR bgcolor=WHITE style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin \" ALIGN = \"CENTER\"  ><b></b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid; border-top:thin\" ALIGN = \"CENTER\"><b></b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Gr.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>St.Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid\" ALIGN = \"CENTER\"><b>Net Wt</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("<TH style=\" border-bottom:thin solid ; border-top:thin\" ALIGN = \"CENTER\"><b>Amount</b></TH>");
                sbStart.AppendLine("</TR>");

                  

                decimal purGrosswt = 0;
                decimal purStonewt = 0;
                decimal purNetwt = 0;
                decimal purStoneAmt = 0;
                decimal purAmount = 0;
                int MaxPageRows = 2;
                string salesmanCode = purchaseMaster.operator_code;
                for (int i = 0; i < purchaseDet.Count; i++) {
                    sbStart.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"LEFT\" ><b>{0}</b></TD>", SIGlobals.Globals.GetAliasName(db, purchaseDet[i].gs_code, purchaseDet[i].item_name, companyCode, branchCode)));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", purchaseDet[i].purchase_rate));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", purchaseDet[i].gwt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", purchaseDet[i].swt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", purchaseDet[i].nwt));
                    decimal dmd_amt = Convert.ToDecimal(purchaseDet[i].diamond_amount.ToString());
                    decimal Rounddmd_amt = Math.Round(dmd_amt);
                    decimal itemm_amt = Convert.ToDecimal(purchaseDet[i].item_amount);
                    decimal Rounditemm_amt = Math.Round(itemm_amt);
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Rounddmd_amt));
                    sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; border-top:thin \" ALIGN = \"RIGHT\"><b>{0}</b></TD>", Rounditemm_amt));
                    sbStart.AppendLine("</TR>");
                    salesmanCode = purchaseDet[i].sal_code;
                    purGrosswt += Convert.ToDecimal(purchaseDet[i].gwt);
                    purStonewt += Convert.ToDecimal(purchaseDet[i].swt);
                    purNetwt += Convert.ToDecimal(purchaseDet[i].nwt);
                    purStoneAmt += Convert.ToDecimal(purchaseDet[i].diamond_amount);
                    purAmount += Convert.ToDecimal(purchaseDet[i].item_amount);
                }


                for (int j = 0; j < MaxPageRows; j++) {
                    sbStart.AppendLine("<TR>");
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\" ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine(string.Format("<TD style=\"border-top:thin; border-bottom:thin\"  ALIGN = \"RIGHT\"><b>{0} </b></TD>", "&nbsp"));
                    sbStart.AppendLine("</TR>");
                }
                purAmount = Math.Round(purAmount, 2, MidpointRounding.ToEven);
                sbStart.AppendLine("<TR  bgcolor=WHITE  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sbStart.AppendLine("<TH  style=\"border-top:thin solid; border-bottom:thin solid\" colspan=2 ALIGN = \"LEFT\"><b>Totals</b></TH>");
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", purGrosswt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", purStonewt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", purNetwt));
                decimal RoundpurStoneAmt = Math.Round(purStoneAmt);
                decimal RoundpurAmount = Math.Round(purAmount);
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", RoundpurStoneAmt));
                sbStart.AppendLine(string.Format("<TH style=\"border-top:thin solid; border-bottom:thin solid\" ALIGN = \"RIGHT\"><b>{0}</b></TH>", RoundpurAmount));
                sbStart.AppendLine("</TR>");

                List<KTTU_PURCHASE_STONE_DETAILS> purStone = db.KTTU_PURCHASE_STONE_DETAILS.Where(st => st.company_code == companyCode
                                                                                                    && st.branch_code == branchCode
                                                                                                    && st.est_no == estNo).ToList();
                if (purStone != null && purStone.Count > 0) {
                    for (int j = 0; j < purStone.Count; j++) {
                        sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan =  7 ALIGN = \"LEFT\"><b>{0}) {1}\\{2}*{3}</b></TD>", j + 1, purStone[j].name, purStone[j].rate, purStone[j].carrat));
                    }
                }
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin; \" colspan =  7 ALIGN = \"LEFT\"><b>Date :{0}</b></TD>", purchaseMaster.p_date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture)));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("<TR>");
                sbStart.AppendLine(string.Format("<TD style=\"border-bottom:thin solid; \" colspan =  7 ALIGN = \"LEFT\"><b>OPR: {0} / Sal: {1}</b></TD>", purchaseMaster.operator_code, salesmanCode));
                sbStart.AppendLine("</TR>");
                sbStart.AppendLine("</TABLE>");
                return sbStart.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public ProdigyPrintVM GetOldPurchaseEstimatePrint(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            PrintConfiguration pc = new PrintConfiguration();
            var printConfig = pc.GetPrintConfigurationForDraftDocuments(companyCode, branchCode, "PUR_EST");
            if (printConfig == "HTML") {
                var htmlPrintData = GetThermalPrint40Column(companyCode, branchCode, estNo, false, out error);
                printObject.ContinueNextPrint = false;
                printObject.Data = new PrintConfiguration().Base64Encode(htmlPrintData);
                printObject.PrintType = "HTML";
            }
            else {
                var dotMatrixPrintData = PurchaseEstimationDotMatrixPrint(companyCode, branchCode, estNo, out error);
                printObject.ContinueNextPrint = true;
                printObject.Data = dotMatrixPrintData;
                printObject.PrintType = "RAW";
            }

            return printObject;
        }
        #endregion

        #region Attachment
        public IQueryable<AllPurchaseVM> GetAllPurchaseEstimations(string companyCode, string branchCode)
        {


            #region Old Working code
            //List<AllPurchaseVM> lstOfAllPurchase = new List<AllPurchaseVM>();
            //DataTable tblData = SIGlobals.Globals.ExecuteQuery("SELECT m.est_no AS [EstNo], " +
            //"        m.cust_name AS[Customer], " +
            //"        CONVERT(VARCHAR, m.p_date, 103) AS[Date], " +
            //"        m.total_purchase_amount AS[Amount], " +
            //"        SUM(d.gwt) AS[Gross Wt], " +
            //"        SUM(d.item_no) AS[Qty], " +
            //"        Cust_id AS[CustId], " +
            //"        m.total_tax_amount AS GSTAmount" +
            //" FROM KTTU_PURCHASE_EST_MASTER m, " +
            //"      KTTU_PURCHASE_EST_DETAILS d" +
            //" WHERE m.bill_no = 0" +
            //"       AND m.total_purchase_amount > 0" +
            //"       AND m.est_no = d.est_no" +
            //"       AND m.est_no NOT IN" +
            //" (" +
            //"     SELECT Ref_BillNo" +
            //"     FROM KTTU_PAYMENT_DETAILS" +
            //"     WHERE trans_type = 'A'" +
            //" )" +
            //"       AND m.est_no NOT IN" +
            //" (" +
            //"     SELECT est_no" +
            //"     FROM kttu_sales_est_master" +
            //" )" +
            //"       AND m.est_no NOT IN" +
            //" (" +
            //"     SELECT est_no" +
            //"     FROM kttu_purchase_est_master_test" +
            //" )" +
            //" GROUP BY m.est_no," +
            //"          m.cust_name," +
            //"          CONVERT(VARCHAR, m.p_date, 103)," +
            //"          m.total_purchase_amount," +
            //"          Cust_id," +
            //"          m.total_tax_amount" +
            //" ORDER BY m.est_no; ");


            //for (int i = 0; i < tblData.Rows.Count; i++) {
            //    AllPurchaseVM purchase = new AllPurchaseVM();
            //    purchase.EstNo = Convert.ToInt32(tblData.Rows[i]["EstNo"]);
            //    purchase.GrossWt = Convert.ToDecimal(tblData.Rows[i]["Gross Wt"]);
            //    purchase.GSTAmount = tblData.Rows[i]["GSTAmount"] == DBNull.Value ? "" : Convert.ToString(tblData.Rows[i]["GSTAmount"]);
            //    purchase.Qty = Convert.ToInt32(tblData.Rows[i]["Qty"]);
            //    purchase.Date = Convert.ToString(tblData.Rows[i]["Date"]);
            //    purchase.CustID = Convert.ToInt32(tblData.Rows[i]["CustID"]);
            //    purchase.Amount = Convert.ToString(tblData.Rows[i]["Amount"]);
            //    purchase.Customer = Convert.ToString(tblData.Rows[i]["Customer"]);
            //    lstOfAllPurchase.Add(purchase);
            //}
            //return lstOfAllPurchase.AsQueryable();
            #endregion

            var result = (from pem in db.KTTU_PURCHASE_EST_MASTER
                          join ped in db.KTTU_PURCHASE_EST_DETAILS
                          on new { CompanCode = pem.company_code, BranchCode = pem.branch_code, EstNo = pem.est_no }
                          equals new { CompanCode = ped.company_code, BranchCode = ped.branch_code, EstNo = ped.est_no }
                          where pem.bill_no == 0 && pem.total_purchase_amount > 0 && pem.company_code == companyCode && pem.branch_code == branchCode
                          group new { pem, ped } by new
                          {
                              pem.est_no,
                              pem.total_tax_amount,
                              pem.p_date,
                              pem.cust_id,
                              pem.total_purchase_amount,
                              pem.cust_name
                          } into g
                          select new AllPurchaseVM()
                          {
                              EstNo = g.Key.est_no,
                              Customer = g.Key.cust_name,
                              Date = g.Key.p_date,
                              Amount = g.Key.total_purchase_amount,
                              GrossWt = g.Sum(x => x.ped.gwt),
                              Qty = g.Sum(x => x.ped.item_no),
                              CustID = g.Key.cust_id,
                              GSTAmount = g.Key.total_tax_amount
                          });
            var paymentDet = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.trans_type == "A" && pay.company_code == companyCode && pay.branch_code == branchCode);
            var salesEstMaster = db.KTTU_SALES_EST_MASTER.Where(sem => sem.company_code == companyCode && sem.branch_code == branchCode);
            //TODO: Let's have look at it later
            //var purchaseEstMaster = db.KTTU_PURCHASE_EST_MASTER_TEST.Where(p => p.company_code == companyCode && p.branch_code == branchCode);
            //var purchaseEstMaster = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.company_code == companyCode && p.branch_code == branchCode);
            var finalResult = (from r in result
                               where !paymentDet.Any(pay => pay.Ref_BillNo == r.EstNo.ToString())
                               && !salesEstMaster.Any(s => s.est_no == r.EstNo)
                               //&& !purchaseEstMaster.Any(p => p.est_no == r.EstNo)
                               select new AllPurchaseVM()
                               {
                                   EstNo = r.EstNo,
                                   Customer = r.Customer,
                                   Date = r.Date,
                                   Amount = r.Amount,
                                   GrossWt = r.GrossWt,
                                   Qty = r.Qty,
                                   CustID = r.CustID,
                                   GSTAmount = r.GSTAmount
                               });
            return finalResult.AsQueryable();
        }

        public int GetAllOrdersCount(string companyCode, string branchCode)
        {
            List<AllPurchaseVM> lstAllPurchase = GetAllPurchaseEstimations(companyCode, branchCode).ToList();
            return lstAllPurchase.Count();
        }

        public List<SearchParamVM> GetAllSearchParams()
        {
            List<SearchParamVM> lstSearchParams = new List<SearchParamVM>();
            lstSearchParams.Add(new SearchParamVM() { Key = "ESTNO", Value = "Est No" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CUSTOMER", Value = "Customer" });
            lstSearchParams.Add(new SearchParamVM() { Key = "DATE", Value = "Date" });
            lstSearchParams.Add(new SearchParamVM() { Key = "AMOUNT", Value = "Amount" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GROSSWT", Value = "Gross Wt" });
            lstSearchParams.Add(new SearchParamVM() { Key = "QTY", Value = "Quantity" });
            lstSearchParams.Add(new SearchParamVM() { Key = "CUSTID", Value = "Customer ID" });
            lstSearchParams.Add(new SearchParamVM() { Key = "GSTAMOUNT", Value = "GST Amount" });
            return lstSearchParams;
        }

        public IQueryable<AllPurchaseVM> GetAllSearchResult(string companyCode, string branchCode, string searchType, string searchValue)
        {
            List<AllPurchaseVM> lstAllPurchaseVM = GetAllPurchaseEstimations(companyCode, branchCode).ToList();
            switch (searchType.ToUpper()) {
                case "ESTNO":
                    return lstAllPurchaseVM.Where(search => search.EstNo == Convert.ToInt32(searchValue)).AsQueryable<AllPurchaseVM>();
                case "CUSTOMER":
                    return lstAllPurchaseVM.Where(search => search.Customer.Contains(searchValue)).AsQueryable<AllPurchaseVM>();
                case "DATE":
                //return lstAllPurchaseVM.Where(search => search.Date == Convert.ToString(searchValue)).AsQueryable<AllPurchaseVM>();
                case "GROSSWT":
                    return lstAllPurchaseVM.Where(search => search.GrossWt == Convert.ToDecimal(searchValue)).AsQueryable<AllPurchaseVM>();
                case "AMOUNT":
                //return lstAllPurchaseVM.Where(search => search.Amount == Convert.ToString(searchValue)).AsQueryable<AllPurchaseVM>();
                case "QTY":
                    return lstAllPurchaseVM.Where(search => search.Qty == Convert.ToDecimal(searchValue)).AsQueryable<AllPurchaseVM>();
                case "CUSTID":
                    return lstAllPurchaseVM.Where(search => search.CustID == Convert.ToDecimal(searchValue)).AsQueryable<AllPurchaseVM>();
                case "GSTAMOUNT":
                    //return lstAllPurchaseVM.Where(search => search.GSTAmount == Convert.ToString(searchValue)).AsQueryable<AllPurchaseVM>();
                    break;
            }
            return lstAllPurchaseVM.AsQueryable<AllPurchaseVM>();
        }

        public List<PurchaseEstMasterVM> GetAttachedOldGoldPurchase(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            try {
                List<KTTU_PAYMENT_DETAILS> payment = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.series_no == estNo && pay.pay_mode == "PE" && pay.trans_type == "A").ToList();
                List<PurchaseEstMasterVM> lstAllOldGoldPurchase = new List<PurchaseEstMasterVM>();
                foreach (KTTU_PAYMENT_DETAILS payDet in payment) {
                    PurchaseEstMasterVM om = new PurchaseEstMasterVM();
                    om = new OldGoldPurchaseBL().GetPurchaseEstimationDetails(companyCode, branchCode, estNo, out error);
                    lstAllOldGoldPurchase.Add(om);
                }
                return lstAllOldGoldPurchase;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool AttachOldGoldPurchaseEstimation(List<PaymentVM> payment, out ErrorVM error)
        {
            error = null;
            if (payment == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Old Gold Purchase Details",
                    field = "",
                    index = 0,
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }

            try {
                int estNo = payment[0].SeriesNo;
                string companyCode = payment[0].CompanyCode;
                string branchCode = payment[0].BranchCode;

                #region Validate Underlying Sales Estimation
                if (estNo == 0) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Purchase Estimation Number",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                KTTU_SALES_EST_MASTER salEstimation = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo
                            && est.company_code == companyCode
                            && est.branch_code == branchCode).FirstOrDefault();
                if (salEstimation == null) {
                    error = new ErrorVM()
                    {
                        description = "The underlying sales estimation does not exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                if (salEstimation.bill_no != 0) {
                    error = new ErrorVM()
                    {
                        description = "The Sales Estimation is billed already. The bill number is :" + salEstimation.bill_no.ToString(),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                #endregion

                #region Remove already attached purchases
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                            && est.pay_mode == "PE"
                                                                            && est.trans_type == "A"
                                                                            && est.company_code == companyCode
                                                                            && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }
                #endregion

                #region Get the Serial No. of the payments
                var payDetailSlNo = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                           && est.trans_type == "A"
                                                                           && est.company_code == companyCode
                                                                           && est.branch_code == branchCode).Select(x => x.sno).DefaultIfEmpty(0).Max();
                int paySlNo = 1;
                if (payDetailSlNo > 0) {
                    paySlNo = payDetailSlNo + 1;
                }
                #endregion

                #region Saving

                string objID = salEstimation.obj_id;
                decimal totalAttachedPurchaseAmount = 0;
                foreach (PaymentVM pay in payment) {
                    int oldGoldPurchaseEstNo = Convert.ToInt32(pay.RefBillNo);
                    KTTU_PURCHASE_EST_MASTER purchase = db.KTTU_PURCHASE_EST_MASTER.Where(p => p.est_no == oldGoldPurchaseEstNo && p.company_code == companyCode && p.branch_code == branchCode).FirstOrDefault();
                    if (purchase == null) {
                        error = new ErrorVM()
                        {
                            description = "Invalid Old Gold Purchase Estimation. Purchase estimation does not exist.",
                            field = "",
                            index = 0,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (purchase.bill_no != 0) {
                        error = new ErrorVM()
                        {
                            description = "Old Gold Purchase Estimation is billed already. The bill number is :" + purchase.bill_no.ToString(),
                            field = "",
                            index = 0,
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    KTTU_PAYMENT_DETAILS kpd = new KTTU_PAYMENT_DETAILS();
                    kpd.obj_id = objID;
                    kpd.company_code = companyCode;
                    kpd.branch_code = branchCode;
                    kpd.series_no = pay.SeriesNo; // Purchase Estimation Number
                    kpd.receipt_no = 0;
                    kpd.sno = paySlNo;
                    kpd.trans_type = "A";
                    kpd.pay_mode = "PE";
                    kpd.pay_details = "";
                    kpd.pay_date = purchase.p_date;
                    kpd.pay_amt = purchase.grand_total;
                    kpd.Ref_BillNo = pay.RefBillNo; // Old Gold Purchase EstNo (oldGoldPurchaseEstNo)
                    kpd.party_code = null;
                    kpd.bill_counter = null;
                    kpd.is_paid = "Y";
                    kpd.bank = "";
                    kpd.bank_name = "";
                    kpd.cheque_date = pay.ChequeDate;
                    kpd.card_type = "";
                    kpd.expiry_date = pay.ExpiryDate;
                    kpd.cflag = "N";
                    kpd.card_app_no = "";
                    kpd.scheme_code = null;
                    kpd.sal_bill_type = null;
                    kpd.operator_code = pay.OperatorCode; // Need to send from JSON
                    kpd.session_no = 0;
                    kpd.UpdateOn = SIGlobals.Globals.GetDateTime();
                    kpd.group_code = null;
                    kpd.amt_received = 0;
                    kpd.bonus_amt = 0;
                    kpd.win_amt = 0;
                    kpd.ct_branch = null;
                    kpd.fin_year = 0;
                    kpd.CardCharges = 0;
                    kpd.cheque_no = "0";
                    kpd.New_Bill_No = pay.SeriesNo.ToString();
                    kpd.Add_disc = Convert.ToDecimal(0.00);
                    kpd.isOrdermanual = "N";
                    kpd.currency_value = Convert.ToDecimal(0.00);
                    kpd.exchange_rate = Convert.ToDecimal(0.00);
                    kpd.currency_type = null;
                    kpd.tax_percentage = Convert.ToDecimal(0.00);
                    kpd.cancelled_by = "";
                    kpd.cancelled_remarks = "";
                    kpd.cancelled_date = DateTime.MinValue.ToString();
                    kpd.isExchange = "N";
                    kpd.exchangeNo = 0;
                    kpd.new_receipt_no = "0";
                    kpd.Gift_Amount = Convert.ToDecimal(0.00);
                    kpd.cardSwipedBy = "";
                    kpd.version = 0;
                    kpd.GSTGroupCode = null;
                    kpd.SGST_Percent = Convert.ToDecimal(0.00);
                    kpd.CGST_Percent = Convert.ToDecimal(0.00);
                    kpd.IGST_Percent = Convert.ToDecimal(0.00);
                    kpd.HSN = null;
                    kpd.SGST_Amount = Convert.ToDecimal(0.00);
                    kpd.CGST_Amount = Convert.ToDecimal(0.00);
                    kpd.IGST_Amount = Convert.ToDecimal(0.00);
                    kpd.pay_amount_before_tax = purchase.total_purchase_amount;
                    kpd.pay_tax_amount = Convert.ToDecimal(0.00);
                    kpd.UniqRowID = Guid.NewGuid();
                    db.KTTU_PAYMENT_DETAILS.Add(kpd);

                    // Calculating total Purchase Amount from Estimation.
                    totalAttachedPurchaseAmount = totalAttachedPurchaseAmount + Convert.ToDecimal(purchase.total_purchase_amount);
                    paySlNo = paySlNo + 1;
                }

                #endregion

                #region Update to Estimation Master
                // Need to update total Purchase Amount to Estimation Master if exist.
                KTTU_SALES_EST_MASTER estMaster = db.KTTU_SALES_EST_MASTER.Where(est => est.est_no == estNo &&
                                                                                 est.company_code == companyCode
                                                                                 && est.branch_code == branchCode).FirstOrDefault();
                if (estMaster != null) {
                    estMaster.purchase_amount = estMaster.purchase_amount + totalAttachedPurchaseAmount;
                    db.Entry(estMaster).State = System.Data.Entity.EntityState.Modified;
                }
                #endregion

                db.SaveChanges();

                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool RemoveOldGoldAttachement(string companyCode, string branchCode, int estNo, out ErrorVM error)
        {
            error = null;
            if (estNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Purchase Estimation",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }
            try {
                List<KTTU_PAYMENT_DETAILS> paymentDel = db.KTTU_PAYMENT_DETAILS.Where(est => est.series_no == estNo
                                                                                        && est.pay_mode == "PE"
                                                                                        && est.trans_type == "A"
                                                                                        && est.company_code == companyCode
                                                                                        && est.branch_code == branchCode).ToList();
                foreach (KTTU_PAYMENT_DETAILS pay in paymentDel) {
                    db.KTTU_PAYMENT_DETAILS.Remove(pay);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Public Methods
        public bool ValidationPurchaseEstimation(int estNo, string companyCode, string branchCode, string type, int refNo, out ErrorVM error)
        {
            // Alternate for this [dbo].[usp_ValidatePurchaseEstNo] Procedure
            error = null;
            if (estNo > 0) {
                string strEstNo = Convert.ToString(estNo);
                KTTU_PURCHASE_MASTER purchaseMaster = db.KTTU_PURCHASE_MASTER.Where(p => p.est_no == estNo
                                                                                    && p.company_code == companyCode
                                                                                    && p.branch_code == branchCode).FirstOrDefault();
                if (purchaseMaster == null) {
                    error.description = string.Format("Purchase Est. No: {0} is invalid.", estNo);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (purchaseMaster.bill_no != 0) {
                    error.description = string.Format("'Purchase Est. No: {0} is already billed. Bill No:{0} ", estNo, purchaseMaster.bill_no);
                    error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                    return false;
                }

                if (type == "PE") {
                    var payments = db.KTTU_PAYMENT_DETAILS.Where(pay => pay.Ref_BillNo == strEstNo
                                                                           && pay.company_code == companyCode
                                                                           && pay.branch_code == branchCode
                                                                           && pay.trans_type == "A"
                                                                           && pay.pay_mode == "PE"
                                                                           && pay.cflag != "Y").FirstOrDefault();
                    if (payments.series_no != 0 && purchaseMaster.est_no != refNo) {
                        error.description = string.Format("'Purchase Est. No: {0} is already is already adjusted towards Sales Est. No:{1} ", estNo, payments.series_no);
                        error.ErrorStatusCode = System.Net.HttpStatusCode.NotFound;
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion
    }
}
