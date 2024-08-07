using Newtonsoft.Json;
using ProdigyAPI.BL.BusinessLayer.APIHandler;
using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.BL.ViewModel.Receipts;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Receipts
{
    public class BarcodeReceiptBL
    {
        MagnaDbEntities db = null;

        public BarcodeReceiptBL()
        {
            db = new MagnaDbEntities(true);
        }

        public BarcodeReceiptBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public dynamic GetReceiptFromList(string companyCode, string branchCode)
        {
            var partyList = db.KSTU_SUPPLIER_MASTER
                .Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.voucher_code == "VB" && x.obj_status != "C")
                .OrderBy(y => y.party_name)
                .Select(z => new { Code = z.party_code, Name = z.party_name }).ToList();
            return partyList;
        }

        public bool GetIssueFromAPI(string companyCode, string branchCode, string issueBranchCode, int issueNo, out BarcodeIssueHeaderOutputVM barcodeIssueOutput, out ErrorVM error)
        {
            error = null;
            barcodeIssueOutput = new BarcodeIssueHeaderOutputVM();
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            try {
                string methodPath = string.Format("api/stock-transfers/barcode-issue/get");
                string query = string.Format("requestCompanyCode={0}&requestBranchCode={1}&issuedBranchCode={2}&issueNo={3}",
                    companyCode, branchCode, issueBranchCode, issueNo);
                var methodType = HttpMethod.Get;
                using (var client = new HttpClient()) {
                    var request = requestHandler.BuildHttpRequest(methodPath, query, null, methodType, "PRODIGYC");
                    var response = client.SendAsync(request).Result;
                    var responseData = response.Content.ReadAsStringAsync().Result;
                    if (response.IsSuccessStatusCode) {
                        object parsedJson = JsonConvert.DeserializeObject(responseData);
                        var settings = new JsonSerializerSettings
                        {
                            NullValueHandling = NullValueHandling.Ignore,
                            MissingMemberHandling = MissingMemberHandling.Ignore
                        };
                        barcodeIssueOutput = JsonConvert.DeserializeObject<BarcodeIssueHeaderOutputVM>(parsedJson.ToString(), settings);
                    }
                    else {
                        error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Request issue is not found." };
                        return false;
                    }
                }
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }

        public bool GetIssueDetails(string companyCode, string branchCode, string issueBranchCode, int issueNo, out BarcodeReceiptVM barcodeReceiptVM, out ErrorVM error)
        {
            barcodeReceiptVM = new BarcodeReceiptVM();
            error = null;
            BarcodeIssueHeaderOutputVM barcodeIssueOutput = new BarcodeIssueHeaderOutputVM();
            HttpRequestHandler requestHandler = new HttpRequestHandler();
            try {
                var result = GetIssueFromAPI(companyCode, branchCode, issueBranchCode, issueNo, out barcodeIssueOutput, out error);
                if (!result)
                    return false;
                barcodeReceiptVM = TransferBarcodeReceiptFields(barcodeIssueOutput);
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }

        private BarcodeReceiptVM TransferBarcodeReceiptFields(BarcodeIssueHeaderOutputVM barcodeIssueOutput)
        {
            try {
                var receiptDetails = new List<BarcodeReceiptDetailVM>();
                var issueLines = barcodeIssueOutput.BarcodeIssueLines.OrderBy(s => s.sno);
                foreach (var x in issueLines) {
                    List<BarcodeReceiptStoneDetailVM> bsdList = new List<BarcodeReceiptStoneDetailVM>();
                    if (barcodeIssueOutput.BarcodeStoneDetails != null) {
                        var barcodeStone = barcodeIssueOutput.BarcodeStoneDetails.Where(bs => bs.barcode_no == x.barcode_no).ToList();
                        var barStone = barcodeStone.OrderBy(bs => bs.sl_no);
                        foreach (var item in barStone) {
                            var bsd = new BarcodeReceiptStoneDetailVM
                            {
                                BarcodeNo = item.barcode_no,
                                SlNo = item.sl_no,
                                Type = item.type,
                                Name = item.name,
                                Qty = item.qty,
                                Rate = item.rate,
                                Carrat = item.carrat,
                                Amount = item.amount
                            };
                            bsdList.Add(bsd);
                        }
                    }

                    int SlNo = 1;
                    BarcodeReceiptDetailVM rd = new BarcodeReceiptDetailVM
                    {
                        SlNo = x.sno,
                        ItemCode = x.item_name,
                        GSCode = x.gs_code,
                        CounterCode = x.counter_code,
                        BarcodeNo = x.barcode_no,
                        Qty = x.quantity,
                        GrossWt = x.gwt,
                        StoneWt = x.swt,
                        NetWt = x.nwt,
                        Dcts = Convert.ToDecimal(x.dcts),
                        Amount = Convert.ToDecimal(x.amount),
                        PurityPercent = Convert.ToDecimal(x.purity_perc),
                        Rate = Convert.ToDecimal(x.rate),
                        PureWeight = Convert.ToDecimal(x.pure_wt),
                        BarcodeReceiptStoneDetails = bsdList
                    };
                    receiptDetails.Add(rd);
                    SlNo++;
                }
                if (barcodeIssueOutput == null)
                    return null;

                BarcodeReceiptVM br = new BarcodeReceiptVM
                {
                    CompanyCode = barcodeIssueOutput.company_code,
                    BranchCode = barcodeIssueOutput.issue_to,
                    IssueNo = barcodeIssueOutput.issue_no,
                    IssueDate = barcodeIssueOutput.issue_date,
                    IssueBranchCode = barcodeIssueOutput.branch_code,
                    TotalLineCount = receiptDetails.Count(),
                    TotalQty = receiptDetails.Sum(r => r.Qty),
                    TotalGrossWt = receiptDetails.Sum(r => r.GrossWt),
                    TotalStoneWt = receiptDetails.Sum(r => r.StoneWt),
                    TotalNetWt = receiptDetails.Sum(r => r.NetWt),
                    TotalDcts = receiptDetails.Sum(r => r.Dcts),
                    TotalAmount = receiptDetails.Sum(r => r.Amount),
                    ReceiptDetails = receiptDetails,
                    ScannedBarcodes = new List<BarcodeReceiptDetailVM>()
                };

                return br;
            }
            catch (Exception) {
                throw;
            }
        }

        public bool PostReceipt(BarcodeReceiptVM barcodeReceiptVM, string userID, bool isBarcodeReceipt, out int receiptNo, out ErrorVM error)
        {
            error = null;
            receiptNo = 0;
            if (barcodeReceiptVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is nothing to post." };
                return false;
            }
            var recMastster = db.KTTU_BARCODED_RECEIPT_MASTER.Where(x => x.company_code == barcodeReceiptVM.CompanyCode
                && x.branch_code == barcodeReceiptVM.IssueBranchCode && x.issue_no == barcodeReceiptVM.IssueNo
                && x.Transfer_Type == "RB" && x.cflag != "Y").FirstOrDefault();
            if (recMastster != null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue has been already received. The receipt No. is " + recMastster.receipt_no.ToString() };
                return false;
            }
            if (barcodeReceiptVM.ReceiptDetails == null || barcodeReceiptVM.ReceiptDetails.Count <= 0) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Receipt line details are not found." };
                return false;
            }

            if (isBarcodeReceipt == true) {
                var recLinesWithoutBarcodes = barcodeReceiptVM.ReceiptDetails.Where(rl => rl.BarcodeNo == null
                    || rl.BarcodeNo == "").ToList();
                if (recLinesWithoutBarcodes != null && recLinesWithoutBarcodes.Count > 0) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Barcode No. is required in all the lines." };
                    return false;
                }
                if (barcodeReceiptVM.ScannedBarcodes == null || barcodeReceiptVM.ScannedBarcodes.Count <= 0) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Scanned barcode data is not found." };
                    return false;
                }

                #region Tried different ways to get items from one list which are not there in another
                ////Only one solution worked.
                //var issueLines = barcodeReceiptVM.ReceiptDetails.Select(p => new BarcodeList { BarcodeNo = p.BarcodeNo }).ToList();
                //var scannedLines = barcodeReceiptVM.ScannedBarcodes.Select(p => new BarcodeList { BarcodeNo = p.BarcodeNo }).ToList();
                //// Solution #1 didn't work
                //var unScanned = from rd in issueLines
                //                where !scannedLines.Contains(rd)
                //                select rd;
                //var ml = unScanned.ToList();
                //// Solution #2 didn't work
                //var missingList = issueLines.Where(item => !scannedLines.Contains(item)).ToList();
                //// Solution #3 didn't work
                //var ml1 = issueLines.Except(scannedLines).ToList(); ;
                //List<BarcodeList> bc = new List<BarcodeList>();
                //foreach (var b in ml1)
                //    bc.Add(b); 
                #endregion

                //This has worked! Credit: https://stackoverflow.com/questions/3944803/use-linq-to-get-items-in-one-list-that-are-not-in-another-list
                var missingList = barcodeReceiptVM.ReceiptDetails
                    .Where(p => !barcodeReceiptVM.ScannedBarcodes.Any(p2 => p2.BarcodeNo == p.BarcodeNo))
                    .ToList();

                if (missingList != null && missingList.Count() > 0) {
                    string missingBarcodes = string.Join(",  ", missingList.Select(x => x.BarcodeNo).ToList());
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "All barcodes are not scanned. Please scan the pending barcodes and try again. " + missingBarcodes };
                    return false;
                }
            }
            else {
                //For non tag receipts, a value must be specified for PureWeight, PurityPercent & Rate
                var valAttrCheck = barcodeReceiptVM.ReceiptDetails.Where(vac => vac.PureWeight == 0
                    || vac.PurityPercent == 0 || vac.Rate == 0).ToList();
                if (valAttrCheck != null) {
                    if (valAttrCheck.Count > 0) {
                        error = new ErrorVM { description = "The attributes PureWeight, PurityPercent & Rate should have some value.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                        return false;
                    }
                }
            }

            BarcodeIssueHeaderOutputVM barcodeIssueOutput = null;
            bool functionResult = GetIssueFromAPI(barcodeReceiptVM.CompanyCode, barcodeReceiptVM.BranchCode, barcodeReceiptVM.IssueBranchCode,
                barcodeReceiptVM.IssueNo, out barcodeIssueOutput, out error);
            if (!functionResult) {
                return false;
            }

            functionResult = SaveReceipt(barcodeIssueOutput, userID, out receiptNo, out error);
            if (!functionResult) {
                return false;
            }

            return true;
        }

        private bool SaveReceipt(BarcodeIssueHeaderOutputVM barcodeIssueOutput, string userID, out int receiptNo, out ErrorVM error)
        {
            error = null;
            receiptNo = 0;
            try {
                string companyCode = barcodeIssueOutput.company_code;
                string branchCode = barcodeIssueOutput.issue_to;
                var finYear = SIGlobals.Globals.GetFinancialYear(db, companyCode, branchCode);
                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;

                #region Receipt Header
                receiptNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "29", true);
                string receiptMasterObjID = SIGlobals.Globals.GetMagnaGUID("KTTU_BARCODED_RECEIPT_MASTER", receiptNo, companyCode, branchCode);

                KTTU_BARCODED_RECEIPT_MASTER rm = new KTTU_BARCODED_RECEIPT_MASTER();
                var receiptHeader = barcodeIssueOutput;
                rm.obj_id = receiptMasterObjID;
                rm.company_code = companyCode;
                rm.branch_code = branchCode;
                rm.receipt_no = receiptNo;
                rm.issue_no = receiptHeader.issue_no;
                rm.receipt_type = "RB";
                rm.receipt_date = applicationDate;
                rm.received_from_branch = receiptHeader.branch_code;
                rm.received_from = receiptHeader.branch_code;
                rm.gs_type = receiptHeader.gs_type;
                rm.sal_code = receiptHeader.sal_code;
                rm.operator_code = userID;
                rm.obj_status = "O";
                rm.cflag = "N";
                rm.cancelled_by = "";
                rm.UpdateOn = updatedTimestamp;
                rm.total_amount = barcodeIssueOutput.BarcodeIssueLines.Sum(x => x.amount);
                rm.tolerance_percent = receiptHeader.tolerance_percent;
                rm.final_amount = barcodeIssueOutput.BarcodeIssueLines.Sum(x => x.amount) + Convert.ToDecimal(receiptHeader.CGST_Amount) + Convert.ToDecimal(receiptHeader.SGST_Amount) + Convert.ToDecimal(receiptHeader.IGST_Amount);
                rm.ShiftID = receiptHeader.ShiftID;
                rm.remarks = receiptHeader.remarks;
                rm.Transfer_Type = receiptHeader.Transfer_Type;
                rm.SGST_Percent = receiptHeader.SGST_Percent;
                rm.IGST_Percent = receiptHeader.IGST_Percent;
                rm.CGST_Percent = receiptHeader.CGST_Percent;
                rm.IGST_Amount = receiptHeader.IGST_Amount;
                rm.CGST_Amount = receiptHeader.CGST_Amount;
                rm.SGST_Amount = receiptHeader.SGST_Amount;
                rm.GSTGroupCode = receiptHeader.GSTGroupCode;
                rm.HSN = receiptHeader.HSN;
                rm.total_mc = receiptHeader.total_mc;
                rm.TDSPerc = receiptHeader.TDSPerc;
                rm.TDS_Amount = receiptHeader.TDS_Amount;
                rm.Net_Amount = receiptHeader.Net_Amount;
                rm.stone_charges = receiptHeader.stone_charges;
                rm.diamond_charges = receiptHeader.diamond_charges;
                rm.material_type = receiptHeader.material_type;
                rm.other_charges = receiptHeader.other_charges;
                rm.UniqRowID = Guid.NewGuid();
                rm.round_off = receiptHeader.round_off;
                rm.isReservedReceipt = null;
                rm.store_location_id = storeLocationId;
                db.KTTU_BARCODED_RECEIPT_MASTER.Add(rm);
                #endregion

                #region Barcode Receipt Detail
                int slNo = 1;
                foreach (var rd in barcodeIssueOutput.BarcodeIssueLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();

                    KTTU_BARCODED_RECEIPT_DETAILS receiptDetail = new KTTU_BARCODED_RECEIPT_DETAILS();
                    receiptDetail.obj_id = receiptMasterObjID;
                    receiptDetail.company_code = companyCode;
                    receiptDetail.branch_code = branchCode;
                    receiptDetail.receipt_no = receiptNo;
                    receiptDetail.sno = slNo;
                    receiptDetail.barcode_no = rd.barcode_no;
                    receiptDetail.item_name = rd.item_name;
                    receiptDetail.quantity = rd.quantity;
                    receiptDetail.gwt = rd.gwt;
                    receiptDetail.swt = rd.swt;
                    receiptDetail.nwt = rd.nwt;
                    receiptDetail.counter_code = rd.counter_code;
                    receiptDetail.cflag = "N";
                    receiptDetail.UpdateOn = updatedTimestamp;
                    receiptDetail.gs_code = rd.gs_code;
                    receiptDetail.amount = rd.amount;
                    receiptDetail.rate = rd.rate;
                    receiptDetail.dcts = rd.dcts;
                    receiptDetail.Fin_Year = rd.Fin_Year;
                    receiptDetail.batch_id = rd.batch_id;
                    receiptDetail.supplier_code = string.Empty;
                    receiptDetail.BReceiptNo = rd.BReceiptNo;
                    receiptDetail.pur_mc_type = 0;
                    receiptDetail.pur_mc_rate = 0;
                    receiptDetail.pur_rate = 0;
                    receiptDetail.barcode_date = rd.barcode_date;
                    receiptDetail.BSno = rd.BSno;
                    receiptDetail.design_code = rd.design_code;
                    receiptDetail.purity_perc = rd.purity_perc;
                    receiptDetail.pure_wt = rd.pure_wt;
                    receiptDetail.CGST_Amount = rd.CGST_Amount;
                    receiptDetail.CGST_Percent = rd.CGST_Percent;
                    receiptDetail.IGST_Amount = rd.IGST_Amount;
                    receiptDetail.IGST_Percent = rd.IGST_Percent;
                    receiptDetail.SGST_Amount = rd.SGST_Amount;
                    receiptDetail.SGST_Percent = rd.SGST_Percent;
                    receiptDetail.pur_making_charges = rd.pur_making_charges;
                    receiptDetail.stone_charges = rd.stone_charges;
                    receiptDetail.diamond_charges = rd.diamond_charges;
                    receiptDetail.hallmark_charges = rd.hallmark_charges;
                    receiptDetail.GSTGroupCode = "";
                    receiptDetail.HSN = ""; //TODO: Get HSN code
                    receiptDetail.UniqRowID = Guid.NewGuid();

                    #region Barcode Issue Stone Lines
                    if (barcodeIssueOutput.BarcodeIssueLineStones != null) {
                        var barcodeIssueStoneLines = barcodeIssueOutput.BarcodeIssueLineStones.Where(bils => bils.item_sno == slNo).ToList();
                        if (barcodeIssueStoneLines != null) {
                            int stoneReceiptSlNo = 1;
                            foreach (var srd in barcodeIssueStoneLines) {
                                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                                KTTU_ISSUE_RECEIPTS_STONE_DETAILS stoneDet = new KTTU_ISSUE_RECEIPTS_STONE_DETAILS
                                {
                                    obj_id = receiptMasterObjID,
                                    company_code = companyCode,
                                    branch_code = branchCode,
                                    issue_no = 0,
                                    sno = stoneReceiptSlNo,
                                    receipt_no = receiptNo,
                                    item_sno = slNo,
                                    type = "R",
                                    name = srd.name,
                                    qty = srd.qty,
                                    carrat = srd.carrat,
                                    rate = srd.rate,
                                    amount = srd.amount,
                                    gs_code = srd.gs_code,
                                    UpdateOn = updatedTimestamp,
                                    Fin_Year = finYear,
                                    IR_Type = "IB",
                                    party_code = receiptHeader.branch_code,
                                    swt = srd.swt,
                                    stone_damage_carat = srd.stone_damage_carat,
                                    stone_damage_Gms = srd.stone_damage_Gms,
                                    sub_Lot_No = srd.sub_Lot_No,
                                    UniqRowID = Guid.NewGuid(),
                                    stonetype = srd.stonetype,
                                    color = srd.color,
                                    cut = srd.cut,
                                    symmetry = srd.symmetry,
                                    fluorescence = srd.fluorescence,
                                    CertificateNo = srd.CertificateNo,
                                    seiveSize = srd.seiveSize,
                                    clarity = srd.clarity,
                                    polish = srd.polish,
                                    shape = srd.shape
                                };

                                db.KTTU_ISSUE_RECEIPTS_STONE_DETAILS.Add(stoneDet);
                                stoneReceiptSlNo++;
                            }
                        }
                    }
                    #endregion

                    slNo++;
                    db.KTTU_BARCODED_RECEIPT_DETAILS.Add(receiptDetail);
                }
                #endregion                

                #region Barcode Master
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                foreach (var bd in barcodeIssueOutput.BarcodeDetails) {
                    var barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == companyCode
                        && bar.branch_code == branchCode && bar.barcode_no == bd.barcode_no && bar.sold_flag != "Y")
                        .Select(b1 => new { barcode_no = b1.barcode_no }).FirstOrDefault();
                    if (barcodeMaster != null) {
                        error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = string.Format("The barcode No. {0} already exists.", barcodeMaster.barcode_no) };
                        return false;
                    }
                    KTTU_BARCODE_MASTER bm = new KTTU_BARCODE_MASTER();
                    var barcodeMasterObjId = SIGlobals.Globals.GetMagnaGUID(new string[] { "KTTU_BARCODE_MASTER", bd.barcode_no }, companyCode, branchCode);
                    bm.obj_id = barcodeMasterObjId;
                    bm.company_code = companyCode;
                    bm.branch_code = branchCode;
                    bm.barcode_no = bd.barcode_no;
                    bm.batch_no = bd.batch_no;
                    bm.sal_code = bd.sal_code;
                    bm.operator_code = bd.operator_code;
                    bm.date = bd.date;
                    bm.counter_code = bd.counter_code;
                    bm.gs_code = bd.gs_code;
                    bm.item_name = bd.item_name;
                    bm.gwt = bd.gwt;
                    bm.swt = bd.swt;
                    bm.nwt = bd.nwt;
                    bm.grade = bd.grade;
                    bm.catalog_id = bd.catalog_id;
                    bm.making_charge_per_rs = bd.making_charge_per_rs;
                    bm.wast_percent = bd.wast_percent;
                    bm.qty = bd.qty;
                    bm.item_size = bd.item_size;
                    bm.design_no = bd.design_no;
                    bm.piece_rate = bd.piece_rate;
                    bm.daimond_amount = bd.daimond_amount;
                    bm.stone_amount = bd.stone_amount;
                    bm.order_no = bd.order_no;
                    bm.sold_flag = "N";
                    bm.product_code = bd.product_code;
                    bm.hallmark_charges = bd.hallmark_charges;
                    bm.remarks = bd.remarks;
                    bm.supplier_code = bd.supplier_code;
                    bm.ordered_company_code = bd.ordered_company_code;
                    bm.ordered_branch_code = bd.ordered_branch_code;
                    bm.karat = bd.karat;
                    bm.mc_amount = bd.mc_amount;
                    bm.wastage_grms = bd.wastage_grms;
                    bm.mc_percent = bd.mc_percent;
                    bm.mc_type = bd.mc_type;
                    bm.old_barcode_no = bd.old_barcode_no;
                    bm.prod_ida = bd.prod_ida;
                    bm.prod_tagno = bd.prod_tagno;
                    bm.UpdateOn = updatedTimestamp;
                    bm.Lot_No = bd.Lot_No;
                    bm.tag_wt = bd.tag_wt;
                    bm.isConfirmed = bd.isConfirmed;
                    bm.confirmedBy = bd.confirmedBy;
                    bm.confirmedDate = bd.confirmedDate;
                    bm.current_wt = bd.current_wt;
                    bm.MC_For = bd.MC_For;
                    bm.diamond_no = bd.diamond_no;
                    bm.batch_id = bd.batch_id;
                    bm.add_wt = bd.add_wt;
                    bm.weightRead = bd.weightRead;
                    bm.confirmedweightRead = bd.confirmedweightRead;
                    bm.party_name = bd.party_name;
                    bm.design_name = bd.design_name;
                    bm.item_size_name = bd.item_size_name;
                    bm.master_design_code = bd.master_design_code;
                    bm.master_design_name = bd.master_design_name;
                    bm.vendor_model_no = bd.vendor_model_no;
                    bm.pur_mc_gram = bd.pur_mc_gram;
                    bm.mc_per_piece = bd.mc_per_piece;
                    bm.Tagging_Type = bd.Tagging_Type;
                    bm.BReceiptNo = bd.BReceiptNo;
                    bm.BSNo = bd.BSNo;
                    bm.Issue_To = bd.Issue_To;
                    bm.pur_mc_amount = bd.pur_mc_amount;
                    bm.pur_mc_type = bd.pur_mc_type;
                    bm.pur_rate = bd.pur_rate;
                    bm.sr_batch_id = bd.sr_batch_id;
                    bm.total_selling_mc = bd.total_selling_mc;
                    bm.pur_diamond_amount = bd.pur_diamond_amount;
                    bm.total_purchase_mc = bd.total_purchase_mc;
                    bm.pur_stone_amount = bd.pur_stone_amount;
                    bm.pur_purity_percentage = bd.pur_purity_percentage;
                    bm.pur_wastage_type = bd.pur_wastage_type;
                    bm.pur_wastage_type_value = bd.pur_wastage_type_value;
                    bm.UniqRowID = Guid.NewGuid();
                    bm.certification_no = bd.certification_no;
                    bm.ref_no = bd.ref_no;
                    bm.receipt_type = bd.receipt_type;
                    bm.EntryDocType = "BR";
                    bm.EntryDate = applicationDate;
                    bm.EntryDocNo = receiptNo.ToString();
                    bm.ExitDocType = "";
                    bm.ExitDate = SIGlobals.Globals.GetDefaultDate();
                    bm.ExitDocNo = "";
                    bm.OnlineStock = bd.OnlineStock;
                    bm.is_shuffled = bd.is_shuffled;
                    bm.shuffled_date = SIGlobals.Globals.GetDefaultDate();
                    bm.Collections = bd.Collections;
                    bm.isActive = bd.isActive;
                    bm.product_description = bd.product_description;
                    db.KTTU_BARCODE_MASTER.Add(bm);
                }
                #endregion

                #region Barcode Stone
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                foreach (var bsd in barcodeIssueOutput.BarcodeStoneDetails) {
                    KTTU_BARCODE_STONE_DETAILS barcodeStone = new KTTU_BARCODE_STONE_DETAILS();
                    var barcodeMasterObjId = SIGlobals.Globals.GetMagnaGUID(new string[] { "KTTU_BARCODE_MASTER", bsd.barcode_no }, companyCode, branchCode);
                    barcodeStone.obj_id = barcodeMasterObjId;
                    barcodeStone.company_code = companyCode;
                    barcodeStone.branch_code = branchCode;
                    barcodeStone.sl_no = bsd.sl_no;
                    barcodeStone.barcode_no = bsd.barcode_no;
                    barcodeStone.type = bsd.type;
                    barcodeStone.name = bsd.name;
                    barcodeStone.qty = bsd.qty;
                    barcodeStone.carrat = bsd.carrat;
                    barcodeStone.rate = bsd.rate;
                    barcodeStone.amount = bsd.amount;
                    barcodeStone.clarity = bsd.clarity;
                    barcodeStone.color = bsd.color;
                    barcodeStone.prod_ida = bsd.prod_ida;
                    barcodeStone.prod_tagno = bsd.prod_tagno;
                    barcodeStone.old_barcode_no = bsd.old_barcode_no;
                    barcodeStone.UpdateOn = updatedTimestamp;
                    barcodeStone.stone_type = bsd.stone_type;
                    barcodeStone.stone_gs_type = bsd.stone_gs_type;
                    barcodeStone.Fin_Year = bsd.Fin_Year;
                    barcodeStone.uom = bsd.uom;
                    barcodeStone.pur_cost = bsd.pur_cost;
                    barcodeStone.stone_code = bsd.stone_code;
                    barcodeStone.shape = bsd.shape;
                    barcodeStone.cut = bsd.cut;
                    barcodeStone.polish = bsd.polish;
                    barcodeStone.symmetry = bsd.symmetry;
                    barcodeStone.fluorescence = bsd.fluorescence;
                    barcodeStone.certificate = bsd.certificate;
                    barcodeStone.UniqRowID = Guid.NewGuid();
                    barcodeStone.pur_rate = bsd.pur_rate;
                    barcodeStone.Size = bsd.Size;
                    db.KTTU_BARCODE_STONE_DETAILS.Add(barcodeStone);
                }
                #endregion

                #region Barcode Image Url
                foreach (var x in barcodeIssueOutput.BarcodeImageUrls) {
                    KTTU_BARCODE_IMAGE_URL imageUrl = new KTTU_BARCODE_IMAGE_URL();
                    var barcodeMasterObjId = SIGlobals.Globals.GetMagnaGUID(new string[] { "KTTU_BARCODE_MASTER", x.barcode_no }, companyCode, branchCode);
                    imageUrl.obj_id = barcodeMasterObjId;
                    imageUrl.company_code = companyCode;
                    imageUrl.branch_code = branchCode;
                    imageUrl.barcode_no = x.barcode_no;
                    imageUrl.URL = x.URL;
                    db.KTTU_BARCODE_IMAGE_URL.Add(imageUrl);
                }
                #endregion

                #region Online Inventory posting
                foreach (var x in barcodeIssueOutput.OnlineInventoryDetails) {
                    KTTU_ONLINE_INVENTARY_DETAILS onlineInv = new KTTU_ONLINE_INVENTARY_DETAILS();
                    onlineInv.company_code = companyCode;
                    onlineInv.branch_code = branchCode;
                    onlineInv.barcode_no = x.barcode_no;
                    onlineInv.portal_id = x.portal_id;
                    onlineInv.isActive = x.isActive;
                    onlineInv.gs_code = x.gs_code;
                    onlineInv.item_name = x.item_name;
                    onlineInv.qty = x.qty;
                    onlineInv.gwt = x.gwt;
                    onlineInv.nwt = x.nwt;
                    db.KTTU_ONLINE_INVENTARY_DETAILS.Add(onlineInv);
                }
                #endregion

                #region Increment Serial No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "29");
                #endregion

                #region Stock Posting
                var barcodeIssLines = from iLine in barcodeIssueOutput.BarcodeIssueLines
                                      select new BarcodeReceiptDetailVM
                                      {
                                          BarcodeNo = iLine.barcode_no,
                                          GSCode = iLine.gs_code,
                                          CounterCode = iLine.counter_code,
                                          ItemCode = iLine.item_name,
                                          Qty = iLine.quantity,
                                          GrossWt = iLine.gwt,
                                          StoneWt = iLine.swt,
                                          NetWt = iLine.nwt
                                      };
                bool stockUpdated = UpdateStock(companyCode, branchCode, db, barcodeIssLines.ToList(), receiptNo, false, out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region Stone Stock Posting
                //UpdateStoneStock
                if (barcodeIssueOutput.BarcodeIssueLineStones != null) {
                    var stoneStockVM = barcodeIssueOutput.BarcodeIssueLineStones
                        .Select(st => new BarcodeReceiptStoneDetailVM
                        {
                            Type = st.gs_code,
                            Name = st.name,
                            Qty = st.qty,
                            Carrat = st.carrat,
                            Amount = st.amount,
                            Rate = st.rate
                        });
                    //Note the use of postReverse, it is true in this context since we have to reverse the stock posting.
                    var stockStockPostingSucceeded = UpdateStoneStock(companyCode, branchCode,
                        db, stoneStockVM.ToList(), receiptNo, postReverse: false, error: out error);
                    if (!stockStockPostingSucceeded) {
                        return false;
                    }
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

        private bool UpdateStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeReceiptDetailVM> barcodeReceiptLines, int receiptNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            #region Post Counter Stock
            var generalStockJournal =
                       from sl in barcodeReceiptLines
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
                           DocumentNo = receiptNo,
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
            bool gsStockPostSuccess = stockPost.GSStockPost(dbContext, summarizedGSStockJournal.ToList(), postReverse, out errorMessage);
            if (!gsStockPostSuccess) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = errorMessage };
                return false;
            }
            #endregion

            return true;
        }

        public dynamic PendingBarcodes(BarcodeReceiptVM barcodeReceiptVM, out ErrorVM error)
        {
            error = null;
            if (barcodeReceiptVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is no detail." };
                return false;
            }
            if (barcodeReceiptVM.ReceiptDetails == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue details doesn't exist." };
                return false;
            }
            if (barcodeReceiptVM.ScannedBarcodes == null) {
                barcodeReceiptVM.ScannedBarcodes = new List<BarcodeReceiptDetailVM>();
            }

            var missingList = barcodeReceiptVM.ReceiptDetails
                .Where(p => !barcodeReceiptVM.ScannedBarcodes.Any(p2 => p2.BarcodeNo == p.BarcodeNo))
                .Select(x => new { SlNo = x.SlNo, BarcodeNo = x.BarcodeNo, Item = x.ItemCode, GrossWt = x.GrossWt })
                .ToList();

            return missingList;
        }

        public dynamic ImportedBarcodeSummary(BarcodeReceiptVM barcodeReceiptVM, out ErrorVM error)
        {
            error = null;
            if (barcodeReceiptVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is no detail." };
                return false;
            }
            if (barcodeReceiptVM.ReceiptDetails == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Issue details doesn't exist." };
                return false;
            }

            var importedList = (from rd in barcodeReceiptVM.ReceiptDetails
                                group rd by new { rd.ItemCode } into groupedRd
                                select new
                                {
                                    ItemCode = groupedRd.Key.ItemCode,
                                    Qty = groupedRd.Sum(x => x.Qty),
                                    GrossWt = groupedRd.Sum(x => x.GrossWt),
                                    NetWt = groupedRd.Sum(x => x.NetWt)
                                }).ToList();

            return importedList;
        }

        public dynamic ScannedBarcodeSummary(BarcodeReceiptVM barcodeReceiptVM, out ErrorVM error)
        {
            error = null;
            if (barcodeReceiptVM == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "There is no detail available to report." };
                return false;
            }
            if (barcodeReceiptVM.ScannedBarcodes == null) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "No scanned barcodes present." };
                return false;
            }

            var scannedList = (from rd in barcodeReceiptVM.ScannedBarcodes
                               group rd by new { rd.ItemCode } into groupedRd
                               select new
                               {
                                   ItemCode = groupedRd.Key.ItemCode,
                                   Qty = groupedRd.Sum(x => x.Qty),
                                   GrossWt = groupedRd.Sum(x => x.GrossWt),
                                   NetWt = groupedRd.Sum(x => x.NetWt)
                               }).ToList();

            return scannedList;
        }

        public List<ListOfValue> List(string companyCode, string branchCode, DateTime date, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from bim in db.KTTU_BARCODED_RECEIPT_MASTER
                            join sm in db.KSTU_SUPPLIER_MASTER
                            on new { CompanyCode = bim.company_code, BranchCode = bim.branch_code, PartyCode = bim.received_from }
                            equals new { CompanyCode = sm.company_code, BranchCode = sm.branch_code, PartyCode = sm.party_code }
                            where bim.company_code == companyCode && bim.branch_code == branchCode && bim.cflag != "Y"
                            && DbFunctions.TruncateTime(bim.receipt_date) == date.Date
                            select new {ReceiptNo = bim.receipt_no, PartyName = sm.party_name }).ToList();

                var result = data.Distinct().Select(d => new ListOfValue { Code = d.ReceiptNo.ToString(),
                    Name = d.PartyName
                }).OrderByDescending(d => d.Code);
                return result.ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool CancelReceipt(string companyCode, string branchCode, int receiptNo, string userID, string cancelRemarks, out ErrorVM error)
        {
            error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };

            try {
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();
                #region Receipt Master
                var receiptMaster = db.KTTU_BARCODED_RECEIPT_MASTER.Where(x => x.company_code == companyCode
                                && x.branch_code == branchCode && x.receipt_no == receiptNo).FirstOrDefault();
                if (receiptMaster == null) {
                    error.description = "Receipt information is not found for issue number: " + receiptNo.ToString();
                    return false;
                }
                if (receiptMaster.cflag == "Y") {
                    error.description = string.Format("The receipt number {0} is already cancelled.", receiptNo);
                    return false;
                }

                receiptMaster.cflag = "Y";
                receiptMaster.cancelled_by = userID;
                receiptMaster.UpdateOn = updatedTimestamp;
                receiptMaster.remarks = cancelRemarks;
                #endregion

                #region Receipt Detail
                var receiptLines = db.KTTU_BARCODED_RECEIPT_DETAILS.Where(x => x.company_code == companyCode
                                        && x.branch_code == branchCode && x.receipt_no == receiptNo).ToList();
                if (receiptLines == null) {
                    error.description = "Receipt details is not found for receipt number: " + receiptNo.ToString();
                    return false;
                }
                updatedTimestamp = SIGlobals.Globals.GetDateTime();
                foreach (var rl in receiptLines) {
                    rl.cflag = "Y";
                    rl.UpdateOn = updatedTimestamp;
                    db.Entry(rl).State = System.Data.Entity.EntityState.Modified;
                    if (!string.IsNullOrEmpty(rl.barcode_no)) {
                        var barcodeMaster = db.KTTU_BARCODE_MASTER.Where(bar => bar.company_code == rl.company_code
                            && bar.branch_code == rl.branch_code && bar.barcode_no == rl.barcode_no).FirstOrDefault();

                        barcodeMaster.sold_flag = "Y";
                        barcodeMaster.UpdateOn = updatedTimestamp;
                        db.Entry(barcodeMaster).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                #endregion

                #region Item Stock Reversal
                //To Reuse existing code, I need to fill the ViewModel and call the same stock update method.
                var barcodeRecLines = from iLine in receiptLines
                                      select new BarcodeReceiptDetailVM
                                      {
                                          BarcodeNo = iLine.barcode_no,
                                          GSCode = iLine.gs_code,
                                          CounterCode = iLine.counter_code,
                                          ItemCode = iLine.item_name,
                                          Qty = iLine.quantity,
                                          GrossWt = iLine.gwt,
                                          StoneWt = iLine.swt,
                                          NetWt = iLine.nwt
                                      };
                bool stockUpdated = UpdateStock(companyCode, branchCode, db, barcodeRecLines.ToList(), receiptNo, true, out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #region Stone Stock Reversal
                //UpdateStoneStock
                var stStockDetail = db.KTTU_ISSUE_RECEIPTS_STONE_DETAILS.Where(ss => ss.company_code == companyCode
                    && ss.branch_code == branchCode && ss.receipt_no == receiptNo && ss.type == "R").ToList();
                if (stStockDetail != null) {
                    //Because swt is nullable, I've to first select the data and then get the list.
                    var stoneStockVM = stStockDetail
                        .Select(st => new BarcodeReceiptStoneDetailVM
                        {
                            Type = st.gs_code,
                            Name = st.name,
                            Qty = st.qty,
                            Carrat = st.carrat,
                            Amount = st.amount,
                            Rate = st.rate
                        });
                    //Note the use of postReverse, it is true in this context since we have to reverse the stock posting.
                    var stockStockPostingSucceeded = UpdateStoneStock(companyCode, branchCode,
                        db, stoneStockVM.ToList(), receiptNo, postReverse: true, error: out error);
                    if (!stockStockPostingSucceeded) {
                        return false;
                    }
                }
                #endregion

                #region Reverse Account posting
                //TODO: To be implemented later
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }

            return true;
        }

        public bool UpdateStoneStock(string companyCode, string branchCode, MagnaDbEntities dbContext, List<BarcodeReceiptStoneDetailVM> stoneLines, int receiptNo, bool postReverse, out ErrorVM error)
        {
            error = null;
            if (stoneLines == null) {
                return true;
            }
            foreach (var sl in stoneLines) {
                if (postReverse) {
                    sl.Qty = sl.Qty * -1;
                    sl.Carrat = sl.Carrat * -1;
                }
                var stoneStockLine = dbContext.KTTU_STONE_COUNTER_STOCK.Where(x => x.company_code == companyCode
                    && x.branch_code == branchCode && x.gs_code == sl.Type && x.stone_name == sl.Name).FirstOrDefault();
                if (stoneStockLine == null) {
                    error = new ErrorVM { description = "Stone counter stock does not exist for stone " + sl.Name, ErrorStatusCode = System.Net.HttpStatusCode.NotFound };
                    return false;
                }

                stoneStockLine.receipt_qty = stoneStockLine.receipt_qty + Convert.ToInt32(sl.Qty);
                stoneStockLine.receipt_carat = stoneStockLine.receipt_carat + Convert.ToDecimal(sl.Carrat);
                stoneStockLine.closing_qty = stoneStockLine.closing_qty + Convert.ToInt32(sl.Qty);
                stoneStockLine.closing_carat = stoneStockLine.closing_carat + Convert.ToDecimal(sl.Carrat);
                db.Entry(stoneStockLine).State = System.Data.Entity.EntityState.Modified;
            }

            return true;
        }

        public ProdigyPrintVM Print(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintBarcodeReceipt(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        public ProdigyPrintVM PrintDet(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintBarcodeReceiptDet(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        protected string PrintBarcodeReceipt(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                var dtReceiptBill = db.KTTU_BARCODED_RECEIPT_MASTER.Where(b => b.company_code == companyCode
                                                                            && b.branch_code == branchCode
                                                                            && b.receipt_no == receiptNo).FirstOrDefault();
                if (dtReceiptBill == null) {
                    error = new ErrorVM()
                    {
                        description = "Barcoded Receipt Number not Exists",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }
                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                string partyCode = dtReceiptBill.received_from;
                int issueNo = dtReceiptBill.issue_no;

                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, partyCode);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();
                string DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dtReceiptBill.receipt_date));

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                for (int j = 0; j < 3; j++) {
                    sb.AppendLine("<TR style=border:0>");
                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }


                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
                sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin; border-right:thin;\" ALIGN = \"left\"><b>GSTIN {0}</b></TD>", company.tin_no));
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" ALIGN = \"right\"><b>PAN {0}</b></TD>", company.pan_no));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "RECEIPT DETAILS"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.party_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address1 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address2 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address3 + "</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + state.tinno + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >PAN &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", supplier.pan_no));
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >GSTIN &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\">{0}</td>", supplier.TIN));
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + receiptNo + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt Date &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", DateTime));
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueNo + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Receipt Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + SIGlobals.Globals.GetIRTypeName(db, companyCode, branchCode, dtReceiptBill.receipt_type) + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Received By &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + dtReceiptBill.sal_code + "</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state_code + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 10 ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");

                if (string.Compare(Convert.ToString(company.state_code), Convert.ToString(state.tinno)) == 0) {
                    if (string.Compare(dtReceiptBill.receipt_type.ToString(), "RM") == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\">JOB WORK RECEIPT(NOT AS SUPPLY)</TD>"));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTRA-STATE STOCK RECEIPT</TD>"));
                        sb.AppendLine("</TR>");
                    }
                }
                else {
                    if (string.Compare(dtReceiptBill.receipt_type.ToString(), "RM") == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\">JOB WORK RECEIPT(NOT AS SUPPLY)</TD>"));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=10 ALIGN = \"CENTER\"> INTER-STATE STOCK RECEIPT</TD>"));
                        sb.AppendLine("</TR>");
                    }
                }

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=10 ALIGN = \"CENTER\">ORIGINAL/DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");

                string strIssueBillDetails = string.Format("EXEC [usp_IssueReceipts_HTMLPrints] '{0}', '{1}', '{2}', '{3}'",
                            receiptNo, companyCode, branchCode, 'R');
                DataTable dtIssueBillDetails = SIGlobals.Globals.ExecuteQuery(strIssueBillDetails);

                if (dtIssueBillDetails != null && dtIssueBillDetails.Rows.Count > 1) {
                    sb.AppendLine("<Table font-size=9pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");  //FRAME=BOX RULES=NONE
                    sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TH width=\"80\" style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[0].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"left\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[1].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[2].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"CENTER\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[3].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"left\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[4].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[5].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[6].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[7].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[8].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[9].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[10].ColumnName));
                    sb.AppendLine(string.Format("<TH style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TH>", dtIssueBillDetails.Columns[11].ColumnName));
                    sb.AppendLine("</TR>");

                    for (int i = 0; i < dtIssueBillDetails.Rows.Count - 1; i++) {
                        sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                        for (int j = 0; j < dtIssueBillDetails.Columns.Count; j++) {
                            if (string.Compare(dtIssueBillDetails.Columns[j].DataType.FullName.ToString(), "System.String") == 0 || j == 0)
                                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            else {
                                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i][j], "&nbsp"));
                            }
                        }
                        sb.AppendLine("</TR>");
                    }
                    for (int i = 0; i < 10 - dtIssueBillDetails.Rows.Count - 1; i++) {
                        if (i == 0) {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD colspan = {1}>{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                            sb.AppendLine("</TR>");
                        }
                        else {
                            sb.AppendLine("<TR>");
                            sb.AppendLine(string.Format("<TD style=\"border:0\";  colspan = {1}>{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                            sb.AppendLine("</TR>");
                        }
                    }
                }

                sb.AppendLine("<TR bgcolor='#FFFACD'  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                int columnCOunt = dtIssueBillDetails.Columns.Count;
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan = 5 ALIGN = \"left\"><b>Total</b></TD>"));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 7], "&nbsp", columnCOunt - 6));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                 , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 6], "&nbsp", columnCOunt - 5));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                     , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 5], "&nbsp", columnCOunt - 4));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 4], "&nbsp", columnCOunt - 3));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 3], "&nbsp", columnCOunt - 2));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 2], "&nbsp"));
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}{1}</b></TD>"
                         , dtIssueBillDetails.Rows[dtIssueBillDetails.Rows.Count - 1][columnCOunt - 1], "&nbsp"));
                sb.AppendLine("</TR>");
                decimal finalAmount = Convert.ToDecimal(dtReceiptBill.final_amount);
                finalAmount = finalAmount > 0 ? finalAmount : Convert.ToDecimal(dtReceiptBill.total_amount);
                if (Convert.ToDecimal(dtReceiptBill.IGST_Amount) > 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD colspan=11 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", dtReceiptBill.IGST_Percent.ToString()));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", dtReceiptBill.IGST_Amount.ToString()));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD  colspan=11 ALIGN = \"right\"><b>Final Amount : </b></TD>"));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\"  ALIGN = \"right\"><b>{0}</b></TD>", finalAmount));
                    sb.AppendLine("</TR>");
                }
                string strWords = string.Empty;
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(finalAmount);

                if (!string.IsNullOrEmpty(dtReceiptBill.remarks.ToString())) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-right:thin ; border-bottom:0 \"; colspan={2} ALIGN = \"left\"><b>{0}{1}{1}</b></TD>", "Remarks :" + dtReceiptBill.remarks.ToString(), "&nbsp", dtIssueBillDetails.Columns.Count));
                    sb.AppendLine("</TR>");
                }
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin ; border-bottom:0 \"; colspan={2} ALIGN = \"left\"><b>{0}{1}{1}</b></TD>", strWords, "&nbsp", dtIssueBillDetails.Columns.Count));
                sb.AppendLine("</TR>");

                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan ={3} style=border-right:0 ALIGN = \"RIGHT\"><b>For {0}{2}<br><br><br>{1}{2}</b></TD>", company.company_name, "Authorized Signatory", "&nbsp", dtIssueBillDetails.Columns.Count));
                sb.AppendLine("</TR>");
                sb.AppendLine("</Table>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        protected string PrintBarcodeReceiptDet(string companyCode, string branchCode, int receiptNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                var dtReceiptBill = db.KTTU_BARCODED_RECEIPT_MASTER.Where(b => b.company_code == companyCode
                                                                           && b.branch_code == branchCode
                                                                           && b.receipt_no == receiptNo).FirstOrDefault();
                if (dtReceiptBill == null) {
                    error = new ErrorVM()
                    {
                        description = "Barcoded Receipt Number not Exists",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                }


                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                string partyCode = dtReceiptBill.received_from;
                int issueNo = dtReceiptBill.issue_no;

                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, partyCode);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();
                string DateTime = string.Format("{0:dd/MM/yyyy}", Convert.ToDateTime(dtReceiptBill.receipt_date));

                sb.AppendLine("<html>");
                sb.AppendLine("<head>");
                sb.AppendLine(SIGlobals.Globals.GetStyle());
                sb.AppendLine("</head>");
                sb.AppendLine("<body>");

                for (int j = 0; j < 3; j++) {
                    sb.AppendLine("<TR style=border:0>");
                    sb.AppendLine(string.Format("<TD style=border:0 ALIGN = \"RIGHT\"><b>{0}</b></TD>", "&nbsp"));
                    sb.AppendLine("</TR>");
                }

                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"800\" style=\"border-collapse:collapse\" >");
                sb.AppendLine("<Table font-size=12pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"800\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" ALIGN = \"left\"><b>GSTIN {0}</b></TD>", company.tin_no));
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" ALIGN = \"right\"><b>PAN {0}</b></TD>", company.pan_no));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=9 ALIGN = \"CENTER\"><b>{0}</b></TD>", company.company_name));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=9 ALIGN = \"CENTER\"><b>{0},{1},{2}-{3}</b></TD>", company.address1, company.address2, company.address3, company.pin_code));
                sb.AppendLine("</TR>");


                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"400\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "RECEIPT DETAILS"));
                sb.AppendLine("</TR>");


                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=12pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.party_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address1 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address2 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.address3 + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + state.tinno + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >PAN  &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\" >{0}</td>", supplier.pan_no));
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >GSTIN  &nbsp&nbsp&nbsp </td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin \"  style=\"border-top:thin\" >{0}</td>", supplier.TIN));
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=12pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + receiptNo + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt Date &nbsp&nbsp</td>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin ; border-top:thin\" >{0}</td>", DateTime));
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Receipt Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + SIGlobals.Globals.GetIRTypeName(db, companyCode, branchCode, dtReceiptBill.receipt_type) + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State Code &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + company.state_code + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan =9 ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");

                if (string.Compare(company.state_code.ToString(), state.tinno.ToString()) == 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=9 ALIGN = \"CENTER\"> INTRA-STATE STOCK TRANSFER</TD>"));
                    sb.AppendLine("</TR>");
                }
                else {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=9 ALIGN = \"CENTER\"> INTER-STATE STOCK TRANSFER</TD>"));
                    sb.AppendLine("</TR>");
                }

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=9 ALIGN = \"CENTER\">ORIGINAL/DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");

                DataTable dtIssueBillDetails = SIGlobals.Globals.ConvertListToDataTable(db.KTTU_BARCODED_RECEIPT_DETAILS.Where(b => b.company_code == companyCode && b.branch_code == branchCode && b.receipt_no == receiptNo).OrderBy(x => x.sno).ToList());

                decimal TotalQty = 0, TotalGwt = 0, TotalSwt = 0, TotalNwt = 0, TotalDcarat = 0, TotalAmt = 0;
                sb.AppendLine("<Table font-size=9pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"800\">");  //FRAME=BOX RULES=NONE

                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<th style=\"border-top:none; \" align = \"center\">S.No</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Barcode No</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Item</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Qty</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Gr.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">St.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">N.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Dcts</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Amount</th>");

                sb.AppendLine("</TR>");

                for (int i = 0; i < dtIssueBillDetails.Rows.Count; i++) {
                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["sno"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["barcode_no"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["item_name"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["quantity"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["gwt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["swt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["nwt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["dcts"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtIssueBillDetails.Rows[i]["amount"].ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                    TotalQty += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["quantity"]);
                    TotalGwt += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["gwt"]);
                    TotalSwt += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["swt"]);
                    TotalNwt += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["nwt"]);
                    TotalAmt += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["amount"]);
                    TotalDcarat += Convert.ToDecimal(dtIssueBillDetails.Rows[i]["dcts"]);
                }
                for (int i = 0; i < 20 - dtIssueBillDetails.Rows.Count; i++) {
                    if (i == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left:thin ; border-bottom:thin ; border-right:thin \";  style=font-size=\"12pt\" colspan =9 >{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD style=\"border-left:thin ; border-top:thin ; border-bottom:thin; border-right:thin \";  style=font-size=\"12pt\"  colspan =9 >{0}</TD>", "&nbsp", dtIssueBillDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                }

                sb.AppendLine("<TR bgcolor='#FFFACD'>");
                sb.Append(string.Format("<TD colspan=3 style=font-size=\"12pt\" ALIGN = \"left\"><b>{0}</b></TD>", "Total  "));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"right\"><b>{0}</b></TD>", TotalQty));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalGwt));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalSwt));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalNwt));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalDcarat));
                sb.Append(string.Format("<TD style=font-size=\"12pt\" ALIGN = \"RIGHT\"><b>{0}</b></TD>", TotalAmt));
                sb.AppendLine("</TR>");
                if (Convert.ToDecimal(dtReceiptBill.IGST_Amount) > 0) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD colspan=8 ALIGN = \"right\"><b>IGST Amt @ {0} % : </b></TD>", dtReceiptBill.IGST_Percent.ToString()));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" ALIGN = \"right\"><b>{0}</b></TD>", dtReceiptBill.IGST_Amount.ToString()));
                    sb.AppendLine("</TR>");
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD colspan=8 ALIGN = \"right\"><b>Final Amount : </b></TD>"));
                    sb.AppendLine(string.Format("<TD  style=\"border-right:thin\"  ALIGN = \"right\"><b>{0}</b></TD>", dtReceiptBill.final_amount.ToString()));
                    sb.AppendLine("</TR>");
                }
                decimal Inword = 0.0M;
                string strWords = string.Empty;
                Inword = Convert.ToDecimal(dtReceiptBill.final_amount) > 0 ? Convert.ToDecimal(dtReceiptBill.final_amount) : Convert.ToDecimal(dtReceiptBill.total_amount);
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Inword);

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin ; border-bottom:0 \"; colspan=9 ALIGN = \"left\"><b>{0}{1}{1}</b></TD>", strWords, "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<TD style=font-size=\"12pt\" colspan =9 style=border-right:0 ALIGN = \"RIGHT\"><b>For {0}{2}<br><br><br>{1}{2}</b></TD>", company.company_name, "Authorized Signatory", "&nbsp"));
                sb.AppendLine("</tr>");
                sb.AppendLine("</Table>");
                sb.AppendLine("</body>");
                sb.AppendLine("</html>");
                sb.AppendLine();
                sb.AppendLine();
                sb.AppendLine();
                return sb.ToString();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

    }
}
