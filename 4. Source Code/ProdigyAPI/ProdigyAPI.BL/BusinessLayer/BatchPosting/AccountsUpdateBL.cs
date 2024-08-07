using System;
using System.Globalization;
using System.Data;
using ProdigyAPI.Model.MagnaDb;
using System.Linq;
using System.Data.SqlClient;
using ProdigyAPI.BL.ViewModel.Error;

namespace ProdigyAPI.BL.BusinessLayer.BatchPosting
{
    public class AccountsUpdateBL
    {
        MagnaDbEntities db = null;

        public AccountsUpdateBL()
        {
            db = new MagnaDbEntities(true);
        }

        public AccountsUpdateBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }
        
        public string clientID
        {
            get { return GetClientID(); }
        }

        bool updateSchemeCollection = false;
        bool updateOrders = false;

        private DateTime AccUpdateTimeStamp;
        KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans = new KSTU_ACC_VOUCHER_TRANSACTIONS();
        DataTable dtCOPayments = new DataTable();

        DataTable dtChequeCardBalance = new DataTable();
        DataTable dtChequeCardDeposits = new DataTable();
        DataTable dtHIPurchase = new DataTable();
        DataTable dtNarration = new DataTable();

        DataTable dtPurPay = new DataTable();
        DataTable dtPayments = new DataTable();
        DataTable dtDeduction = new DataTable();

        bool isHI = false;
        bool isHIFirst = false;
        bool isSales = false;
        bool isPayPurchase = false;
        bool isPurchase = false;
        bool isSalesReturn = false;
        bool isPayments = false;
        bool isCOPayments = false;
        bool isCredit = false;
        bool isCashPaid = false;
        bool isChit = false;
        bool isJPACash = false;
        bool isJPACheque = false;
        bool isOtherTran = false;
        bool isCheqFirstTime = false;
        bool isHIPayments = false;
        bool isPurchaseEpay = false;
        string cashPaidReceiptNo = string.Empty;
        int accCode = 0;

        string gs_code = string.Empty;
        bool isPurchaseRoundOff = false;
        bool isSalesRoundOff = false;
        bool isVat = false;
        bool isEdu = false;
        bool isHEdu = false;
        bool isExcise = false;
        private int nCreditVoucherNo = 0;
        private int nCashVoucherNo = 0;
        private int nCashCount = 0;
        private int nCreditCount = 0;
        private int nBankVoucherNo = 0;
        private int nHIPurchaseNo = 0;
        private int nHIPurchaseCount = 0;
        private int nChitCollectionNo = 0;
        private int nChitCollectioncount = 0;
        string memberShip = string.Empty;

        string branchCode = string.Empty;
        string companyCode = string.Empty;
        string ObjectId = string.Empty;
        string ObjectXml = string.Empty;
        string ObjectType = string.Empty;
        string userId = string.Empty;

        public bool Post(string _companyCode, string _branchCode, DateTime _accountsUpdateDate, bool _updateSchemeCollection, bool _updateOrders, string _userID, out string errorMessage)
        {
            errorMessage = string.Empty;
            companyCode = _companyCode;
            branchCode = _branchCode;
            AccUpdateTimeStamp = _accountsUpdateDate;
            updateSchemeCollection = _updateSchemeCollection;
            updateOrders = _updateOrders;
            userId = _userID;

            using (var transaction = db.Database.BeginTransaction()) {
                if (!SalesPurchaseAccountsUpdate(out errorMessage)) {
                    transaction.Rollback();
                    return false;
                }
                transaction.Commit();
            }

            //if(!JournalTransferHO(out errorXml))
            //    return false;

            return true;
        }

