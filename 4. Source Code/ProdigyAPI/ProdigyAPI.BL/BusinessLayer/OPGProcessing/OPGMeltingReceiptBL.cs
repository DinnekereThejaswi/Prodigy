using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.BusinessLayer.Stock;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.OPGProcessing;
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
using ProdigyAPI.BL.BusinessLayer.Sales;

namespace ProdigyAPI.BL.BusinessLayer.OPGProcessing
{
    public class OPGMeltingReceiptBL
    {
        MagnaDbEntities db = null;

        public OPGMeltingReceiptBL()
        {
            db = new MagnaDbEntities(true);
        }

        public OPGMeltingReceiptBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<ListOfValue> GetReceiptFromList(string companyCode, string branchCode)
        {
            var partyList = from sm in db.KSTU_SUPPLIER_MASTER
                            join sg in db.KSTU_SUPPLIER_GROUP
                            on new { CC = sm.company_code, BC = sm.branch_code, GS = sm.party_code }
                                equals new { CC = sg.company_code, BC = sg.branch_code, GS = sg.party_code }
                            where sm.company_code == companyCode && sm.branch_code == branchCode
                                && sm.party_code != "HO" && sm.obj_status != "C"
                                && sg.ir_code == "RL"
                            orderby sm.party_name
                            select new ListOfValue
                            {
                                Code = sm.party_code,
                                Name = sm.party_name
                            };

            return partyList.ToList();
        }

