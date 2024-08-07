using ProdigyAPI.BL.BusinessLayer.Sales;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 2021-05-19
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    /// <summary>
    /// This Provides Methods related to Account Voucher Entry Module
    /// </summary>
    public class ExpanseVoucherEntryBL
    {
        #region Delcaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructor
        public ExpanseVoucherEntryBL()
        {
            dbContext = new MagnaDbEntities();
        }

        public ExpanseVoucherEntryBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Methods
        public dynamic GetVendorMaster(string companyCode, string branchCode, string ledgerType, out ErrorVM error)
        {
            error = null;
            try {
                if (ledgerType.Equals("J")) {
                    if (SIGlobals.Globals.GetConfigurationValue(dbContext, "04022021", companyCode, branchCode) == 1) {
                        var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                                              && exp.branch_code == branchCode
                                                                                              && exp.obj_status != "C"
                                                                                              && (exp.ledger_type == "CR" || exp.tin != "")).Select(ex => new
                                                                                              {
                                                                                                  Code = ex.acc_code,
                                                                                                  Name = ex.acc_name
                                                                                              }).OrderBy(exp => exp.Name);
                        return data;
                    }
                    else {
                        var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                       && exp.branch_code == branchCode
                                                                       && exp.obj_status != "C"
                                                                       && exp.tin != "").Select(ex => new
                                                                       {
                                                                           Code = ex.acc_code,
                                                                           Name = ex.acc_name
                                                                       }).OrderBy(exp => exp.Name);
                        return data;
                    }

                }
                else if (ledgerType.Equals("C")) {
                    var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                       && exp.branch_code == branchCode
                                                                       && exp.obj_status != "C"
                                                                       && exp.ledger_type == "C").Select(ex => new
                                                                       {
                                                                           Code = ex.acc_code,
                                                                           Name = ex.acc_name
                                                                       }).OrderBy(exp => exp.Name);
                    return data;
                }
                else if (ledgerType.Equals("B")) {
                    var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                       && exp.branch_code == branchCode
                                                                       && exp.obj_status != "C"
                                                                       && exp.ledger_type == "B").Select(ex => new
                                                                       {
                                                                           Code = ex.acc_code,
                                                                           Name = ex.acc_name
                                                                       }).OrderBy(exp => exp.Name);
                    return data;
                }
                return null;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetLedgers(string companyCode, string branchCode, string ledgerType, out ErrorVM error)
        {
            error = null;
            try {
                if (ledgerType == "J") {
                    string[] types = { "E" };
                    var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                   && exp.branch_code == branchCode
                                                                   && exp.obj_status != "C"
                                                                   && types.Contains(exp.v_type)
                                                                   && exp.tin != "").Select(ex => new
                                                                   {
                                                                       Code = ex.acc_code,
                                                                       Name = ex.acc_name
                                                                   }).OrderBy(ord => ord.Name).ToList();
                    return data;
                }
                else if (ledgerType == "C") {
                    string[] types = { "E", "S" };
                    var data = dbContext.vGetexpenseledgers.Where(exp => exp.company_code == companyCode
                                                                   && exp.branch_code == branchCode
                                                                   && exp.obj_status != "C"
                                                                   && types.Contains(exp.v_type)
                                                                   && exp.tin != "").Select(ex => new
                                                                   {
                                                                       Code = ex.acc_code,
                                                                       Name = ex.acc_name
                                                                   }).OrderBy(ord => ord.Name).ToList();

                    return data;
                }
                else {
                    var data = SIGlobals.Globals.ExecuteQuery("SELECT acc_code as Code," +
                                                                " acc_name as Name" +
                                                                " FROM vGetexpenseledger" +
                                                                " WHERE COMPANY_CODE = '" + companyCode + "'" +
                                                                "      AND BRANCH_CODE ='" + branchCode + "'" +
                                                                "      AND obj_status != 'C'" +
                                                                "      AND v_type IN('E')" +
                                                                " AND ledger_type != 'B'" +
                                                                " UNION" +
                                                                " SELECT acc_code as Code," +
                                                                "       acc_name as Name" +
                                                                " FROM vGetexpenseledger el," +
                                                                "     KSTU_SUPPLIER_MASTER sm" +
                                                                " WHERE el.party_code = sm.party_code" +
                                                                "      AND el.COMPANY_CODE = '" + companyCode + "'" +
                                                                "      AND el.BRANCH_CODE = '" + branchCode + "'" +
                                                                "      AND el.obj_status != 'C'" +
                                                                "      AND v_type IN('E', 'S')" +
                                                                " AND ledger_type != 'B'" +
                                                                " ORDER BY ACC_NAME; ");
                    return data;
                }
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public IEnumerable<TDSExpanseDetailsVM> GetExpanseLedgerEntry(string companyCode, string branchCode, DateTime dateTime, out ErrorVM error)
        {
            error = null;
            try {

                var data = (from ted in dbContext.KTTU_TDS_EXPENSE_DETAILS
                            join alm in dbContext.KSTU_ACC_LEDGER_MASTER on new
                            {
                                CompanyCode = ted.company_code,
                                BranchCode = ted.branch_code,
                                AccCode = ted.acc_code
                            } equals new
                            {
                                CompanyCode = alm.company_code,
                                BranchCode = alm.branch_code,
                                AccCode = alm.acc_code
                            }
                            where ted.company_code == companyCode
                                  && ted.branch_code == branchCode
                                  && ted.cflag == "N"
                                  && ted.acc_type == "O"
                                  && DbFunctions.TruncateTime(ted.expense_date) == DbFunctions.TruncateTime(dateTime.Date)
                            select new TDSExpanseDetailsVM()
                            {
                                SNo = ted.sno,
                                ObjID = ted.obj_id,
                                ExpenseNo = ted.expense_no,
                                CompanyCode = ted.company_code,
                                BranchCode = ted.branch_code,
                                SupplierCode = ted.suppliercode == null ? "" : ted.suppliercode,
                                SupplierName = ted.suppliername,
                                ObjStatus = ted.obj_status,
                                AccCode = ted.acc_code,
                                AccName = ted.acc_name,
                                InvoiceNo = ted.invoiceno,
                                Description = ted.description,
                                InvoiceDate = ted.invoicedate,
                                Amount = ted.amount,
                                CGSTAmount = ted.CGST_Amount,
                                CGSTPercent = ted.CGST_Percent,
                                SGSTAmount = ted.SGST_Amount,
                                SGSTPercent = ted.SGST_Percent,
                                IGSTAmount = ted.IGST_Amount,
                                IGSTPercent = ted.IGST_Percent,
                                CESSAmount = ted.CESS_Amount,
                                GSTGroupCode = ted.GSTGroupCode,
                                AccType = ted.acc_type,
                                TotalAmount = ted.total_amount,
                                RoundOff = ted.roundoff,
                                FinalAmount = ted.final_Amount,
                                TDSPercentage = ted.tds_precentage,
                                TDSAmount = ted.tds_amount,
                                ChqNo = ted.chq_no,
                                ChqDate = ted.chq_date,
                                PartyCode = ted.party_code,
                                OperatorCode = ted.operator_code,
                                HSN = ted.HSN == null ? "" : ted.HSN,
                                AccountType = ted.acc_type,
                                HSNDescription = ted.hsnDescription,
                                IsEligible = ted.IsEligible,
                                TaxName = ted.tax_name == null ? "" : ted.tax_name,
                                CFlag = ted.cflag == null ? "" : ted.cflag,
                                CancelledBy = ted.cancelled_by == null ? "" : ted.cancelled_by,
                                GSTNo = ted.GST_No == null ? "" : ted.GST_No,
                                OtherPartyName = ted.Other_Party_Name == null ? "" : ted.Other_Party_Name,
                                Remarks = ted.remarks == null ? "" : ted.remarks,
                                PANNo = ted.PAN_No == null ? "" : ted.PAN_No
                            }).ToList();

                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetTDSLedger(string companyCode, string branchCode, int? supplierCode, out ErrorVM error)
        {
            error = null;
            var tdsMaster = dbContext.KSTS_TDS_MASTER.Where(tds => tds.company_code == companyCode && tds.branch_code == branchCode)
                                                       .Select(tds => tds.tds_id).ToList();
            try {
                if (supplierCode == null) {

                    var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                            && led.branch_code == branchCode
                                                                            && led.jflag == "T" && tdsMaster.Contains(led.tds_id ?? 0)).Select(led => new
                                                                            {
                                                                                Code = led.acc_code,
                                                                                Name = led.acc_name
                                                                            }).ToList().OrderBy(led => led.Name);
                    return data;
                }
                else {
                    var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                           && led.branch_code == branchCode
                                                                           && led.acc_code == supplierCode && tdsMaster.Contains(led.tds_id ?? 0)).Select(led => new
                                                                           {
                                                                               Code = led.acc_code,
                                                                               Name = led.acc_name
                                                                           }).ToList().OrderBy(led => led.Name);
                    return data;
                }
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return excp;
            }
        }

        public dynamic GetCessLedger(string companyCode, string branchCode, int? supplierCode, out ErrorVM error)
        {
            error = null;
            try {
                var cessMaster = dbContext.KSTS_CESS_MASTER.Where(cess => cess.company_code == companyCode && cess.branch_code == branchCode)
                .Select(tds => tds.cess_id).ToList();
                if (supplierCode == null) {
                    var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                       && led.branch_code == branchCode
                                                                       && cessMaster.Contains(led.cess_id ?? 0)).Select(led => new
                                                                       {
                                                                           Code = led.acc_code,
                                                                           Name = led.acc_name
                                                                       }).ToList().OrderBy(led => led.Name);
                    return data;
                }
                else {
                    var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                      && led.branch_code == branchCode
                                                                      && led.acc_code == supplierCode && cessMaster.Contains(led.cess_id ?? 0)).Select(led => new
                                                                      {
                                                                          Code = led.acc_code,
                                                                          Name = led.acc_name
                                                                      }).ToList().OrderBy(led => led.Name);
                    return data;
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetGSTGroupCode(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                if (SIGlobals.Globals.GetConfigurationValue(dbContext, "04102018", companyCode, branchCode) == 0) {
                    var data = dbContext.GSTGroups.Where(gst => gst.IsActive == true).ToList();
                    return data;
                }
                else {
                    //SELECT code, h.[Description] as [Desc]FROM dbo.gstgroup h where isActive= 1 and code between 0 and 28
                    // The abover query will run like bellow when I use LINQ
                    //SELECT code, h.[Description] as [Desc]FROM dbo.gstgroup h where isActive= 1 and code between '0' and '28'
                    // Am getting only 3 records, If i CompareTo(0) used bellow it throw error related to DataType so i have ran that query and returning the data.
                    //var data = dbContext.GSTGroups.Where(gst => gst.IsActive == true
                    //                                                         && gst.Code.CompareTo("0") >= 0
                    //                                                         && gst.Code.CompareTo("28") <= 0).ToList();
                    //return data;

                    var data = SIGlobals.Globals.ExecuteQuery("SELECT code, h.[Description] as [Desc] FROM dbo.gstgroup h where isActive= 1 and code between 0 and 28");
                    return data;
                }
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetTINandPAN(string companyCode, string branchCode, int supplierCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.vGetexpenseledgers.Where(led => led.company_code == companyCode
                                                                && led.branch_code == branchCode
                                                                && led.acc_code == supplierCode)
                                                       .Select(led => new
                                                       {
                                                           TIN = led.tin,
                                                           PAN = led.pan
                                                       });
                return data;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }

        }

        public TDSExpanseDetailsVM CalculateTDSAndGST(TDSExpanseDetailsVM tds, out ErrorVM error)
        {
            error = null;
            try {
                decimal Amount = Convert.ToDecimal(tds.Amount);
                decimal tdsPercentage = Convert.ToDecimal(tds.TDSPercentage);
                decimal tdsAmount = 0;
                if (SIGlobals.Globals.GetConfigurationValue(dbContext, "23052019", tds.CompanyCode, tds.BranchCode) == 0) {
                    tdsAmount = Math.Round(Amount * (tdsPercentage / 100), 0, MidpointRounding.ToEven);
                }
                else {
                    tdsAmount = Math.Round(Amount * (tdsPercentage / 100), 0, MidpointRounding.ToEven);
                }

                if (SIGlobals.Globals.GetConfigurationValue(dbContext, "18022021", tds.CompanyCode, tds.BranchCode) == 1) {
                    tdsAmount = Math.Ceiling(Amount * (tdsPercentage / 100));
                    tds.TDSAmount = tdsAmount;
                }
                else {
                    tds.TDSAmount = tdsAmount;
                }
                return tds;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool SaveExpanseLedgerEntry(List<TDSExpanseDetailsVM> expDet, out ErrorVM error, out int expenseNumber)
        {
            error = null;
            expenseNumber = 0;
            int expenseNo = 0;

            #region Basic Validation
            if (expDet.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            #endregion

            #region Advanced Validation
            #endregion

            #region Saving
            string companyCode = expDet[0].CompanyCode;
            string branchCode = expDet[0].BranchCode;
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int finYear = SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode);
            int finAttach = Convert.ToInt32(finYear.ToString().Substring(0, 2));
            KSTU_COMPANY_MASTER company = dbContext.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            using (var transaction = dbContext.Database.BeginTransaction()) {
                try {
                    int voucherSeqNo = 1;
                    int voucherNo = Convert.ToInt32(finAttach + "" + SIGlobals.Globals.GetNewSerialNo(dbContext, "19", companyCode, branchCode));
                    foreach (var tds in expDet) {
                        KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();

                        #region Expense Details
                        KTTU_TDS_EXPENSE_DETAILS tdsExp = new KTTU_TDS_EXPENSE_DETAILS();
                        expenseNo = Convert.ToInt32(finAttach + "" + SIGlobals.Globals.GetNewTransactionSerialNo(dbContext, "120", companyCode, branchCode));
                        string tempObjectID = "KTTU_TDS_EXPENSE_DETAILS"
                                              + SIGlobals.Globals.Separator + expenseNo
                                              + SIGlobals.Globals.Separator + companyCode
                                              + SIGlobals.Globals.Separator + tds.BranchCode;

                        tdsExp.obj_id = SIGlobals.Globals.GetHashcode(tempObjectID);
                        tdsExp.expense_no = expenseNo;
                        tdsExp.expense_date = tds.ExpenseDate == DateTime.MinValue ? appDate : tds.ExpenseDate;
                        tdsExp.company_code = tds.CompanyCode;
                        tdsExp.branch_code = tds.BranchCode;
                        tdsExp.suppliercode = tds.SupplierCode;
                        tdsExp.suppliername = tds.SupplierName;
                        tdsExp.sno = tds.SNo;
                        tdsExp.acc_code = tds.AccCode;
                        tdsExp.acc_name = tds.AccName;
                        tdsExp.invoiceno = tds.InvoiceNo;
                        tdsExp.invoicedate = tds.InvoiceDate;
                        tdsExp.amount = tds.Amount;
                        tdsExp.tax_name = tds.TaxName;
                        tdsExp.tax_percntage = tds.TaxPercentage;
                        tdsExp.tax_amount = tds.TaxAmount;
                        tdsExp.total_amount = tds.TotalAmount;
                        tdsExp.tds_precentage = tds.TDSPercentage;
                        tdsExp.tds_amount = tds.TDSAmount;
                        tdsExp.cflag = tds.CFlag;
                        tdsExp.cancelled_by = tds.CancelledBy;
                        tdsExp.operator_code = tds.OperatorCode;
                        tdsExp.description = tds.Description;
                        tdsExp.obj_status = tds.ObjStatus;
                        tdsExp.SGST_Percent = tds.SGSTPercent;
                        tdsExp.SGST_Amount = tds.SGSTAmount;
                        tdsExp.CGST_Percent = tds.CGSTPercent;
                        tdsExp.CGST_Amount = tds.CGSTAmount;
                        tdsExp.IGST_Percent = tds.IGSTPercent;
                        tdsExp.IGST_Amount = tds.IGSTAmount;
                        tdsExp.HSN = tds.HSN;
                        tdsExp.GSTGroupCode = tds.GSTGroupCode;
                        tdsExp.acc_type = tds.AccType;
                        tdsExp.chq_no = tds.ChqNo;
                        tdsExp.chq_date = tds.ChqDate == null ? appDate : tds.ChqDate;
                        tdsExp.GST_No = tds.GSTNo;
                        tdsExp.Other_Party_Name = tds.OtherPartyName;
                        tdsExp.party_code = tds.PartyCode;
                        tdsExp.CESS_Percent = tds.CESSPercent;
                        tdsExp.CESS_Amount = tds.CESSAmount;
                        tdsExp.remarks = tds.Remarks;
                        tdsExp.PAN_No = tds.PANNo;
                        tdsExp.IsEligible = tds.IsEligible.ToUpper() == "TRUE" ? "Y" : "N";
                        tdsExp.hsnDescription = tds.HSNDescription;
                        tdsExp.roundoff = tds.RoundOff;
                        tdsExp.final_Amount = tds.FinalAmount;
                        dbContext.KTTU_TDS_EXPENSE_DETAILS.Add(tdsExp);
                        dbContext.SaveChanges();
                        SIGlobals.Globals.UpdateSeqenceNumber(dbContext, "120", companyCode, branchCode);
                        #endregion

                        #region Account Vouther Details

                        int txtSeqNo = Convert.ToInt32(finAttach + "" + SIGlobals.Globals.GetNewSerialNo(dbContext, "03", companyCode, branchCode));
                        int registered = 0;
                        bool? isRegistered = false;
                        bool isInterstate = false;
                        int? smithStateCode = 0;

                        decimal cgstAmount = 0;
                        decimal sgstAmount = 0;
                        decimal igstAmount = 0;
                        decimal cessAmount = 0;

                        decimal cgstPercent = 0;
                        decimal sgstPercent = 0;
                        decimal igstPercent = 0;
                        decimal cessPercent = 0;

                        decimal TotalAmount = 0;
                        decimal Amount = 0;

                        string gstCode = tds.GSTGroupCode;
                        int tdsid = tds.TDSID;
                        int cessAccCode = tds.CessAccountCode;
                        string HSNCode = string.Empty;
                        string GSTServiceCode = string.Empty;
                        string isEligable = "Y";

                        int supplierCode = Convert.ToInt32(tds.SupplierCode);


                        var accLedgerDet = dbContext.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.company_code == tds.CompanyCode
                                                                                    && acc.branch_code == tds.BranchCode
                                                                                    && acc.acc_code == tds.AccCode).FirstOrDefault();
                        smithStateCode = accLedgerDet.state_code;
                        if (company.state_code != smithStateCode) {
                            isInterstate = true;
                        }
                        new SalesEstimationBL().GetGSTComponentValues(gstCode, tds.Amount, isInterstate, out sgstPercent, out sgstAmount,
                            out cgstPercent, out cgstAmount, out igstPercent, out igstAmount, out cessPercent, out cessAmount, companyCode, branchCode);

                        cgstAmount = Convert.ToDecimal(tds.CGSTAmount);
                        sgstAmount = Convert.ToDecimal(tds.SGSTAmount);
                        igstAmount = Convert.ToDecimal(tds.IGSTAmount);

                        HSNCode = tds.HSN;

                        // As per magna neet to take GSTServiceGroupCode Column, But its not available, in magna it throw an error
                        //GSTServiceCode = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.ir_code == "RM" && ir.company_code == companyCode && ir.branch_code == branchCode).FirstOrDefault().gst

                        //var supplierType = dbContext.vGetexpenseledgers.Where(led => led.company_code == tds.CompanyCode
                        //                                                            && led.branch_code == tds.BranchCode
                        //                                                            && led.acc_code == supplierCode).FirstOrDefault();
                        //if (supplierType != null) {
                        //    if (supplierType.tin != "") {
                        //        registered = 1;
                        //        isRegistered = true;
                        //        kavt.narration = "Expense Voucher Registered";
                        //    }
                        //    else {
                        //        registered = 0;
                        //        isRegistered = false;
                        //        kavt.narration = "Expense Voucher Un-Registered";
                        //    }
                        //}

                        //if (tds.AccountType == "J") {
                        //    kavt.acc_code_master = 0;
                        //    kavt.acc_type = "J";
                        //}
                        //else if (tds.AccountType == "C") {
                        //    kavt.acc_code_master = supplierCode;
                        //    kavt.acc_type = "J";
                        //}
                        //else {
                        //    kavt.acc_code_master = supplierCode;
                        //    kavt.acc_type = "J";
                        //}
                        //string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS" + SIGlobals.Globals.Separator
                        //              + voucherNo + SIGlobals.Globals.Separator + kavt.acc_type
                        //              + SIGlobals.Globals.Separator + kavt.acc_code_master
                        //              + SIGlobals.Globals.Separator + kavt.trans_type
                        //              + SIGlobals.Globals.Separator + companyCode
                        //              + SIGlobals.Globals.Separator + branchCode;

                        //kavt.obj_id = SIGlobals.Globals.GetHashcode(temp);
                        //kavt.company_code = companyCode;
                        //kavt.branch_code = branchCode;
                        //kavt.txt_seq_no = txtSeqNo;
                        //kavt.voucher_no = voucherNo;
                        //kavt.New_voucher_no = voucherNo;
                        //kavt.party_name = tds.SupplierName;
                        //kavt.receipt_no = Convert.ToString(expenseNo);
                        //kavt.cflag = "N";
                        //kavt.acc_code = supplierCode;
                        //kavt.cflag = "N";
                        //kavt.branch_code = branchCode;
                        //kavt.cflag = "N";
                        //kavt.company_code = companyCode;
                        //kavt.contra_seq_no = 0;
                        //kavt.fin_period = finYear;
                        //kavt.fin_year = finYear;
                        //kavt.is_approved = "N";
                        //kavt.Is_Authorized = "N";
                        //kavt.is_tds = "N";
                        //kavt.Is_Verified = "N";
                        //kavt.reconsile_flag = "N";
                        //kavt.section_id = "0";
                        //kavt.subledger_acc_code = 0;
                        //kavt.TDS_amount = 0;
                        //kavt.trans_type = "EXP";
                        //kavt.voucher_type = "EV";
                        //kavt.chq_no = "0";

                        //kavt.voucher_date = appDate;
                        //kavt.chq_date = appDate;
                        //kavt.reconsile_date = appDate;
                        //kavt.inserted_on = appDate;
                        //kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                        //kavt.approved_date = appDate;
                        //kavt.Verified_Date = appDate;
                        //kavt.Authorized_Date = appDate;
                        //kavt.cancelled_date = appDate;
                        //kavt.narration = tds.Description;
                        //kavt.UniqRowID = Guid.NewGuid();

                        if (tds.AccountType == "C" && tds.AccountType == "B") {
                            for (int j = 0; j < 6; j++) {
                                if (j == 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    TotalAmount = tds.Amount;

                                    if (tds.IsEligible.Equals("N"))
                                        Amount = kavt.dr_amount = TotalAmount +
                                                +sgstAmount + cgstAmount + igstAmount;
                                    else
                                        Amount = kavt.dr_amount = Convert.ToDecimal(tds.Amount);
                                }


                                else if (j == 1 && sgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, sgstPercent, isRegistered, "SGST", companyCode, branchCode);

                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "SGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N"))
                                        Amount = kavt.dr_amount = sgstAmount;
                                    else
                                        Amount = kavt.dr_amount = 0;
                                }

                                else if (j == 2 && cgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "CGST", companyCode, branchCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N")) {
                                        Amount = kavt.dr_amount = cgstAmount;
                                    }
                                    else {
                                        Amount = kavt.dr_amount = 0;
                                    }
                                }
                                else if (j == 3 && igstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "IGST", companyCode, branchCode);

                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N")) {
                                        Amount = kavt.dr_amount = cgstAmount;
                                    }
                                    else {
                                        Amount = kavt.dr_amount = 0;
                                    }

                                }
                                else if (j == 4 && Convert.ToDecimal(tds.CESSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = cessAccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.CESSAmount);

                                }
                                else if (j == 5 && Convert.ToDecimal(tds.RoundOff) != 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    decimal roundoff = Convert.ToDecimal(tds.RoundOff);
                                    if (roundoff != 0) {
                                        if (roundoff > 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            Amount = kavt.cr_amount = roundoff;
                                            kavt.dr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";

                                        }
                                        else if (roundoff < 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            Amount = kavt.dr_amount = -roundoff;
                                            kavt.cr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";
                                        }
                                    }

                                }
                                else {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.cr_amount = 0;
                                    kavt.dr_amount = 0;
                                    Amount = 0;
                                }
                                dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                                SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);
                                voucherSeqNo++;
                            }
                        }
                        else {
                            for (int j = 0; j < 9; j++) {
                                if (j == 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    TotalAmount = tds.Amount;
                                    if (isEligable != "Y")
                                        Amount = kavt.dr_amount = TotalAmount +
                                            +sgstAmount + cgstAmount + igstAmount;
                                    else
                                        Amount = kavt.dr_amount = tds.Amount;
                                }
                                else if (j == 1 && sgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, sgstPercent, isRegistered, "SGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "SGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                        return false;
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (isEligable.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;
                                }

                                else if (j == 2 && cgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "CGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                        return false;
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (isEligable.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;

                                }
                                else if (j == 3 && igstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, igstPercent, isRegistered, "IGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "IGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                        return false;
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;

                                }
                                else if (j == 4 && Convert.ToDecimal(tds.CESSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = cessAccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.CESSAmount);

                                }

                                else if (j == 5) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    Amount = kavt.cr_amount = Convert.ToDecimal(tds.FinalAmount);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.dr_amount = 0;
                                }

                                else if (j == 6 && Convert.ToDecimal(tds.TDSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.TDSAmount);

                                }

                                else if (j == 7 && Convert.ToDecimal(tds.TDSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetAccountCodeForTDSExpense(dbContext, tdsid, companyCode, branchCode);
                                    Amount = kavt.cr_amount = Convert.ToDecimal(tds.TDSAmount);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.dr_amount = 0;

                                }
                                else if (j == 8 && Convert.ToDecimal(tds.RoundOff) != 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    decimal roundoff = Convert.ToDecimal(tds.RoundOff);
                                    if (roundoff != 0) {
                                        if (roundoff > 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);


                                            if (kavt.acc_code == 0) {
                                                error = new ErrorVM()
                                                {
                                                    description = string.Format("Please Map the  AccCode For {0} in Acc Code Master", "45"),
                                                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                                };
                                                return false;
                                            }
                                            Amount = kavt.cr_amount = roundoff;
                                            kavt.dr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";
                                            kavt.voucher_seq_no = voucherSeqNo;

                                        }
                                        else if (roundoff < 0) {
                                            kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            if (kavt.acc_code == 0) {
                                                error = new ErrorVM()
                                                {
                                                    description = string.Format("Please Map the  AccCode For {0} in Acc Code Master", "45"),
                                                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                                };
                                                return false;
                                            }
                                            Amount = kavt.dr_amount = -roundoff;
                                            kavt.cr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";
                                            kavt.voucher_seq_no = voucherSeqNo;
                                        }
                                    }

                                }
                                else {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.cr_amount = 0;
                                    kavt.dr_amount = 0;
                                    Amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                }
                                dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                                dbContext.SaveChanges();
                                SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);
                                voucherSeqNo++;
                            }
                        }
                        SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "19", companyCode, branchCode);
                        #endregion

                        #region Update Cheque List
                        KSTU_ACC_CHEQUE_MASTER chqMaster = dbContext.KSTU_ACC_CHEQUE_MASTER.Where(chq => chq.company_code == companyCode
                                                                                                    && chq.branch_code == branchCode
                                                                                                    && chq.chq_no == tds.ChqNo).FirstOrDefault();
                        if (chqMaster != null) {
                            chqMaster.chq_issued = "Y";
                            chqMaster.chq_issue_date = appDate;
                            dbContext.Entry(chqMaster).State = System.Data.Entity.EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                        #endregion
                    }

                    transaction.Commit();
                    expenseNumber = expenseNo;
                    return true;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return false;
                }
            }
            #endregion
        }

        public bool UpdateExpanseLedgerEntry(int expNo, List<TDSExpanseDetailsVM> expDet, out ErrorVM error)
        {
            error = null;

            #region Basic Validation
            if (expDet.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Details",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            #endregion

            #region Advanced Validation
            #endregion

            #region Updation, Deletion and Re-Inserting
            string companyCode = expDet[0].CompanyCode;
            string branchCode = expDet[0].BranchCode;
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int finYear = SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode);
            int finAttach = Convert.ToInt32(finYear.ToString().Substring(0, 2));
            KSTU_COMPANY_MASTER company = dbContext.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            using (var transaction = dbContext.Database.BeginTransaction()) {
                try {
                    int voucherSeqNo = 1;
                    int voucherNo = 0;
                    int expenseNo = expNo;
                    foreach (var tds in expDet) {
                        KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();

                        #region Expense Details
                        KTTU_TDS_EXPENSE_DETAILS tdsExp = dbContext.KTTU_TDS_EXPENSE_DETAILS.Where(exp => exp.expense_no == expNo
                                                                                                && exp.sno == tds.SNo
                                                                                                && exp.company_code == companyCode
                                                                                                && exp.branch_code == branchCode).FirstOrDefault();
                        tdsExp.expense_no = tds.ExpenseNo;
                        tdsExp.expense_date = tds.ExpenseDate;
                        tdsExp.company_code = tds.CompanyCode;
                        tdsExp.branch_code = tds.BranchCode;
                        tdsExp.suppliercode = tds.SupplierCode;
                        tdsExp.suppliername = tds.SupplierName;
                        tdsExp.sno = tds.SNo;
                        tdsExp.acc_code = tds.AccCode;
                        tdsExp.acc_name = tds.AccName;
                        tdsExp.invoiceno = tds.InvoiceNo;
                        tdsExp.invoicedate = tds.InvoiceDate;
                        tdsExp.amount = tds.Amount;
                        tdsExp.tax_name = tds.TaxName == null ? "" : tds.TaxName;
                        tdsExp.tax_percntage = tds.TaxPercentage;
                        tdsExp.tax_amount = tds.TaxAmount;
                        tdsExp.total_amount = tds.TotalAmount;
                        tdsExp.tds_precentage = tds.TDSPercentage;
                        tdsExp.tds_amount = tds.TDSAmount;
                        tdsExp.cflag = tds.CFlag == null ? "N" : tds.CFlag;
                        tdsExp.cancelled_by = tds.CancelledBy;
                        tdsExp.operator_code = tds.OperatorCode;
                        tdsExp.description = tds.Description;
                        tdsExp.obj_status = tds.ObjStatus;
                        tdsExp.SGST_Percent = tds.SGSTPercent;
                        tdsExp.SGST_Amount = tds.SGSTAmount;
                        tdsExp.CGST_Percent = tds.CGSTPercent;
                        tdsExp.CGST_Amount = tds.CGSTAmount;
                        tdsExp.IGST_Percent = tds.IGSTPercent;
                        tdsExp.IGST_Amount = tds.IGSTAmount;
                        tdsExp.HSN = tds.HSN;
                        tdsExp.GSTGroupCode = tds.GSTGroupCode;
                        tdsExp.acc_type = tds.AccType;
                        tdsExp.chq_no = tds.ChqNo;
                        tdsExp.chq_date = tds.ChqDate;
                        tdsExp.GST_No = tds.GSTNo;
                        tdsExp.Other_Party_Name = tds.OtherPartyName;
                        tdsExp.party_code = tds.PartyCode;
                        tdsExp.CESS_Percent = tds.CESSPercent;
                        tdsExp.CESS_Amount = tds.CESSAmount;
                        tdsExp.remarks = tds.Remarks;
                        tdsExp.PAN_No = tds.PANNo;
                        tdsExp.IsEligible = tds.IsEligible.ToUpper() == "TRUE" ? "Y" : "N";
                        tdsExp.hsnDescription = tds.HSNDescription;
                        tdsExp.roundoff = tds.RoundOff;
                        tdsExp.final_Amount = tds.FinalAmount;
                        dbContext.Entry(tdsExp).State = System.Data.Entity.EntityState.Modified;
                        dbContext.SaveChanges();
                        #endregion

                        #region Account Vouther Details

                        #region Deleting Existing Records
                        string strExpNo = Convert.ToString(expNo);
                        int txtSeqNo = 0;
                        List<KSTU_ACC_VOUCHER_TRANSACTIONS> delKavt = dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.receipt_no == strExpNo
                                                                                                                     && acc.company_code == companyCode
                                                                                                                     && acc.branch_code == branchCode
                                                                                                                     && acc.trans_type == "EXP").ToList();
                        if (delKavt.Count > 0) {
                            voucherNo = delKavt[0].voucher_no;
                            txtSeqNo = delKavt[0].txt_seq_no;
                            dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.RemoveRange(delKavt);
                        }
                        #endregion


                        int registered = 0;
                        bool? isRegistered = false;
                        bool isInterstate = false;
                        int? smithStateCode = 0;

                        decimal cgstAmount = 0;
                        decimal sgstAmount = 0;
                        decimal igstAmount = 0;
                        decimal cessAmount = 0;

                        decimal cgstPercent = 0;
                        decimal sgstPercent = 0;
                        decimal igstPercent = 0;
                        decimal cessPercent = 0;

                        decimal TotalAmount = 0;
                        decimal Amount = 0;

                        string gstCode = tds.GSTGroupCode;
                        int tdsid = tds.TDSID;
                        int cessAccCode = tds.CessAccountCode;
                        string HSNCode = string.Empty;
                        string GSTServiceCode = string.Empty;

                        int supplierCode = Convert.ToInt32(tds.SupplierCode);
                        var supplierType = dbContext.vGetexpenseledgers.Where(led => led.company_code == tds.CompanyCode
                                                                                    && led.branch_code == tds.BranchCode
                                                                                    && tds.AccCode == supplierCode).FirstOrDefault();
                        if (supplierType != null) {
                            if (supplierType.tin != "") {
                                registered = 1;
                                isRegistered = true;
                                kavt.narration = "Expense Voucher Registered";
                            }
                            else {
                                registered = 0;
                                isRegistered = false;
                                kavt.narration = "Expense Voucher Un-Registered";
                            }
                        }

                        var accLedgerDet = dbContext.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.company_code == tds.CompanyCode
                                                                                    && acc.branch_code == tds.BranchCode
                                                                                    && acc.acc_code == tds.AccCode).FirstOrDefault();
                        smithStateCode = accLedgerDet.state_code;
                        if (company.state_code != smithStateCode) {
                            isInterstate = true;
                        }
                        new SalesEstimationBL().GetGSTComponentValues(gstCode, tds.Amount, isInterstate, out sgstPercent, out sgstAmount,
                            out cgstPercent, out cgstAmount, out igstPercent, out igstAmount, out cessPercent, out cessAmount, companyCode, branchCode);

                        cgstAmount = Convert.ToDecimal(tds.CGSTAmount);
                        sgstAmount = Convert.ToDecimal(tds.SGSTAmount);
                        igstAmount = Convert.ToDecimal(tds.IGSTAmount);

                        HSNCode = tds.HSN;

                        // As per magna neet to take GSTServiceGroupCode Column, But its not available, in magna it throw an error
                        //GSTServiceCode = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.ir_code == "RM" && ir.company_code == companyCode && ir.branch_code == branchCode).FirstOrDefault().gst

                        //if (tds.AccountType == "J") {
                        //    kavt.acc_code_master = 0;
                        //    kavt.acc_type = "J";
                        //}
                        //else if (tds.AccountType == "C") {
                        //    kavt.acc_code_master = supplierCode;
                        //    kavt.acc_type = "J";
                        //}
                        //else {
                        //    kavt.acc_code_master = supplierCode;
                        //    kavt.acc_type = "J";
                        //}
                        //kavt.voucher_no = voucherNo;
                        //kavt.New_voucher_no = voucherNo;
                        //kavt.txt_seq_no = txtSeqNo;
                        //kavt.party_name = tds.SupplierName;
                        //kavt.receipt_no = Convert.ToString(expNo);
                        //kavt.cflag = "N";
                        //kavt.acc_code = supplierCode;
                        //kavt.cflag = "N";
                        //kavt.approved_date = appDate;
                        //kavt.Authorized_Date = appDate;
                        //kavt.branch_code = branchCode;
                        //kavt.cflag = "N";
                        //kavt.chq_date = appDate;
                        //kavt.company_code = companyCode;
                        //kavt.contra_seq_no = 0;
                        //kavt.fin_period = finYear;
                        //kavt.fin_year = finYear;
                        //kavt.is_approved = "N";
                        //kavt.Is_Authorized = "N";
                        //kavt.is_tds = "N";
                        //kavt.Is_Verified = "N";
                        //kavt.reconsile_date = appDate;
                        //kavt.reconsile_flag = "N";
                        //kavt.section_id = "0";
                        //kavt.subledger_acc_code = 0;
                        //kavt.TDS_amount = 0;
                        //kavt.trans_type = "EXP";
                        //kavt.Verified_Date = appDate;
                        //kavt.voucher_date = appDate;
                        //kavt.voucher_type = "EV";
                        //kavt.chq_no = "0";
                        //kavt.narration = tds.Description;

                        if (tds.AccountType == "C" && tds.AccountType == "B") {
                            #region Cash and Bank Account Posting
                            for (int j = 0; j < 6; j++) {
                                if (j == 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    TotalAmount = tds.Amount;

                                    if (tds.IsEligible.Equals("N"))
                                        Amount = kavt.dr_amount = TotalAmount +
                                                +sgstAmount + cgstAmount + igstAmount;
                                    else
                                        Amount = kavt.dr_amount = Convert.ToDecimal(tds.Amount);
                                }
                                else if (j == 1 && sgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, sgstPercent, isRegistered, "SGST", companyCode, branchCode);

                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "SGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N"))
                                        Amount = kavt.dr_amount = sgstAmount;
                                    else
                                        Amount = kavt.dr_amount = 0;
                                }
                                else if (j == 2 && cgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "CGST", companyCode, branchCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", cgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N")) {
                                        Amount = kavt.dr_amount = cgstAmount;
                                    }
                                    else {
                                        Amount = kavt.dr_amount = 0;
                                    }
                                }
                                else if (j == 3 && igstAmount > 0) {
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "IGST", companyCode, branchCode);

                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", igstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("N")) {
                                        Amount = kavt.dr_amount = cgstAmount;
                                    }
                                    else {
                                        Amount = kavt.dr_amount = 0;
                                    }

                                }
                                else if (j == 4 && Convert.ToDecimal(tds.CESSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = cessAccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.CESSAmount);

                                }
                                else if (j == 5 && Convert.ToDecimal(tds.RoundOff) != 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    decimal roundoff = Convert.ToDecimal(tds.RoundOff);
                                    if (roundoff != 0) {
                                        if (roundoff > 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            Amount = kavt.cr_amount = roundoff;
                                            kavt.dr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";

                                        }
                                        else if (roundoff < 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            Amount = kavt.dr_amount = -roundoff;
                                            kavt.cr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";
                                        }
                                    }

                                }
                                else {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.cr_amount = 0;
                                    kavt.dr_amount = 0;
                                    Amount = 0;
                                }
                                dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                                SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);
                                voucherSeqNo++;
                            }
                            #endregion
                        }
                        else {
                            #region Journal Entry
                            for (int j = 0; j < 9; j++) {
                                if (j == 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    TotalAmount = tds.Amount;
                                    if (tds.IsEligible != "Y")
                                        Amount = kavt.dr_amount = TotalAmount +
                                            +sgstAmount + cgstAmount + igstAmount;
                                    else
                                        Amount = kavt.dr_amount = tds.Amount;
                                }
                                else if (j == 1 && sgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, sgstPercent, isRegistered, "SGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "SGST", sgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;
                                }
                                else if (j == 2 && cgstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, cgstPercent, isRegistered, "CGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "CGST", cgstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;

                                }
                                else if (j == 3 && igstAmount > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetGSTAccountCodeReceived(dbContext, gstCode, igstPercent, isRegistered, "IGST", branchCode, companyCode);
                                    if (kavt.acc_code == 0) {
                                        error = new ErrorVM()
                                        {
                                            description = string.Format("{0} @ {1}% ledger mapping is not done , please map the ledger in GST posting setup", "IGST", igstPercent),
                                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                        };
                                    }
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    if (tds.IsEligible.Equals("Y")) {
                                        Amount = kavt.dr_amount = sgstAmount;
                                    }
                                    else
                                        Amount = kavt.dr_amount = 0;

                                }
                                else if (j == 4 && Convert.ToDecimal(tds.CESSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = cessAccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.CESSAmount);

                                }

                                else if (j == 5) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    Amount = kavt.cr_amount = Convert.ToDecimal(tds.FinalAmount);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.dr_amount = 0;
                                }

                                else if (j == 6 && Convert.ToDecimal(tds.TDSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = tds.AccCode;
                                    kavt.cr_amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    Amount = kavt.dr_amount = Convert.ToDecimal(tds.TDSAmount);

                                }

                                else if (j == 7 && Convert.ToDecimal(tds.TDSAmount) > 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.acc_code = SIGlobals.Globals.GetAccountCodeForTDSExpense(dbContext, tdsid, companyCode, branchCode);
                                    Amount = kavt.cr_amount = Convert.ToDecimal(tds.TDSAmount);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    kavt.dr_amount = 0;

                                }
                                else if (j == 8 && Convert.ToDecimal(tds.RoundOff) != 0) {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.voucher_seq_no = voucherSeqNo;
                                    decimal roundoff = Convert.ToDecimal(tds.RoundOff);
                                    if (roundoff != 0) {
                                        if (roundoff > 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);


                                            if (kavt.acc_code == 0) {
                                                error = new ErrorVM()
                                                {
                                                    description = string.Format("Please Map the  AccCode For {0} in Acc Code Master", "45"),
                                                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                                };
                                                return false;
                                            }
                                            Amount = kavt.cr_amount = roundoff;
                                            kavt.dr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";

                                        }
                                        else if (roundoff < 0) {
                                            kavt.acc_code = SIGlobals.Globals.GetMagnaAccountCode(dbContext, 45, companyCode, branchCode);
                                            if (kavt.acc_code == 0) {
                                                error = new ErrorVM()
                                                {
                                                    description = string.Format("Please Map the  AccCode For {0} in Acc Code Master", "45"),
                                                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                                                };
                                                return false;
                                            }
                                            Amount = kavt.dr_amount = -roundoff;
                                            kavt.cr_amount = 0.00M;
                                            kavt.narration = "Expense Voucher (Round off Amount)";
                                        }
                                    }

                                }
                                else {
                                    kavt = GetNewAccountVoucherTransaction(tds, voucherNo, txtSeqNo, expenseNo, companyCode, branchCode, out isRegistered);
                                    kavt.cr_amount = 0;
                                    kavt.dr_amount = 0;
                                    Amount = 0;
                                    kavt.voucher_seq_no = voucherSeqNo;
                                }
                                dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);
                                voucherSeqNo++;
                            }
                            #endregion
                        }
                        #endregion

                        #region Update Cheque List
                        KSTU_ACC_CHEQUE_MASTER chqMaster = dbContext.KSTU_ACC_CHEQUE_MASTER.Where(chq => chq.company_code == companyCode
                                                                                                    && chq.branch_code == branchCode
                                                                                                    && chq.chq_no == tds.ChqNo).FirstOrDefault();
                        if (chqMaster != null) {
                            chqMaster.chq_issued = "Y";
                            chqMaster.chq_issue_date = appDate;
                            dbContext.Entry(chqMaster).State = System.Data.Entity.EntityState.Modified;
                        }
                        dbContext.SaveChanges();
                        #endregion
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return false;
                }
            }
            #endregion
        }

        public dynamic GetLedgerDetails(string companyCode, string branchCode, int supplierCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.company_code == companyCode
                                                                            && led.branch_code == branchCode
                                                                            && led.acc_code == supplierCode).Select(led => new
                                                                            {
                                                                                StateCode = led.state_code
                                                                            }).FirstOrDefault();
                return data.StateCode == null ? 0 : data.StateCode;
            }
            catch (Exception excp) {

                error = new ErrorVM().GetErrorDetails(excp);
                return excp;
            }
        }
        #endregion

        #region Private Methods
        private KSTU_ACC_VOUCHER_TRANSACTIONS GetNewAccountVoucherTransaction(TDSExpanseDetailsVM tds, int voucherNo, int txtSeqNo, int expenseNo,
            string companyCode, string branchCode, out bool? isRegistred)
        {
            isRegistred = true;
            KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();
            DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            int finYear = SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode);

            int supplierCode = Convert.ToInt32(tds.SupplierCode);
            var supplierType = dbContext.vGetexpenseledgers.Where(led => led.company_code == tds.CompanyCode
                                                                        && led.branch_code == tds.BranchCode
                                                                        && led.acc_code == supplierCode).FirstOrDefault();
            if (supplierType != null) {
                if (supplierType.tin != "") {
                    kavt.narration = "Expense Voucher Registered";
                    isRegistred = true;
                }
                else {
                    isRegistred = false;
                    kavt.narration = "Expense Voucher Un-Registered";
                }
            }

            if (tds.AccountType == "J") {
                kavt.acc_code_master = 0;
                kavt.acc_type = "J";
            }
            else if (tds.AccountType == "C") {
                kavt.acc_code_master = supplierCode;
                kavt.acc_type = "J";
            }
            else {
                kavt.acc_code_master = supplierCode;
                kavt.acc_type = "J";
            }
            string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS" + SIGlobals.Globals.Separator
                          + voucherNo + SIGlobals.Globals.Separator + kavt.acc_type
                          + SIGlobals.Globals.Separator + kavt.acc_code_master
                          + SIGlobals.Globals.Separator + kavt.trans_type
                          + SIGlobals.Globals.Separator + companyCode
                          + SIGlobals.Globals.Separator + branchCode;

            kavt.obj_id = SIGlobals.Globals.GetHashcode(temp);
            kavt.company_code = companyCode;
            kavt.branch_code = branchCode;
            kavt.txt_seq_no = txtSeqNo;
            kavt.voucher_no = voucherNo;
            kavt.New_voucher_no = voucherNo;
            kavt.party_name = tds.SupplierName;
            kavt.receipt_no = Convert.ToString(expenseNo);
            kavt.cflag = "N";
            kavt.acc_code = supplierCode;
            kavt.cflag = "N";
            kavt.branch_code = branchCode;
            kavt.cflag = "N";
            kavt.company_code = companyCode;
            kavt.contra_seq_no = 0;
            kavt.fin_period = finYear;
            kavt.fin_year = finYear;
            kavt.is_approved = "N";
            kavt.Is_Authorized = "N";
            kavt.is_tds = "N";
            kavt.Is_Verified = "N";
            kavt.reconsile_flag = "N";
            kavt.section_id = "0";
            kavt.subledger_acc_code = 0;
            kavt.TDS_amount = 0;
            kavt.trans_type = "EXP";
            kavt.voucher_type = "EV";
            kavt.chq_no = "0";

            kavt.voucher_date = appDate;
            kavt.chq_date = appDate;
            kavt.reconsile_date = appDate;
            kavt.inserted_on = appDate;
            kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
            kavt.approved_date = appDate;
            kavt.Verified_Date = appDate;
            kavt.Authorized_Date = appDate;
            kavt.cancelled_date = appDate;
            kavt.narration = tds.Description;
            kavt.UniqRowID = Guid.NewGuid();

            return kavt;
        }
        #endregion
    }
}