        private bool SalesPurchaseAccountsUpdate(out string errorXml)
        {
            errorXml = string.Empty;
            try {
                string accCodeQuery = string.Empty;
                string accstartdate = AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000");
                string accenddate = AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59");
                var endDate = AccUpdateTimeStamp.Date.AddSeconds(59).AddMinutes(59).AddHours(23);

                nCreditVoucherNo = GetScalarValue<int>(string.Format("SELECT COALESCE(voucher_no,0) FROM KSTU_ACC_VOUCHER_TRANSACTIONS WHERE acc_type = 'O' AND trans_type = 'BILL' AND voucher_date BETWEEN '{0}' AND '{1}' AND company_code = '{2}' AND branch_code = '{3}'", accstartdate, accenddate, companyCode, branchCode));
                nCashVoucherNo = GetScalarValue<int>(string.Format("SELECT COALESCE(voucher_no,0) FROM KSTU_ACC_VOUCHER_TRANSACTIONS WHERE acc_type = 'C' AND trans_type = 'BILL' AND voucher_date BETWEEN '{0}' AND '{1}' AND company_code = '{2}' AND branch_code = '{3}'", accstartdate, accenddate, companyCode, branchCode));
                nHIPurchaseNo = GetScalarValue<int>(string.Format("SELECT COALESCE(voucher_no,0) FROM KSTU_ACC_VOUCHER_TRANSACTIONS WHERE acc_type = 'O' AND trans_type = 'HIS' AND voucher_date BETWEEN '{0}' AND '{1}' AND company_code = '{2}' AND branch_code = '{3}'", accstartdate, accenddate, companyCode, branchCode));
                nChitCollectionNo = GetScalarValue<int>(string.Format("SELECT COALESCE(voucher_no,0) FROM KSTU_ACC_VOUCHER_TRANSACTIONS WHERE acc_type = 'O' AND trans_type = 'CHIT' AND voucher_date BETWEEN '{0}' AND '{1}' AND company_code = '{2}' AND branch_code = '{3}'", accstartdate, accenddate, companyCode, branchCode));

                nBankVoucherNo = GetScalarValue<int>(string.Format("SELECT COALESCE(max(voucher_no),0) FROM KSTU_ACC_VOUCHER_TRANSACTIONS \n"
               + "WHERE trans_type = 'BILL' AND acc_code_master not in (1,0) AND company_code = '{1}' AND branch_code = '{2}' AND voucher_date between '{3}' AND '{4}' and reconsile_flag !='Y'", voucherTrans.acc_code_master, companyCode, branchCode, accstartdate, accenddate));

                #region Delete Vouchers

                //Delelting Sales & Purchase
                //string deleterecord = string.Format("delete from KSTU_ACC_VOUCHER_TRANSACTIONS WHERE trans_type in ('BILL','Pur-C','CHIT','HIS') AND company_code = '{0}' AND branch_code = '{1}' AND voucher_date BETWEEN '{2}' AND '{3}' and reconsile_flag !='Y'", branchCode, companyCode, accstartdate, accenddate);

                //if (!CGlobalsDB.Delete(deleterecord))
                //    return false;
                var transTypeList = new string[] { "BILL", "Pur-C", "CHIT", "HIS" };
                var vhrToDelete = db.KSTU_ACC_VOUCHER_TRANSACTIONS.Where(x => x.branch_code == branchCode && x.company_code == companyCode
                    && x.reconsile_flag != "Y" && x.voucher_date >= AccUpdateTimeStamp.Date && voucherTrans.voucher_date <= endDate
                    && transTypeList.Contains(x.trans_type)).ToList();
                if (vhrToDelete != null)
                    db.KSTU_ACC_VOUCHER_TRANSACTIONS.RemoveRange(vhrToDelete);

                #endregion

                DataTable dtCredit = GetCreditLedgers();
                DataTable dtHIPayments = GetHIAdj();
                DataTable dtRound = GetSalesRoundOff();

                dtPayments = GetPaymentsDetails();
                dtPurPay = GetPurPay();
                dtCOPayments = GetChitandOrdAdj();


                decimal voucherAmount = 0.00m;
                voucherTrans = new KSTU_ACC_VOUCHER_TRANSACTIONS();

                DataTable dtFinacialDetails = GetFinancialDetails();
                voucherTrans.fin_period = Convert.ToInt32(dtFinacialDetails.Rows[0]["fin_seq_no"]);
                voucherTrans.fin_year = Convert.ToInt32(dtFinacialDetails.Rows[0]["fin_year"]);

                voucherTrans.acc_code_master = 0;
                voucherTrans.acc_type = "O";
                voucherTrans.trans_type = "BILL";
                voucherTrans.company_code = companyCode;
                voucherTrans.branch_code = branchCode;
                voucherTrans.chq_date = AccUpdateTimeStamp;
                voucherTrans.chq_no = string.Empty;
                voucherTrans.contra_seq_no = 0;
                voucherTrans.receipt_no = string.Empty;
                voucherTrans.reconsile_date = AccUpdateTimeStamp;
                voucherTrans.reconsile_flag = "N";

                #region Sales Update
                DataTable dtSales = GetSalesAmount();

                if (dtSales != null && dtSales.Rows.Count > 0) {
                    isSales = true;
                    for (int i = 0; i < dtSales.Rows.Count; i++) {
                        isSales = true;
                        gs_code = Convert.ToString(dtSales.Rows[i]["GStock"]);
                        decimal taxVal = 0;
                        decimal exciseVal = 0;
                        decimal hedVal = 0;
                        decimal eduVal = 0;
                        decimal round_off = Convert.ToDecimal(dtSales.Rows[i]["round_off"]);
                        decimal taxPer = Convert.ToDecimal(dtSales.Rows[i]["taxper"]);
                        decimal exciseper = Convert.ToDecimal(dtSales.Rows[i]["excise_duty_percent"]);
                        decimal hedcessper = Convert.ToDecimal(dtSales.Rows[i]["hed_cess_percent"]);
                        decimal edcessper = Convert.ToDecimal(dtSales.Rows[i]["ed_cess_percent"]);
                        decimal sgstPer = Convert.ToDecimal(dtSales.Rows[i]["SGST_Percent"]);
                        decimal cgstPer = Convert.ToDecimal(dtSales.Rows[i]["CGST_Percent"]);
                        decimal igstPer = Convert.ToDecimal(dtSales.Rows[i]["IGST_Percent"]);
                        string strisEDApplicable = string.Empty;
                        decimal cgstAmount = 0.00m, sgstAmount = 0, igstAmount = 0;

                        // DataTable dtS = SIGlobals.Globals.GetDataTable(string.Format("SELECT coalesce(sum(pay_amount_before_tax) + sum(pay_tax_amount),0) AS Amount,convert(varchar,pay_date,103) as pay_date FROM KTTU_PAYMENT_DETAILS kpd,KTTU_SALES_MASTER ksm WHERE kpd.series_no = ksm.bill_no AND kpd.trans_type = 'S' AND kpd.cflag = 'N' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND ksm.gstype = '{4}' AND kpd.sal_bill_type is null AND (kpd.pay_mode != 'C') AND kpd.cflag = 'N' GROUP BY convert(varchar,pay_date,103)", branchCode, companyCode, accstartdate, accenddate, gs_code, GetDefaultCurrencyCode()));
                        // DataTable dtSS = SIGlobals.Globals.GetDataTable(string.Format("SELECT COALESCE(SUM(pay_amt),0) AS Amount,convert(varchar,pay_date,103) FROM KTTU_PAYMENT_DETAILS kpd,KTTU_STONE_SALES_MASTER ksm,KTTU_SALES_DETAILS ksd WHERE ksm.bill_no = ksd.bill_no and kpd.series_no = ksm.bill_no AND kpd.trans_type = 'SS' AND kpd.cflag = 'N' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND ksd.gs_code = '{4}' AND kpd.sal_bill_type is null AND kpd.pay_mode != 'C' AND kpd.cflag = 'N' GROUP BY convert(varchar,pay_date,103)", branchCode, companyCode, accstartdate, accenddate, gs_code,GetDefaultCurrencyCode()));
                        // DataTable dbalance = SIGlobals.Globals.GetDataTable(string.Format("SELECT sum(COALESCE(balance_amt,0)) as balance,convert(varchar,bill_date,103) as bill_date FROM KTTU_SALES_MASTER ksm WHERE\n"
                        //         + " bill_date >= '{2}' AND bill_date <= '{3}'  \n"
                        //         + "AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND gstype = '{4}' AND cflag = 'N' and balance_amt>0\n" +
                        //" GROUP BY convert(varchar,bill_date,103)", branchCode, companyCode, accstartdate, accenddate, gs_code));


                        // DataTable dpaid = SIGlobals.Globals.GetDataTable(string.Format("SELECT sum(COALESCE(abs(balance_amt),0)) as balance,convert(varchar,bill_date,103) as bill_date FROM KTTU_SALES_MASTER ksm WHERE\n"
                        //         + " bill_date >= '{2}' AND bill_date <= '{3}'  \n"
                        //         + "AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND gstype = '{4}' AND cflag = 'N' and balance_amt<0\n" +
                        //" GROUP BY convert(varchar,bill_date,103)", branchCode, companyCode, accstartdate, accenddate, gs_code));

                        //Kavitha uncomment item_additional_discount

                        //                    DataTable dtS = SIGlobals.Globals.GetDataTable(string.Format(@";with sales as (SELECT (((COALESCE(item_total_after_discount,0) + COALESCE(item_additional_discount,0)) / ( (total_bill_amount))) * (pay_amt)) AS Amount,convert(varchar,pay_date,103) as pay_date 
                        //                    FROM KTTU_PAYMENT_DETAILS kpd,KTTU_SALES_MASTER ksm,KTTU_SALES_DETAILS ksd WHERE ksm.bill_no = ksd.bill_no and kpd.series_no = ksm.bill_no 
                        //                    AND kpd.trans_type = 'S' AND kpd.cflag = 'N' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND ksm.company_code = '{0}' 
                        //                    AND ksm.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND ksd.gs_code = '{4}' AND kpd.sal_bill_type is null 
                        //                     AND (kpd.pay_mode != 'C') AND kpd.cflag = 'N' and KSD.SGST_Percent='{5}' and KSD.CGST_Percent='{6}' and KSD.IGST_Percent='{7}'  ) select sum(amount) as amount,pay_date from sales group by pay_date", branchCode, companyCode, accstartdate, accenddate, gs_code, sgstPer, cgstPer, igstPer));

                        DataTable dtS = SIGlobals.Globals.GetDataTable(string.Format(@";with sales as (SELECT round((((COALESCE(item_total_after_discount,0)) / ( (total_bill_amount))) * (pay_amt)),0) AS Amount,convert(varchar,pay_date,103) as pay_date 
                    FROM KTTU_PAYMENT_DETAILS kpd,KTTU_SALES_MASTER ksm,KTTU_SALES_DETAILS ksd WHERE ksm.bill_no = ksd.bill_no and kpd.series_no = ksm.bill_no 
                    AND kpd.trans_type = 'S' AND kpd.cflag = 'N' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND ksm.company_code = '{0}' 
                    AND ksm.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND ksd.gs_code = '{4}' AND kpd.sal_bill_type is null 
                     AND (kpd.pay_mode != 'C') AND kpd.cflag = 'N' and KSD.SGST_Percent='{5}' and KSD.CGST_Percent='{6}' and KSD.IGST_Percent='{7}'  ) select round(sum(amount),0) as amount,pay_date from sales group by pay_date", 
                     companyCode, branchCode, accstartdate, accenddate, gs_code, sgstPer, cgstPer, igstPer));

                        DataTable dtSS = SIGlobals.Globals.GetDataTable(string.Format(@"SELECT COALESCE(SUM(pay_amt),0) AS Amount,convert(varchar,pay_date,103) 
                    FROM KTTU_PAYMENT_DETAILS kpd,KTTU_STONE_SALES_MASTER ksm WHERE kpd.series_no = ksm.bill_no 
                    AND kpd.trans_type = 'SS' AND kpd.cflag = 'N' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' 
                    AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}'  
                    AND kpd.sal_bill_type is null AND kpd.pay_mode != 'C' AND kpd.cflag = 'N'  
                    AND ksm.bill_no in (Select distinct bill_no from KTTU_STONE_SALES_DETAILS ksd where ksd.bill_no =ksm.bill_no AND ksd.gs_code = '{4}')
                    GROUP BY convert(varchar,pay_date,103)"
                        , companyCode, branchCode, accstartdate, accenddate, gs_code, sgstPer, cgstPer, igstPer));

                        DataTable dbalance = SIGlobals.Globals.GetDataTable(string.Format(";with balance as (SELECT round(((COALESCE(balance_amt,0) / ( total_bill_amount)) * (item_total_after_discount)),0) as balance,convert(varchar,bill_date,103) as bill_date FROM KTTU_SALES_MASTER ksm,KTTU_SALES_DETAILS KSD WHERE\n"
                                + "ksm.bill_no = ksd.bill_no  AND bill_date >= '{2}' AND bill_date <= '{3}'  \n"
                                + "AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND KSD.company_code = '{0}' AND KSD.branch_code = '{1}' AND gs_code = '{4}' and KSD.SGST_Percent='{5}' and KSD.CGST_Percent='{6}' and KSD.IGST_Percent='{7}' AND cflag = 'N' and balance_amt>0\n" +
                       " ) select abs(sum(balance)) as amount,bill_date from balance group by bill_date", companyCode, branchCode, accstartdate, accenddate, gs_code, sgstPer, cgstPer, igstPer));


                        DataTable dpaid = SIGlobals.Globals.GetDataTable(string.Format(";with balance as (SELECT round(((COALESCE(balance_amt,0) / ( total_bill_amount)) * (item_total_after_discount)),0) as balance,convert(varchar,bill_date,103) as bill_date FROM KTTU_SALES_MASTER ksm,KTTU_SALES_DETAILS KSD WHERE\n"
                                + "ksm.bill_no = ksd.bill_no  AND bill_date >= '{2}' AND bill_date <= '{3}'  \n"
                                + "AND ksm.company_code = '{0}' AND ksm.branch_code = '{1}' AND KSD.company_code = '{0}' AND KSD.branch_code = '{1}' AND gs_code = '{4}' and KSD.SGST_Percent='{5}' and KSD.CGST_Percent='{6}' and KSD.IGST_Percent='{7}' AND cflag = 'N' and balance_amt<0\n" +
                                 ") select abs(sum(balance)) as amount,bill_date from balance group by bill_date", companyCode, branchCode, accstartdate, accenddate, gs_code, sgstPer, cgstPer, igstPer));


                        if (dtS.Rows.Count == 0 && dbalance.Rows.Count > 0) {
                            for (int m = 0; m < dbalance.Rows.Count; m++) {
                                dtS.Rows.Add(1);
                                dtS.Rows[m][0] = 0M;
                                dtS.Rows[m][1] = dbalance.Rows[m][1];
                                dtS.AcceptChanges();
                            }
                        }
                        bool IsSalesRoundoffUpdated = false;

                        decimal SalesBillAmount = 0M;
                        for (int k = 0; k < dtS.Rows.Count; k++) {

                            if (dbalance.Rows.Count > 0 && dpaid.Rows.Count > 0) {
                                SalesBillAmount = Convert.ToDecimal(dtS.Rows[k][0]) + Convert.ToDecimal(dbalance.Rows[k][0]) - Convert.ToDecimal(dpaid.Rows[k][0]);
                            }
                            else if (dpaid.Rows.Count > 0) {
                                SalesBillAmount = Convert.ToDecimal(dtS.Rows[k][0]) - Convert.ToDecimal(dpaid.Rows[k][0]);
                            }
                            else if (dbalance.Rows.Count > 0) {
                                SalesBillAmount = Convert.ToDecimal(dtS.Rows[k][0]) + Convert.ToDecimal(dbalance.Rows[k][0]);
                            }


                            else
                                SalesBillAmount = Convert.ToDecimal(dtS.Rows[k][0]);

                            taxVal = decimal.Round(((SalesBillAmount) - (((SalesBillAmount) * 100) / (100 + taxPer))), 2);
                            //SalesBillAmount -= round_off;
                            string tempDateTime = dtS.Rows[k][1].ToString();
                            voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            exciseVal = decimal.Round(((SalesBillAmount - taxVal) - (((SalesBillAmount - taxVal) * 100) / (100 + exciseper))), 2);
                            eduVal = decimal.Round(((exciseVal * edcessper) / 100), 2);
                            hedVal = decimal.Round(((exciseVal * hedcessper) / 100), 2);

                            //cgstAmount = decimal.Round((SalesBillAmount * cgstPer) / 100, 2);
                            //sgstAmount = decimal.Round((SalesBillAmount * sgstPer) / 100, 2);
                            //igstAmount = decimal.Round((SalesBillAmount * igstPer) / 100, 2);
                            //cgstValue = decimal.Round(dVal - (dVal * 100) / (100 + cgstper), 2);
                            decimal gstPer = cgstPer + sgstPer + igstPer;
                            decimal gstAmount = decimal.Round(((SalesBillAmount) - (((SalesBillAmount) * 100) / (100 + gstPer))), 2);
                            decimal finalGSTAmount = gstAmount / 2;

                            if (igstPer > 0)
                                igstAmount = gstAmount;
                            else {
                                cgstAmount = finalGSTAmount;
                                sgstAmount = finalGSTAmount;
                            }

                            voucherAmount = decimal.Round(SalesBillAmount - taxVal - exciseVal - eduVal - hedVal - cgstAmount - sgstAmount - igstAmount, 2);

                            if (GetConfigurationValue("2002") == 1) {
                                if (igstAmount > 0)
                                    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 7 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                                else
                                    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            }
                            else
                                accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));

                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isExcise = true;
                            voucherAmount = exciseVal;
                            accCode = 9;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isExcise = false;

                            isHEdu = true;
                            voucherAmount = hedVal;
                            accCode = 10;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isHEdu = false;

                            isEdu = true;
                            voucherAmount = eduVal;
                            accCode = 11;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isEdu = false;
                            isVat = true;
                            int taxid = GetVatID(taxPer, companyCode, branchCode);
                            voucherAmount = taxVal;
                            //accCode = GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", taxid, companyCode, branchCode));
                            accCode = GetAccountCode("T", "S", taxid.ToString());
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isSales = false;
                            isVat = false;
                            if (cgstAmount > 0) {
                                voucherTrans.narration = "OUTPUT CGST COLLECTED THROUGH SALES";
                                if (gs_code.Equals("WT")) {
                                    if (gstPer == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(93);
                                    else //18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(118);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPer == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(118);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(138);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(84);
                                voucherTrans.cr_amount = voucherAmount = cgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (sgstAmount > 0) {
                                if (gs_code.Equals("WT")) {
                                    if (gstPer == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(94);
                                    else //18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(117);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPer == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(117);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(137);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(85);

                                voucherTrans.narration = "OUTPUT SGST COLLECTED THROUGH SALES";
                                voucherTrans.cr_amount = voucherAmount = sgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (igstAmount > 0) {
                                if (gs_code.Equals("WT")) {
                                    if (gstPer == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(95);
                                    else // 18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(119);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPer == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(119);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(139);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(86);

                                voucherTrans.narration = "OUTPUT IGST COLLECTED THROUGH SALES";
                                voucherTrans.cr_amount = voucherAmount = igstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }


                        }

                        isSales = true;
                        if (!IsSalesRoundoffUpdated) {

                            isSalesRoundOff = true;
                            if (dtRound.Rows.Count > 0) {
                                //if (i == dtRound.Rows.Count)
                                //{
                                //    voucherAmount = Convert.ToDecimal(dtRound.Rows[0][0].ToString());
                                //    accCode = GetMagnaAccountCode(45);
                                //    if (voucherAmount != 0)
                                //    {
                                //        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                //            return false;
                                //    }

                                //}
                            }

                            isSalesRoundOff = false;
                            IsSalesRoundoffUpdated = true;
                        }

                        for (int k = 0; k < dtSS.Rows.Count; k++) {
                            isSales = true;
                            SalesBillAmount = Convert.ToDecimal(dtSS.Rows[k][0]);
                            string tempDateTime = dtSS.Rows[k][1].ToString();
                            voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            taxVal = decimal.Round((SalesBillAmount - ((SalesBillAmount * 100) / (100 + taxPer))), 0);

                            decimal gstPer = cgstPer + sgstPer + igstPer;
                            decimal gstAmount = decimal.Round(((SalesBillAmount) - (((SalesBillAmount) * 100) / (100 + gstPer))), 2);
                            decimal finalGSTAmount = gstAmount / 2;

                            if (igstPer > 0)
                                igstAmount = gstAmount;
                            else {
                                cgstAmount = finalGSTAmount;
                                sgstAmount = finalGSTAmount;
                            }

                            voucherAmount = Math.Round(SalesBillAmount - cgstAmount - sgstAmount - igstAmount, 0, MidpointRounding.ToEven);
                            accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isExcise = true;
                            voucherAmount = exciseVal;
                            accCode = 9;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isExcise = false;

                            isHEdu = true;
                            voucherAmount = hedVal;
                            accCode = 10;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isHEdu = false;

                            isEdu = true;
                            voucherAmount = eduVal;
                            accCode = 11;
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isEdu = false;
                            isVat = true;
                            //int taxid = GetVatID(taxPer, branchCode, companyCode);
                            //voucherAmount = taxVal;
                            //accCode = GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", taxid, companyCode, branchCode));
                            //if (voucherAmount > 0)
                            //{
                            //    if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                            //        return false;
                            //}
                            isSales = false;
                            isVat = false;
                            if (cgstAmount > 0) {
                                voucherTrans.narration = "Stone Sales CGST Collected";
                                voucherTrans.acc_code = accCode = GetMagnaAccountCode(84);
                                voucherTrans.cr_amount = voucherAmount = cgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (sgstAmount > 0) {
                                voucherTrans.acc_code = accCode = GetMagnaAccountCode(85);
                                voucherTrans.narration = "Stone Sales SGST Collected";
                                voucherTrans.cr_amount = voucherAmount = sgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (igstAmount > 0) {
                                voucherTrans.acc_code = accCode = GetMagnaAccountCode(86);
                                voucherTrans.narration = "Stone Sales IGST Collected";
                                voucherTrans.cr_amount = voucherAmount = igstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                        }

                    }
                    isSales = false;
                }
                #endregion

                #region PurchaseAdjustedUpdate
                DataTable dtPurchase = GetPurchaseAdjustedBills();


                if (dtPurchase != null && dtPurchase.Rows.Count > 0) {
                    bool IsPurchaseRoundoffUpdated = false;
                    isPurchase = true;

                    for (int i = 0; i < dtPurchase.Rows.Count; i++) {
                        gs_code = Convert.ToString(dtPurchase.Rows[i]["GStock"]);
                        decimal sgstPer = Convert.ToDecimal(dtPurchase.Rows[i]["SGST_Percent"]);
                        decimal cgstPer = Convert.ToDecimal(dtPurchase.Rows[i]["CGST_Percent"]);
                        decimal igstPer = Convert.ToDecimal(dtPurchase.Rows[i]["IGST_Percent"]);
                        decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0;

                        //DataTable dtPurchaseAdjustedBills = SIGlobals.Globals.GetDataTable(string.Format("SELECT COALESCE(SUM(gold_amount),0) + COALESCE(SUM(diamond_amount),0) ,convert(varchar,kpd.pay_date,103) FROM KTTU_PAYMENT_DETAILS kpd,KTTU_PURCHASE_MASTER kpm,KTTU_PURCHASE_DETAILS kppd WHERE kpm.bill_no = kpd.Ref_BillNo  AND kppd.bill_no = kpm.bill_no AND kpd.trans_type = 'S' AND kpd.pay_mode = 'PB' AND kpd.cflag = 'N' AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND kppd.gs_code = '{4}' AND kpd.cflag = 'N' AND sal_bill_type IS NULL AND itemwise_tax_percentage ='{5}' GROUP BY convert(varchar,kpd.pay_date,103) \n", branchCode, companyCode, accstartdate, accenddate, gs_code,Convert.ToDecimal(dtPurchase.Rows[i]["tax"])));
                        DataTable dtPurchaseAdjustedBills = SIGlobals.Globals.GetDataTable(string.Format("SELECT round(COALESCE(SUM(item_amount),0),0) ,convert(varchar,kpd.pay_date,103) FROM KTTU_PAYMENT_DETAILS kpd,KTTU_PURCHASE_MASTER kpm,KTTU_PURCHASE_DETAILS kppd WHERE kpm.bill_no = kpd.Ref_BillNo  AND kppd.bill_no = kpm.bill_no AND kpd.trans_type = 'S' AND kpd.pay_mode = 'PB' AND kpd.cflag = 'N' AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kppd.company_code = '{0}' AND kppd.branch_code = '{1}' AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}' AND kppd.gs_code = '{4}' AND kpd.cflag = 'N' AND sal_bill_type IS NULL AND itemwise_tax_percentage ='{5}' AND kppd.CGST_Percent ='{6}' AND kppd.SGST_Percent ='{7}' AND kppd.IGST_Percent ='{8}' GROUP BY convert(varchar,kpd.pay_date,103) \n", companyCode, branchCode, accstartdate, accenddate, gs_code, Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), cgstPer, sgstPer, igstPer));
                        decimal dtPurchaseAdjustedDiamondBills = GetScalarValue<decimal>(string.Format(";with PurchaseDiamond as (SELECT  isnull((select COALESCE(SUM(amount),0)  from kttu_purchase_stone_details where kpd.bill_no=bill_no and gs_code='OD'),0) as Damt FROM KTTU_PURCHASE_DETAILS kpd,KTTU_PURCHASE_MASTER kpm WHERE kpm.bill_no = kpd.bill_no AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kpm.p_date >= '{2}' AND kpm.p_date <= '{3}' AND kpm.pur_item = '{4}' AND kpm.cflag = 'N' and kpm.bill_no in (select distinct ref_billNo from kttu_payment_details where trans_type='S' and pay_mode='PB' and cflag='N' and company_code='{0}' and branch_code='{1}') AND itemwise_tax_percentage ='{5}' AND kpd.CGST_Percent ='{6}' AND kpd.SGST_Percent ='{7}' AND kpd.IGST_Percent ='{8}'  GROUP BY convert(varchar,kpm.p_date,103),kpd.bill_no )  select sum(dAmt) as dAmt from PurchaseDiamond\n", companyCode, branchCode, accstartdate, accenddate, gs_code, Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), cgstPer, sgstPer, igstPer));
                        decimal PurchaseDiamondAmountCashPaid = GetScalarValue<decimal>(string.Format(";with PurchaseDiamond as (SELECT  isnull((select COALESCE(SUM(amount),0) from kttu_purchase_stone_details where kpd.bill_no=bill_no and gs_code='OD'),0) as dAmt,convert(varchar,kpm.p_date,103) as date FROM KTTU_PURCHASE_DETAILS kpd,KTTU_PURCHASE_MASTER kpm WHERE kpm.bill_no = kpd.bill_no AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kpm.p_date >= '{2}' AND kpm.p_date <= '{3}' AND kpm.pur_item = '{4}' AND kpm.cflag = 'N' and kpm.bill_no in (select distinct series_no from kttu_payment_details where trans_type='P' and cflag='N' and company_code='{0}' and branch_code='{1}') AND itemwise_tax_percentage ='{5}' AND kpd.CGST_Percent ='{6}' AND kpd.SGST_Percent ='{7}' AND kpd.IGST_Percent ='{8}'  GROUP BY convert(varchar,kpm.p_date,103) ,kpd.bill_no )  select sum(dAmt) as dAmt from PurchaseDiamond\n", companyCode, branchCode, accstartdate, accenddate, gs_code, Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), cgstPer, sgstPer, igstPer));
                        decimal dtPurchaseAdjustedStoneBills = GetScalarValue<decimal>(string.Format(";with PurchaseStone as (SELECT isnull((select COALESCE(SUM(amount),0)  from kttu_purchase_stone_details where kpd.bill_no=bill_no and gs_code='OST' AND company_code = '{0}' AND branch_code = '{1}' ),0)as SAmt FROM KTTU_PURCHASE_DETAILS kpd,KTTU_PURCHASE_MASTER kpm WHERE kpm.bill_no = kpd.bill_no AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kpm.p_date >= '{2}' AND kpm.p_date <= '{3}' AND kpm.pur_item = '{4}' AND kpm.cflag = 'N' AND itemwise_tax_percentage ='{5}' AND kpd.CGST_Percent ='{6}' AND kpd.SGST_Percent ='{7}' AND kpd.IGST_Percent ='{8}' GROUP BY convert(varchar,kpm.p_date,103),kpd.bill_no) select sum(SAmt) as SAmt from PurchaseStone \n", companyCode, branchCode, accstartdate, accenddate, gs_code, Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), cgstPer, sgstPer, igstPer));
                        decimal PurchaseStoneAmountCashPaid = GetScalarValue<decimal>(string.Format(";with PurchaseDiamond as (SELECT  isnull((select COALESCE(SUM(amount),0) from kttu_purchase_stone_details where kpd.bill_no=bill_no and gs_code='OST' AND company_code = '{0}' AND branch_code = '{1}'),0) as dAmt,convert(varchar,kpm.p_date,103) as date FROM KTTU_PURCHASE_DETAILS kpd,KTTU_PURCHASE_MASTER kpm WHERE kpm.bill_no = kpd.bill_no AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}' AND kpm.p_date >= '{2}' AND kpm.p_date <= '{3}' AND kpm.pur_item = '{4}' AND kpm.cflag = 'N' and kpm.bill_no in (select distinct series_no from kttu_payment_details where trans_type='P' and cflag='N' and company_code='{0}' and branch_code='{1}') AND itemwise_tax_percentage ='{5}' AND kpd.CGST_Percent ='{6}' AND kpd.SGST_Percent ='{7}' AND kpd.IGST_Percent ='{8}'  GROUP BY convert(varchar,kpm.p_date,103) ,kpd.bill_no )  select sum(dAmt) as dAmt from PurchaseDiamond\n", companyCode, branchCode, accstartdate, accenddate, gs_code, Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), cgstPer, sgstPer, igstPer));

                        decimal total_tax_amount = Convert.ToDecimal(dtPurchase.Rows[i]["total_tax_amount"]);
                        decimal purchaseAdjustedBillAmount = 0;
                        for (int k = 0; k < dtPurchaseAdjustedBills.Rows.Count; k++) {
                            if (GetConfigurationValue("2001") == 1) {
                                if (dtPurchaseAdjustedStoneBills == 0 && dtPurchaseAdjustedDiamondBills == 0)
                                    purchaseAdjustedBillAmount = Convert.ToDecimal(dtPurchaseAdjustedBills.Rows[k][0]);
                                else if (dtPurchaseAdjustedStoneBills != 0 && dtPurchaseAdjustedDiamondBills == 0)
                                    purchaseAdjustedBillAmount = Convert.ToDecimal(dtPurchaseAdjustedBills.Rows[k][0]) - (dtPurchaseAdjustedStoneBills - PurchaseStoneAmountCashPaid);
                                else if (dtPurchaseAdjustedStoneBills == 0 && dtPurchaseAdjustedDiamondBills != 0)
                                    purchaseAdjustedBillAmount = Convert.ToDecimal(dtPurchaseAdjustedBills.Rows[k][0]) - (dtPurchaseAdjustedDiamondBills);// - PurchaseDiamondAmountCashPaid);

                                else
                                    purchaseAdjustedBillAmount = Convert.ToDecimal(dtPurchaseAdjustedBills.Rows[k][0]) - ((dtPurchaseAdjustedDiamondBills + dtPurchaseAdjustedStoneBills));// - (PurchaseDiamondAmountCashPaid+PurchaseStoneAmountCashPaid));
                            }
                            else
                                purchaseAdjustedBillAmount = Convert.ToDecimal(dtPurchaseAdjustedBills.Rows[k][0]);

                            string tempDateTime = dtPurchaseAdjustedBills.Rows[k][1].ToString();
                            voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                            decimal gstPer = cgstPer + sgstPer + igstPer;
                            decimal gstAmount = (purchaseAdjustedBillAmount * gstPer) / 100;
                            decimal finalGSTAmount = gstAmount / 2;

                            if (igstPer > 0)
                                igstAmount = gstAmount;
                            else {
                                cgstAmount = finalGSTAmount;
                                sgstAmount = finalGSTAmount;
                            }
                            voucherAmount = decimal.Round(purchaseAdjustedBillAmount, 2);
                            accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 3 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (GetConfigurationValue("2001") == 1) // Bhima Bangalore Slitting Stone diamond posting
                            {
                                if (dtPurchaseAdjustedDiamondBills > 0) {
                                    voucherAmount = Math.Abs(dtPurchaseAdjustedDiamondBills);
                                    accCode = GetScalarValue<int>(string.Format(@"select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='OD' 
                                AND gs_seq_no = 3 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                                    if (voucherAmount > 0) {
                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }
                                }

                                if (dtPurchaseAdjustedStoneBills > 0) {
                                    voucherAmount = Math.Abs(dtPurchaseAdjustedStoneBills - PurchaseStoneAmountCashPaid);
                                    accCode = GetScalarValue<int>(string.Format(@"select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='OST' 
                                AND gs_seq_no = 3 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                                    if (voucherAmount > 0) {
                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }
                                }
                            }

                            if (total_tax_amount > 0) {
                                isVat = true;
                                int taxid = GetVatID(Convert.ToDecimal(dtPurchase.Rows[i]["tax"]), companyCode, branchCode);
                                voucherAmount = total_tax_amount;
                                //accCode = GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", taxid, companyCode, branchCode));
                                accCode = GetAccountCode("T", "P", taxid.ToString());
                                if (voucherAmount > 0) {
                                    if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                        return false;
                                }


                            }
                            isVat = false;
                            //for (int j = 0; j < 2; j++)
                            //{
                            //    voucherTrans.acc_type = "O";
                            //    voucherTrans.acc_code_master = 0;
                            //    voucherTrans.narration = "Purchase GST Amount";
                            //    if (cgstAmount > 0)
                            //    {

                            //        if (j == 0)
                            //        {
                            //            accCode = GetMagnaAccountCode(81);
                            //            voucherAmount = cgstAmount;
                            //            voucherTrans.cr_amount = 0;
                            //            voucherTrans.dr_amount = cgstAmount;
                            //        }
                            //        else
                            //        {
                            //            accCode = GetMagnaAccountCode(90);
                            //            voucherAmount = cgstAmount;
                            //            voucherTrans.cr_amount = cgstAmount;
                            //            voucherTrans.dr_amount = 0;
                            //        }
                            //        voucherTrans.acc_code = accCode;
                            //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                            //            return false;
                            //    }

                            //    if (sgstAmount > 0)
                            //    {
                            //        if (j == 0)
                            //        {
                            //            accCode = GetMagnaAccountCode(82);
                            //            voucherAmount = cgstAmount;
                            //            voucherTrans.cr_amount = 0;
                            //            voucherTrans.dr_amount = sgstAmount;
                            //        }
                            //        else
                            //        {
                            //            accCode = GetMagnaAccountCode(91);
                            //            voucherAmount = sgstAmount;
                            //            voucherTrans.cr_amount = sgstAmount;
                            //            voucherTrans.dr_amount = 0;
                            //        }
                            //        voucherTrans.acc_code = accCode;
                            //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                            //            return false;
                            //    }



                            //    if (igstAmount > 0)
                            //    {
                            //        if (j == 0)
                            //        {
                            //            accCode = GetMagnaAccountCode(83);
                            //            voucherAmount = igstAmount;
                            //            voucherTrans.cr_amount = 0;
                            //            voucherTrans.dr_amount = igstAmount;
                            //        }
                            //        else
                            //        {
                            //            accCode = GetMagnaAccountCode(92);
                            //            voucherAmount = igstAmount;
                            //            voucherTrans.cr_amount = igstAmount;
                            //            voucherTrans.dr_amount = 0;

                            //        }
                            //        voucherTrans.acc_code = accCode;
                            //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                            //            return false;
                            //    }

                            // }
                            isPurchase = false;
                        }
                        isPurchase = true;
                        DataTable dtSP = SIGlobals.Globals.GetDataTable(string.Format("SELECT COALESCE(SUM(pay_amt),0),convert(varchar,kpd.pay_date,103) FROM KTTU_PAYMENT_DETAILS kpd,KTTU_STONE_PURCHASE_MASTER kpm WHERE kpm.bill_no = kpd.Ref_BillNo AND kpd.trans_type = 'SS' AND kpd.pay_mode = 'PB' AND kpd.cflag = 'N' AND kpm.company_code = '{0}' AND kpm.branch_code = '{1}' AND kpd.company_code = '{0}' AND kpd.branch_code = '{1}'  AND kpd.pay_date >= '{2}' AND kpd.pay_date <= '{3}'  AND kpd.cflag = 'N' AND sal_bill_type IS NULL \n" +
                            " and bill_no in(select  kspd.bill_no from KTTU_STONE_PURCHASE_DETAILS kspd ,KTTU_STONE_PURCHASE_MASTER  kp where  kspd.bill_no = kp.bill_no AND kspd.gs_code = '{4}' \n" +
                            " AND kp.company_code = '{0}' AND kp.branch_code = '{1}' AND kspd.company_code = '{0}' AND kspd.branch_code = '{1}' AND kp.sales_date >= '{2}' AND kp.sales_date <= '{3}'  and cflag ='N')  GROUP BY convert(varchar,kpd.pay_date,103)"
                            , companyCode, branchCode, accstartdate, accenddate, gs_code));
                        for (int k = 0; k < dtSP.Rows.Count; k++) {

                            purchaseAdjustedBillAmount = Convert.ToDecimal(dtSP.Rows[k][0]);
                            string tempDateTime = dtSP.Rows[k][1].ToString();
                            voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                            voucherAmount = decimal.Round(purchaseAdjustedBillAmount, 2);
                            accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                        }
                    }
                    isPurchase = false;
                }
                #endregion

                #region SalesReturn Update
                DataTable dtSalesReturn = GetSalesReturnAmount();
                if (dtSalesReturn != null && dtSalesReturn.Rows.Count > 0) {
                    for (int i = 0; i < dtSalesReturn.Rows.Count; i++) {
                        isSalesReturn = true;

                        decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0, FinalAmount = 0;
                        decimal sgstPer = Convert.ToDecimal(dtSalesReturn.Rows[i]["SGST_Percent"]);
                        decimal cgstPer = Convert.ToDecimal(dtSalesReturn.Rows[i]["CGST_Percent"]);
                        decimal igstPer = Convert.ToDecimal(dtSalesReturn.Rows[i]["IGST_Percent"]);

                        FinalAmount = Convert.ToDecimal(dtSalesReturn.Rows[i]["Amount"]);
                        cgstAmount = decimal.Round((voucherAmount * cgstPer) / 100, 8);
                        sgstAmount = decimal.Round((voucherAmount * sgstPer) / 100, 8);
                        igstAmount = decimal.Round((voucherAmount * igstPer) / 100, 8);

                        decimal gstPer = cgstPer + sgstPer + igstPer;
                        decimal gstAmount = decimal.Round(((FinalAmount) - (((FinalAmount) * 100) / (100 + gstPer))), 2);
                        decimal finalGSTAmount = gstAmount / 2;

                        if (igstPer > 0)
                            igstAmount = gstAmount;
                        else {
                            cgstAmount = finalGSTAmount;
                            sgstAmount = finalGSTAmount;
                        }

                        voucherAmount = (FinalAmount - sgstAmount - cgstAmount - igstAmount);

                        gs_code = Convert.ToString(dtSalesReturn.Rows[i]["GStock"]);
                        DataTable dtSRS = SIGlobals.Globals.GetDataTable(string.Format("SELECT ksm.tax,ksm.gstype FROM KTTU_SALES_MASTER ksm WHERE ksm.gstype = '{2}' AND ksm.bill_date >= '{0}' AND bill_date <= '{1}' AND company_code='{3}' and branch_code='{4}' GROUP BY ksm.gstype,ksm.tax", accstartdate, accenddate, gs_code, companyCode, branchCode));
                        if (dtSRS.Rows.Count == 0)
                            dtSRS = SIGlobals.Globals.GetDataTable(string.Format("SELECT sm.tax,sm.gstype FROM KTTU_SR_MASTER ksm ,KTTU_SALES_MASTER sm WHERE ksm.voture_bill_no = sm.bill_no AND sm.gstype = '{2}' AND ksm.sr_date >= '{0}' AND ksm.sr_date <= '{1}' AND company_code='{3}' and branch_code='{4}'  GROUP BY sm.gstype,sm.tax", accstartdate, accenddate, gs_code, companyCode, branchCode));

                        decimal taxPer = 0;
                        if (dtSRS.Rows.Count > 0) {
                            taxPer = Convert.ToDecimal(dtSRS.Rows[0][0]);
                        }
                        else {
                            taxPer = decimal.Round(Convert.ToDecimal(dtSalesReturn.Rows[i]["Amount"]), 2);
                        }
                        decimal exciseVal = 0;
                        decimal hedVal = 0;
                        decimal eduVal = 0;
                        accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 4 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                        isExcise = true;
                        voucherAmount = exciseVal;
                        accCode = 9;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                        isExcise = false;

                        isHEdu = true;
                        voucherAmount = hedVal;
                        accCode = 10;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                        isHEdu = false;

                        isEdu = true;
                        voucherAmount = eduVal;
                        accCode = 11;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                        isEdu = false;
                        isVat = true;
                        voucherAmount = -Convert.ToDecimal(dtSalesReturn.Rows[i]["Tax"]);
                        int taxid = GetScalarValue<int>(string.Format("SELECT tax_Id FROM KSTS_SALETAX_MASTER WHERE tax = {0} AND company_code = '{1}' AND branch_code = '{2}'", taxPer, companyCode, branchCode));
                        //accCode = GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", taxid, companyCode, branchCode));
                        accCode = GetAccountCode("T", "P", taxid.ToString());
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (cgstAmount > 0) {
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(84);
                            voucherAmount = cgstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (sgstAmount > 0) {
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(85);
                            voucherAmount = sgstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (igstAmount > 0) {
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(86);
                            voucherAmount = igstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        isVat = false;
                    }
                    isSalesReturn = false;
                }

                #endregion

                #region Credit Bill Update
                if (dtCredit != null && dtCredit.Rows.Count > 0) {
                    for (int i = 0; i < dtCredit.Rows.Count; i++) {
                        isCredit = true;
                        voucherAmount = Convert.ToDecimal(dtCredit.Rows[i]["balance_amt"]);
                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                        accCode = 8;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, 0, "", voucherAmount))
                                return false;
                        }
                    }
                }

                isCredit = false;
                #endregion

                #region Chit and Order Adjustment
                if (dtCOPayments != null && dtCOPayments.Rows.Count > 0) {
                    voucherTrans.acc_type = "O";
                    for (int i = 0; i < dtCOPayments.Rows.Count; i++) {
                        isCOPayments = true;

                        decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0;
                        decimal sgstPer = Convert.ToDecimal(dtCOPayments.Rows[i]["SGST_Percent"]);
                        decimal cgstPer = Convert.ToDecimal(dtCOPayments.Rows[i]["CGST_Percent"]);
                        decimal igstPer = Convert.ToDecimal(dtCOPayments.Rows[i]["IGST_Percent"]);

                        string tempDateTime = dtCOPayments.Rows[i]["Column1"].ToString();
                        voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                        cgstAmount = Convert.ToDecimal(dtCOPayments.Rows[i]["CGST_Amount"]);
                        sgstAmount = Convert.ToDecimal(dtCOPayments.Rows[i]["SGST_Amount"]);
                        igstAmount = Convert.ToDecimal(dtCOPayments.Rows[i]["IGST_Amount"]);
                        voucherAmount = decimal.Round(Convert.ToDecimal(dtCOPayments.Rows[i]["Amount"]) - cgstAmount - sgstAmount - igstAmount, 2);

                        if (string.Compare(dtCOPayments.Rows[i]["pay_mode"].ToString(), "OP") == 0) {
                            accCode = 7;
                        }

                        if (string.Compare(dtCOPayments.Rows[i]["pay_mode"].ToString(), "CT") == 0) {
                            if (string.Compare(dtCOPayments.Rows[i]["schtype"].ToString(), "W") == 0) {
                                accCode = 13;
                            }
                            else if (string.Compare(dtCOPayments.Rows[i]["schtype"].ToString(), "B") == 0) {
                                accCode = 12;
                            }
                            else {
                                if (string.Compare(dtCOPayments.Rows[i]["scheme_code"].ToString(), "AP") == 0) {
                                    string scheme_code = "AV";
                                    if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                        accCode = GetMagnaAccountCode(60);
                                    else
                                        accCode = GetScalarValue<int>(string.Format(@"select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code ='{0}' 
                                              and company_code='{1}' and branch_code='{2}'"
                                                  , scheme_code, companyCode, branchCode));
                                }
                                else {
                                    if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                        accCode = GetMagnaAccountCode(60);
                                    else
                                        accCode = GetScalarValue<int>(string.Format(@"select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code ='{0}' 
                                   and company_code='{1}' and branch_code='{2}'"
                                       , Convert.ToString(dtCOPayments.Rows[i]["scheme_code"]), companyCode, branchCode));
                                }
                            }
                        }

                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (Convert.ToDecimal(dtCOPayments.Rows[i]["total_tax_amount"]) > 0) {
                            isVat = true;

                            int taxid = GetVatID(Convert.ToDecimal(dtCOPayments.Rows[i]["taxPer"]), companyCode, branchCode);
                            voucherAmount = Convert.ToDecimal(dtCOPayments.Rows[i]["total_tax_amount"]);
                            //accCode = GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", taxid, companyCode, branchCode));
                            accCode = GetAccountCode("T", "P", taxid.ToString());
                            if (voucherAmount > 0) {
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }
                            isVat = false;

                        }

                        isCOPayments = false;


                        if (cgstAmount > 0) {
                            voucherTrans.narration = "Order CGST Asjusted through Sales";
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(84);
                            voucherTrans.dr_amount = voucherAmount = cgstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (sgstAmount > 0) {
                            voucherTrans.narration = "Order SGST Adjusted through Sales";
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(85);
                            voucherTrans.dr_amount = voucherAmount = sgstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (igstAmount > 0) {
                            voucherTrans.narration = "Order IGST Adjusted through Sales";
                            voucherTrans.acc_code = accCode = GetMagnaAccountCode(86);
                            voucherTrans.dr_amount = voucherAmount = igstAmount;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                    }

                }

                #endregion

                #region HI Payments
                if (dtHIPayments != null && dtHIPayments.Rows.Count > 0) {
                    voucherTrans.acc_type = "O";
                    for (int i = 0; i < dtHIPayments.Rows.Count; i++) {
                        isHIPayments = true;
                        string tempDateTime = dtHIPayments.Rows[i]["Column1"].ToString();
                        voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        voucherAmount = decimal.Round(Convert.ToDecimal(dtHIPayments.Rows[i]["Amount"]), 2);

                        if (string.Compare(dtHIPayments.Rows[i]["pay_mode"].ToString(), "HI") == 0) {
                            accCode = 9;
                        }

                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                    }
                    isHIPayments = false;
                }

                #endregion

                #region isCashPaid

                string cashQuery = string.Format("EXEC [usp_acc_Purchase_Sales_Amount] '{2}','{3}','{0}','{1}','{4}','{5}'"
               , accstartdate, accenddate, companyCode, branchCode, "PC", GetDefaultCurrencyCode());

                DataTable dtCash = SIGlobals.Globals.GetDataTable(cashQuery);
                decimal OrderAmount = 0.00M;


                if (dtCash.Rows.Count > 0) {
                    isCashPaid = true;

                    decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0;

                    for (int k = 0; k < dtCash.Rows.Count; k++) {
                        string tempDateTime = dtCash.Rows[k]["Column2"].ToString();
                        voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        voucherAmount = Convert.ToDecimal(dtCash.Rows[k][0]);
                        if (string.Compare(Convert.ToString(dtCash.Rows[k]["trans_type"]), "RC") == 0) {
                            voucherTrans.acc_code = Convert.ToInt32(dtCash.Rows[k]["acc_code"]);
                            voucherTrans.chq_no = dtCash.Rows[k]["chq_no"].ToString();
                            voucherTrans.party_name = dtCash.Rows[k]["Cust Name"].ToString();
                            cashPaidReceiptNo = dtCash.Rows[k]["bill_no"].ToString();
                            if (!CreateVoucherTransaction(out errorXml, 2, "B", voucherAmount))
                                return false;

                        }
                        else if (string.Compare(Convert.ToString(dtCash.Rows[k]["trans_type"]), "RO") == 0) {
                            cgstAmount = sgstAmount = igstAmount = 0;
                            if(dtCash.Rows[k]["CGST_Amount"] != DBNull.Value || dtCash.Rows[k]["CGST_Amount"] == null)
                                cgstAmount = Convert.ToDecimal(dtCash.Rows[k]["CGST_Amount"]);
                            if (dtCash.Rows[k]["SGST_Amount"] != DBNull.Value || dtCash.Rows[k]["SGST_Amount"] == null)
                                sgstAmount = Convert.ToDecimal(dtCash.Rows[k]["SGST_Amount"]);
                            if (dtCash.Rows[k]["IGST_Amount"] != DBNull.Value || dtCash.Rows[k]["IGST_Amount"] == null)
                                igstAmount = Convert.ToDecimal(dtCash.Rows[k]["IGST_Amount"]);

                            voucherAmount = decimal.Round(Convert.ToDecimal(dtCash.Rows[k][0]) - cgstAmount - sgstAmount - igstAmount, 2);

                            voucherTrans.acc_code = 7;
                            voucherTrans.chq_no = string.Empty;
                            voucherTrans.party_name = dtCash.Rows[k]["Cust Name"].ToString();
                            cashPaidReceiptNo = dtCash.Rows[k]["bill_no"].ToString();
                            OrderAmount = voucherAmount;
                            if (!CreateVoucherTransaction(out errorXml, 3, "C", voucherAmount))
                                return false;


                            voucherTrans.narration = "Order GST ";
                            if (cgstAmount > 0) {
                                voucherTrans.acc_code = GetMagnaAccountCode(84);
                                voucherAmount = cgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, 3, "C", voucherAmount))
                                    return false;
                            }

                            if (sgstAmount > 0) {
                                voucherTrans.acc_code = GetMagnaAccountCode(85);
                                voucherAmount = sgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, 3, "C", voucherAmount))
                                    return false;
                            }

                            if (igstAmount > 0) {
                                voucherTrans.acc_code = GetMagnaAccountCode(86);
                                voucherAmount = igstAmount;
                                if (!CreateVoucherTransaction(out errorXml, 3, "C", voucherAmount))
                                    return false;
                            }

                        }
                        else {
                            if (voucherAmount != 0) {
                                if (!CreateVoucherTransaction(out errorXml, 0, "", voucherAmount))
                                    return false;

                                if (!CreateVoucherTransaction(out errorXml, 1, "C", voucherAmount))
                                    return false;

                            }
                        }
                    }
                    isCashPaid = false;

                }
                #endregion

                #region Payments
                if (dtPayments != null && dtPayments.Rows.Count > 0) {
                    for (int i = 0; i < dtPayments.Rows.Count; i++) {
                        isPayments = true;
                        decimal taxPer = 0;
                        decimal taxVal = 0;
                        decimal ExcisePer = 0;
                        decimal EduPercent = 0;
                        decimal HighEduPercent = 0;
                        decimal EduVal = 0;
                        decimal ExciseVal = 0;
                        decimal HighEduVal = 0;
                        decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0;
                        string tempDateTime = dtPayments.Rows[i]["pay_date"].ToString();

                        voucherTrans.currency_type = string.Empty;
                        voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        voucherTrans.chq_no = string.Empty;

                        string payMode = dtPayments.Rows[i]["pay_mode"].ToString();
                        string transType = dtPayments.Rows[i]["trans_type"].ToString();
                        string currencyType = dtPayments.Rows[i]["currencyType"].ToString();
                        decimal sgstPer = Convert.ToDecimal(dtPayments.Rows[i]["SGST_Percent"]);
                        decimal cgstPer = Convert.ToDecimal(dtPayments.Rows[i]["CGST_Percent"]);
                        decimal igstPer = Convert.ToDecimal(dtPayments.Rows[i]["IGST_Percent"]);
                        decimal gstPercentage = cgstPer + sgstPer + igstPer;

                        if (string.Compare(payMode, "C") == 0) {
                            voucherTrans.acc_type = "C";
                            gs_code = Convert.ToString(dtPayments.Rows[i]["gstype"]);

                            if (GetConfigurationValue("2002") == 1) {
                                if (igstAmount > 0)
                                    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 7 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                                else
                                    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            }
                            else
                                accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                        }

                        if (string.Compare(payMode, "Q") == 0 || string.Compare(payMode, "D") == 0) {
                            if (string.Compare(transType, "P") == 0 || string.Compare(transType, "SP") == 0) {
                                voucherTrans.acc_type = "B";
                                gs_code = Convert.ToString(dtPayments.Rows[i]["gstype"]);
                                voucherTrans.acc_code_master = Convert.ToInt32(dtPayments.Rows[i]["acc_code"]);
                                accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                            }
                            else {
                                accCode = 2;
                                voucherTrans.acc_type = "O";
                            }
                        }

                        if (string.Compare(payMode, "GV") == 0) {
                            voucherTrans.acc_type = "O";
                            if (string.Compare(transType, "S") == 0 || string.Compare(transType, "SS") == 0)
                                accCode = 4;
                            else
                                accCode = 0;
                        }

                        if (string.Compare(payMode, "EP") == 0) {
                            voucherTrans.acc_type = "O";
                            if (string.Compare(transType, "S") == 0 || string.Compare(transType, "SS") == 0)
                                accCode = 5;
                            else
                                accCode = 0;
                        }

                        if (string.Compare(payMode, "OGS") == 0 || string.Compare(payMode, "EKM") == 0) {
                            voucherTrans.acc_type = "O";
                            if (string.Compare(transType, "S") == 0 || string.Compare(transType, "SS") == 0)
                                accCode = 9;
                            else
                                accCode = 0;
                        }

                        if (string.Compare(payMode, "R") == 0) {

                            voucherTrans.acc_type = "O";

                            int accCodes = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where acc_name like'{0}%' AND company_code='{1}' AND branch_code='{2}'", dtPayments.Rows[i]["bank"].ToString(), companyCode, branchCode));
                            if (string.Compare(dtPayments.Rows[i]["acc_code"].ToString(), "0.00") == 0 || string.Compare(dtPayments.Rows[i]["acc_code"].ToString(), "0") == 0)
                                accCode = accCodes;
                            else {
                                if (string.Compare(transType, "S") == 0 || string.Compare(transType, "SS") == 0)
                                    accCode = Convert.ToInt32(dtPayments.Rows[i]["acc_code"]);
                                else
                                    accCode = 0;
                            }
                        }

                        if (string.Compare(payMode, "CV") == 0) {
                            voucherTrans.acc_type = "O";
                            voucherTrans.currency_type = currencyType;
                            if (string.Compare(transType, "S") == 0 || string.Compare(transType, "SS") == 0)
                                accCode = GetMagnaAccountCode(50);
                            else
                                accCode = 0;
                        }

                        if ((string.Compare(payMode, "C") == 0)) {
                            if ((string.Compare(transType, "SS") == 0) || (string.Compare(transType, "SP") == 0))
                                taxPer = GetScalarValue<decimal>(string.Format("SELECT tax FROM KTTU_STONE_SALES_MASTER WHERE gs_type = '{0}' AND sales_date BETWEEN '{1}' AND '{2}' AND company_code = '{3}' AND branch_code = '{4}'", gs_code, accstartdate, accenddate, companyCode, branchCode));
                            else {
                                taxPer = GetScalarValue<decimal>(string.Format("SELECT tax FROM KTTU_SALES_MASTER WHERE gstype = '{0}' AND bill_date BETWEEN '{1}' AND '{2}' AND company_code = '{3}' AND branch_code = '{4}'", gs_code, accstartdate, accenddate, companyCode, branchCode));
                                if (taxPer == 0)
                                    taxPer = GetScalarValue<decimal>(string.Format("SELECT tax FROM KSTS_GS_ITEM_ENTRY WHERE gs_code = '{0}'  AND company_code = '{1}' AND branch_code = '{2}'", gs_code, companyCode, branchCode));

                                ExcisePer = GetScalarValue<decimal>(string.Format("SELECT excise_duty_percent FROM KTTU_SALES_MASTER WHERE gstype = '{0}' AND bill_date BETWEEN '{1}' AND '{2}' AND company_code = '{3}' AND branch_code = '{4}'", gs_code, accstartdate, accenddate, companyCode, branchCode));
                                EduPercent = GetScalarValue<decimal>(string.Format("SELECT ed_cess_percent FROM KTTU_SALES_MASTER WHERE gstype = '{0}' AND bill_date BETWEEN '{1}' AND '{2}' AND company_code = '{3}' AND branch_code = '{4}'", gs_code, accstartdate, accenddate, companyCode, branchCode));
                                HighEduPercent = GetScalarValue<decimal>(string.Format("SELECT hed_cess_percent FROM KTTU_SALES_MASTER WHERE gstype = '{0}' AND bill_date BETWEEN '{1}' AND '{2}' AND company_code = '{3}' AND branch_code = '{4}'", gs_code, accstartdate, accenddate, companyCode, branchCode));
                            }
                        }

                        if ((string.Compare(payMode, "C") == 0 && string.Compare(transType, "P") != 0 && (taxPer > 0 || cgstPer > 0 || sgstPer > 0 || igstPer > 0)) || (string.Compare(payMode, "C") == 0 && string.Compare(transType, "SP") != 0 && (taxPer > 0 || cgstPer > 0 || sgstPer > 0 || igstPer > 0))) {

                            decimal grandtotal = Convert.ToDecimal(dtPayments.Rows[i]["Amount"]);
                            decimal billAmount = grandtotal;
                            decimal gstPer = cgstPer + sgstPer + igstPer;

                            decimal gstAmount = decimal.Round(((billAmount) - (((billAmount) * 100) / (100 + gstPer))), 2);
                            decimal finalGSTAmount = gstAmount / 2;

                            if (igstPer > 0)
                                igstAmount = gstAmount;
                            else {
                                cgstAmount = finalGSTAmount;
                                sgstAmount = finalGSTAmount;
                            }
                            taxVal = decimal.Round(grandtotal - (billAmount * 100) / (100 + taxPer), 2);
                            ExciseVal = decimal.Round((grandtotal - taxVal) - ((grandtotal - taxVal) * 100) / (100 + ExcisePer), 2);
                            EduVal = decimal.Round(((ExciseVal * EduPercent) / 100), 2);
                            HighEduVal = decimal.Round(((ExciseVal * HighEduPercent) / 100), 2);

                            voucherAmount = decimal.Round((grandtotal - ExciseVal - EduVal - HighEduVal - cgstAmount - sgstAmount - igstAmount), 2);
                        }
                        else {
                            voucherAmount = decimal.Round(Convert.ToDecimal(dtPayments.Rows[i]["Amount"]), 2);
                        }

                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, payMode, voucherAmount))
                                return false;
                        }

                        if (string.Compare(payMode, "C") == 0 && string.Compare(transType, "P") != 0 && string.Compare(transType, "SP") != 0) {
                            if (ExcisePer > 0 && ExciseVal > 0) {
                                isExcise = true;
                                accCode = 9;
                                voucherAmount = ExciseVal;

                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;

                                isExcise = false;
                            }
                            if (EduPercent > 0 && EduVal > 0) {
                                isEdu = true;
                                accCode = 11;
                                voucherAmount = EduVal;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                                isEdu = false;
                            }
                            if (HighEduPercent > 0 && HighEduVal > 0) {
                                isHEdu = true;
                                accCode = 10;

                                voucherAmount = HighEduVal;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;

                                isHEdu = false;
                            }
                            //if (taxPer > 0 && taxVal > 0)
                            //{
                            //    isVat = true;
                            //    int taxid = GetVatID(taxPer, branchCode, companyCode);
                            //    if (string.Compare(transType, "S") == 0)
                            //        accCode = GetAccountCode("T", "S", taxid.ToString());
                            //    else
                            //        accCode = CGlobals.GetVatAccCode(taxid, companyCode, branchCode);

                            //    voucherAmount = taxVal;
                            //    if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                            //        return false;


                            //}
                            isPayments = false;
                            isVat = false;
                            if (cgstAmount > 0) {
                                if (string.Compare(transType, "SS") == 0)
                                    voucherTrans.narration = "Stone Sales CGST Collected";
                                else
                                    voucherTrans.narration = "OUTPUT CGST COLLECTED THROUGH SALES";

                                if (gs_code.Equals("WT")) {
                                    if (gstPercentage == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(93);
                                    else //18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(118);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPercentage == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(118);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(138);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(84);
                                voucherTrans.cr_amount = voucherAmount = cgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (sgstAmount > 0) {
                                if (string.Compare(transType, "SS") == 0)
                                    voucherTrans.narration = "Stone Sales SGST Collected";
                                else
                                    voucherTrans.narration = "OUTPUT SGST COLLECTED THROUGH SALES";
                                if (gs_code.Equals("WT")) {
                                    if (gstPercentage == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(94);
                                    else //18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(117);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPercentage == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(117);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(137);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(85);
                                voucherTrans.cr_amount = voucherAmount = sgstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                            if (igstAmount > 0) {
                                if (string.Compare(transType, "SS") == 0)
                                    voucherTrans.narration = "Stone Sales IGST Collected";
                                else
                                    voucherTrans.narration = "OUTPUT IGST COLLECTED THROUGH SALES";
                                if (gs_code.Equals("WT")) {
                                    if (gstPercentage == 28)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(95);
                                    else // 18
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(119);
                                }
                                else if ((gs_code.Equals("INF") && GetConfigurationValue("2005") == 1) || (gs_code.Equals("SLF") && GetConfigurationValue("2005") == 1)) {
                                    if (gstPercentage == 18)
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(119);
                                    else //12
                                        voucherTrans.acc_code = accCode = GetMagnaAccountCode(139);
                                }
                                else
                                    voucherTrans.acc_code = accCode = GetMagnaAccountCode(86);

                                voucherTrans.cr_amount = voucherAmount = igstAmount;
                                if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                    return false;
                            }

                        }

                    }

                }
                isPayments = true;
                isPayPurchase = true;
                for (int j = 0; j < dtPurPay.Rows.Count; j++) {
                    isPayPurchase = true;
                    isPayments = true;
                    isPurchaseEpay = true;
                    string payMode = dtPurPay.Rows[j]["pay_mode"].ToString();
                    string transType = dtPurPay.Rows[j]["trans_type"].ToString();
                    decimal taxPer = Convert.ToDecimal(dtPurPay.Rows[j]["taxPer"]);
                    decimal sgstPer = Convert.ToDecimal(dtPurPay.Rows[j]["SGST_Percent"]);
                    decimal cgstPer = Convert.ToDecimal(dtPurPay.Rows[j]["CGST_Percent"]);
                    decimal igstPer = Convert.ToDecimal(dtPurPay.Rows[j]["IGST_Percent"]);

                    decimal taxVal = 0;
                    decimal cgstAmount = 0, sgstAmount = 0, igstAmount = 0;
                    decimal itemPercent = 0;

                    decimal diamond = GetScalarValue<decimal>(string.Format(@"select coalesce(sum(diamond_amount),0) from kttu_purchase_details where company_code='{0}' 
                 and branch_code='{1}' and bill_no='{2}'", companyCode, branchCode, dtPurPay.Rows[j]["bill_no"].ToString()));
                    decimal itemValue = GetScalarValue<decimal>(string.Format(@"select coalesce(sum(item_amount),0) from kttu_purchase_details where company_code='{0}' 
                 and branch_code='{1}' and bill_no='{2}'", companyCode, branchCode, dtPurPay.Rows[j]["bill_no"].ToString()));

                    if (payMode.Equals("C")) {
                        voucherTrans.acc_type = "C";
                        voucherTrans.acc_code_master = 1;
                    }
                    else if (payMode.Equals("EP")) {
                        voucherTrans.acc_type = "O";
                        voucherTrans.acc_code_master = 0;
                    }
                    else {
                        voucherTrans.acc_type = "B";
                        voucherTrans.acc_code_master = Convert.ToInt32(dtPurPay.Rows[j]["acc_code"]);
                    }
                    voucherTrans.chq_no = dtPurPay.Rows[j]["cheque_no"].ToString();
                    string tempDateTime = dtPurPay.Rows[j]["Column1"].ToString();
                    voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    decimal grandtotal = Convert.ToDecimal(dtPurPay.Rows[j]["Amount"]);
                    taxVal = decimal.Round(grandtotal - (grandtotal * 100) / (100 + taxPer), 2);
                    cgstAmount = decimal.Round(grandtotal - (grandtotal * 100) / (100 + sgstPer), 2);
                    sgstAmount = decimal.Round(grandtotal - (grandtotal * 100) / (100 + cgstPer), 2);
                    igstAmount = decimal.Round(grandtotal - (grandtotal * 100) / (100 + igstPer), 2);
                    voucherAmount = decimal.Round(Convert.ToDecimal(dtPurPay.Rows[j]["Amount"]), 2) - taxVal;

                    gs_code = Convert.ToString(dtPurPay.Rows[j]["gstype"]);

                    if (diamond > 0) {
                        itemPercent = Math.Round(((diamond / itemValue) * voucherAmount), 2);
                        voucherAmount = voucherAmount - itemPercent;
                    }

                    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 3 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));
                    //accCode = GetScalarValue<int>(string.Format("SELECT dbo.[ufn_GetAccountCode] ('{0}','{1}','{2}','{3}','{4}')", "GS","P",gs_code, branchCode, companyCode));
                    voucherTrans.acc_code = accCode;

                    if (voucherAmount > 0) {
                        if (!CreateVoucherTransaction(out errorXml, j, payMode, voucherAmount))
                            return false;
                    }

                    if (diamond > 0) {
                        voucherAmount = itemPercent;
                        accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='OD' AND gs_seq_no = 3 and company_code='{1}' and branch_code='{2}'", gs_code, companyCode, branchCode));

                        if (!CreateVoucherTransaction(out errorXml, j, payMode, voucherAmount))
                            return false;
                    }

                    if (taxVal > 0) {
                        int taxid = GetVatID(taxPer, companyCode, branchCode);
                        accCode = GetVatAccCode(taxid, branchCode, companyCode);

                        if (!CreateVoucherTransaction(out errorXml, j, payMode, taxVal))
                            return false;
                    }

                    if (payMode.Equals("EP")) {
                        isPurchaseEpay = false;
                        voucherAmount = grandtotal;
                        accCode = 5;
                        if (!CreateVoucherTransaction(out errorXml, j, payMode, voucherAmount))
                            return false;
                    }

                    voucherTrans.chq_no = string.Empty;
                    isPayPurchase = false;
                    isPayments = false;
                    isPayments = true;
                    isPurchaseEpay = false;
                    //for (int k = 0; k < 2; k++)
                    //{
                    //    voucherTrans.acc_type = "O";
                    //    voucherTrans.acc_code_master = 0;
                    //    if (cgstAmount > 0)
                    //    {
                    //        if (k == 0)
                    //        {
                    //            accCode = GetMagnaAccountCode(90);
                    //            voucherAmount = cgstAmount;
                    //            voucherTrans.cr_amount = 0;
                    //            voucherTrans.dr_amount = cgstAmount;
                    //        }
                    //        else
                    //        {
                    //            accCode = GetMagnaAccountCode(81);
                    //            voucherAmount = cgstAmount;
                    //            voucherTrans.cr_amount = cgstAmount;
                    //            voucherTrans.dr_amount = 0;
                    //        }
                    //        voucherTrans.acc_code = accCode;
                    //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                    //            return false;
                    //    }

                    //    if (sgstAmount > 0)
                    //    {
                    //        if (k == 0)
                    //        {
                    //            accCode = GetMagnaAccountCode(91);
                    //            voucherAmount = cgstAmount;
                    //            voucherTrans.cr_amount = 0;
                    //            voucherTrans.dr_amount = sgstAmount;
                    //        }
                    //        else
                    //        {
                    //            accCode = GetMagnaAccountCode(82);
                    //            voucherAmount = sgstAmount;
                    //            voucherTrans.cr_amount = sgstAmount;
                    //            voucherTrans.dr_amount = 0;
                    //        }
                    //        voucherTrans.acc_code = accCode;
                    //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                    //            return false;
                    //    }

                    //    if (igstAmount > 0)
                    //    {
                    //        if (k == 0)
                    //        {
                    //            accCode = GetMagnaAccountCode(92);
                    //            voucherAmount = igstAmount;
                    //            voucherTrans.cr_amount = 0;
                    //            voucherTrans.dr_amount = igstAmount;
                    //        }
                    //        else
                    //        {
                    //            accCode = GetMagnaAccountCode(83);
                    //            voucherAmount = igstAmount;
                    //            voucherTrans.cr_amount = igstAmount;
                    //            voucherTrans.dr_amount = 0;

                    //        }
                    //        voucherTrans.acc_code = accCode;
                    //        if (!CreateVoucherTransaction(out errorXml, j, "", voucherAmount))
                    //            return false;
                    //    }
                    //}
                }


                #endregion

                #region Chit Collection
                DataTable dtChit = GetChitCollection();
                if (dtChit != null && dtChit.Rows.Count > 0) {
                    isChit = true;
                    for (int i = 0; i < dtChit.Rows.Count; i++) {
                        if (i == 0) {
                            //string strDelete = string.Format("DELETE FROM KTTU_PAYMENT_DETAILS WHERE trans_type = 'CT' AND pay_date BETWEEN '{0} 00:00:00' AND '{0} 23:59:59' AND company_code = '{1}' AND branch_code = '{2}'", 
                            //    AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode, companyCode);

                            //if (!CGlobalsDB.Delete(strDelete))
                            //    return false;

                            var pmtsToDelete = db.KTTU_PAYMENT_DETAILS.Where(x => x.company_code == branchCode && x.branch_code == companyCode
                                && x.trans_type == "CT" && System.Data.Entity.DbFunctions.TruncateTime(x.pay_date) >= System.Data.Entity.DbFunctions.TruncateTime(AccUpdateTimeStamp)).ToList();
                            if (pmtsToDelete != null)
                                db.KTTU_PAYMENT_DETAILS.RemoveRange(pmtsToDelete);


                            if (!UpdatePaymentDetails())
                                return false;
                        }
                        string schemeCode = dtChit.Rows[i]["scheme_code"].ToString();
                        voucherAmount = Convert.ToDecimal(dtChit.Rows[i]["Total"].ToString());
                        accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, companyCode, branchCode);
                        accCode = GetScalarValue<int>(accCodeQuery);

                        if (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("C") && voucherAmount > 0) {
                            voucherTrans.narration = "Scheme Collection " + dtChit.Rows[i]["No.Of Bills"].ToString() + " Bill(s)";
                            voucherTrans.voucher_date = AccUpdateTimeStamp;
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                        if (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("Q") && voucherAmount > 0 || (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("D")) && voucherAmount > 0) {
                            voucherTrans.narration = "Scheme Collection. Cheque/DD/CC" + dtChit.Rows[i]["No.Of Bills"].ToString() + " Bill(s)";

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = true;
                            accCode = 43;
                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                return false;
                            }

                            isCheqFirstTime = false;
                        }
                        if (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("R")) {
                            voucherTrans.narration = "Scheme Colletion. Card" + dtChit.Rows[i]["No.Of Bills"].ToString() + " Bill(s)";

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = true;
                            accCode = Convert.ToInt32(dtChit.Rows[i]["acc_code"]);
                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                return false;
                            }

                            isCheqFirstTime = false;
                        }

                        if (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("GV")) {
                            voucherTrans.narration = "Scheme Colletion. Gift voucher" + dtChit.Rows[i]["No.Of Bills"].ToString() + " Bill(s)";

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = true;
                            accCode = Convert.ToInt32(dtChit.Rows[i]["acc_code"]);
                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                return false;
                            }

                            isCheqFirstTime = false;
                        }

                        if (accCode != 0 && dtChit.Rows[i]["Payment_Type"].ToString().Equals("EP")) {
                            voucherTrans.narration = "Scheme Colletion. E Payments" + dtChit.Rows[i]["No.Of Bills"].ToString() + " Bill(s)";

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = true;
                            accCode = Convert.ToInt32(dtChit.Rows[i]["acc_code"]);
                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                return false;
                            }

                            isCheqFirstTime = false;
                        }
                    }
                    isChit = false;
                }
                #endregion

                #region ChitClosure
                DataTable dtJPACash = GetChitCashCollection();
                if (dtJPACash != null && dtJPACash.Rows.Count > 0) {
                    isPayments = false;
                    isJPACash = true;
                    for (int i = 0; i < dtJPACash.Rows.Count; i++) {
                        string schemeCode = dtJPACash.Rows[i]["Scheme_Code"].ToString();
                        voucherAmount = Convert.ToDecimal(dtJPACash.Rows[i]["chit_amt"].ToString());
                        if (GetConfigurationValue("30012019") == 1) // bhima CPC
                            accCode = GetMagnaAccountCode(60);
                        else //Kavitha Scheme wise
                        {
                            accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, companyCode, branchCode);
                            accCode = GetScalarValue<int>(accCodeQuery);
                        }
                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                        voucherTrans.receipt_no = dtJPACash.Rows[i]["chit_trans_no"].ToString();
                        if (GetConfigurationValue("28012019") == 1)
                            voucherTrans.acc_code_master = GetAccountPostingSetup("PM", "SC"); //scheme Cash

                        string scheme = GetScalarValue<string>(string.Format("select acc_name from kstu_acc_ledger_master where scheme_code='{0}' \n"
                        + " and company_code='{1}' and branch_code='{2}' " +
                        "", dtJPACash.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));

                        if (accCode != 0 && voucherAmount > 0) {
                            if (string.Compare(clientID, "1") == 0) {
                                string memberShip = " " + dtJPACash.Rows[i]["scheme_code"].ToString() + "/" + dtJPACash.Rows[i]["group_code"].ToString() + "/ Mem No: " + dtJPACash.Rows[i]["chit_membshipNo"].ToString();
                                voucherTrans.narration = scheme + " Scheme Closed in  " + dtJPACash.Rows[i]["closed_at"].ToString() + memberShip + " BY CASH";
                            }
                            else
                                voucherTrans.narration = "Jewel Purchase Cash Closing";

                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;

                        }
                    }
                    isJPACash = false;
                }
                #endregion

                #region ChitChequeClosure
                DataTable dtJPACheque = GetChitChequeCollection();
                if (dtJPACheque != null && dtJPACheque.Rows.Count > 0) {
                    isPayments = false;
                    isJPACheque = true;
                    for (int i = 0; i < dtJPACheque.Rows.Count; i++) {
                        string schemeCode = dtJPACheque.Rows[i]["Scheme_Code"].ToString();
                        voucherAmount = Convert.ToDecimal(dtJPACheque.Rows[i]["chit_amt"].ToString());
                        if (GetConfigurationValue("30012019") == 1) // bhima CPC
                            accCode = GetMagnaAccountCode(60);
                        else //Kavitha Scheme wise
                        {
                            accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, companyCode, branchCode);
                            accCode = GetScalarValue<int>(accCodeQuery);
                        }
                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                        voucherTrans.receipt_no = dtJPACheque.Rows[i]["chit_trans_no"].ToString();

                        string scheme = GetScalarValue<string>(string.Format("select acc_name from kstu_acc_ledger_master where scheme_code='{0}' \n"
                        + " and company_code='{1}' and branch_code='{2}' " +
                        "", dtJPACheque.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));


                        if (accCode != 0 && voucherAmount > 0) {
                            if (string.Compare(clientID, "1") == 0) {
                                //voucherTrans.acc_code_master = 1112;
                                voucherTrans.acc_code_master = GetMagnaAccountCode(342);
                                string memberShip = " " + dtJPACheque.Rows[i]["scheme_code"].ToString() + "/" + dtJPACheque.Rows[i]["group_code"].ToString() + "/ Mem No: " + dtJPACheque.Rows[i]["chit_membshipNo"].ToString();
                                voucherTrans.narration = scheme + " SCHEME CLOSED IN  " + dtJPACheque.Rows[i]["closed_at"].ToString() + memberShip + " BY CHEQUE";
                            }
                            else {
                                voucherTrans.acc_code_master = GetMagnaAccountCode(342);
                                // voucherTrans.acc_code_master = Convert.ToInt32(dtJPACheque.Rows[i]["acc_code"].ToString());
                                voucherTrans.narration = "Jewel Purchase Cheque Closing";
                            }
                            voucherTrans.acc_type = "B";
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;

                        }
                    }
                    isJPACheque = false;
                }
                #endregion

                #region Cheque AND Card Deposits
                string query = string.Format("SELECT HOCODE FROM KSTU_COMPANY_MASTER WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode);
                string strHOCODE = GetScalarValue<string>(query);
                int nCompanyCount = GetScalarValue<int>(string.Format("SELECT COUNT(*) FROM KSTU_COMPANY_MASTER WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));

                if (!string.IsNullOrEmpty(strHOCODE)) {
                    dtChequeCardBalance = GetChequeAndCardBalance();
                    for (int i = 0; i < dtChequeCardBalance.Rows.Count; i++) {
                        isCheqFirstTime = false;
                        CreateBranchVoucherTransaction(dtChequeCardBalance.Rows[i]["pay_mode"].ToString(), strHOCODE, Convert.ToDecimal(dtChequeCardBalance.Rows[i]["Amount"]), i, false);
                        isCheqFirstTime = true;
                        CreateBranchVoucherTransaction(dtChequeCardBalance.Rows[i]["pay_mode"].ToString(), strHOCODE, Convert.ToDecimal(dtChequeCardBalance.Rows[i]["Amount"]), i, false);
                        isCheqFirstTime = false;
                    }
                }
                #endregion

                #region Cheque AND Card Receipts For HO
                if (string.IsNullOrEmpty(strHOCODE) && nCompanyCount > 1) {
                    dtChequeCardDeposits = GetChequeAndCardDeposits();
                    for (int i = 0; i < dtChequeCardDeposits.Rows.Count; i++) {
                        strHOCODE = dtChequeCardDeposits.Rows[i]["branch_code"].ToString();
                        isCheqFirstTime = false;
                        CreateBranchVoucherTransaction(dtChequeCardDeposits.Rows[i]["pay_mode"].ToString(), strHOCODE, Convert.ToDecimal(dtChequeCardDeposits.Rows[i]["Amount"]), i, true);
                        isCheqFirstTime = true;
                        CreateBranchVoucherTransaction(dtChequeCardDeposits.Rows[i]["pay_mode"].ToString(), strHOCODE, Convert.ToDecimal(dtChequeCardDeposits.Rows[i]["Amount"]), i, true);
                        isCheqFirstTime = false;
                    }
                }
                #endregion

                #region Other Branch Order Or Chit Adjustments

                DataTable dtOtherBranch = OtherBranchTransactions();
                if (dtOtherBranch != null && dtOtherBranch.Rows.Count > 0) {
                    isOtherTran = true;
                    for (int i = 0; i < dtOtherBranch.Rows.Count; i++) {

                        isPayments = false;
                        //isCheqFirstTime = false;
                        //if (string.Compare(dtOtherBranch.Rows[i]["pay_mode"].ToString(), "CT") == 0)
                        //{
                        //    Amount = Convert.ToDecimal(dtOtherBranch.Rows[i]["Amount"].ToString());
                        //    //Amount = Convert.ToDecimal(dtOtherBranch.Rows[i]["Amount"].ToString()) + Convert.ToDecimal(dtOtherBranch.Rows[i]["WinAmt"].ToString()) + Convert.ToDecimal(dtOtherBranch.Rows[i]["Bonus"].ToString());
                        //    string schemeCode = dtOtherBranch.Rows[i]["Scheme_Code"].ToString();
                        //    if (string.Compare(schemeCode, "AP") == 0)
                        //        schemeCode = "AV";

                        //    accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, branchCode, companyCode);
                        //    accCode = GetScalarValue<int>(accCodeQuery);
                        //    //if (accCode == 0)
                        //    //{
                        //    //    string schemeQuery=string.Format("select branch_code,scheme_code,scheme_name from chstu_scheme where branch_code='{0}' and scheme_code='{1}' ",dtOtherBranch.Rows[i]["party_code"].ToString(),schemeCode);
                        //    //}
                        //    voucherTrans.voucher_date = AccUpdateTimeStamp;
                        //    voucherTrans.receipt_no = string.Empty;
                        //    if (accCode != 0 && Amount > 0)
                        //    {
                        //        voucherTrans.narration = dtOtherBranch.Rows[i]["party_code"].ToString() + "Scheme Adjustments";
                        //        if (Amount > 0)
                        //        {

                        //            if (!CreateVoucherTransaction(out errorXml, i, "",Amount))
                        //                return false;
                        //            voucherTrans.narration = dtOtherBranch.Rows[i]["party_code"].ToString() + "Winner Adjustments";

                        //            Amount = Convert.ToDecimal(dtOtherBranch.Rows[i]["WinAmt"].ToString());
                        //            if (!CreateVoucherTransaction(out errorXml, i, "",Amount))
                        //                return false;
                        //            voucherTrans.narration = dtOtherBranch.Rows[i]["party_code"].ToString() + "Bonus Adjustments";

                        //            Amount = Convert.ToDecimal(dtOtherBranch.Rows[i]["Bonus"].ToString());
                        //            if (!CreateVoucherTransaction(out errorXml, i, "",Amount))
                        //                return false;
                        //        }

                        //    }
                        //}
                        //else
                        //{
                        //    Amount = Convert.ToDecimal(dtOtherBranch.Rows[i]["Amount"].ToString());
                        //    accCode = 7;
                        //    voucherTrans.acc_type = "O";
                        //    voucherTrans.narration = dtOtherBranch.Rows[i]["party_code"].ToString() + " Order Adjustments";
                        //    voucherTrans.voucher_date = AccUpdateTimeStamp;
                        //    voucherTrans.receipt_no = string.Empty;
                        //    if (Amount > 0)
                        //    {
                        //        if (!CreateVoucherTransaction(out errorXml, i, "",Amount))
                        //            return false;
                        //    }
                        //}

                        voucherAmount = Convert.ToDecimal(dtOtherBranch.Rows[i]["Amount"].ToString()) + Convert.ToDecimal(dtOtherBranch.Rows[i]["WinAmt"].ToString()) + +Convert.ToDecimal(dtOtherBranch.Rows[i]["Bonus"].ToString());
                        isCheqFirstTime = true;
                        accCode = GetScalarValue<int>(string.Format("SELECT acc_code FROM KSTU_ACC_LEDGER_MASTER WHERE company_code = '{0}' AND branch_code = '{1}' AND party_code = '{2}'", companyCode, branchCode, dtOtherBranch.Rows[i]["party_code"].ToString()));
                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                        string scheme = GetScalarValue<string>(string.Format("select acc_name from kstu_acc_ledger_master where scheme_code='{0}' \n"
                        + " and company_code='{1}' and branch_code='{2}' " +
                        "", dtOtherBranch.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));

                        if (string.Compare(dtOtherBranch.Rows[i]["pay_mode"].ToString(), "CT") == 0) {
                            memberShip = " " + dtOtherBranch.Rows[i]["scheme_code"].ToString() + "/" + dtOtherBranch.Rows[i]["group_code"].ToString() + "/ Mem No: " + dtOtherBranch.Rows[i]["from"].ToString() + "- " + dtOtherBranch.Rows[i]["To"].ToString() + "/" + dtOtherBranch.Rows[i]["party_code"].ToString();
                            voucherTrans.narration = " Branch " + dtOtherBranch.Rows[i]["party_code"].ToString() + " " + scheme + " Scheme Closed in  " + branchCode + memberShip;
                            voucherTrans.voucher_type = "IB";
                        }
                        else if (string.Compare(dtOtherBranch.Rows[i]["pay_mode"].ToString(), "BC") == 0) {
                            voucherTrans.narration = "OLD SUVARNADHARA SCHEME CLOSED - " + dtOtherBranch.Rows[i]["From"].ToString() + " " + dtOtherBranch.Rows[i]["narration"].ToString();
                            voucherTrans.voucher_type = "BC";
                        }

                        else {
                            voucherTrans.narration = " Branch " + dtOtherBranch.Rows[i]["party_code"].ToString() + " Order Closed in  " + branchCode;
                            voucherTrans.voucher_type = "IB";
                        }

                        voucherTrans.acc_type = "O";

                        voucherTrans.receipt_no = string.Empty;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                    }
                    isOtherTran = false;
                }
                #endregion

                #region HI Purchase
                dtHIPurchase = SIGlobals.Globals.GetDataTable(string.Format("Select company_code,branch_code,cust_name,hi_scheme_no,hi_scheme_amount,purchase_billNo,gs_code,convert(varchar,hi_scheme_date,103) from kttu_hi_master  \n" +
                                        " where hi_scheme_date BETWEEN '{0}' AND '{1}' and company_code ='{2}' and branch_code='{3}' and cflag!='Y' \n" +
                                         "", AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode));

                if (dtHIPurchase != null && dtHIPurchase.Rows.Count > 0) {
                    voucherTrans.acc_type = "O";
                    for (int i = 0; i < dtHIPurchase.Rows.Count; i++) {
                        isHI = true;
                        string tempDateTime = dtHIPurchase.Rows[i]["column1"].ToString();
                        voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                        voucherAmount = decimal.Round(Convert.ToDecimal(dtHIPurchase.Rows[i]["hi_scheme_amount"]), 2);
                        voucherTrans.receipt_no = dtHIPurchase.Rows[i]["purchase_billNo"].ToString() + ',' + dtHIPurchase.Rows[i]["hi_scheme_no"].ToString();
                        voucherTrans.narration = "(HELP India Created Through Purchase Bill no :" + dtHIPurchase.Rows[i]["purchase_billNo"].ToString() + ")  MSNo : " + dtHIPurchase.Rows[i]["hi_scheme_no"].ToString();
                        voucherTrans.party_name = dtHIPurchase.Rows[i]["cust_name"].ToString();

                        accCode = 9;
                        isHIFirst = true;
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }
                        isHIFirst = false;
                        string gscode = Convert.ToString(dtHIPurchase.Rows[i]["gs_code"]);
                        accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 2 and company_code='{1}' and branch_code='{2}'", gscode, companyCode, branchCode));
                        if (voucherAmount > 0) {
                            if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                return false;
                        }

                    }
                    isHI = false;

                }

                #endregion

                if ((GetConfigurationValue("28012019") == 0)) //Bhima BLR
                {
                    #region Saving Scheme Collections

                    if (updateSchemeCollection) {
                        string chitServer = string.Empty;
                        string dbname = string.Empty;
                        string connectionQuery = string.Empty;
                        string narrationAppend = string.Empty;

                        connectionQuery = string.Format("Select * from dbo.CHIT_CONNECTION_TABLE where company_code='{0}' and branch_code='{1}'", companyCode, branchCode);
                        DataTable dtServer = SIGlobals.Globals.GetDataTable(connectionQuery);
                        if (chitServer != null || dbname != null) {
                            chitServer = dtServer.Rows[0]["ChitServer"].ToString();
                            dbname = dtServer.Rows[0]["DatabaseName"].ToString();
                        }
                        else
                            throw new Exception("Scheme server details not found");
                        DataTable dtCollection = new DataTable();
                        //using Linked server
                        if (GetConfigurationValue("15032018") == 0) {
                            dtCollection = SIGlobals.Globals.GetDataTable(string.Format("SELECT * FROM OPENQUERY([{2}],'EXEC {3}.dbo.usp_CollectionSummaryToMagna ''{0} 00:00:00.000'',''{0} 23:59:59.998'',''{1}'',''Day Close is not done for the selected date range.''')", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode, chitServer, dbname));
                        }
                        /*else {
                            using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                                webService.ServiceHeaderValue = GetServiceHeader();
                                if (webService.ServiceHeaderValue == null) {
                                    CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                    return false;
                                }
                                AccountsCollectionSummary[] accSummary = webService.GetAccountsCollectionSummary(AccUpdateTimeStamp.Date, companyCode);
                                dtCollection = SIGlobals.Globals.GetDataTableFromObjects(accSummary);
                            }
                        }*/
                        if (dtCollection != null && dtCollection.Rows.Count > 0) {
                            isChit = true;
                            for (int i = 0; i < dtCollection.Rows.Count; i++) {
                                isPayments = false;
                                string schemeCode = string.Empty;
                                if (string.Compare(dtCollection.Rows[i]["scheme_code"].ToString().Trim(), "AP") == 0)
                                    schemeCode = "AV";
                                else
                                    schemeCode = dtCollection.Rows[i]["scheme_code"].ToString();

                                string scheme = string.Empty;

                                scheme = GetScalarValue<string>(string.Format("select acc_name from kstu_acc_ledger_master where scheme_code='{0}' \n"
                                + " and company_code='{1}' and branch_code='{2}' " +
                                "", dtCollection.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));

                                if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                    accCode = GetMagnaAccountCode(60);
                                else //Kavitha Scheme wise
                                {
                                    accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, companyCode, branchCode);
                                    accCode = GetScalarValue<int>(accCodeQuery);
                                }

                                voucherAmount = Convert.ToDecimal(dtCollection.Rows[i]["AMOUNT"].ToString());

                                if (string.Compare(dtCollection.Rows[i]["COLLECTION_BRANCH_CODE"].ToString().Trim(), branchCode) != 0 && string.Compare(dtCollection.Rows[i]["CREDIT_BRANCH_CODE"].ToString().Trim(), branchCode) == 0) {
                                    //using Linked server
                                    if (GetConfigurationValue("15032018") == 0) {
                                        string narration = string.Format("select case when min(chit_membshipNo ) = max(chit_membshipNo) then group_code + ' / '+ convert(varchar,min(chit_membshipNo)) else group_code + ' / '+ convert(varchar,min(chit_membshipNo)) + ' - ' + convert(varchar,max(chit_membshipno)) END  as narration \n" +
                                               " from {5}.{6}.dbo.CHTU_PAYMENT_DETAILS where branch_code='{0}' and Payment_Date between '{1}' and '{2}'  \n" +
                                               " and Receipt_branch='{3}' and payment_type='{4}' and scheme_code ='{7}' group by scheme_code,Receipt_branch,branch_code,group_code,payment_type,Payment_Date ",
                                                branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00:000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), dtCollection.Rows[i]["collection_branch_code"].ToString(), dtCollection.Rows[i]["mode"].ToString(), chitServer, dbname, dtCollection.Rows[i]["scheme_code"].ToString());
                                        dtNarration = SIGlobals.Globals.GetDataTable(narration);
                                    }
                                    /*else {
                                        using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                                            webService.ServiceHeaderValue = GetServiceHeader();
                                            if (webService.ServiceHeaderValue == null) {
                                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                                return false;
                                            }
                                            NarrationList[] narration = webService.GetNarationDetailsForSameBranch(AccUpdateTimeStamp.Date, companyCode, dtCollection.Rows[i]["mode"].ToString(), dtCollection.Rows[i]["scheme_code"].ToString(), dtCollection.Rows[i]["collection_branch_code"].ToString());
                                            dtNarration = SIGlobals.Globals.GetDataTableFromObjects(narration);
                                        }
                                    }*/
                                    if (dtNarration != null && dtNarration.Rows.Count > 0) {
                                        for (int l = 0; l < dtNarration.Rows.Count; l++) {
                                            if (l == 0)
                                                narrationAppend = dtNarration.Rows[l]["narration"].ToString();
                                            else
                                                narrationAppend += ',' + dtNarration.Rows[l]["narration"].ToString();
                                        }
                                    }

                                    voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS  IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;

                                    isCheqFirstTime = false;
                                    if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                        return false;

                                    isCheqFirstTime = true;
                                    accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["collection_branch_code"].ToString(), companyCode, branchCode));
                                    if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                        return false;

                                    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                        return false;
                                    }

                                    isCheqFirstTime = false;

                                }

                                else if (string.Compare(branchCode, dtCollection.Rows[i]["credit_branch_code"].ToString()) == 0) {
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("C") && voucherAmount > 0) {
                                        isPayments = false;
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS.CASH - Receipt(s)";
                                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && voucherAmount > 0 || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("D")) && voucherAmount > 0) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. CHEQUE - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = 43;
                                        accCode = GetMagnaAccountCode(43);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;

                                        #region Cheque Transfer to CPC
                                        //if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && Amount > 0)
                                        //{
                                        //    voucherTrans.narration = scheme + " SCHEME CHEQUE TRANSFER TO CPC";
                                        //    isCheqFirstTime = false;
                                        //    accCode = 43;
                                        //    if (!UpdateVoucherTransaction(out errorXml, Amount))
                                        //        return false;

                                        //    isCheqFirstTime = true;

                                        //    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where party_code ='{0}' and company_code='{1}' and branch_code='{2}'", "CPC", branchCode, companyCode));

                                        //    if (!UpdateVoucherTransaction(out errorXml, Amount))
                                        //        return false;

                                        //    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16"))
                                        //    {
                                        //        return false;
                                        //    }

                                        //    isCheqFirstTime = false;
                                        //}
                                        #endregion
                                    }
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("R")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. CARD - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        //accCode = 40;
                                        accCode = GetMagnaAccountCode(40);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("GV")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. GIFT VOUCHER - Receipt(s)";

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = Convert.ToInt32(dtCollection.Rows[i]["acc_code"]);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("E") || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("B")) || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("EP"))) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. E PAYMENTS - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = 5;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("M")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. NACH - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = GetMagnaAccountCode(71);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                }

                                else {
                                    //using Linked server
                                    if (GetConfigurationValue("15032018") == 0) {
                                        string narration = string.Format("select case when min(chit_membshipNo ) = max(chit_membshipNo) then group_code + ' / '+ convert(varchar,min(chit_membshipNo)) else group_code + ' / '+ convert(varchar,min(chit_membshipNo)) + ' - ' + convert(varchar,max(chit_membshipno)) END  as narration \n" +
                                                " from {5}.{6}.dbo.CHTU_PAYMENT_DETAILS where branch_code='{0}' and Payment_Date between '{1}' and '{2}'  \n" +
                                                " and Receipt_branch='{3}' and payment_type='{4}' and scheme_code ='{7}' group by scheme_code,Receipt_branch,branch_code,group_code,payment_type,Payment_Date ",
                                                dtCollection.Rows[i]["credit_branch_code"].ToString(), AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00:000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), branchCode, dtCollection.Rows[i]["mode"].ToString(), chitServer, dbname, dtCollection.Rows[i]["scheme_code"].ToString());
                                        dtNarration = SIGlobals.Globals.GetDataTable(narration);
                                    }
                                    /*else {
                                        using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                                            webService.ServiceHeaderValue = GetServiceHeader();
                                            if (webService.ServiceHeaderValue == null) {
                                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                                return false;
                                            }
                                            NarrationList[] narration = webService.GetNarationDetailsForSameBranch(AccUpdateTimeStamp.Date, companyCode, dtCollection.Rows[i]["mode"].ToString(), dtCollection.Rows[i]["scheme_code"].ToString(), dtCollection.Rows[i]["collection_branch_code"].ToString());
                                            dtNarration = SIGlobals.Globals.GetDataTableFromObjects(narration);
                                        }
                                    }*/
                                    if (dtNarration != null && dtNarration.Rows.Count > 0) {
                                        for (int k = 0; k < dtNarration.Rows.Count; k++) {
                                            if (k == 0)
                                                narrationAppend = dtNarration.Rows[k]["narration"].ToString();
                                            else
                                                narrationAppend += ',' + dtNarration.Rows[k]["narration"].ToString();
                                        }

                                    }
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("C") && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CASH IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));

                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("R")) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CARD IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        accCode = 40;

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }
                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("E") || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("B")) || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("EP"))) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - Bank Transfer IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        accCode = 5;

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && voucherAmount > 0 || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("D")) && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CHEQUE IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = 43;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("M") && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - NATCH IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = 71;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }
                                    else {
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode));
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;

                                    }

                                }
                            }
                            isChit = false;
                        }

                    }
                    #endregion
                }

                else {

                    #region Saving Scheme Collections - ReceiptWise

                    if (updateSchemeCollection) {
                        string chitServer = string.Empty;
                        string dbname = string.Empty;
                        string connectionQuery = string.Empty;
                        string narrationAppend = string.Empty;

                        connectionQuery = string.Format("Select * from dbo.CHIT_CONNECTION_TABLE where company_code='{0}' and branch_code='{1}'", companyCode, branchCode);
                        DataTable dtServer = SIGlobals.Globals.GetDataTable(connectionQuery);
                        if (chitServer != null || dbname != null) {
                            chitServer = dtServer.Rows[0]["ChitServer"].ToString();
                            dbname = dtServer.Rows[0]["DatabaseName"].ToString();
                        }
                        else
                            throw new Exception("Scheme server details not found");
                        DataTable dtCollection = new DataTable();
                        //using Linked server
                        if (GetConfigurationValue("15032018") == 0) {
                            dtCollection = SIGlobals.Globals.GetDataTable(string.Format("SELECT * FROM OPENQUERY([{2}],'EXEC {3}.dbo.usp_CollectionSummaryToMagna ''{0} 00:00:00.000'',''{0} 23:59:59.998'',''{1}'',''Day Close is not done for the selected date range.''')", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode, chitServer, dbname));
                        }
                        /*else {
                            using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                                webService.ServiceHeaderValue = GetServiceHeader();
                                if (webService.ServiceHeaderValue == null) {
                                    CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                    return false;
                                }
                                AccountsCollectionSummary[] accSummary = webService.GetAccountsCollectionSummary(AccUpdateTimeStamp.Date, companyCode);
                                dtCollection = SIGlobals.Globals.GetDataTableFromObjects(accSummary);
                            }
                        }*/
                        if (dtCollection != null && dtCollection.Rows.Count > 0) {
                            isChit = true;
                            for (int i = 0; i < dtCollection.Rows.Count; i++) {
                                isPayments = false;
                                string schemeCode = string.Empty;
                                if (string.Compare(dtCollection.Rows[i]["scheme_code"].ToString().Trim(), "AP") == 0)
                                    schemeCode = "AV";
                                else
                                    schemeCode = dtCollection.Rows[i]["scheme_code"].ToString();

                                string scheme = string.Empty;

                                scheme = GetScalarValue<string>(string.Format("select acc_name from kstu_acc_ledger_master where scheme_code='{0}' \n"
                                + " and company_code='{1}' and branch_code='{2}' " +
                                "", dtCollection.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));

                                if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                    accCode = GetMagnaAccountCode(60);
                                else //Kavitha Scheme wise
                                {
                                    accCodeQuery = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", schemeCode, companyCode, branchCode);
                                    accCode = GetScalarValue<int>(accCodeQuery);
                                }

                                voucherAmount = Convert.ToDecimal(dtCollection.Rows[i]["AMOUNT"].ToString());

                                if (string.Compare(dtCollection.Rows[i]["COLLECTION_BRANCH_CODE"].ToString().Trim(), branchCode) != 0 && string.Compare(dtCollection.Rows[i]["CREDIT_BRANCH_CODE"].ToString().Trim(), branchCode) == 0) {
                                    //using Linked server
                                    if (GetConfigurationValue("15032018") == 0) {
                                        string narration = string.Format("select case when min(chit_membshipNo ) = max(chit_membshipNo) then group_code + ' / '+ convert(varchar,min(chit_membshipNo)) else group_code + ' / '+ convert(varchar,min(chit_membshipNo)) + ' - ' + convert(varchar,max(chit_membshipno)) END  as narration \n" +
                                               " from {5}.{6}.dbo.CHTU_PAYMENT_DETAILS where branch_code='{0}' and Payment_Date between '{1}' and '{2}'  \n" +
                                               " and Receipt_branch='{3}' and payment_type='{4}' and scheme_code ='{7}' group by scheme_code,Receipt_branch,branch_code,group_code,payment_type,Payment_Date ",
                                                branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00:000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), dtCollection.Rows[i]["collection_branch_code"].ToString(), dtCollection.Rows[i]["mode"].ToString(), chitServer, dbname, dtCollection.Rows[i]["scheme_code"].ToString());
                                        dtNarration = SIGlobals.Globals.GetDataTable(narration);
                                    }
                                    /*else {
                                        using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                                            webService.ServiceHeaderValue = GetServiceHeader();
                                            if (webService.ServiceHeaderValue == null) {
                                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                                return false;
                                            }
                                            NarrationList[] narration = webService.GetNarationDetailsForSameBranch(AccUpdateTimeStamp.Date, companyCode, dtCollection.Rows[i]["mode"].ToString(), dtCollection.Rows[i]["scheme_code"].ToString(), dtCollection.Rows[i]["collection_branch_code"].ToString());
                                            dtNarration = SIGlobals.Globals.GetDataTableFromObjects(narration);
                                        }
                                    }*/
                                    if (dtNarration != null && dtNarration.Rows.Count > 0) {
                                        for (int l = 0; l < dtNarration.Rows.Count; l++) {
                                            if (l == 0)
                                                narrationAppend = dtNarration.Rows[l]["narration"].ToString();
                                            else
                                                narrationAppend += ',' + dtNarration.Rows[l]["narration"].ToString();
                                        }
                                    }

                                    voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS  IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;

                                    isCheqFirstTime = false;
                                    if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                        return false;

                                    isCheqFirstTime = true;
                                    //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["collection_branch_code"].ToString(), branchCode, companyCode));
                                    accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["collection_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                    if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                        return false;

                                    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                        return false;
                                    }

                                    isCheqFirstTime = false;

                                }

                                else if (string.Compare(branchCode, dtCollection.Rows[i]["credit_branch_code"].ToString()) == 0) {
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("C") && voucherAmount > 0) {
                                        isPayments = false;
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS.CASH - Receipt(s)";
                                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                                        voucherTrans.acc_code_master = GetAccountPostingSetup("PM", "SC"); //scheme Cash
                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && voucherAmount > 0 || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("D")) && voucherAmount > 0) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. CHEQUE - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = 43;
                                        //accCode = GetMagnaAccountCode(43);
                                        accCode = GetAccountPostingSetup("PM", "SQ"); //scheme cheque
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;

                                        #region Cheque Transfer to CPC
                                        //if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && Amount > 0)
                                        //{
                                        //    voucherTrans.narration = scheme + " SCHEME CHEQUE TRANSFER TO CPC";
                                        //    isCheqFirstTime = false;
                                        //    accCode = 43;
                                        //    if (!UpdateVoucherTransaction(out errorXml, Amount))
                                        //        return false;

                                        //    isCheqFirstTime = true;

                                        //    accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where party_code ='{0}' and company_code='{1}' and branch_code='{2}'", "CPC", branchCode, companyCode));

                                        //    if (!UpdateVoucherTransaction(out errorXml, Amount))
                                        //        return false;

                                        //    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16"))
                                        //    {
                                        //        return false;
                                        //    }

                                        //    isCheqFirstTime = false;
                                        //}
                                        #endregion
                                    }
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("R")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. CARD - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        //accCode = 40;
                                        //accCode = GetMagnaAccountCode(40);
                                        accCode = GetAccountPostingSetup("PM", "SCR"); //scheme card

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("GV")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. GIFT VOUCHER - Receipt(s)";

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = Convert.ToInt32(dtCollection.Rows[i]["acc_code"]);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("E") || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("B")) || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("EP"))) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. E PAYMENTS - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = 5;
                                        accCode = GetAccountPostingSetup("PM", "SEP"); //scheme e-Payment
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("O")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. ONLINE BANK TRANSFER - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        //accCode = GetMagnaAccountCode(47);
                                        accCode = GetAccountPostingSetup("PM", "SOT"); //scheme online Transfer
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("M")) {
                                        voucherTrans.narration = scheme + " SCHEME COLLECTIONS. NACH - Receipt(s)";
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = GetMagnaAccountCode(71);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }
                                }

                                else {
                                    //using Linked server
                                    if (GetConfigurationValue("15032018") == 0) {
                                        string narration = string.Format("select case when min(chit_membshipNo ) = max(chit_membshipNo) then group_code + ' / '+ convert(varchar,min(chit_membshipNo)) else group_code + ' / '+ convert(varchar,min(chit_membshipNo)) + ' - ' + convert(varchar,max(chit_membshipno)) END  as narration \n" +
                                                " from {5}.{6}.dbo.CHTU_PAYMENT_DETAILS where branch_code='{0}' and Payment_Date between '{1}' and '{2}'  \n" +
                                                " and Receipt_branch='{3}' and payment_type='{4}' and scheme_code ='{7}' group by scheme_code,Receipt_branch,branch_code,group_code,payment_type,Payment_Date ",
                                                dtCollection.Rows[i]["credit_branch_code"].ToString(), AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00:000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), branchCode, dtCollection.Rows[i]["mode"].ToString(), chitServer, dbname, dtCollection.Rows[i]["scheme_code"].ToString());
                                        dtNarration = SIGlobals.Globals.GetDataTable(narration);
                                    }
                                    else {
                                        /*using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL()))
                                        {
                                            webService.ServiceHeaderValue = GetServiceHeader();
                                            if (webService.ServiceHeaderValue == null)
                                            {
                                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                                return false;
                                            }
                                            NarrationList[] narration = webService.GetNarationDetailsForSameBranch(AccUpdateTimeStamp.Date, companyCode, dtCollection.Rows[i]["mode"].ToString(), dtCollection.Rows[i]["scheme_code"].ToString(), dtCollection.Rows[i]["collection_branch_code"].ToString());
                                            dtNarration = SIGlobals.Globals.GetDataTableFromObjects(narration);
                                        }*/
                                    }
                                    if (dtNarration != null && dtNarration.Rows.Count > 0) {
                                        for (int k = 0; k < dtNarration.Rows.Count; k++) {
                                            if (k == 0)
                                                narrationAppend = dtNarration.Rows[k]["narration"].ToString();
                                            else
                                                narrationAppend += ',' + dtNarration.Rows[k]["narration"].ToString();
                                        }

                                    }
                                    if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("C") && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CASH IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        voucherTrans.voucher_date = AccUpdateTimeStamp;
                                        // accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        voucherTrans.acc_code_master = GetAccountPostingSetup("PM", "SC");
                                        if (!CreateVoucherTransaction(out errorXml, i, "", voucherAmount))
                                            return false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("R")) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CARD IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        // accCode = 40;
                                        accCode = GetAccountPostingSetup("PM", "SCR"); //scheme card
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }
                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("E") || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("B")) || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("EP"))) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - Bank Transfer IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;

                                        //accCode = 5;
                                        accCode = GetAccountPostingSetup("PM", "SEP"); //scheme cheque

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("Q") && voucherAmount > 0 || (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("D")) && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - CHEQUE IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = 43;
                                        accCode = GetAccountPostingSetup("PM", "SQ"); //scheme cheque
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("M") && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS - NATCH IN " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        accCode = 71;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }

                                    else if (accCode != 0 && dtCollection.Rows[i]["MODE"].ToString().Equals("O") && voucherAmount > 0) {
                                        voucherTrans.narration = "BRANCH " + dtCollection.Rows[i]["credit_branch_code"].ToString() + " " + scheme + " SCHEME COLLECTIONS. ONLINE BANK TRANSFER " + dtCollection.Rows[i]["collection_branch_code"].ToString() + " " + narrationAppend;
                                        isCheqFirstTime = false;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = 71;
                                        accCode = GetMagnaAccountCode(47);
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;
                                    }
                                    else {
                                        isCheqFirstTime = false;
                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        isCheqFirstTime = true;
                                        //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtCollection.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                                        accCode = GetSchemeOtherBranch(dtCollection.Rows[i]["credit_branch_code"].ToString(), companyCode, branchCode, schemeCode);

                                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                            return false;

                                        if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                            return false;
                                        }

                                        isCheqFirstTime = false;

                                    }

                                }
                            }
                            isChit = false;
                        }

                    }
                    #endregion

                }

                #region Interbranch Closure

                if (updateSchemeCollection) {
                    DataTable dtInterBranchSchemeClosure = new DataTable();
                    //using Linked server
                    if (GetConfigurationValue("15032018") == 0) {
                        dtInterBranchSchemeClosure = SIGlobals.Globals.GetDataTable(string.Format("exec [usp_InterBranchSchemeClosure] '{0} 00:00:00.000','{1} 23:59:59.998','{2}'",
                       AccUpdateTimeStamp.ToString("MM/dd/yyyy"), AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode));
                    }
                    /*else {
                        using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                            webService.ServiceHeaderValue = GetServiceHeader();
                            if (webService.ServiceHeaderValue == null) {
                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                return false;
                            }
                            AccountsClosureSummary[] clsSummary = webService.GetInterBranchClosureSummary(AccUpdateTimeStamp.Date, companyCode);
                            dtInterBranchSchemeClosure = SIGlobals.Globals.GetDataTableFromObjects(clsSummary);
                        }
                    }*/
                    if (dtInterBranchSchemeClosure != null && dtInterBranchSchemeClosure.Rows.Count > 0) {

                        for (int i = 0; i < dtInterBranchSchemeClosure.Rows.Count; i++) {

                            isCheqFirstTime = false;
                            voucherTrans.narration = string.Format("{0} SCHEME CLOSED AT {1} / {2} {3} - {4}, INV NO: {5}", dtInterBranchSchemeClosure.Rows[i]["scheme_code"].ToString(), dtInterBranchSchemeClosure.Rows[i]["closed_branch_code"].ToString()
                                   , dtInterBranchSchemeClosure.Rows[i]["group"].ToString(), dtInterBranchSchemeClosure.Rows[i]["StartMember"].ToString(), dtInterBranchSchemeClosure.Rows[i]["EndMember"].ToString(), dtInterBranchSchemeClosure.Rows[i]["bill_no"].ToString());

                            if (Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["SchemeAmount"]) > 0) {
                                accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='{0}' and company_code='{1}' and branch_code='{2}'", dtInterBranchSchemeClosure.Rows[i]["closed_branch_code"].ToString(), companyCode, branchCode));
                                voucherAmount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["SchemeAmount"]), 2);
                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;
                            }

                            isCheqFirstTime = true;

                            if (Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["Amount"]) > 0) {
                                string tempDateTime = dtInterBranchSchemeClosure.Rows[i]["billing_date"].ToString();
                                //voucherTrans.voucher_date = DateTime.ParseExact(tempDateTime, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                                voucherAmount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["Amount"]), 2);

                                if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                    accCode = GetMagnaAccountCode(60);
                                else {
                                    if (string.Compare(dtInterBranchSchemeClosure.Rows[i]["scheme_code"].ToString(), "AP") == 0) {
                                        string scheme_code = "AV";
                                        accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code ='{0}' and company_code='{1}' and branch_code='{2}'", scheme_code, companyCode, branchCode));
                                    }
                                    else {
                                        accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code ='{0}' and company_code='{1}' and branch_code='{2}'", Convert.ToString(dtInterBranchSchemeClosure.Rows[i]["scheme_code"]), companyCode, branchCode));
                                    }
                                }

                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;

                            }

                            if (Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["BONUS"]) > 0) {
                                accCode = 12;
                                voucherAmount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["BONUS"]), 2);
                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;
                            }
                            if (Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["WINNER"]) > 0) {
                                accCode = 13;
                                voucherAmount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["WINNER"]), 2);
                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;
                            }

                            if (Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["Deduction"]) > 0) {
                                if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                    accCode = GetMagnaAccountCode(60);
                                else
                                    accCode = GetScalarValue<int>(string.Format(@"select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}'
                            and company_code='{1}' and branch_code='{2}'", dtInterBranchSchemeClosure.Rows[i]["scheme_code"].ToString()
                                   , companyCode, branchCode));
                                voucherAmount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["Deduction"]), 2);
                                voucherTrans.dr_amount = decimal.Round(Convert.ToDecimal(dtInterBranchSchemeClosure.Rows[i]["Deduction"]), 2);
                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;
                                isCheqFirstTime = false;
                                accCode = GetMagnaAccountCode(44);

                                if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                    return false;
                            }
                            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "16")) {
                                return false;
                            }

                            isCheqFirstTime = false;

                        }
                    }
                }


                #endregion

                #region Deduction Amount
                if (updateSchemeCollection) {
                    string queryDeduction = string.Format("exec [usp_SchemeDeductionAmount] '{0} 00:00:00.000','{1} 23:59:59.998','{2}','{3}'",
                        AccUpdateTimeStamp.ToString("MM/dd/yyyy"), AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode, companyCode);
                    dtDeduction = SIGlobals.Globals.GetDataTable(queryDeduction);
                    for (int k = 0; k < dtDeduction.Rows.Count; k++) {
                        isOtherTran = true;

                        voucherAmount = Convert.ToDecimal(dtDeduction.Rows[k]["DEDUCTION Amount"]);
                        if (GetConfigurationValue("30012019") == 1) // bhima CPC
                            accCode = GetMagnaAccountCode(60);
                        else
                            accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", dtDeduction.Rows[k]["scheme_code"].ToString(), companyCode, branchCode));
                        //voucherTrans.acc_type = "J";
                        voucherTrans.narration = string.Format("{0} Deduction Amount from branch {1} ", dtDeduction.Rows[k]["scheme_code"].ToString(), dtDeduction.Rows[k]["closed_branch"].ToString());
                        isCheqFirstTime = true;

                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                            return false;

                        isCheqFirstTime = false;
                        accCode = GetMagnaAccountCode(44);

                        if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                            return false;

                        isOtherTran = false;
                    }
                }

                #endregion

                #region Cheque Return
                if (updateSchemeCollection) {
                    DataTable dtSchemeclosure = new DataTable();
                    //using Linked server
                    if (GetConfigurationValue("15032018") == 0) {
                        dtSchemeclosure = SIGlobals.Globals.GetDataTable(string.Format("exec usp_ChequeReturnSummary '{0} 00:00:00.000','{1} 23:59:59.998','{2}'",
                       AccUpdateTimeStamp.ToString("MM/dd/yyyy"), AccUpdateTimeStamp.ToString("MM/dd/yyyy"), branchCode));
                    }
                    /*else {
                        using (POSIntegration webService = new POSIntegration(CGlobals.GetWSURL())) {
                            webService.ServiceHeaderValue = GetServiceHeader();
                            if (webService.ServiceHeaderValue == null) {
                                CGlobals.ShowMessage("Failed to get web service authentication details. Please contact the administrator.", CGlobals.MessageType.Information);
                                return false;
                            }
                            ChequeReturnSummary[] chqReturnSummary = webService.GetChequeReturnSummary(AccUpdateTimeStamp.Date, companyCode);
                            dtSchemeclosure = SIGlobals.Globals.GetDataTableFromObjects(chqReturnSummary);
                        }
                    }*/
                    if (dtSchemeclosure != null && dtSchemeclosure.Rows.Count > 0) {
                        for (int i = 0; i < dtSchemeclosure.Rows.Count; i++) {
                            isOtherTran = true;
                            //voucherTrans.acc_type = "J";
                            voucherAmount = Convert.ToDecimal(dtSchemeclosure.Rows[i]["AMOUNT"].ToString());
                            //isCheqFirstTime = false;

                            //accCode = GetScalarValue<int>(string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where party_code='CPC' and company_code='{1}' and branch_code='{2}'", dtSchemeclosure.Rows[i]["credit_branch_code"].ToString(), branchCode, companyCode));
                            voucherTrans.narration = string.Format("{0} CUSTOMER CHEQUE RETURNED AT {1} / {2} {3} - {4} - chq no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                            , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());

                            //if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                            //    return false;

                            //isCheqFirstTime = true;

                            //accCode = 43;

                            //voucherTrans.narration = string.Format("{0} CUSTOMER CHEQUE RETURNED AT {1} / {2} {3} - {4} - chq no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                            //  , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());

                            //if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                            //    return false;

                            isCheqFirstTime = false;

                            //accCode = 43;

                            if (dtSchemeclosure.Rows[i]["MODE"].ToString().Equals("Q")) {
                                accCode = 43;
                                voucherTrans.narration = string.Format("{0} CUSTOMER CHEQUE RETURNED AT {1} / {2} {3} - {4} - chq no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                             , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());

                            }
                            else {
                                accCode = GetMagnaAccountCode(71);
                                voucherTrans.narration = string.Format("{0} CUSTOMER NACH RETURNED AT {1} / {2} {3} - {4} - NACH no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                                , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());
                            }

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = true;

                            if (GetConfigurationValue("30012019") == 1) // bhima CPC
                                accCode = GetMagnaAccountCode(60);
                            else
                                accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where scheme_code='{0}' and company_code='{1}' and branch_code='{2}'", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), companyCode, branchCode));

                            if (dtSchemeclosure.Rows[i]["MODE"].ToString().Equals("Q")) {
                                voucherTrans.narration = string.Format("{0} CUSTOMER CHEQUE RETURNED AT {1} / {2} {3} - {4} - chq no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                                  , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());
                            }
                            else {
                                voucherTrans.narration = string.Format("{0} CUSTOMER NACH RETURNED AT {1} / {2} {3} - {4} - NACH no - {5}", dtSchemeclosure.Rows[i]["scheme_code"].ToString(), dtSchemeclosure.Rows[i]["credit_branch_code"].ToString()
                                , dtSchemeclosure.Rows[i]["group"].ToString(), dtSchemeclosure.Rows[i]["StartMember"].ToString(), dtSchemeclosure.Rows[i]["EndMember"].ToString(), dtSchemeclosure.Rows[i]["chq_no"].ToString());

                            }

                            if (!UpdateVoucherTransaction(out errorXml, voucherAmount))
                                return false;

                            isCheqFirstTime = false;
                        }
                    }

                }

                #endregion

                #region Inter Branch Order
                if (updateOrders) {
                    DataTable dtBranch = SIGlobals.Globals.GetDataTable(string.Format("Select * from CHIT_CONNECTION_TABLE where branch_code!='{0}'", branchCode));
                    if (dtBranch != null && dtBranch.Rows.Count > 0) {
                        for (int i = 0; i < dtBranch.Rows.Count; i++) {
                            string interBranch = string.Format("UPDATE d SET d.bill_no = c.bill_no FROM OPENQUERY({1}, 'SELECT * FROM {2}.dbo.kttu_branch_order_master \n" +
                            "where  branch_code=''{0}''') c JOIN kttu_order_master d \n" +
                            "ON c.obj_id = d.obj_id WHERE D.branch_code ='{0}' \n" +
                            "and d.order_no=c.order_no and d.bill_no=0 and d.closed_branch='{3}'\n"
                            , branchCode, dtBranch.Rows[i]["CHITServer"].ToString(), dtBranch.Rows[i]["DatabaseName"].ToString(), dtBranch.Rows[i]["branch_code"].ToString());

                            //if (CGlobalsDB.ExecuteQuery(interBranch)) {
                            //    // CGlobals.ShowMessage(string.Format("Order Bill Number Updated Successfully"), CGlobals.MessageType.Information);
                            //}
                            //else
                            //    throw new Exception("Failed to update schemes, please check connection.");
                            SIGlobals.Globals.ExecuteSQL(interBranch, db, null);

                        }

                    }
                }
                #endregion
                voucherAmount = 0;
                db.SaveChanges();
            }
            catch (Exception ex) {
                errorXml = new ErrorVM().GetErrorDetails(ex).description;
                return false;

            }
            return true;
        }

        private void CreateBranchVoucherTransaction(string strType, string HOCODE, decimal Amount, int index, bool isdeposit)
        {
            if (!isCheqFirstTime) {
                voucherTrans.acc_code = Convert.ToInt32(GetPartyAccount(HOCODE));
                if (!isdeposit) {
                    if (strType.Equals("Q")) {
                        voucherTrans.chq_no = dtChequeCardBalance.Rows[index]["cheque_no"].ToString();
                        voucherTrans.chq_date = Convert.ToDateTime(dtChequeCardBalance.Rows[index]["pay_date"]);
                        voucherTrans.narration = "Cheque Collections Transferred To HO";
                    }
                    else {
                        voucherTrans.chq_no = string.Empty;
                        voucherTrans.chq_date = AccUpdateTimeStamp;
                        voucherTrans.narration = "Card Collections Transferred To HO";
                    }
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.voucher_seq_no = 1;
                }
                else {
                    if (strType.Equals("Q")) {
                        voucherTrans.chq_no = dtChequeCardDeposits.Rows[index]["cheque_no"].ToString();
                        voucherTrans.chq_date = Convert.ToDateTime(dtChequeCardDeposits.Rows[index]["pay_date"]);
                        voucherTrans.narration = "Cheque Collections Received From Branch " + "{" + HOCODE + "}";
                    }
                    else {
                        voucherTrans.chq_no = string.Empty;
                        voucherTrans.chq_date = AccUpdateTimeStamp;
                        voucherTrans.narration = "Card Collections Received From Branch " + "{" + HOCODE + "}";
                    }
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.dr_amount = 0;
                    voucherTrans.voucher_seq_no = 1;
                }
            }
            else {
                if (!isdeposit) {
                    if (strType.Equals("Q")) {
                        voucherTrans.acc_code = 2;
                        voucherTrans.narration = "Cheque Collections Transferred To HO";
                    }
                    else {
                        voucherTrans.acc_code = Convert.ToInt32(dtChequeCardBalance.Rows[index]["cheque_no"]);
                        voucherTrans.narration = "Card Collections Transferred To HO";
                    }
                    voucherTrans.chq_date = AccUpdateTimeStamp;
                    voucherTrans.voucher_seq_no = 2;
                    voucherTrans.chq_no = string.Empty;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.dr_amount = 0;
                }
                else {
                    if (strType.Equals("Q")) {
                        voucherTrans.acc_code = 2;
                        voucherTrans.narration = "Cheques Collections Received From Branch " + "{" + HOCODE + "}";
                    }
                    else {
                        voucherTrans.acc_code = Convert.ToInt32(dtChequeCardDeposits.Rows[index]["cheque_no"]);
                        voucherTrans.narration = "Cards Collections Received From Branch " + "{" + HOCODE + "}";
                    }
                    voucherTrans.chq_date = AccUpdateTimeStamp;
                    voucherTrans.voucher_seq_no = 2;
                    voucherTrans.chq_no = string.Empty;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Amount;
                }
            }
            voucherTrans.receipt_no = string.Empty;
            voucherTrans.subledger_acc_code = 0;
            voucherTrans.trans_type = "BILL";
            voucherTrans.acc_code_master = 0;
            voucherTrans.acc_type = "O";
            voucherTrans.branch_code = branchCode;
            voucherTrans.company_code = companyCode;
            voucherTrans.approved_by = string.Empty;
            voucherTrans.contra_seq_no = 0;
            voucherTrans.voucher_date = AccUpdateTimeStamp;
            voucherTrans.reconsile_date = AccUpdateTimeStamp;
            voucherTrans.approved_date = AccUpdateTimeStamp;
            voucherTrans.cflag = "N";
            voucherTrans.reconsile_flag = "N";
            var updatedDateTime = SIGlobals.Globals.GetDateTime();
            voucherTrans.inserted_on = updatedDateTime;
            voucherTrans.UpdateOn = updatedDateTime;            

            if (voucherTrans.voucher_seq_no == 1) {
                voucherTrans.voucher_no = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "10");
                if (voucherTrans.voucher_no < 1) {
                    return;
                }
            }

            string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS"
                     + SIGlobals.Globals.Separator + voucherTrans.voucher_no + SIGlobals.Globals.Separator + voucherTrans.acc_type + SIGlobals.Globals.Separator + voucherTrans.acc_code_master + SIGlobals.Globals.Separator + voucherTrans.trans_type + SIGlobals.Globals.Separator + companyCode + SIGlobals.Globals.Separator + branchCode;

            this.ObjectId = voucherTrans.obj_id = SIGlobals.Globals.GetHashcode(temp);
            this.ObjectType = "Voucher Entry";

            db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(voucherTrans);

            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "03")) {
                return;
            }

            if (voucherTrans.voucher_seq_no == 2) {
                if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "10")) {
                    return;
                }
            }
        }

        private bool CreateVoucherTransaction(out string errorXml, int index, string mode, decimal Amount)
        {
            errorXml = string.Empty;

            bool isNewVoucher = false;
            voucherTrans.receipt_no = string.Empty;
            voucherTrans.trans_type = "BILL";
            voucherTrans.voucher_type = string.Empty;

            if (string.Compare(voucherTrans.acc_type.ToString(), "B") != 0) {
                voucherTrans.acc_code_master = 0;
            }

            if (isSales) {
                voucherTrans.acc_code = accCode;
                if (!isVat) {
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "Sales-Credit";
                }
                else {
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "GST amount collected";
                }
            }
            if (isPurchase) {
                voucherTrans.acc_code = accCode;
                voucherTrans.dr_amount = Amount;
                voucherTrans.cr_amount = 0;
                voucherTrans.trans_type = "Pur-C";
                voucherTrans.narration = "Purchase Bill Adj. To Sales";
            }
            if (isSalesReturn) {
                if (!isVat) {
                    //accCode = GetScalarValue<int>(string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where gs_code ='{0}' AND gs_seq_no = 4 and company_code='{1}' and branch_code='{2}'", code, branchCode, companyCode));
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "SR Adj. To Sales";
                }
                else {
                    //int taxid = CGlobals.GetTaxIdByGsType(code);
                    //int taxid = GetScalarValue<int>(string.Format("SELECT tax_Id FROM KSTS_SALETAX_MASTER WHERE tax = {0} AND company_code = '{1}' AND bramch_code = '{2}'", taxPer, branchCode, companyCode));
                    //accCode = GetScalarValue<int>(string.Format("SELECT acc_code FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}' ", taxid, companyCode, branchCode));
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = (Amount);
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "SR GST Adjustment";
                }
            }

            if (isCredit) {

                voucherTrans.acc_code = accCode;
                voucherTrans.acc_type = "O";
                if (Amount > 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Credit Sales Bill";
                }
                else if (Amount < 0) {
                    voucherTrans.dr_amount = -Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Cash Paid To Party";
                }
            }

            if (isCashPaid) {
                //voucherTrans.acc_code = GetScalarValue<int>(string.Format("SELECT acc_code FROM KSTU_ACC_LEDGER_MASTER WHERE acc_code = 6 AND company_code = '{0}' AND branch_code = '{1}'", branchCode, companyCode));
                if (index == 0) {
                    voucherTrans.acc_code = GetScalarValue<int>(string.Format("SELECT acc_code FROM KSTU_ACC_LEDGER_MASTER WHERE acc_code = 6 AND company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));
                    voucherTrans.acc_type = "O";
                    voucherTrans.dr_amount = 0;
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "Cash Paid to Party";
                }
                if (index == 1) {
                    voucherTrans.acc_code_master = 1;
                    voucherTrans.acc_type = "C";
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Cash Paid To Party";
                    voucherTrans.voucher_type = "PC";
                }

                if (index == 2) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_type = "B";
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "Cheque Paid To Party";
                    voucherTrans.voucher_type = "RC";
                    voucherTrans.receipt_no = cashPaidReceiptNo;
                }
                if (index == 3) {
                    voucherTrans.acc_code_master = 1;
                    voucherTrans.acc_type = "C";
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "Order Advance through Sales Bill";
                    voucherTrans.voucher_type = "RO";
                    voucherTrans.receipt_no = cashPaidReceiptNo;
                }
            }

            if (isCOPayments) {
                voucherTrans.acc_code = accCode;
                if (Amount > 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_type = "O";
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    if (string.Compare(dtCOPayments.Rows[index]["pay_mode"].ToString(), "OP") == 0)
                        voucherTrans.narration = "Order Adv. Adjusted";

                    if (string.Compare(dtCOPayments.Rows[index]["pay_mode"].ToString(), "CT") == 0) {
                        string accName = GetScalarValue<string>(string.Format("select coalesce(acc_name,'') from kstu_acc_ledger_master where scheme_Code= '{0}' and company_code='{1}' and branch_code='{2}' "
                            , dtCOPayments.Rows[index]["scheme_code"].ToString(), companyCode, branchCode));

                        if (string.Compare(dtCOPayments.Rows[index]["schtype"].ToString(), "W") == 0) {
                            voucherTrans.narration = string.Format("Being Saving Scheme Winner Amount ( {0} ) Adjusted To Sales", accName);
                        }
                        else if (string.Compare(dtCOPayments.Rows[index]["schtype"].ToString(), "B") == 0) {
                            voucherTrans.narration = string.Format("Being Saving Scheme Discount ( {0} ) Adjusted To Sales", accName);
                        }
                        else {
                            voucherTrans.narration = string.Format("Being Saving Scheme ( {0} ) Adjusted To Sales", accName);
                        }
                    }
                    voucherTrans.receipt_no = dtCOPayments.Rows[index]["series_no"].ToString();
                }
            }


            if (isHIPayments) {
                voucherTrans.acc_code = accCode;
                if (Convert.ToDecimal(Amount) > 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_type = "O";
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.receipt_no = dtCOPayments.Rows[index]["series_no"].ToString();
                    voucherTrans.narration = "HELP INDIA SCHEME Adjusted to Bill - " + voucherTrans.receipt_no;

                }
            }

            if (isPayments) {
                string payMode = string.Empty;
                string transType = string.Empty;

                if (!isPayPurchase) {
                    payMode = dtPayments.Rows[index]["pay_mode"].ToString();
                    transType = dtPayments.Rows[index]["trans_type"].ToString();
                }

                else {
                    payMode = dtPurPay.Rows[index]["pay_mode"].ToString();
                    transType = dtPurPay.Rows[index]["trans_type"].ToString();
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "C") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    if (isVat) {
                        voucherTrans.narration = "By Cash Sales-GST Collected";
                    }
                    else if (isExcise) {
                        voucherTrans.narration = "By Cash Sales-ED Collected";
                    }
                    else if (isEdu) {
                        voucherTrans.narration = "By Cash Sales-Edu Cess Collected";
                    }
                    else if (isHEdu) {
                        voucherTrans.narration = "By Cash Sales-High Edu Cess Collected";
                    }
                    else if (isOtherTran) {
                        // voucherTrans.receipt_no = memberShip;
                    }
                    else {
                        voucherTrans.narration = "By Cash Sales";
                    }
                }
                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "C") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "By Cash Stone Sales";
                }
                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "Q") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : Cheque";
                }
                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "Q") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Sales Collection : Cheque";
                }
                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "D") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : DD";
                }
                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "D") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Sales Collection : DD";
                }
                if (string.Compare(transType, "P") == 0 && string.Compare(payMode, "C") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Cash Paid To Party";
                }

                if (string.Compare(transType, "SP") == 0 && string.Compare(payMode, "C") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Cash Paid To Party";
                }

                if (string.Compare(transType, "P") == 0 && string.Compare(payMode, "Q") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Cheque Payment";
                }

                if (string.Compare(transType, "P") == 0 && string.Compare(payMode, "EP") == 0) {
                    voucherTrans.acc_code = accCode;

                    if (!isPurchaseEpay) {
                        voucherTrans.dr_amount = 0;
                        voucherTrans.cr_amount = Amount;

                    }
                    else {
                        voucherTrans.dr_amount = Amount;
                        voucherTrans.cr_amount = 0;
                    }

                    voucherTrans.narration = "Purchase E-Payment";
                }


                if (string.Compare(transType, "SP") == 0 && string.Compare(payMode, "Q") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Cheque Payment";
                }

                if (string.Compare(transType, "SP") == 0 && string.Compare(payMode, "EP") == 0) {
                    voucherTrans.acc_code = accCode;
                    if (!isPurchaseEpay) {
                        voucherTrans.dr_amount = 0;
                        voucherTrans.cr_amount = Amount;
                    }
                    else {
                        voucherTrans.dr_amount = Amount;
                        voucherTrans.cr_amount = 0;
                    }
                    voucherTrans.narration = "Stone E-Payment";
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "R") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : Card";
                }

                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "R") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Sales Collection : Card";
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "GV") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 4;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : Gift Voucher";
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "CV") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = GetMagnaAccountCode(50);
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = string.Format("Sales Collection : Exchange Ledger of '{0}' currency", voucherTrans.currency_type);
                }

                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "GV") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 4;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Sales Collection : Gift Voucher";
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "EP") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 5;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : E Payments";
                }

                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "OGS") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 9;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : Old Scheme";
                }


                if (string.Compare(transType, "S") == 0 && string.Compare(payMode, "EKM") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 9;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Sales Collection : Old Scheme";
                }

                if (string.Compare(transType, "SS") == 0 && string.Compare(payMode, "EP") == 0) {
                    voucherTrans.acc_code_master = 0;
                    voucherTrans.acc_code = 5;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = "Stone Sales Collection : E Payments";
                }

                if (string.Compare(transType, "P") == 0 && string.Compare(payMode, "R") == 0) {
                    voucherTrans.acc_code = accCode;
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.narration = "Credit Payment";
                }

                string PayMode = payMode.Substring(0, 1);

                if (string.Compare(transType, "S") == 0 && string.Compare(PayMode, "B") == 0) {
                    voucherTrans.acc_type = "O";
                    voucherTrans.acc_code = 7;
                    voucherTrans.dr_amount = Amount;
                    voucherTrans.cr_amount = 0;
                    voucherTrans.narration = string.Format("Branch Order Adjusted");
                }

            }
            if (isChit) {
                voucherTrans.acc_code = accCode;
                voucherTrans.trans_type = "CHIT";
                voucherTrans.acc_type = "C";
                voucherTrans.acc_code_master = 1;
                voucherTrans.cr_amount = Amount;
                voucherTrans.dr_amount = 0;
            }

            if (isJPACash) {
                voucherTrans.acc_code = accCode;
                voucherTrans.trans_type = "CHIT";
                voucherTrans.acc_type = "C";
                voucherTrans.acc_code_master = 1;
                voucherTrans.cr_amount = 0;
                voucherTrans.dr_amount = Amount;
            }

            if (isJPACheque) {
                voucherTrans.acc_code = accCode;
                voucherTrans.trans_type = "CHIT";
                voucherTrans.acc_type = "B";
                voucherTrans.cr_amount = 0;
                voucherTrans.dr_amount = Amount;
            }

            if (isOtherTran) {
                voucherTrans.acc_code_master = 0;
                voucherTrans.acc_code = accCode;
                //voucherTrans.acc_type = "J";

                if (!isCheqFirstTime) {
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.dr_amount = 0;
                }
                else {
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Amount;
                    //voucherTrans.receipt_no = memberShip;
                }
            }
            if (isHI) {
                voucherTrans.acc_code_master = 0;
                voucherTrans.acc_code = accCode;
                voucherTrans.acc_type = "O";
                voucherTrans.trans_type = "HIS";


                if (isHIFirst) {
                    voucherTrans.cr_amount = Amount;
                    voucherTrans.dr_amount = 0;

                }
                else {
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Amount;
                }
            }

            if (isSalesRoundOff) {
                if (Amount > 0) {
                    // voucherTrans.dr_amount = Math.Abs(Amount);
                    //voucherTrans.cr_amount = 0;
                    voucherTrans.cr_amount = Math.Abs(Amount);
                    voucherTrans.dr_amount = 0;
                }
                else {
                    //voucherTrans.dr_amount = 0;
                    //voucherTrans.cr_amount = Math.Abs(Amount);
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Math.Abs(Amount);
                }

                voucherTrans.acc_code_master = 0;
                voucherTrans.acc_code = accCode;
                voucherTrans.acc_type = "O";
                voucherTrans.narration = string.Format("Sales Round-off");
            }

            if (isPurchaseRoundOff) {
                if (Amount > 0) {
                    voucherTrans.dr_amount = 0;
                    voucherTrans.cr_amount = Math.Abs(Amount);
                }
                else {
                    voucherTrans.cr_amount = 0;
                    voucherTrans.dr_amount = Math.Abs(Amount);
                }

                voucherTrans.acc_code_master = 0;
                voucherTrans.acc_code = accCode;
                voucherTrans.acc_type = "O";
                voucherTrans.narration = string.Format("Purchase Adj. Round-off");
            }

            voucherTrans.txt_seq_no = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "03");
            if (voucherTrans.txt_seq_no < 1) {
                return false;
            }

            if (string.Compare(voucherTrans.acc_type.ToString(), "C") == 0) {
                voucherTrans.acc_code_master = 1;
                if (nCashVoucherNo == 0) {
                    nCashVoucherNo = GetNewSeriesNo("KSTS_ACC_VOUCHER_SEQ_NOS", voucherTrans.acc_code_master.ToString());
                    if (nCashVoucherNo < 1) {
                        return false;
                    }

                    voucherTrans.voucher_no = nCashVoucherNo;
                    isNewVoucher = true;
                    nCashCount = 1;
                }
                else {
                    voucherTrans.voucher_no = nCashVoucherNo;
                    nCashCount = nCashCount + 1;
                }
                voucherTrans.voucher_seq_no = nCashCount;
            }
            if (string.Compare(voucherTrans.acc_type.ToString(), "O") == 0 && string.Compare(voucherTrans.trans_type.ToString(), "HIS") != 0) {
                if (nCreditVoucherNo == 0) {
                    nCreditVoucherNo = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "10");
                    if (nCreditVoucherNo < 1) {
                        return false;
                    }
                    voucherTrans.voucher_no = nCreditVoucherNo;
                    isNewVoucher = true;
                    nCreditCount = 1;
                }
                else {
                    voucherTrans.voucher_no = nCreditVoucherNo;
                    nCreditCount = nCreditCount + 1;
                }
                voucherTrans.voucher_seq_no = nCreditCount;
            }
            if (string.Compare(voucherTrans.trans_type.ToString(), "HIS") == 0) {
                if (nHIPurchaseNo == 0) {
                    nHIPurchaseNo = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "24");
                    if (nHIPurchaseNo < 1) {
                        return false;
                    }
                    voucherTrans.voucher_no = nHIPurchaseNo;
                    isNewVoucher = true;
                    nCreditCount = 1;
                }
                else {
                    voucherTrans.voucher_no = nHIPurchaseNo;
                    nHIPurchaseCount = nHIPurchaseCount + 1;
                }
                voucherTrans.voucher_seq_no = nHIPurchaseCount;
            }
            if (string.Compare(voucherTrans.acc_type.ToString(), "B") == 0) {
                if (nBankVoucherNo != 0) {
                    voucherTrans.voucher_no = nBankVoucherNo;
                }
                else {
                    if (string.Compare(voucherTrans.acc_code_master.ToString(), "0") == 0 && string.Compare(voucherTrans.acc_type, "B") == 0)
                        voucherTrans.voucher_no = GetNewSeriesNo("KSTS_ACC_VOUCHER_SEQ_NOS", "2");
                    else {
                        voucherTrans.voucher_no = GetNewSeriesNo("KSTS_ACC_VOUCHER_SEQ_NOS", "2");
                    }
                    if (voucherTrans.voucher_no < 1) {
                        return false;
                    }
                    isNewVoucher = true;
                }
                voucherTrans.voucher_seq_no = GetScalarValue<int>(string.Format("SELECT COALESCE(MAX(kavt.voucher_seq_no),0) \n"
                                + "FROM KSTU_ACC_VOUCHER_TRANSACTIONS kavt WHERE kavt.trans_type = 'BILL' AND kavt.acc_code_master = {0} AND company_code = '{1}' AND kavt.branch_code = '{2}' AND voucher_date BETWEEN '{3}' AND '{4}' AND kavt.cflag = 'N'", voucherTrans.acc_code_master, companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.000")));
                voucherTrans.voucher_seq_no = voucherTrans.voucher_seq_no + 1;
            }

            string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS"
                     + SIGlobals.Globals.Separator + voucherTrans.voucher_no + SIGlobals.Globals.Separator + voucherTrans.acc_type + SIGlobals.Globals.Separator + voucherTrans.acc_code_master + SIGlobals.Globals.Separator + voucherTrans.trans_type + SIGlobals.Globals.Separator + companyCode + SIGlobals.Globals.Separator + branchCode;

            this.ObjectId = voucherTrans.obj_id = SIGlobals.Globals.GetHashcode(temp);
            this.ObjectType = "Voucher Entry";
            var currentTime = SIGlobals.Globals.GetDateTime();
            voucherTrans.inserted_on = currentTime;
            voucherTrans.UpdateOn = currentTime;

            KSTU_ACC_VOUCHER_TRANSACTIONS newVoucher = TranferFields(voucherTrans);
            db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(newVoucher);

            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "03")) {
                return false;
            }
            if (isNewVoucher) {
                if (string.Compare(voucherTrans.acc_type.ToString(), "B") == 0) {
                    if (!IncrementSeriesNo("KSTS_ACC_VOUCHER_SEQ_NOS", "2")) {
                        return false;
                    }
                }
                if (string.Compare(voucherTrans.acc_type.ToString(), "C") == 0) {

                    if (!IncrementSeriesNo("KSTS_ACC_VOUCHER_SEQ_NOS", voucherTrans.acc_code_master.ToString())) {
                        return false;
                    }
                }
                if (string.Compare(voucherTrans.acc_type.ToString(), "O") == 0 && string.Compare(voucherTrans.trans_type.ToString(), "HIS") != 0) {
                    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "10")) {
                        return false;
                    }
                }
                if (string.Compare(voucherTrans.trans_type.ToString(), "HIS") == 0) {
                    if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "24")) {
                        return false;
                    }
                }
            }
            return true;
        }

        private KSTU_ACC_VOUCHER_TRANSACTIONS TranferFields(KSTU_ACC_VOUCHER_TRANSACTIONS voucherTrans)
        {
            KSTU_ACC_VOUCHER_TRANSACTIONS newVoucherTxn = new KSTU_ACC_VOUCHER_TRANSACTIONS
            {
                obj_id = voucherTrans.obj_id,
                branch_code = voucherTrans.branch_code,
                company_code = voucherTrans.company_code,
                txt_seq_no = voucherTrans.txt_seq_no,
                voucher_no = voucherTrans.voucher_no,
                voucher_seq_no = voucherTrans.voucher_seq_no,
                acc_type = voucherTrans.acc_type,
                voucher_date = voucherTrans.voucher_date,
                acc_code = voucherTrans.acc_code,
                dr_amount = voucherTrans.dr_amount,
                cr_amount = voucherTrans.cr_amount,
                chq_no = voucherTrans.chq_no,
                chq_date = voucherTrans.chq_date,
                fin_year = voucherTrans.fin_year,
                fin_period = voucherTrans.fin_period,
                acc_code_master = voucherTrans.acc_code_master,
                narration = voucherTrans.narration,
                contra_seq_no = voucherTrans.contra_seq_no,
                reconsile_flag = voucherTrans.reconsile_flag,
                reconsile_date = voucherTrans.reconsile_date,
                receipt_no = voucherTrans.receipt_no,
                trans_type = voucherTrans.trans_type,
                inserted_on = voucherTrans.inserted_on,
                cflag = voucherTrans.cflag,
                UpdateOn = voucherTrans.UpdateOn,
                approved_by = voucherTrans.approved_by,
                approved_date = voucherTrans.approved_date,
                is_approved = voucherTrans.is_approved,
                subledger_acc_code = voucherTrans.subledger_acc_code,
                party_name = voucherTrans.party_name,
                Cancelled_by = voucherTrans.Cancelled_by,
                Cancelled_remarks = voucherTrans.Cancelled_remarks,
                voucher_type = voucherTrans.voucher_type,
                reconsile_by = voucherTrans.reconsile_by,
                currency_type = voucherTrans.currency_type,
                New_voucher_no = voucherTrans.New_voucher_no,
                section_id = voucherTrans.section_id,
                is_tds = voucherTrans.is_tds,
                TDS_amount = voucherTrans.TDS_amount,
                Is_Verified = voucherTrans.Is_Verified,
                Verified_By = voucherTrans.Verified_By,
                Verified_Date = voucherTrans.Verified_Date,
                Verified_Remarks = voucherTrans.Verified_Remarks,
                Is_Authorized = voucherTrans.Is_Authorized,
                Authorized_By = voucherTrans.Authorized_By,
                Authorized_Date = voucherTrans.Authorized_Date,
                Authorized_Remarks = voucherTrans.Authorized_Remarks,
                exchange_rate = voucherTrans.exchange_rate,
                currency_value = voucherTrans.currency_value,
                pay_mode = voucherTrans.pay_mode,
                cancelled_date = voucherTrans.cancelled_date,
                UniqRowID = Guid.NewGuid()
            };
            return newVoucherTxn;
        }

        private bool CreateLedgerAccount(out string errorXml, string name, int cust_id, string scheme_code)
        {
            KSTU_ACC_LEDGER_MASTER ledgerMaster = new KSTU_ACC_LEDGER_MASTER();
            errorXml = string.Empty;
            ledgerMaster.acc_name = name;
            ledgerMaster.acc_code = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "02");
            if (ledgerMaster.acc_code < 1) {
                return false;
            }

            ledgerMaster.acc_type = "O";
            ledgerMaster.branch_code = branchCode;
            ledgerMaster.company_code = companyCode;
            ledgerMaster.scheme_code = scheme_code;
            ledgerMaster.cust_id = cust_id;
            ledgerMaster.group_id = 34;
            ledgerMaster.opn_bal = 0;
            ledgerMaster.opn_bal_type = "D";
            ledgerMaster.party_code = string.Empty;
            ledgerMaster.gs_code = string.Empty;
            ledgerMaster.gs_seq_no = 0;
            ledgerMaster.obj_status = "O";
            string temp = "KSTU_ACC_LEDGER_MASTER" + SIGlobals.Globals.Separator + ledgerMaster.acc_code + SIGlobals.Globals.Separator
                + companyCode + SIGlobals.Globals.Separator + branchCode;

            ledgerMaster.obj_id = this.ObjectId = SIGlobals.Globals.GetHashcode(temp);
            db.KSTU_ACC_LEDGER_MASTER.Add(ledgerMaster);
            accCode = ledgerMaster.acc_code;
            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "02")) {
                return false;
            }
            return true;
        }

        private bool UpdateVoucherTransaction(out string errorXml, decimal Amount)
        {
            errorXml = string.Empty;

            voucherTrans.trans_type = "CHIT";
            voucherTrans.voucher_date = AccUpdateTimeStamp;

            //if (voucherTrans.voucher_no < 1)
            //{
            //    return false;
            //}

            int nVouNo = voucherTrans.voucher_no;

            voucherTrans.acc_code = accCode;
            int nCount = GetScalarValue<int>(string.Format("SELECT Count(voucher_no) FROM KSTU_ACC_VOUCHER_TRANSACTIONS WHERE voucher_no = {0} AND branch_code = '{3}' AND company_code = '{4}' AND trans_type = '{2}' AND acc_type = 'O' AND voucher_date BETWEEN '{1} 00:00:00' AND '{1} 23:59:59'"
                , nVouNo, AccUpdateTimeStamp.ToString("MM/dd/yyyy"), voucherTrans.trans_type, branchCode, companyCode));

            voucherTrans.acc_code_master = 0;
            voucherTrans.acc_type = "O";
            voucherTrans.branch_code = branchCode;
            voucherTrans.chq_date = AccUpdateTimeStamp;
            voucherTrans.chq_no = string.Empty;
            voucherTrans.company_code = companyCode;
            voucherTrans.contra_seq_no = 0;
            voucherTrans.receipt_no = "0";
            if (!isCheqFirstTime) {
                voucherTrans.dr_amount = 0;
                voucherTrans.cr_amount = Amount;
            }
            else {
                voucherTrans.dr_amount = Amount;
                voucherTrans.cr_amount = 0;
            }
            voucherTrans.reconsile_date = AccUpdateTimeStamp;
            voucherTrans.reconsile_flag = "N";
            voucherTrans.txt_seq_no = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "03");
            if (voucherTrans.txt_seq_no < 1) {
                return false;
            }

            if (string.Compare(voucherTrans.acc_type.ToString(), "O") == 0 && string.Compare(voucherTrans.trans_type.ToString(), "HIS") != 0) {
                if (nChitCollectionNo == 0) {
                    nChitCollectionNo = GetNewSeriesNo("KSTS_ACC_SEQ_NOS", "16");
                    if (nChitCollectionNo < 1) {
                        return false;
                    }
                    voucherTrans.voucher_no = nChitCollectionNo;
                    nChitCollectioncount = 1;
                }
                else {
                    voucherTrans.voucher_no = nChitCollectionNo;
                    nChitCollectioncount = nChitCollectioncount + 1;
                }
                voucherTrans.voucher_seq_no = nChitCollectioncount;
            }

            var updatedDateTime = SIGlobals.Globals.GetDateTime();
            voucherTrans.inserted_on = updatedDateTime;
            voucherTrans.UpdateOn = updatedDateTime;

            //voucherTrans.voucher_seq_no = nCount + 1;
            string temp = "KSTU_ACC_VOUCHER_TRANSACTIONS"
                    + SIGlobals.Globals.Separator + voucherTrans.voucher_no + SIGlobals.Globals.Separator + voucherTrans.acc_type + SIGlobals.Globals.Separator + voucherTrans.acc_code_master + SIGlobals.Globals.Separator + voucherTrans.trans_type + SIGlobals.Globals.Separator + companyCode + SIGlobals.Globals.Separator + branchCode;

            this.ObjectId = voucherTrans.obj_id = SIGlobals.Globals.GetHashcode(temp);
            this.ObjectType = "Voucher Entry";
            var newVoucherEntry = TranferFields(voucherTrans);
            db.KSTU_ACC_VOUCHER_TRANSACTIONS.Add(newVoucherEntry);

            if (!IncrementSeriesNo("KSTS_ACC_SEQ_NOS", "03")) {
                return false;
            }

            return true;
        }

        private bool UpdatePaymentDetails()
        {
            string strDB = GetScalarValue<string>(string.Format("SELECT pos_db FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));
            string strTable1 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '1'", companyCode, branchCode));
            string strTable2 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '2'", companyCode, branchCode));
            string query = string.Format("SELECT * FROM {3}.{2} d,{3}.{1} m WHERE d.Payment_Date>='{0} 00:00:00.000' AND d.Payment_Date<'{0} 23:59:59' and  m.CancelFlag != 'Y' AND d.ChqRet_Flag != 'Y' AND m.Receipt_No=d.Receipt_No AND m.obj_id=d.obj_id  ORDER BY d.Receipt_No"
                , AccUpdateTimeStamp.ToString("MM/dd/yyyy"), strTable1, strTable2, strDB);
            DataTable dtNew = SIGlobals.Globals.GetDataTable(query);
            KTTU_PAYMENT_DETAILS paymentDetails = new KTTU_PAYMENT_DETAILS();
            if (dtNew != null && dtNew.Rows.Count > 0) {
                for (int i = 0; i < dtNew.Rows.Count; i++) {
                    paymentDetails.bank = dtNew.Rows[i]["Bank_Code"].ToString();
                    paymentDetails.bank_name = dtNew.Rows[i]["Bank_Code"].ToString();
                    paymentDetails.bill_counter = string.Empty;
                    paymentDetails.company_code = companyCode;
                    paymentDetails.branch_code = branchCode;
                    paymentDetails.card_type = string.Empty;
                    paymentDetails.cflag = "N";
                    paymentDetails.cheque_date = AccUpdateTimeStamp;
                    if (string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "Q") == 0 || string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "D") == 0) {
                        paymentDetails.cheque_no = dtNew.Rows[i]["Chq_DD_No"].ToString();
                        paymentDetails.Ref_BillNo = string.Empty;
                    }
                    else if (string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "R") == 0) {
                        paymentDetails.cheque_no = dtNew.Rows[i]["acc_code"].ToString();
                        paymentDetails.Ref_BillNo = dtNew.Rows[i]["Chq_DD_No"].ToString();
                        paymentDetails.card_app_no = dtNew.Rows[i]["Card_ApprovalNo"].ToString();
                    }
                    else {
                        paymentDetails.cheque_no = "0";
                        paymentDetails.Ref_BillNo = string.Empty;
                    }
                    paymentDetails.expiry_date = AccUpdateTimeStamp;
                    paymentDetails.is_paid = "N";
                    paymentDetails.party_code = string.Empty;
                    paymentDetails.pay_amt = Convert.ToDecimal(dtNew.Rows[i]["Payment_Amt"].ToString());
                    paymentDetails.pay_date = Convert.ToDateTime(dtNew.Rows[i]["Payment_Date"].ToString());
                    paymentDetails.pay_details = string.Empty;
                    paymentDetails.pay_mode = dtNew.Rows[i]["Payment_Type"].ToString();
                    paymentDetails.series_no = Convert.ToInt32(dtNew.Rows[i]["Receipt_No"].ToString());
                    paymentDetails.sno = 1;
                    paymentDetails.obj_id = dtNew.Rows[i]["obj_id"].ToString();
                    paymentDetails.trans_type = "CT";
                    paymentDetails.receipt_no = 0;
                    paymentDetails.scheme_code = dtNew.Rows[i]["Scheme_Code"].ToString();
                    db.KTTU_PAYMENT_DETAILS.Add(paymentDetails);

                }
            }

            query = string.Format("SELECT * FROM  {3}.{2} d,{3}.{1} m WHERE d.Payment_Date>='{0} 00:00:00.000' AND d.Payment_Date<'{0} 23:59:59' and  m.CancelFlag ='Y' AND d.ChqRet_Flag ='Y' AND m.Receipt_No=d.Receipt_No AND m.obj_id=d.obj_id  ORDER BY d.Receipt_No", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), strTable1, strTable2, strDB);
            dtNew = SIGlobals.Globals.GetDataTable(query);
            if (dtNew != null && dtNew.Rows.Count > 0) {
                for (int i = 0; i < dtNew.Rows.Count; i++) {
                    paymentDetails.bank = dtNew.Rows[i]["Bank_Code"].ToString();
                    paymentDetails.bank_name = dtNew.Rows[i]["Bank_Code"].ToString();
                    paymentDetails.bill_counter = string.Empty;
                    paymentDetails.branch_code = branchCode;
                    paymentDetails.card_type = string.Empty;
                    paymentDetails.cflag = "N";
                    paymentDetails.cheque_date = AccUpdateTimeStamp;
                    if (string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "Q") == 0 || string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "D") == 0) {
                        paymentDetails.cheque_no = dtNew.Rows[i]["Chq_DD_No"].ToString();
                        paymentDetails.Ref_BillNo = string.Empty;
                    }
                    else if (string.Compare(dtNew.Rows[i]["Payment_Type"].ToString(), "R") == 0) {
                        paymentDetails.cheque_no = dtNew.Rows[i]["acc_code"].ToString();
                        paymentDetails.Ref_BillNo = dtNew.Rows[i]["Chq_DD_No"].ToString();
                        paymentDetails.card_app_no = dtNew.Rows[i]["Card_ApprovalNo"].ToString();
                    }
                    else {
                        paymentDetails.cheque_no = "0";
                        paymentDetails.Ref_BillNo = string.Empty;
                    }
                    paymentDetails.company_code = companyCode;
                    paymentDetails.expiry_date = AccUpdateTimeStamp;
                    paymentDetails.is_paid = "N";
                    paymentDetails.party_code = string.Empty;
                    paymentDetails.pay_amt = Convert.ToDecimal(dtNew.Rows[i]["Payment_Amt"].ToString());
                    paymentDetails.pay_date = Convert.ToDateTime(dtNew.Rows[i]["ChqRet_Date"].ToString());
                    paymentDetails.pay_details = string.Empty;
                    paymentDetails.pay_mode = "QR";
                    paymentDetails.Ref_BillNo = string.Empty;
                    paymentDetails.series_no = Convert.ToInt32(dtNew.Rows[i]["Receipt_No"].ToString());
                    paymentDetails.sno = Convert.ToInt32(dtNew.Rows[i]["Receipt_No"].ToString());
                    paymentDetails.obj_id = dtNew.Rows[i]["obj_id"].ToString();
                    paymentDetails.trans_type = "CT";
                    paymentDetails.receipt_no = 0;
                    db.KTTU_PAYMENT_DETAILS.Add(paymentDetails);

                }
            }

            return true;
        }

        private DataTable GetSalesAmount()
        {
            string query = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{0}','{1}','{2}','{3}','{4}','{5}'"
            , companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000")
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), "S", GetDefaultCurrencyCode());
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetSalesRoundOff()
        {
            string query = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{0}','{1}','{2}','{3}','{4}','{5}'"
            , companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000")
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), "RR", GetDefaultCurrencyCode());
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetPurchaseAdjustedBills()
        {
            string query = string.Format(" Exec [usp_acc_Purchase_Sales_Amount] '{0}','{1}','{2}','{3}','{4}','{5}'"
           , companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000")
           , AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), "P", GetDefaultCurrencyCode());
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetSalesReturnAmount()
        {
            string query = string.Format(" Exec [usp_acc_Purchase_Sales_Amount] '{0}','{1}','{2}','{3}','{4}','{5}'"
            , companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000")
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"), "SR", GetDefaultCurrencyCode());

            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetRepairAmount()
        {
            string query = string.Format("SELECT item_type AS GStock,SUM(total_repair_amount) AS Amount,0 AS Tax,'RP' AS BillType  \n"
           + "FROM KTTU_REPAIR_ISSUE_MASTER WHERE company_code = '{0}' AND branch_code = '{1}' AND issue_date >= '{2}' AND issue_date <= '{3}'  \n"
           + "GROUP BY item_type  \n"
           + "", companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59.998"));
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetChitCollection()
        {
            string query = string.Empty;
            string strDB = GetScalarValue<string>(string.Format("SELECT pos_db FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));
            string strTable1 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '1'", companyCode, branchCode));
            string strTable2 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '2'", companyCode, branchCode));
            if (!string.IsNullOrEmpty(strDB) || !string.IsNullOrEmpty(strDB))
                query = string.Format("select d.Scheme_code,sum(Receipt_Amt) as Total,d.Payment_Type,Count(*) as [No.Of Bills],d.acc_code from {3}.{1} m, {3}.{2} d where m.CancelFlag!='Y' and d.ChqRet_Flag!='Y' and Receipt_Date>='{0} 00:00:00.000' and Receipt_Date <'{0} 23:59:59.998' and d.Scheme_code=m.Scheme_code and d.obj_id=m.obj_id group by d.Scheme_code,d.Payment_Type,d.acc_code", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), strTable1, strTable2, strDB);
            else
                query = string.Format("SELECT Scheme_code,sum(Payment_Amt) as Total,Payment_Type,Count(*) as [No.Of Bills],Receipt_branch,'' AS opening_branch \n"
                + "FROM CHTU_PAYMENT_DETAILS where CancelFlag!='Y' and ChqRet_Flag!='Y' and Payment_Date>='{0} 00:00:00.000' and Payment_Date <'{0} 23:59:59'  group by Scheme_code,Payment_Type,Receipt_branch \n"
                + "UNION \n"
                + "SELECT Scheme_code,sum(Payment_Amt) as Total,Payment_Type,Count(*) as [No.Of Bills],Receipt_branch,branch_code AS opening_branch \n"
                + "FROM CHTU_PAYMENT_DETAILS where CancelFlag!='Y' and ChqRet_Flag!='Y' and Payment_Date>='{0} 00:00:00.000' and Payment_Date <'{0} 23:59:59' AND branch_code != Receipt_branch  group by Scheme_code,Payment_Type,Receipt_branch,branch_code \n"
                + "", AccUpdateTimeStamp.ToString("MM/dd/yyyy"));
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetPaymentsDetails()
        {
            string query = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{2}','{3}','{0}','{1}','{4}','{5}'"
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode, "PD", GetDefaultCurrencyCode());

            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetPurPay()
        {
            string query = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{2}','{3}','{0}','{1}','{4}','{5}'"
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode, "PM", GetDefaultCurrencyCode());

            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetChitandOrdAdj()
        {
            string strCO = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{2}','{3}','{0}','{1}','{4}','{5}'"
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59")
            , companyCode, branchCode, "O", GetDefaultCurrencyCode());
            return SIGlobals.Globals.GetDataTable(strCO);
        }

        private DataTable GetCreditLedgers()
        {
            string query = string.Format(" Exec [usp_acc_Purchase_Sales_Amount] '{0}','{1}','{2}','{3}','{4}','{5}'"
                    , companyCode, branchCode, AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000")
                    , AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), "CR", GetDefaultCurrencyCode());
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetChitCashCollection()
        {
            string query = string.Empty;
            string strDB = GetScalarValue<string>(string.Format("SELECT pos_db FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));
            string strTable1 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '3'", companyCode, branchCode));
            if (!string.IsNullOrEmpty(strDB) || !string.IsNullOrEmpty(strDB))
                query = string.Format("select * from {2}.{1} where closing_mode ='C' AND Closing_Date >='{0} 00:00:00.000' and Closing_Date <'{0} 23:59:59'", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), strTable1, strDB);
            else
                query = string.Format("select * from CHTU_CHIT_CLOSURE where closing_mode ='C' AND Closing_Date >='{0} 00:00:00.000' and Closing_Date <'{0} 23:59:59'", AccUpdateTimeStamp.ToString("MM/dd/yyyy"));
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetChitChequeCollection()
        {
            string query = string.Empty;
            string strDB = GetScalarValue<string>(string.Format("SELECT pos_db FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}'", companyCode, branchCode));
            string strTable1 = GetScalarValue<string>(string.Format("SELECT pos_table FROM KSTU_CHIT_TABLE WHERE company_code = '{0}' AND branch_code = '{1}' AND obj_id = '3'", companyCode, branchCode));
            if (!string.IsNullOrEmpty(strDB) || !string.IsNullOrEmpty(strDB))
                query = string.Format("select * from {2}.{1} where closing_mode ='Q' AND Closing_Date >='{0} 00:00:00.000' and Closing_Date <'{0} 23:59:59'", AccUpdateTimeStamp.ToString("MM/dd/yyyy"), strTable1, strDB);
            else
                query = string.Format("select * from CHTU_CHIT_CLOSURE where closing_mode ='Q' AND Closing_Date >='{0} 00:00:00.000' and Closing_Date <'{0} 23:59:59'", AccUpdateTimeStamp.ToString("MM/dd/yyyy"));

            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetSavingsClosure(string closing_mode)
        {
            string query = string.Format("select * from CHTU_CHIT_CLOSURE where closing_mode ='{0}' AND Closing_Date >='{1} 00:00:00.000' and Closing_Date <'{1} 23:59:59'", 
                closing_mode, AccUpdateTimeStamp.ToString("MM/dd/yyyy"));
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetHIAdj()
        {
            string strCO = string.Format("SELECT SUM(pay_amt) AS Amount,series_no,pay_mode,ksm.cust_id,ksm.cust_name,kpd.scheme_code,convert(varchar,kpd.pay_date,103),'' AS schtype   FROM KTTU_PAYMENT_DETAILS kpd,KTTU_SALES_MASTER ksm \n"
           + "WHERE trans_type = 'S' AND pay_mode = 'HI' AND kpd.cflag != 'Y'  \n"
           + "AND pay_date BETWEEN '{0}' AND '{1}' \n"
           + "AND kpd.company_code = '{2}' AND kpd.branch_code = '{3}' \n"
           + "AND ksm.company_code = '{2}' AND ksm.branch_code = '{3}' \n"
           + "AND series_no = bill_no AND  (sal_bill_type IS NULL OR sal_bill_type IS NULL) and party_code is null\n"
           + "GROUP BY series_no,pay_mode,cust_id,cust_name,kpd.scheme_code,convert(varchar,kpd.pay_date,103)  \n"
           , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode);
            return SIGlobals.Globals.GetDataTable(strCO);
        }

        private DataTable OtherBranchTransactions()
        {
            string strOtherBranchTran = string.Format("Exec [usp_acc_Purchase_Sales_Amount] '{2}','{3}','{0}','{1}','{4}','{5}'"
            , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59")
            , companyCode, branchCode, "IB", GetDefaultCurrencyCode());

            return SIGlobals.Globals.GetDataTable(strOtherBranchTran);
        }

        private DataTable GetChequeAndCardBalance()
        {
            string query = string.Format("SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'S' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'O' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'CT' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'SS' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'R' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode \n"
                 , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode);
            return SIGlobals.Globals.GetDataTable(query);
        }

        private DataTable GetChequeAndCardDeposits()
        {
            string query = string.Format("SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode,company_code,branch_code \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'S' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode,company_code,branch_code \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'O' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'CT' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'Q' AND trans_type = 'SS' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode,company_code,branch_code  \n"
                 + "UNION ALL \n"
                 + "SELECT SUM(pay_amt) AS Amount,pay_date,cheque_no,pay_mode,company_code,branch_code \n"
                 + "FROM KTTU_PAYMENT_DETAILS WHERE pay_mode = 'R' AND pay_date BETWEEN '{0}' AND '{1}' AND cflag = 'N' AND company_code='{2}' AND branch_code='{3}' GROUP BY pay_date,cheque_no,pay_mode,company_code,branch_code \n"
                 , AccUpdateTimeStamp.ToString("MM/dd/yyyy 00:00:00.000"), AccUpdateTimeStamp.ToString("MM/dd/yyyy 23:59:59"), companyCode, branchCode);
            return SIGlobals.Globals.GetDataTable(query);
        }

        /*
        private ServiceHeader GetServiceHeader()
        {
            ServiceHeader soapHeader = new ServiceHeader();
            DataTable dtAuth = new DataTable();
            dtAuth = CGlobals.GetWSAuthenticationDetails();
            if (dtAuth == null || dtAuth.Rows.Count == 0) {
                return null;
            }
            soapHeader.appUserID = dtAuth.Rows[0]["wsuserid"].ToString();
            soapHeader.appPwd = dtAuth.Rows[0]["wspassword"].ToString();
            return soapHeader;
        }
        */

        public T GetScalarValue<T>(string sql)
        {
            T result = default(T);
            var dt = new DataTable();
            using (var dbContext = new MagnaDbEntities(true)) {
                var conn = dbContext.Database.Connection;
                var connectionState = conn.State;
                try {
                    if (connectionState != ConnectionState.Open)
                        conn.Open();
                    using (var cmd = conn.CreateCommand()) {
                        cmd.CommandText = sql;
                        cmd.CommandType = CommandType.Text;
                        object scalar = cmd.ExecuteScalar();
                        if (scalar != null && scalar != DBNull.Value) {
                            result = (T)Convert.ChangeType(scalar, typeof(T));
                        }
                    }
                }
                catch (Exception) {
                    throw;
                }
            }
            return result;

        }

        public int GetMagnaAccountCode(int obj_id)
        {
            var accCodeMast = db.KSTS_ACC_CODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_id == obj_id).FirstOrDefault();
            if (accCodeMast == null) {
                throw new Exception("Unable to get account code for account :" + obj_id.ToString());
            }
            return accCodeMast.acc_code;
        }

        private bool IncrementSeriesNo(string tableName, string objectID)
        {
            //bool flag;
            //string sql = string.Format("UPDATE {0} SET NextNo = NextNo + 1 WHERE obj_id = '{1}' AND company_code = '{2}' AND branch_code = '{3}'",
            //    tableName, objectID, companyCode, branchCode);
            //flag = (SIGlobals.Globals.ExecuteSQL(sql, db, null) > 0 ? true : false);
            //return flag;

            bool flag = true;
            switch (tableName) {
                case "KSTS_ACC_SEQ_NOS":
                    var accSeq = db.KSTS_ACC_SEQ_NOS.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.obj_id == objectID).FirstOrDefault();
                    if (accSeq == null) {
                        throw new Exception("Account sequence number for " + objectID + " is not found.");
                    }
                    accSeq.nextno = accSeq.nextno + 1;
                    db.Entry(accSeq).State = System.Data.Entity.EntityState.Modified;
                    break;
                case "KSTS_ACC_VOUCHER_SEQ_NOS":
                    var vchSeq = db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.obj_id == objectID).FirstOrDefault();
                    if (vchSeq == null) {
                        throw new Exception("Voucher sequence number for " + objectID + " is not found.");
                    }
                    vchSeq.nextno = vchSeq.nextno + 1;
                    db.Entry(vchSeq).State = System.Data.Entity.EntityState.Modified;
                    break;
                default:
                    flag = false;
                    break;
            }
            return flag;
        }

        private int GetConfigurationValue(string obj_id)
        {
            int configValue = 0;
            var config = db.APP_CONFIG_TABLE.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.obj_id == obj_id).FirstOrDefault();
            if (config != null)
                configValue = Convert.ToInt32(config.value);

            return configValue;
        }

        private int GetNewSeriesNo(string tableName, string obj_id)
        {
            int No = 0;
            if (string.Compare(obj_id, "12") == 0 && string.Compare(tableName, "KSTS_SEQ_NOS") == 0)
            {
                No = SIGetNewSeriesNo(tableName, obj_id, companyCode, branchCode, "");
                return No;
            }
            if (string.Compare(obj_id, "01") == 0 && string.Compare(tableName, "KSTS_ACC_SEQ_NOS") == 0)
            {
                No = SIGetNewSeriesNo(tableName, obj_id, companyCode, branchCode, "");
                return No;
            }

            if (string.Compare(obj_id, "3") == 0 && string.Compare(tableName, "KSTS_SEQ_NOS") == 0) {
                No = SIGetNewSeriesNo(tableName, obj_id, companyCode,  branchCode,"");
                return No;
            }
            else {
                No = SIGetNewSeriesNo(tableName, obj_id, companyCode, branchCode, "");
                return No;
            }
        }

        private int SIGetNewSeriesNo(string tableName, string objectID, string companyCode, string branchCode, string finYear)
        {
            //int num;
            //string sql = string.Format("SELECT NextNo FROM {0} WHERE obj_id = '{1}' AND company_code = '{2}' AND branch_code = '{3}'",
            //    tableName, objectID, companyCode, branchCode);
            //try {
            //    num = GetScalarValue<int>(sql);
            //}
            //catch (Exception) {
            //    throw;
            //}

            //return num;

            int num;
            switch (tableName) {
                case "KSTS_SEQ_NOS":
                    var seqNo = db.KSTS_ACC_SEQ_NOS.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.obj_id == objectID).FirstOrDefault();
                    if (seqNo == null) {
                        throw new Exception("Account sequence number for " + objectID + " is not found.");
                    }
                    num = seqNo.nextno = seqNo.nextno;
                    break;
                case "KSTS_ACC_SEQ_NOS":
                    var accSeq = db.KSTS_ACC_SEQ_NOS.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.obj_id == objectID).FirstOrDefault();
                    if (accSeq == null) {
                        throw new Exception("Account sequence number for " + objectID + " is not found.");
                    }
                    num = accSeq.nextno = accSeq.nextno;
                    break;
                case "KSTS_ACC_VOUCHER_SEQ_NOS":
                    var vchSeq = db.KSTS_ACC_VOUCHER_SEQ_NOS.Where(x => x.company_code == companyCode && x.branch_code == branchCode && x.obj_id == objectID).FirstOrDefault();
                    if (vchSeq == null) {
                        throw new Exception("Voucher sequence number for " + objectID + " is not found.");
                    }
                    num = vchSeq.nextno;
                    break;
                default:
                    throw new Exception(string.Format("Table details not found for : {0}", tableName));
            }
            return num;
        }

        private int GetAccountPostingSetup(string gsCode, string transType)
        {
            string query = string.Format(@"select dbo.Ufn_acc_posting_setup('{0}','{1}','{2}','{3}')"
                                        , companyCode, branchCode, gsCode, transType);
            int acc_code = GetScalarValue<int>(query);
            if (acc_code == 0) {
                throw new Exception(string.Format("Ledger mapping is not done for TransType { 1} , please map the ledger in GST posting setup", transType));

            }
            return acc_code;
        }

        private int GetVatID(decimal taxPer, string company_code, string branch_code)
        {
            return GetScalarValue<int>(string.Format("SELECT tax_Id FROM KSTS_SALETAX_MASTER WHERE tax = {0} AND company_code = '{1}' AND branch_code = '{2}'", 
                taxPer, company_code, branch_code));
        }

        private int GetAccountCode(string type, string AccTransType, string RefCode)
        {
            string query = string.Format(" select dbo.[ufn_GetAccountCode] ('{0}','{1}','{2}','{3}','{4}')"
                , type, AccTransType, RefCode, companyCode, branchCode);
            int accCode = GetScalarValue<int>(query);
            return accCode;
        }

        private string GetDefaultCurrencyCode()
        {
            var companyMast = db.KSTU_COMPANY_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode).FirstOrDefault();
            if (companyMast != null)
                return companyMast.Currency_code;
            else
                return string.Empty;
        }

        private int GetVatAccCode(decimal taxid, string company_code, string branch_code)
        {
            return GetScalarValue<int>(string.Format("SELECT MIN(acc_code) FROM dbo.KSTU_ACC_LEDGER_MASTER WHERE vat_id = {0} AND branch_code = '{1}' AND company_code = '{2}'", 
                taxid, branch_code, company_code));
        }

        private int GetSchemeOtherBranch(string branch, string company_Code, string branch_Code, string SchemeCode)
        {
            string query = string.Format("select isnull(acc_code,0) from kstu_acc_ledger_master where transType='{0}' and company_code='{1}' and branch_code='{2}' and V_TYPE='{3}'"
                , branch, company_Code, branch_Code, SchemeCode);
            int accCode = GetScalarValue<int>(query);
            if (accCode == 0) {
                throw new Exception(string.Format("Gold Tree {0} ledger mapping is not done , please map the ledger in Ledger Master"
                    ,branch));
            }
            return accCode;
        }

        private int GetPartyAccount(string partyCode)
        {
            string query = string.Format("select acc_code from KSTU_ACC_LEDGER_MASTER where party_code='{0}' and company_code='{1}' and branch_code='{2}'", 
                partyCode, companyCode, branchCode);
            int acc_code = GetScalarValue<int>(query);
            if (acc_code == 0) {
                throw new Exception(string.Format("{0}  ledger mapping is not done , please map the ledger "
                   , partyCode));
                
            }
            return acc_code;

        }

        private DataTable GetFinancialDetails()
        {
            string query = string.Format("SELECT * FROM KSTU_ACC_FY_MASTER where company_code='{0}' and branch_code='{1}'", 
                companyCode, branchCode);
            return SIGlobals.Globals.GetDataTable(query);
        }

        private string GetClientID()
        {
            string query = "select ISNULL(obj_id, 1) AS obj_id FROM KSTU_LICENSE_MASTER";
            return GetScalarValue<string>(query);
        }

    }
}
