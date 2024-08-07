using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 26-05-2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    /// <summary>
    /// This Business Layer of Expense Voucher Cancel Module.
    /// </summary>
    public class ExpenseVoucherCancelBL
    {
        #region Delcaration
        MagnaDbEntities dbContext = null;
        #endregion

        #region Constructor
        public ExpenseVoucherCancelBL()
        {
            dbContext = new MagnaDbEntities();
        }

        public ExpenseVoucherCancelBL(MagnaDbEntities db)
        {
            dbContext = db;
        }
        #endregion

        #region Methods
        public IEnumerable<TDSExpanseDetailsVM> GetVoucherDetails(string companyCode, string branchCode, int voucherNo, out ErrorVM error)
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
                                  && ted.expense_no == voucherNo
                            select new TDSExpanseDetailsVM()
                            {
                                SNo = ted.sno,
                                AccName = ted.acc_name,
                                AccCode = ted.acc_code,
                                SupplierCode = ted.suppliercode,
                                SupplierName = ted.suppliername,
                                AccountType=ted.acc_type,
                                InvoiceNo = ted.invoiceno,
                                Description = ted.description,
                                InvoiceDate = ted.invoicedate,
                                Amount = ted.amount,
                                CGSTAmount = ted.CGST_Amount,
                                SGSTAmount = ted.SGST_Amount,
                                IGSTAmount = ted.IGST_Amount,
                                CESSAmount = ted.CESS_Amount,
                                TotalAmount = ted.total_amount,
                                RoundOff = ted.roundoff,
                                FinalAmount = ted.final_Amount,
                                TDSPercentage = ted.tds_precentage,
                                TDSAmount = ted.tds_amount,
                                ChqNo = ted.chq_no,
                                ChqDate = ted.chq_date
                            }).ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public bool CancelVoucherDetails(TDSExpenseCancelVM tds, out ErrorVM error)
        {
            error = null;
            try {
                #region Expense Details
                KTTU_TDS_EXPENSE_DETAILS tdsExp = dbContext.KTTU_TDS_EXPENSE_DETAILS.Where(ts => ts.company_code == tds.CompanyCode
                                                                                            && ts.branch_code == tds.BranchCode
                                                                                            && ts.expense_no == tds.ExpenseNo).FirstOrDefault();
                if (tdsExp == null) {
                    error = new ErrorVM()
                    {
                        description = "Voucher Details not found.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }

                if (tdsExp.cflag == "Y") {
                    error = new ErrorVM()
                    {
                        description = "Voucher Details already cancelled.",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                }

                tdsExp.cflag = "Y";
                tdsExp.remarks = tds.Remarks;
                dbContext.Entry(tdsExp).State = System.Data.Entity.EntityState.Modified;

                #endregion

                #region Account Voucher Transactions
                string strExpenseNo = Convert.ToString(tds.ExpenseNo);
                List<KSTU_ACC_VOUCHER_TRANSACTIONS> accVoucher = dbContext.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(acc => acc.receipt_no == strExpenseNo
                                                                                                                && acc.company_code == tds.CompanyCode
                                                                                                                && acc.branch_code == tds.BranchCode).ToList();
                foreach (KSTU_ACC_VOUCHER_TRANSACTIONS acc in accVoucher) {
                    acc.cflag = "Y";
                    acc.Cancelled_remarks = tds.Remarks;
                    acc.cancelled_date = SIGlobals.Globals.GetApplicationDate(tds.CompanyCode, tds.BranchCode);
                    dbContext.Entry(acc).State = System.Data.Entity.EntityState.Modified;
                }
                dbContext.SaveChanges();
                #endregion
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion
    }
}
