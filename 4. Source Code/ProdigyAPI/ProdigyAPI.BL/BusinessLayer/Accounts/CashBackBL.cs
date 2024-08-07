using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 12-Apr-2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    /// <summary>
    /// Type: Business Layer
    /// Proivdes Methods/Functons related to Business Logic of Cashback.
    /// </summary>
    public class CashBackBL
    {
        #region Declaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructor
        public CashBackBL()
        {
            dbContext = new MagnaDbEntities();
        }

        public CashBackBL(MagnaDbEntities db)
        {
            if (db == null) {
                dbContext = db;
            }
            else {
                dbContext = new MagnaDbEntities();
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Bill Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="billNo"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public CashBackVM GetBillDetails(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            if (!ValidateBill(companyCode, branchCode, billNo, out error))
                return null;
            CashBackVM cashBack = FillCashBackDetails(companyCode, branchCode, billNo, out error);
            return cashBack;
        }

        /// <summary>
        /// Save Cashback Information.
        /// </summary>
        /// <param name="cashBackDet"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SaveCashBackDetails(CashBackTotalVM cashBackDet, out CashBackResponseVM cbr, out ErrorVM error)
        {
            #region Declaration
            error = null;
            cbr = new CashBackResponseVM();
            decimal totalInvoiceAmount = 0;
            decimal totalEligableCashBackAmount = 0;
            decimal totalActualCashBackAmount = 0;
            string companyCode = string.Empty;
            string branchCode = string.Empty;
            string narration = string.Empty;
            int voucherNo = 0;
            int tranSeqNo = 0;
            #endregion

            #region Basic Validation
            if (cashBackDet == null || cashBackDet.ListCashBackVM == null || cashBackDet.ListCashBackVM.Count == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return false;
            }
            foreach (var c in cashBackDet.ListCashBackVM) {
                if (c.ActualCashBack > c.BillAmt) {
                    error = new ErrorVM()
                    {
                        description = string.Format("Cash Back should not be equal or greater than bill value"),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    };
                    return false;
                }
            }
            companyCode = cashBackDet.CompanyCode;
            branchCode = cashBackDet.BranchCode;
            #endregion

            #region Advanced Validation
            foreach (var cash in cashBackDet.ListCashBackVM) {

                if (!ValidateBill(companyCode, branchCode, cash.BillNo, out error))
                    return false;

                CashBackVM cbd = FillCashBackDetails(cash.CompanyCode, cash.BranchCode, cash.BillNo, out error);
                if (error != null)
                    return false;
                if (cash.BillAmt != cbd.BillAmt) {
                    error = new ErrorVM()
                    {
                        description = string.Format("For the {0} Invoice/Bill Number, Bill Amount is {1} ", cbd.BillNo, cbd.BillAmt),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    };
                    return false;
                }
                if (cash.EligableCashBack != cbd.EligableCashBack) {
                    error = new ErrorVM()
                    {
                        description = string.Format("For the {0} Invoice/Bill Number, Eligbale Cashback is {1} ", cbd.BillNo, cbd.EligableCashBack),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    };
                    return false;
                }
                totalInvoiceAmount = totalInvoiceAmount + cbd.BillAmt;
                totalEligableCashBackAmount = totalEligableCashBackAmount + cbd.EligableCashBack;
                totalActualCashBackAmount = totalActualCashBackAmount + cash.ActualCashBack;
                narration = narration + cash.BillNo.ToString() + ",";
            }

            if (cashBackDet.TotalBillAmount != totalInvoiceAmount) {
                error = new ErrorVM()
                {
                    description = string.Format("There is mismatch in Total Cashback Amount"),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return false;
            }

            if (cashBackDet.TotalEligableCashBackAmount != totalEligableCashBackAmount) {
                error = new ErrorVM()
                {
                    description = string.Format("There is mismatch in Total Eligbale Cashback Amount"),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                };
                return false;
            }
            if (!cashBackDet.IsEPayment) {
                KSTU_TOLERANCE_MASTER tollMaster = SIGlobals.Globals.GetTollerance(dbContext, 6, cashBackDet.CompanyCode, cashBackDet.BranchCode);
                if (tollMaster != null && totalActualCashBackAmount >= tollMaster.Max_Val) {
                    error = new ErrorVM()
                    {
                        description = string.Format("Cash payment is not allowed for more than {0}", Math.Round(tollMaster.Max_Val, 2)),
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                    };
                    return false;
                }
            }
            #endregion

            #region Saving

            using (var transaction = dbContext.Database.BeginTransaction()) {
                try {

                    #region Master and Details
                    DateTime appDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
                    int finYear = SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode);
                    int cashBackNo = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(dbContext, companyCode, branchCode).ToString().Substring(2, 2)
                                                                            + SIGlobals.Globals.GetNewTransactionSerialNo(dbContext, "2020", companyCode, branchCode));


                    SIGlobals.Globals.UpdateSeqenceNumber(dbContext, "2020", companyCode, branchCode);

                    KTTU_CASHBACK_OFFER_MASTER offerMaster = new KTTU_CASHBACK_OFFER_MASTER();
                    offerMaster.company_code = companyCode;
                    offerMaster.branch_code = branchCode;
                    offerMaster.cash_back_no = cashBackNo;
                    offerMaster.cash_back_date = appDate;
                    offerMaster.operator_code = cashBackDet.OperatorCode;
                    offerMaster.total_invoice_amount = totalInvoiceAmount;
                    offerMaster.total_cash_back_amount = totalActualCashBackAmount;
                    offerMaster.voucher_no = 0;
                    offerMaster.cflag = "N";
                    dbContext.KTTU_CASHBACK_OFFER_MASTER.Add(offerMaster);

                    int slno = 1;
                    foreach (var cbd in cashBackDet.ListCashBackVM) {
                        KTTU_CASHBACK_OFFER_DETAILS offerDet = new KTTU_CASHBACK_OFFER_DETAILS();
                        offerDet.company_code = companyCode;
                        offerDet.branch_code = branchCode;
                        offerDet.cash_back_no = cashBackNo;
                        offerDet.sl_no = slno;
                        offerDet.bill_no = cbd.BillNo;
                        offerDet.invoice_amount = cbd.BillAmt;
                        offerDet.eligible_cash_back_amount = cbd.EligableCashBack;
                        offerDet.actual_cash_back_amount = cbd.ActualCashBack;
                        offerDet.remarks = cbd.Remarks;
                        offerDet.voucher_no = 0;
                        offerDet.cash_back_date = appDate;
                        offerDet.cflag = "N";
                        dbContext.KTTU_CASHBACK_OFFER_DETAILS.Add(offerDet);
                        slno += 1;
                    }

                    cbr.CashBackNo = cashBackNo;
                    #endregion

                    #region Account Posting
                    KSTU_ACC_VOUCHER_TRANSACTIONS kavt = new KSTU_ACC_VOUCHER_TRANSACTIONS();
                    if (cashBackDet.IsEPayment) {
                        // Jorunal voucher Entry

                        //Debit Entry
                        voucherNo = new JournalVoucherEntryBL().GetJournalVoucherNo(dbContext, finYear, companyCode, branchCode, out error);
                        tranSeqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                                        dbContext.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == "03"
                                        && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault().nextno);
                        SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);

                        kavt.obj_id = SIGlobals.Globals.GetNewGUID();
                        kavt.company_code = companyCode;
                        kavt.branch_code = branchCode;
                        kavt.txt_seq_no = tranSeqNo;
                        kavt.voucher_no = voucherNo;
                        kavt.voucher_seq_no = 1;
                        kavt.acc_code = SIGlobals.Globals.GetAccCodeFromAccPostingSetup(dbContext, "PM", "CB", companyCode, branchCode);
                        if (kavt.acc_code == 0) {
                            error = new ErrorVM()
                            {
                                description = string.Format("{0}  ledger mapping is not done for TransType {1} , please map the ledger in GST posting setup", "PM", "CB"),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                            };
                            return false;
                        }
                        kavt.acc_code_master = 0;
                        kavt.acc_type = "J";
                        kavt.voucher_date = appDate;
                        kavt.approved_date = appDate;
                        kavt.Authorized_By = "";
                        kavt.Authorized_Date = appDate;
                        kavt.Authorized_Remarks = "";
                        kavt.cflag = "N";
                        kavt.chq_date = appDate;
                        kavt.chq_no = "";
                        kavt.contra_seq_no = 0;
                        kavt.cr_amount = 0;
                        kavt.currency_type = "INR";
                        kavt.dr_amount = totalActualCashBackAmount;
                        kavt.fin_year = finYear;
                        kavt.inserted_on = SIGlobals.Globals.GetDateTime();
                        kavt.Is_Authorized = "N";
                        kavt.is_approved = "N";
                        kavt.is_tds = "N";
                        kavt.Is_Verified = "N";
                        kavt.narration = string.Format("Cash back amount paid against Sales Invoice No. {0}", narration);
                        kavt.New_voucher_no = voucherNo;
                        kavt.party_name = SIGlobals.Globals.GetLedgerName(dbContext, kavt.acc_code, companyCode, branchCode);
                        kavt.receipt_no = Convert.ToString(cashBackNo);
                        kavt.reconsile_by = "";
                        kavt.reconsile_date = appDate;
                        kavt.reconsile_flag = "N";
                        kavt.subledger_acc_code = 0;
                        kavt.TDS_amount = Convert.ToDecimal(0.00);
                        kavt.trans_type = "JOU";
                        kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kavt.UniqRowID = Guid.NewGuid();
                        kavt.Verified_By = "";
                        kavt.Verified_Date = appDate;
                        kavt.Verified_Remarks = "";
                        kavt.voucher_type = "";
                        kavt.cancelled_date = SIGlobals.Globals.GetDateTime();
                        dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);

                        //Credit Entry
                        int crTranSeqNo = Convert.ToInt32(finYear.ToString().Remove(0, 1) +
                                        dbContext.KSTS_ACC_SEQ_NOS.Where(sq => sq.obj_id == "03"
                                        && sq.company_code == companyCode && sq.branch_code == branchCode).FirstOrDefault().nextno);
                        SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);

                        kavt.txt_seq_no = crTranSeqNo;
                        kavt.voucher_seq_no = 2;
                        kavt.dr_amount = 0;
                        kavt.cr_amount = totalActualCashBackAmount;
                        kavt.acc_code = SIGlobals.Globals.GetAccCodeFromAccPostingSetup(dbContext, "PM", "EP", companyCode, branchCode);
                        dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);

                        cbr.VoucherNo = voucherNo;
                        cbr.AccouuntCodeMaster = 0;
                        cbr.TranType = "JOU";
                        cbr.AccType = "J";

                    }
                    else {
                        // Cash voucher Entry
                        tranSeqNo = SIGlobals.Globals.GetAccountSeqNo(dbContext, "03", companyCode, branchCode);
                        voucherNo = SIGlobals.Globals.GetAccountVoucherSeqNo(dbContext, "1", companyCode, branchCode);


                        SIGlobals.Globals.UpdateAccountSeqenceNumber(dbContext, "03", companyCode, branchCode);
                        SIGlobals.Globals.UpdateAccountVourcherSeqenceNumber(dbContext, "1", companyCode, branchCode);

                        string objID = "KSTU_ACC_VOUCHER_TRANSACTIONS" +
                                   SIGlobals.Globals.Separator + Convert.ToString(voucherNo) +
                                   SIGlobals.Globals.Separator + "C" +
                                   SIGlobals.Globals.Separator + 1 +
                                   SIGlobals.Globals.Separator + "PAY" +
                                   SIGlobals.Globals.Separator + companyCode +
                                   SIGlobals.Globals.Separator + branchCode;

                        kavt.obj_id = SIGlobals.Globals.GetHashcode(objID);
                        kavt.company_code = companyCode;
                        kavt.branch_code = branchCode;
                        kavt.txt_seq_no = tranSeqNo;
                        kavt.voucher_no = voucherNo;
                        kavt.voucher_seq_no = 1;
                        kavt.acc_code = SIGlobals.Globals.GetAccCodeFromAccPostingSetup(dbContext, "PM", "CB", companyCode, branchCode);
                        if (kavt.acc_code == 0) {
                            error = new ErrorVM()
                            {
                                description = string.Format("{0}  ledger mapping is not done for TransType {1} , please map the ledger in GST posting setup", "PM", "CB"),
                                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                            };
                            return false;
                        }
                        kavt.acc_code_master = 1;
                        kavt.acc_type = "C";
                        kavt.voucher_date = appDate;
                        kavt.approved_date = appDate;
                        kavt.Authorized_By = "";
                        kavt.Authorized_Date = appDate;
                        kavt.Authorized_Remarks = "";
                        kavt.cflag = "N";
                        kavt.chq_date = appDate;
                        kavt.chq_no = "";
                        kavt.contra_seq_no = 0;
                        kavt.cr_amount = 0;
                        kavt.currency_type = "INR";
                        kavt.dr_amount = totalActualCashBackAmount;
                        kavt.fin_year = finYear;
                        kavt.inserted_on = SIGlobals.Globals.GetDateTime();
                        kavt.Is_Authorized = "N";
                        kavt.is_approved = "N";
                        kavt.is_tds = "N";
                        kavt.Is_Verified = "N";
                        kavt.narration = string.Format("Towards Cash back amount paid against Sales Invoice No. {0}", narration);
                        kavt.New_voucher_no = voucherNo;
                        kavt.party_name = SIGlobals.Globals.GetLedgerName(dbContext, kavt.acc_code, companyCode, branchCode);
                        kavt.receipt_no = Convert.ToString(cashBackNo);
                        kavt.reconsile_by = "";
                        kavt.reconsile_date = appDate;
                        kavt.reconsile_flag = "N";
                        kavt.subledger_acc_code = 0;
                        kavt.TDS_amount = Convert.ToDecimal(0.00);
                        kavt.trans_type = "PAY";
                        kavt.UpdateOn = SIGlobals.Globals.GetDateTime();
                        kavt.UniqRowID = Guid.NewGuid();
                        kavt.Verified_By = "";
                        kavt.Verified_Date = appDate;
                        kavt.Verified_Remarks = "";
                        kavt.voucher_date = appDate;
                        kavt.voucher_type = "";
                        kavt.cancelled_date = SIGlobals.Globals.GetDateTime();
                        dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(kavt);

                        cbr.VoucherNo = voucherNo;
                        cbr.AccouuntCodeMaster = 1;
                        cbr.TranType = "PAY";
                        cbr.AccType = "C";
                    }
                    #endregion

                    #region Updating to Masters and Details
                    dbContext.SaveChanges();
                    KTTU_CASHBACK_OFFER_MASTER updMaster = dbContext.KTTU_CASHBACK_OFFER_MASTER.Where(cash => cash.company_code == companyCode
                                                                                                        && cash.branch_code == branchCode
                                                                                                        && cash.cash_back_no == cashBackNo).FirstOrDefault();
                    updMaster.voucher_no = voucherNo;
                    dbContext.Entry(updMaster).State = System.Data.Entity.EntityState.Modified;

                    var updDet = dbContext.KTTU_CASHBACK_OFFER_DETAILS.Where(d => d.company_code == companyCode
                                                                                                        && d.branch_code == branchCode
                                                                                                        && d.cash_back_no == cashBackNo).ToList();
                    foreach (var upd in updDet) {
                        upd.voucher_no = voucherNo;
                        dbContext.Entry(upd).State = System.Data.Entity.EntityState.Modified;
                    }
                    #endregion

                    dbContext.SaveChanges();
                    transaction.Commit();
                }
                catch (Exception excp) {
                    transaction.Rollback();
                    error = new ErrorVM().GetErrorDetails(excp);
                    return false;
                }
            }
            #endregion

            return true;
        }
        #endregion

        #region Private Methods
        private bool ValidateBill(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            if (billNo == 0) {
                error = new ErrorVM()
                {
                    description = "Invalid Invoice/Bill Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }

            KTTU_SALES_MASTER sales = dbContext.KTTU_SALES_MASTER.Where(bill => bill.company_code == companyCode
                                                                        && bill.branch_code == branchCode
                                                                        && bill.bill_no == billNo).FirstOrDefault();
            if (sales == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Invoice/Bill Number",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }

            if (sales.cflag == "Y") {
                error = new ErrorVM()
                {
                    description = "Invoice/Bill already cancelled.",
                    ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                };
                return false;
            }

            var cashBackDet = dbContext.KTTU_CASHBACK_OFFER_DETAILS.Where(c => c.company_code == companyCode
                                                                            && c.branch_code == branchCode
                                                                            && c.bill_no == billNo).ToList();
            if (cashBackDet != null && cashBackDet.Count > 0) {
                error = new ErrorVM()
                {
                    description = string.Format("Cash Back already issue to this bill {0}", billNo),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            if (SIGlobals.Globals.GetConfigurationValue(dbContext, "24072020", companyCode, branchCode) == 1) {
                var payment = dbContext.KTTU_PAYMENT_DETAILS.Where(pay => pay.company_code == companyCode
                                                                    && pay.branch_code == branchCode
                                                                    && pay.series_no == billNo
                                                                    && pay.trans_type == "S"
                                                                    && pay.pay_mode == "CT").ToList();
                if (payment != null && payment.Count > 0) {
                    error = new ErrorVM()
                    {
                        description = "Cash Back is not applicable for Scheme Adjustment.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }
            }
            return true;
        }

        private CashBackVM FillCashBackDetails(string companyCode, string branchCode, int billNo, out ErrorVM error)
        {
            error = null;
            CashBackVM cashBack = new CashBackVM();
            decimal billAmount = dbContext.KTTU_SALES_DETAILS.Where(sal => sal.company_code == companyCode
                                                                    && sal.branch_code == branchCode
                                                                    && sal.bill_no == billNo)
                                                             .Select(sal => sal.total_amount)
                                                             .DefaultIfEmpty(0).Sum();

            // Bellow Code is throwing Error Like: The specified cast from a materialized 'System.Int32' type to a nullable 'System.Decimal' type is not valid.
            // Refer this link: https://stackoverflow.com/questions/32264382/the-specified-cast-from-a-materialized-system-int32-type-to-the-system-double

            //var cashBackAmount = dbContext.usp_GetCashBackOfferDetails(companyCode, branchCode, Convert.ToInt32(billNo), Convert.ToDecimal(billAmount)).FirstOrDefault();
            //if (cashBackAmount != null && Convert.ToDecimal(cashBackAmount) >= 0) {
            //    cashBack.CompanyCode = companyCode;
            //    cashBack.BranchCode = branchCode;
            //    cashBack.BillNo = billNo;
            //    cashBack.BillAmt = billAmount;
            //    cashBack.EligableCashBack = Convert.ToDecimal(cashBackAmount);
            //    cashBack.ActualCashBack = Convert.ToDecimal(cashBackAmount);
            //    cashBack.Remarks = string.Empty;
            //}

            string sql = "EXEC [dbo].[usp_GetCashBackOfferDetails] " +
                            "@companyCode = N'" + companyCode + "'," +
                            "@branchCode = N'" + branchCode + "'," +
                            "@docNo = " + billNo + "," +
                            "@invoiceAmount = " + billAmount + "";
            DataTable tblData = SIGlobals.Globals.ExecuteQuery(sql);
            if (tblData != null && tblData.Rows.Count > 0) {
                cashBack.CompanyCode = companyCode;
                cashBack.BranchCode = branchCode;
                cashBack.BillNo = billNo;
                cashBack.BillAmt = Math.Round(billAmount, 0, MidpointRounding.ToEven);
                cashBack.EligableCashBack = Convert.ToDecimal(tblData.Rows[0][0]);
                cashBack.ActualCashBack = Convert.ToDecimal(tblData.Rows[0][0]);
                cashBack.Remarks = string.Empty;
            }
            return cashBack;
        }
        #endregion
    }
}