        public List<ListOfValue> GetPendingMeltingIssues(string companyCode, string branchCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                var metalGS = (from gsEntry in db.KSTS_GS_ITEM_ENTRY
                               join gsGrp in db.KSTU_GS_GROUPING
                               on new { CC = gsEntry.company_code, BC = gsEntry.branch_code, GS = gsEntry.gs_code }
                               equals new { CC = gsGrp.company_code, BC = gsGrp.branch_code, GS = gsGrp.gs_code }
                               where gsEntry.object_status != "C" && gsGrp.obj_status != "C"
                                && gsEntry.company_code == companyCode && gsEntry.branch_code == branchCode
                                && gsGrp.ir_code == "IL"
                               orderby gsEntry.item_level1_name
                               select new ListOfValue
                               {
                                   Code = gsEntry.gs_code,
                                   Name = gsEntry.item_level1_name
                               }).ToList();
                return metalGS;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public List<ListOfValue> GetMetalGS(string companyCode, string branchCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                var metalGS = (from gsEntry in db.KSTS_GS_ITEM_ENTRY
                               join gsGrp in db.KSTU_GS_GROUPING
                               on new { CC = gsEntry.company_code, BC = gsEntry.branch_code, GS = gsEntry.gs_code }
                               equals new { CC = gsGrp.company_code, BC = gsGrp.branch_code, GS = gsGrp.gs_code }
                               where gsEntry.object_status != "C" && gsGrp.obj_status != "C"
                                && gsEntry.company_code == companyCode && gsEntry.branch_code == branchCode
                                && gsGrp.ir_code == "RL"
                               orderby gsEntry.item_level1_name
                               select new ListOfValue
                               {
                                   Code = gsEntry.gs_code,
                                   Name = gsEntry.item_level1_name
                               }).ToList();
                return metalGS;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public List<ListOfValue> GetPendingIssues(string companyCode, string branchCode, string partyCode, out string errorMessage)
        {
            errorMessage = string.Empty;
            try {
                string sql = string.Format("SELECT pib.ISSUE_NO AS IssueNo, \n"
                               + "       pib.ISSUE_NO_DISPLAY AS IssueNoDisplay \n"
                               + "FROM   [vPendingIssueNosWithBatchID]  AS pib \n"
                               + "WHERE  pib.company_code = '{0}' \n"
                               + "       AND pib.branch_code = '{1}' \n"
                               + "       AND pib.party_name = '{2}' \n"
                               + "       AND pib.issue_type = 'IL' \n"
                               + "ORDER BY \n"
                               + "       pib.ISSUE_NO_DISPLAY",
                               companyCode, branchCode, partyCode);
                DataTable dt = Globals.GetDataTable(sql);
                if (dt == null || dt.Rows.Count <= 0) {
                    errorMessage = "No pending issue details found.";
                    return null;
                }

                List<ListOfValue> issueList = new List<ListOfValue>();
                foreach (DataRow row in dt.Rows) {
                    ListOfValue lov = new ListOfValue
                    {
                        Code = row["IssueNo"].ToString(),
                        Name = row["IssueNoDisplay"].ToString(),
                    };
                    issueList.Add(lov);
                }

                return issueList;
            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
        }

        public List<OPGMeltingReceiptBatchDetailVM> GetAllBatchDetailForGivenIssue(string companyCode, string branchCode, int issueNo, out string errorMessage)
        {
            errorMessage = string.Empty;
            List<OPGMeltingReceiptBatchDetailVM> batchDetail = new List<OPGMeltingReceiptBatchDetailVM>();
            try {
                var issueDetail = db.KTTU_ISSUE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.issue_no == issueNo).OrderBy(y => y.sl_no).ToList();
                if (issueDetail == null || issueDetail.Count <= 0) {
                    errorMessage = "No batch detail found for the issue number: " + issueNo.ToString();
                    return null;
                }
                batchDetail = issueDetail.Select(m => new OPGMeltingReceiptBatchDetailVM
                {
                    IssueNo = issueNo,
                    GSCode = m.gs_code,
                    ItemCode = m.item_name,
                    BatchId = m.batch_id,
                    Qty = m.units,
                    GrossWt = Convert.ToDecimal(m.gwt),
                    StoneWt = Convert.ToDecimal(m.swt),
                    NetWt = Convert.ToDecimal(m.nwt),
                    AlloyWt = Convert.ToDecimal(m.alloy),
                    PurityPercent = Convert.ToDecimal(m.purity),
                    WastageWt = Convert.ToDecimal(m.wastage),
                    Amount = Convert.ToDecimal(m.item_value)
                }).ToList();

            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
            return batchDetail;
        }

        public OPGMeltingReceiptBatchDetailVM GetSingleBatchInfo(string companyCode, string branchCode, int issueNo, string batchId, out string errorMessage)
        {
            errorMessage = string.Empty;
            OPGMeltingReceiptBatchDetailVM batchInfo = new OPGMeltingReceiptBatchDetailVM();
            try {
                var issDet = db.KTTU_ISSUE_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.issue_no == issueNo && x.batch_id == batchId).OrderBy(y => y.sl_no).FirstOrDefault();
                if (issDet == null) {
                    errorMessage = "No batch detail found for the BatchId: " + batchId.ToString();
                    return null;
                }
                decimal purchaseRate = 0;
                var rateMaster = db.KSTU_RATE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.gs_code == "OGO" && x.karat == "24K").FirstOrDefault();
                if (rateMaster != null) {
                    if (rateMaster.bill_type == "P")
                        purchaseRate = rateMaster.exchange_rate;
                    else
                        purchaseRate = rateMaster.rate;
                }

                batchInfo = new OPGMeltingReceiptBatchDetailVM
                {
                    IssueNo = issueNo,
                    GSCode = issDet.gs_code,
                    ItemCode = issDet.item_name,
                    BatchId = issDet.batch_id,
                    Qty = issDet.units,
                    GrossWt = Convert.ToDecimal(issDet.gwt),
                    StoneWt = Convert.ToDecimal(issDet.swt),
                    NetWt = Convert.ToDecimal(issDet.nwt),
                    AlloyWt = Convert.ToDecimal(issDet.alloy),
                    PurityPercent = Convert.ToDecimal(issDet.purity),
                    WastageWt = Convert.ToDecimal(issDet.wastage),
                    Amount = Convert.ToDecimal(issDet.item_value),
                    Rate = purchaseRate
                };

            }
            catch (Exception ex) {
                errorMessage = new ErrorVM().GetErrorDetails(ex).customDescription;
                return null;
            }
            return batchInfo;
        }

        public bool SaveReceipt(OPGMeltingReceiptHeaderVM meltingReceipt, string userID, out int receiptNo, out ErrorVM error)
        {
            error = null;
            receiptNo = 0;
            try {
                #region 1. Basic Validations
                if (meltingReceipt == null) {
                    error = new ErrorVM { description = "Nothing to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                if (meltingReceipt.OPGReceiptLines == null || meltingReceipt.OPGReceiptLines.Count <= 0) {
                    error = new ErrorVM { description = "There is no line detail to save.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                string companyCode = meltingReceipt.CompanyCode;
                string branchCode = meltingReceipt.BranchCode;
                string errorMessage = string.Empty;

                //Revalidate and check if the batch is valid.
                foreach (var bd in meltingReceipt.OPGReceiptLines) {
                    var dbBatchInfo = GetSingleBatchInfo(companyCode, branchCode, meltingReceipt.IssueNo, bd.BatchId, out errorMessage);
                    if (dbBatchInfo == null) {
                        error = new ErrorVM
                        {
                            description = string.Format("The BatchId {0} does not exist.", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.Rate <= 0) {
                        error = new ErrorVM
                        {
                            description = string.Format("The Rate attribute is required for BatchId {0}", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.CalculatedAmount <= 0) {
                        error = new ErrorVM
                        {
                            description = string.Format("The Amount attribute is required for BatchId {0}", bd.BatchId),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                    if (bd.CalculatedTotalReceiptWeight != dbBatchInfo.GrossWt) {
                        error = new ErrorVM
                        {
                            description = string.Format("The sum of Receipt Gross Wt., Kova Wt. and Melting loss Wt. should be {0} for BatchId {1}, but it is {2}",
                               dbBatchInfo.GrossWt, bd.BatchId, bd.CalculatedTotalReceiptWeight),
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                #endregion

                var applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                var updatedTimestamp = SIGlobals.Globals.GetDateTime();

                var storeLocationId = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode
                    && c.branch_code == branchCode).FirstOrDefault().store_location_id;
                var finYear = Globals.GetFinancialYear(db, companyCode, branchCode);

                #region 2. Post Receipt Master and Receipt Detail Records

                #region -2.1 Receipt Master
                receiptNo = SIGlobals.Globals.GetDocumentNo(db, companyCode, branchCode, "15", true);
                string receiptMasterObjectID = SIGlobals.Globals.GetMagnaGUID("KTTU_RECEIPTS_MASTER", receiptNo, companyCode, branchCode);
                decimal totalAmount = meltingReceipt.OPGReceiptLines.Sum(x => x.CalculatedAmount);
                KTTU_RECEIPTS_MASTER rm = new KTTU_RECEIPTS_MASTER
                {
                    obj_id = receiptMasterObjectID,
                    company_code = companyCode,
                    branch_code = branchCode,
                    receipt_no = receiptNo,
                    receipt_date = applicationDate,
                    gs_type = "",
                    sal_code = userID,
                    operator_code = userID,
                    receipt_type = "RL",
                    Ref_no = "0",
                    cflag = "N",
                    cancelled_by = "",
                    grand_total = totalAmount,
                    charges_acc_code = 0,
                    type = "CPC",
                    UpdateOn = updatedTimestamp,
                    remarks = meltingReceipt.Remarks,
                    issue_no = meltingReceipt.IssueNo,
                    party_name = meltingReceipt.ReceiptFrom,
                    no_of_tags = 0,
                    tag_weight = 0,
                    batch_id = "",
                    lot_ref_id = null,
                    DC_No = "",
                    DC_Date = applicationDate.Date,
                    Inv_Date = applicationDate.Date,
                    Is_hallmarked = "Y",
                    hallmarked_by = "",
                    inv_no = "",
                    cancelled_remarks = "",
                    ShiftID = 0,
                    New_Bill_No = "",
                    Ref_Receipt_No = receiptNo.ToString(),
                    isFixed = null,
                    new_receipt_no = receiptNo.ToString(),
                    adv_type = "R",
                    Invoice_Type_Id = 0,
                    C_Form = "N",
                    TDSPerc = 0,
                    Net_Amount = 0,
                    TDS_Amount = 0,
                    version = 0,
                    stk_type = null,
                    old_receipt_no = null,
                    import_data_id = null,
                    import_content = null,
                    final_amount = totalAmount,
                    GSTGroupCode = "",
                    SGST_Percent = 0,
                    SGST_Amount = 0,
                    CGST_Percent = 0,
                    CGST_Amount = 0,
                    IGST_Percent = 0,
                    IGST_Amount = 0,
                    HSN = "",
                    round_off = 0,
                    pan_no = "",
                    tin_no = "",
                    UniqRowID = Guid.NewGuid(),
                    isReceived = "N",
                    expected_pure_wt = 0,
                    isConfirm = "N",
                    fin_year = finYear,
                    isDealer = "Y",
                    isNwtBased = "Y",
                    isCaratBased = "Y",
                    TCS_Percent = 0,
                    TCS_Amount = 0,
                    store_location_id = storeLocationId
                };
                db.KTTU_RECEIPTS_MASTER.Add(rm);
                #endregion

                #region -2.2 Receipt Detail Table
                int recLineSlNo = 1;
                foreach (var rl in meltingReceipt.OPGReceiptLines) {
                    updatedTimestamp = SIGlobals.Globals.GetDateTime();
                    KTTU_RECEIPTS_DETAILS rd = new KTTU_RECEIPTS_DETAILS
                    {
                        obj_id = receiptMasterObjectID,
                        company_code = companyCode,
                        branch_code = branchCode,
                        receipt_no = receiptNo,
                        sl_no = recLineSlNo,
                        item_name = rl.GSCode,
                        code = "0",
                        counter_code = "",
                        item_no = 0,
                        gwt = rl.ReceiptGrossWt,
                        swt = 0,
                        nwt = rl.ReceiptGrossWt,
                        wastage = rl.CalculatedMeltingLoss,
                        item_amount = rl.CalculatedAmount,
                        std_weight = 0,
                        purity = 0,
                        UpdateOn = updatedTimestamp,
                        oldReceiptNo = null,
                        gs_code = rl.GSCode,
                        batch_id = rl.BatchId,
                        no_tags = 0,
                        tag_wt = 0,
                        dcts = 0,
                        design_code = "",
                        result = "",
                        KOVA_wt = rl.KovaWeight,
                        batch_name = "",
                        supp_dcts = 0,
                        hLoss = 0,
                        supp_swt = 0,
                        Fin_Year = finYear,
                        barcode_no = "",
                        dcts_amount = 0,
                        sales_no = 0,
                        adj_date = updatedTimestamp,
                        mc_type = 0,
                        mc_per_gram = 0,
                        mc_amount = 0,
                        tax_amount = 0,
                        item_description = "",
                        stone_amount = 0,
                        piece_rate = 0,
                        type = "R",
                        isHallMarked = "",
                        wastage_type_id = 0,
                        wastage_type_value = 0,
                        wastageInActualWeight = 0,
                        isScrap = "N",
                        Rate = rl.Rate,
                        Hallmark_charges = 0,
                        certification_charges = 0,
                        other_charges = 0,
                        version = 0,
                        mc_for = null,
                        GSTGroupCode = null,
                        SGST_Percent = 0,
                        SGST_Amount = 0,
                        CGST_Percent = 0,
                        CGST_Amount = 0,
                        IGST_Percent = 0,
                        IGST_Amount = 0,
                        HSN = "",
                        //TID = 0,//identity column
                        UniqRowID = Guid.NewGuid(),
                        wastagedcts = 0,
                        isReceived = "N",
                        isDiamondCalculationFlag = "",
                        mc_percent = 0,
                        mc_per_piece = 0,
                        amount = 0,
                        AddWt = 0,
                        DeductWt = 0,
                        stn_gs_code = null,
                        dm_gs_code = null,
                    };
                    recLineSlNo++;
                    db.KTTU_RECEIPTS_DETAILS.Add(rd);
                }
                #endregion

                #region -2.3 Increment Receipt Series No.
                SIGlobals.Globals.IncrementDocumentNo(db, companyCode, branchCode, "15");
                #endregion

                #region -2.4 Post Receipt Stock
                List<OPGSeparationLineVM> postingLines = meltingReceipt.OPGReceiptLines.Select(
                    x => new OPGSeparationLineVM
                    {
                        GSCode = x.GSCode,
                        ItemCode = x.GSCode,
                        GrossWt = x.ReceiptGrossWt,
                        NetWt = x.ReceiptGrossWt,
                        StoneWt = 0
                    }).ToList();
                bool stockUpdated = new OPGSeparationBL().UpdateOPGGSStock(companyCode, branchCode, db, postingLines, receiptNo, false, StockTransactionType.Receipt, out error);
                if (!stockUpdated) {
                    return false;
                }
                #endregion

                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool CancelReceipt(string companyCode, string branchCode, int receiptNo, string userID, string cancelRemarks, out ErrorVM error)
        {
            error = null;
            try {
                string recType = "RL";
                var receiptMaster = db.KTTU_RECEIPTS_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                    && x.receipt_no == receiptNo && x.receipt_type == recType).FirstOrDefault();
                if (receiptMaster == null) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The document number is not found." };
                    return false;
                }
                if (receiptMaster.cflag == "Y") {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "The document is already cancelled." };
                    return false;
                }
                var applDate = Globals.GetApplicationDate(companyCode, branchCode);
                if (receiptMaster.receipt_date.Date < applDate.Date) {
                    error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = "Only today issues can be cancelled." };
                    return false;
                }
                var receiptDetail = db.KTTU_RECEIPTS_DETAILS.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                   && x.receipt_no == receiptNo).ToList();
                if (receiptDetail == null || receiptDetail.Count <= 0) {
                    error = new ErrorVM
                    {
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                        description = string.Format($"No details found for the receipt no {receiptNo}.")
                    };
                    return false;
                }

                foreach (var rd in receiptDetail) {
                    var issuedBatch = (from im in db.KTTU_ISSUE_MASTER
                                     join id in db.KTTU_ISSUE_DETAILS
                                     on new { CC = im.company_code, BC = im.branch_code, IssueNo = im.issue_no }
                                     equals new { CC = id.company_code, BC = id.branch_code, IssueNo = id.issue_no }
                                     where im.company_code == companyCode && im.branch_code == branchCode && im.cflag != "Y"
                                     && (im.issue_type == "IT" || im.issue_type == "IP")
                                     && id.batch_id == rd.batch_id
                                     select (id)).FirstOrDefault();
                    if (issuedBatch != null) {
                        error = new ErrorVM
                        {
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                            description = string.Format($"The batch Id {rd.batch_id} has already been issued to CPC via issue number: {issuedBatch.issue_no}.")
                        };
                        return false;
                    }
                }

                receiptMaster.cancelled_by = userID;
                receiptMaster.cancelled_remarks = cancelRemarks;
                receiptMaster.cflag = "Y";
                receiptMaster.UpdateOn = Globals.GetDateTime();
                db.Entry(receiptMaster).State = EntityState.Modified;

                #region Post GS Receipt Stock
               
                List<OPGMeltingIssueBatchDetailVM> opgBatchLines = receiptDetail.Select(p => new OPGMeltingIssueBatchDetailVM
                {
                    GSCode = p.gs_code,
                    GrossWt = Convert.ToDecimal(p.gwt),
                    StoneWt = Convert.ToDecimal(p.swt),
                    NetWt = Convert.ToDecimal(p.nwt)
                }).ToList();
                bool stockUpdated = new OPGMeltingIssueBL().UpdateOPGGSStock(companyCode, branchCode, db, opgBatchLines, receiptNo, postReverse: true, stockTransactionType: StockTransactionType.Receipt, error: out error);
                if (!stockUpdated) {
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

        public ProdigyPrintVM Print(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            string printData = PrintOPGMeltingReceipt(companyCode, branchCode, issueNo, out error);
            ProdigyPrintVM printObject = new ProdigyPrintVM();
            printObject.PrintType = "HTML";
            printObject.ContinueNextPrint = true;
            printObject.Data = new PrintConfiguration().Base64Encode(printData);
            return printObject;
        }

        protected string PrintOPGMeltingReceipt(string companyCode, string branchCode, int issueNo, out ErrorVM error)
        {
            error = null;
            StringBuilder sb = new StringBuilder();
            try {
                var dtReceiptMaster = (from rm in db.KTTU_RECEIPTS_MASTER
                                       join sm in db.KSTU_SUPPLIER_MASTER
                                       on new { CompanyCode = rm.company_code, BranchCode = rm.branch_code, PartyCode = rm.party_name }
                                       equals new { CompanyCode = sm.company_code, BranchCode = sm.branch_code, PartyCode = sm.party_code }
                                       where rm.company_code == companyCode && rm.branch_code == branchCode && rm.issue_no == issueNo
                                       select new
                                       {
                                           rm,
                                           sm
                                       }).FirstOrDefault();
                var dtReceiptDetails = SIGlobals.Globals.ExecuteQuery("SELECT DISTINCT " +
                                                                     "      sl_no, " +
                                                                     "      hLoss, " +
                                                                     "      KOVA_wt, " +
                                                                     "      item_no AS units, " +
                                                                     "      batch_id, " +
                                                                     "      ia.HSN, " +
                                                                     "      item_name, " +
                                                                     "      gwt, " +
                                                                     "      swt, " +
                                                                     "      nwt, " +
                                                                     "      item_amount, " +
                                                                     "      mc_per_gram, " +
                                                                     "      wastage, " +
                                                                     "      mc_amount, " +
                                                                     "      mc_type, " +
                                                                     "      wastage_type_value, " +
                                                                     "      wastageInActualWeight, " +
                                                                     "      sl_no, " +
                                                                     "      id.gs_code, " +
                                                                     "      dcts, " +
                                                                     "      ISNULL(std_weight, 0) AS std_weight, " +
                                                                     "     ISNULL(SGST_Percent, 0) AS SGST_Percent, " +
                                                                     "      ISNULL(SGST_Amount, 0) AS SGST_Amount, " +
                                                                     "      ISNULL(CGST_Percent, 0) AS CGST_Percent, " +
                                                                     "      ISNULL(CGST_Amount, 0) AS CGST_Amount, " +
                                                                     "      ISNULL(IGST_Percent, 0) AS IGST_Percent, " +
                                                                     "      ISNULL(IGST_Amount, 0) AS IGST_Amount, " +
                                                                     "      ISNULL(Amount, 0) AS Amount, " +
                                                                     "      ISNULL(" +
                                                                    " (" +
                                                                    "    SELECT SUM(SD.carrat)" +
                                                                    "    FROM KTTU_ISSUE_RECEIPTS_STONE_DETAILS SD" +
                                                                    "    WHERE SD.receipt_no = 2139" +
                                                                    "          AND SD.company_code = 'BH'" +
                                                                    "          AND SD.branch_code = 'KRM'" +
                                                                    "          AND ID.sl_no = SD.item_sno" +
                                                                    "          AND SD.receipt_no = 2139" +
                                                                    " ), 0) AS[carrat]" +
                                                                    " FROM KTTU_RECEIPTS_DETAILS ID, " +
                                                                    "     ITEM_MASTER IA" +
                                                                    " WHERE IA.gs_code = ID.gs_code" +
                                                                    "      AND receipt_no = 2139" +
                                                                    "      AND id.company_code = 'BH'" +
                                                                    "      AND id.branch_code = 'KRM'; ");

                var ReceiptType = db.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.ir_code == dtReceiptMaster.rm.receipt_type
                                                                    && ir.company_code == companyCode
                                                                    && ir.branch_code == branchCode).FirstOrDefault();

                decimal Grosswt = 0, Netwt = 0, StnWt = 0, Totatlwt = 0, PureWt = 0, Value = 0, Qty = 0, itemvalue = 0, totWastageWt = 0;
                decimal SGSTAmt = 0, CGSTAmt = 0, IGSTAmt = 0, TotalAmount = 0;

                KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
                KSTU_SUPPLIER_MASTER supplier = SIGlobals.Globals.GetSupplier(db, companyCode, branchCode, dtReceiptMaster.sm.party_code);
                KSTS_STATE_MASTER state = db.KSTS_STATE_MASTER.Where(st => st.state_name == supplier.state).FirstOrDefault();
                string stateCode = Convert.ToString(state.tinno);

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


                sb.AppendLine("<Table frame=\"border\" border=\"0\" width=\"900\" style=\"border-collapse:collapse\" >");
                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\" border-collapse:collapse; \" width=\"900\">");
                sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");

                string Address = supplier.address1 + supplier.address2 + supplier.address3 + "," + supplier.city + "," + supplier.state;

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 6 ALIGN = \"left\"><b>Consignor &nbsp&nbsp  {0}</b></TD>", supplier.party_name));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=6  ALIGN = \"left\"><b>Address &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp  {0}</b></TD>", Address));
                sb.AppendLine("</TR>");


                if (supplier.TIN != "") {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=6 ALIGN = \"left\"><b>GSTIN &nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp  {0}</b></TD>", supplier.TIN));
                    sb.AppendLine("</TR>");
                }


                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\"  style=\"border-left:thin\">");
                sb.AppendLine(string.Format("<TD width=\"450\" ALIGN = \"LEFT\"><b>DETAILS OF CONSIGNEE </b></TD>"));
                sb.AppendLine(string.Format("<TD width=\"450\" ALIGN = \"LEFT\"><b>{0}</b></TD>", "RECEIPT DETAILS"));
                sb.AppendLine("</TR>");


                sb.AppendLine("<tr>");
                sb.AppendLine("<td>");
                sb.AppendLine("<Table>");
                sb.AppendLine("<Table font-size=14pt;  class=\"boldText\"  style=\"border-collapse:collapse;\" >");

                sb.AppendLine("<tr style=\"border-right:0\"  >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left \" >Name &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + company.company_name + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >Address &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + company.address1 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + company.address2 + "</td>");
                sb.AppendLine("</tr>");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" ><b>&nbsp&nbsp</b></td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + company.address3 + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >GSTIN  &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + company.tin_no + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" align=\"left\" >State &nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \" style=\"border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + company.state_code + "</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("</table>");
                sb.AppendLine("</td>");


                sb.AppendLine("<td>");
                sb.AppendLine("<Table font-size=14pt;   class=\"boldText\"  style=\"border-collapse:collapse;\" >");
                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + issueNo + "</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Issue No &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtReceiptMaster.rm.issue_no + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >Receipt Date &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + dtReceiptMaster.rm.receipt_date + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" > Place of supply &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + company.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Received By &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + dtReceiptMaster.rm.sal_code + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" align=\"left\" >Receipt Type &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin;border-top:thin\" >" + "Melting Receipt" + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin; border-top:thin\" align=\"left\" >State &nbsp&nbsp</td>");
                sb.AppendLine("<td style=\"border-right:thin ; border-top:thin\" >" + supplier.state + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("<tr style=\"border-top:thin\" style=\"border-right:0\" >");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" align=\"left\" >State Code &nbsp&nbsp&nbsp </td>");
                sb.AppendLine("<td style=\"border-right:thin \"  style=\"border-top:thin\" >" + supplier.state_code + "</td>");
                sb.AppendLine("</tr>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("</tr>");


                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\" border-bottom:thin;\" colspan = 6 ALIGN = \"CENTER\"><b>DELIVERY CHALLAN</b></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-bottom:thin ; border-top:thin \" colspan=6 ALIGN = \"CENTER\">JOB WORK RECEIPT (NOT AS SUPPLY) </TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD  style=\"border-right:thin\" colspan=6 ALIGN = \"CENTER\">ORIGINAL/DUPLICATE<br></TD>"));
                sb.AppendLine("</TR>");

                sb.AppendLine("</table>");
                sb.AppendLine("</td>");
                sb.AppendLine("<Table font-size=14pt;  bgcolor= WHITE class=\"boldText\" frame=\"border\" border=\"1\"; style=\"border-collapse:collapse;\" width=\"900\">");
                sb.AppendLine("<TR bgcolor='#FFFACD' style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine("<th style=\"border-top:none; \" align = \"center\">S.No</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Description</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">HSN</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Batch ID</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">Gr.Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">KOVA Wt(g)</th>");
                sb.AppendLine("<th style=\"border-top:none  \" align = \"center\">LOSS</th>");
                sb.AppendLine("</TR>");
                for (int i = 0; i < dtReceiptDetails.Rows.Count; i++) {
                    sb.AppendLine("<TR style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", dtReceiptDetails.Rows[i]["sl_no"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", SIGlobals.Globals.GetBranchGSName(db, companyCode, branchCode, dtReceiptDetails.Rows[i]["item_name"].ToString()), "&nbsp"));
                    string hsn = string.Format("select hsn from ITEM_MASTER where gs_code='{0}' and company_code='{1}' and branch_code='{2}'", dtReceiptDetails.Rows[i]["item_name"].ToString(), companyCode, branchCode);
                    string hsnValue = dtReceiptDetails.Rows[0]["HSN"].ToString();
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"CENTER\">{0}{1}</TD>", hsnValue, "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtReceiptDetails.Rows[i]["batch_id"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtReceiptDetails.Rows[i]["gwt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtReceiptDetails.Rows[i]["KOVA_wt"].ToString(), "&nbsp"));
                    sb.AppendLine(string.Format("<TD style=font-size=\"14pt\" ALIGN = \"RIGHT\">{0}{1}</TD>", dtReceiptDetails.Rows[i]["wastage"].ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                    Grosswt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["gwt"]);
                    StnWt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["swt"]);
                    Netwt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["KOVA_wt"]);
                    itemvalue += Convert.ToDecimal(dtReceiptDetails.Rows[i]["item_amount"]);
                    PureWt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["std_weight"]);
                    totWastageWt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["wastage"]);

                    SGSTAmt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["SGST_Amount"]);
                    CGSTAmt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["CGST_Amount"]);
                    IGSTAmt += Convert.ToDecimal(dtReceiptDetails.Rows[i]["IGST_Amount"]);
                    TotalAmount += Convert.ToDecimal(dtReceiptDetails.Rows[i]["Amount"]);
                }
                for (int i = 0; i < 10 - dtReceiptDetails.Rows.Count - 1; i++) {
                    if (i == 0) {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin soild; border-bottom: thin ; border-right: thin \";  style=font-size=\"14pt\"  colspan = {1}>{0}</TD>", "&nbsp", dtReceiptDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                    else {
                        sb.AppendLine("<TR>");
                        sb.AppendLine(string.Format("<TD  style=\"border-left: thin ; border-top: thin ; border-bottom: thin; border-right: thin \"; style=font-size=\"14pt\" colspan = {1}>{0}</TD>", "&nbsp", dtReceiptDetails.Columns.Count));
                        sb.AppendLine("</TR>");
                    }
                }
                sb.AppendLine("<TR  style=\"color:black; text-decoration:bold;\" align=\"CENTER\">");
                sb.AppendLine(string.Format("<TD style=font-size=\"9pt\" colspan = 4 ALIGN = \"left\"><b>TOTAL </b></TD><TD ALIGN=\"RIGHT\"><b>{0}</b></TD> <TD ALIGN=\"RIGHT\"><b>{1}</b></TD><TD ALIGN=\"RIGHT\"><b>{2}</b></TD>"
                    , Grosswt, Netwt, totWastageWt));
                sb.AppendLine("</TR>");


                decimal Inword = Convert.ToDecimal(dtReceiptMaster.rm.final_amount.ToString());
                string strWords = string.Empty;
                strWords = SIGlobals.Globals.ConvertNumbertoWordsinRupees(Inword);
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin\" colspan=6 ALIGN = \"right\"><b>SGST {0} %</b></td> <td style=\"border-right:thin\" ALIGN = \"right\"><b>{1}{2}</b></TD>", dtReceiptMaster.rm.SGST_Percent.ToString(), dtReceiptMaster.rm.SGST_Amount.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-top:thin ;border-right:thin \"; colspan=6 ALIGN = \"right\"><b>CGST {0} % </b> </td> <td style=\"border-right:thin;border-top:thin \" ALIGN = \"right\" ><b>{1}{2}</b></TD>", dtReceiptMaster.rm.CGST_Percent.ToString(), dtReceiptMaster.rm.CGST_Amount.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-right:thin \"; colspan=6 ALIGN = \"right\"><b>IGST {0} %</b> </td> <td style=\"border-right:thin ;border-top:thin\" ALIGN = \"right\" ><b>{1}{2}</b></TD>", dtReceiptMaster.rm.IGST_Percent.ToString(), dtReceiptMaster.rm.IGST_Amount.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-right:thin \"; colspan=6 ALIGN = \"right\"><b>Total Amount</b></td><td style=\"border-right:thin;border-top:thin \" ALIGN = \"right\" > <b>{0}{1}</b></TD>", dtReceiptMaster.rm.grand_total.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-right:thin \"; colspan=6 ALIGN = \"right\"><b>Round Off</b></td><td style=\"border-right:thin;border-top:thin \" ALIGN = \"right\" > <b>{0}{1}</b></TD>", dtReceiptMaster.rm.round_off.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-top:thin;border-right:thin \"; colspan=6 ALIGN = \"right\"><b>Final Amount</b></td><td style=\"border-right:thin;border-top:thin \" ALIGN = \"right\" > <b>{0}{1}</b></TD>", dtReceiptMaster.rm.final_amount.ToString(), "&nbsp"));
                sb.AppendLine("</TR>");

                if (!string.IsNullOrEmpty(dtReceiptMaster.rm.remarks.ToString())) {
                    sb.AppendLine("<TR>");
                    sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=7 ALIGN = \"left\">{0}{1}{1}</TD>", "Remarks : " + dtReceiptMaster.rm.remarks.ToString(), "&nbsp"));
                    sb.AppendLine("</TR>");
                }

                sb.AppendLine("<TR>");
                sb.AppendLine(string.Format("<TD style=\"border-right:thin \"; colspan=7 ALIGN = \"left\">{0}{1}{1}</TD>", strWords, "&nbsp"));
                sb.AppendLine("</TR>");
                sb.AppendLine("<TR>");
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format("<td style=\"border-right:thin\" colspan=4  align = \"left\"  ><b>{0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Signature of Consignee "));
                sb.AppendLine(string.Format("<td  colspan=4  align = \"right\"  ><b> {0}&nbsp<br><br><br>{1}&nbsp</b></td>", "&nbsp", "Authorized Signatory"));
                sb.AppendLine("</tr>");
                sb.AppendLine("</table>");
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

    }
}
