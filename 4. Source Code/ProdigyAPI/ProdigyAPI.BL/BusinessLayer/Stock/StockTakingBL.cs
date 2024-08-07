using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M (Eshwar)
/// Date: 27/08/2020
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Stock
{
    /// <summary>
    /// This the Buseness Layer Provides All the Methods related to Stock Taking Module.
    /// </summary>
    public class StockTakingBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public string GetStockTakingSummaryReport(string companyCode, string branchCode, int batchNo, out ErrorVM error)
        {
            error = null;
            try {
                var result = (from st in db.KTTU_STOCK_TAKING
                              where st.batch_no == batchNo && st.company_code == companyCode && st.branch_code == branchCode
                              group st by new
                              {
                                  CompanyCode = st.company_code,
                                  BranchCode = st.branch_code,
                                  BatchNo = st.batch_no,
                                  SalesmanCode = st.sal_code,
                                  ItemName = st.item_name,
                                  CounterCode = st.counter_code
                              } into g
                              select new StockTakingReport
                              {
                                  CompanyCode = g.Key.CompanyCode,
                                  BranchCode = g.Key.BranchCode,
                                  BatchNo = g.Key.BatchNo,
                                  Qty = g.Sum(e => e.units),
                                  GrossWt = g.Sum(e => e.gwt),
                                  NetWt = g.Sum(e => e.nwt),
                                  Dcts = g.Sum(e => e.D_cts),
                                  SalesmanCode = g.Key.SalesmanCode,
                                  ItemName = g.Key.ItemName,
                                  CounterCode = g.Key.CounterCode
                              }).ToList();

                if (result.Count <= 0) {
                    return "";
                }
                return GenerateStockTakingReport(result, companyCode, branchCode, true);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetStockTakingDetaildReport(string companyCode, string branchCode, int batchNo, out ErrorVM error)
        {
            error = null;
            try {
                var query = db.KTTU_STOCK_TAKING
                             .Where(s => s.batch_no == batchNo && s.company_code == companyCode && s.branch_code == branchCode)
                             .Select(c => new StockTakingReport
                             {
                                 BatchNo = c.batch_no,
                                 BranchCode = c.branch_code,
                                 CompanyCode = c.company_code,
                                 CounterCode = c.counter_code,
                                 SalesmanCode = c.sal_code,
                                 Qty = c.units,
                                 UpdateOn = c.UpdateOn,
                                 BarcodeNo = c.barcode_no,
                                 Dcts = c.D_cts,
                                 GrossWt = c.gwt,
                                 ItemName = c.item_name,
                                 NetWt = c.nwt
                             }).OrderByDescending(c => c.UpdateOn);

                return GenerateStockTakingReport(query.ToList(), companyCode, branchCode, false);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public string GetStockStoneDetaledReport(string companyCode, string branchCode, int batchNo, out ErrorVM error)
        {
            error = null;
            try {
                decimal caratStandardWt = Convert.ToDecimal(0.2);
                var q = (from kst in db.KTTU_STOCK_TAKING
                         join bcm in db.KTTU_BARCODE_MASTER
                         on new
                         { Company = kst.company_code, Branch = kst.branch_code, Barcode = kst.barcode_no }
                             equals new { Company = bcm.company_code, Branch = bcm.branch_code, Barcode = bcm.barcode_no }
                         join bsm in db.KTTU_BARCODE_STONE_DETAILS
                         on new { Company = kst.company_code, Branch = kst.branch_code, Barcode = kst.barcode_no }
                             equals new { Company = bsm.company_code, Branch = bsm.branch_code, Barcode = bsm.barcode_no } into barcodeStoneLeftJoin
                         from bsm in barcodeStoneLeftJoin.DefaultIfEmpty()
                         where kst.company_code == companyCode && kst.branch_code == branchCode && kst.batch_no == batchNo
                         select new StockStoneDetailsReport
                         {
                             BatchNo = kst.batch_no,
                             BranchCode = kst.branch_code,
                             CompanyCode = kst.company_code,
                             CounterCode = kst.counter_code,
                             BarcodeNo = kst.barcode_no,
                             Qty = kst.units,
                             GrossWt = kst.gwt,
                             NetWt = kst.nwt,
                             Swt = bcm.swt,
                             BarcodeDate = bcm.EntryDate,
                             ItemName = kst.item_name,
                             SalCode = kst.sal_code,
                             UpdateOn = kst.UpdateOn,
                             StType = bsm.type,
                             Carat = bsm.carrat,
                             Weight = bsm.carrat * caratStandardWt,
                         });
                return GenerateStockStoneDetailsReport(q.OrderBy(x => new { x.ItemName, x.UpdateOn }).ToList(), companyCode, branchCode);
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return "";
            }
        }

        public List<DocumentInfoVM> GetStockTakingNos(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var query = db.KTTU_STOCK_TAKING.Where(x => x.company_code == companyCode && x.branch_code == branchCode)
                        .Select(d => new { BatchNo = d.batch_no, Counter = d.counter_code, Salesman = d.sal_code })
                        .Distinct().OrderByDescending(o => o.BatchNo).ToList();
                if (query == null || query.Count <= 0) {
                    error = new ErrorVM { description = "No details found." };
                    return null;
                }
                return query.Select(x => new DocumentInfoVM
                {
                    No = Convert.ToInt32(x.BatchNo),
                    Name = x.BatchNo.ToString() + "| Counter: " + x.Counter + "| Salesman: " + x.Salesman
                }).ToList();

            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }

        }
        #endregion

        #region Protected Methods
        protected string GenerateStockTakingReport(List<StockTakingReport> result, string companyCode, string branchCode, bool isSummary)
        {
            StringBuilder sb = new StringBuilder();
            int tempWidth = 80;
            StringBuilder strLine = new StringBuilder();
            strLine.Append('-', tempWidth);
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', tempWidth);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((tempWidth - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));

            string esc = Convert.ToChar(27).ToString();
            sb.AppendLine();
            sb.Append(esc + Convert.ToChar(64).ToString());
            sb.Append(esc + Convert.ToChar(15).ToString());
            DateTime date = SIGlobals.Globals.GetDateTime();
            sb.AppendLine(string.Format("{0,-20}{1,-60}", "Stock Taking Report on : ", date.ToString("dd/MM/yyyy")));
            sb.AppendLine();

            string salesMan = string.Empty;
            string salCode = result[0].SalesmanCode;
            if (result.Count > 0) {
                salesMan = db.KSTU_SALESMAN_MASTER.Where(sal => sal.company_code == companyCode && sal.branch_code == branchCode && sal.sal_code == salCode).FirstOrDefault().sal_name;
                sb.AppendLine(string.Format("Batch No: {0,-25}{1,45}", result[0].BatchNo, "Staff :" + salesMan));
            }

            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            if (isSummary)
                sb.AppendLine(string.Format("{0,-5}{1,-15}{2,-10}{3,5}{4,15}{5,15}{6,15}", "SNo", "Location", "Item", "Qty", "Gr.wt", "N.wt", "D.Cts"));
            else
                sb.AppendLine(string.Format("{0,-5}{1,-15}{2,-10}{3,10}{4,10}{5,10}{6,10}{7,10}", "SNo", "Location", "Item", "Barcode", "Qty", "Gr.wt", "N.wt", "D.Cts"));

            sb.Append(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine();
            object objSumUnits = result.Sum(r => r.Qty);
            object objSumGwt = result.Sum(r => r.GrossWt);
            object objSumNwt = result.Sum(r => r.NetWt);
            object objSumD_cts = result.Sum(r => r.Dcts);

            if (!isSummary) {
                for (int i = 0; i < result.Count; i++) {
                    sb.Append(string.Format("{0,-5}", i + 1));
                    sb.Append(string.Format("{0,-15}", result[i].CounterCode.ToString()));
                    sb.Append(string.Format("{0,-10}", result[i].ItemName.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].BarcodeNo.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].Qty.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].GrossWt.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].NetWt.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].Dcts.ToString()));
                    sb.AppendLine();
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-40}{1,10}{2,10}{3,10}{4,10}", "Total", objSumUnits, objSumGwt, objSumNwt, objSumD_cts));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            }
            else {
                List<string> items = result.Select(item => item.ItemName).Distinct().ToList();
                string location = result[0].CounterCode;
                string sublocation = result[0].CounterCode;
                for (int i = 0; i < items.Count; i++) {
                    string item = string.Format("{0}", items[i].ToString());
                    object tempTotal = result.Where(it => it.ItemName == item).Sum(it => it.Qty);
                    int Units = Convert.ToInt32(tempTotal);
                    tempTotal = result.Where(it => it.ItemName == item).Sum(it => it.GrossWt);
                    decimal Gwt = Convert.ToDecimal(tempTotal);
                    tempTotal = result.Where(it => it.ItemName == item).Sum(it => it.NetWt);
                    decimal nwt = Convert.ToDecimal(tempTotal);
                    tempTotal = result.Where(it => it.ItemName == item).Sum(it => it.Dcts);
                    decimal D_cts = Convert.ToDecimal(tempTotal);
                    sb.AppendLine(string.Format("{0,-5}{1,-15}{2,-10}{3,5}{4,15}{5,15}{6,15}", i + 1, location, item, Units, Gwt, nwt, D_cts));
                }
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                sb.AppendLine(string.Format("{0,-25}{1,10}{2,15}{3,15}{4,15}", "Total", objSumUnits, objSumGwt, objSumNwt, objSumD_cts));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            }
            sb.Append(esc + Convert.ToChar(64).ToString());
            return sb.ToString();
        }

        protected string GenerateStockStoneDetailsReport(List<StockStoneDetailsReport> result, string companyCode, string branchCode)
        {
            StringBuilder sb = new StringBuilder();
            int tempWidth = 94;// 104;
            StringBuilder strLine = new StringBuilder();
            strLine.Append('-', tempWidth);
            StringBuilder strTransLine = new StringBuilder();
            strTransLine.Append('-', tempWidth);
            decimal Spaces = 0M;
            StringBuilder strSpaces = new StringBuilder();
            Spaces = ((tempWidth - strTransLine.Length) / 3);
            strSpaces.Append(' ', Convert.ToInt32(Spaces));

            string esc = Convert.ToChar(27).ToString();
            sb.Append(esc + Convert.ToChar(64).ToString());
            sb.Append(esc + Convert.ToChar(15).ToString());

            sb.Append(GetHeader(tempWidth, companyCode, branchCode));
            sb.AppendLine();
            sb.Append(esc + Convert.ToChar(77).ToString());

            DateTime date = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            sb.AppendLine(string.Format("{0,-20}{1,-60}", "Stock Taking Report on : ", date.ToString("dd/MM/yyyy")));
            if (result[0].BatchNo > 0)
                sb.AppendLine(string.Format("Batch No: {0} SM: {1}", result[0].BatchNo, result[0].SalCode));

            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine(string.Format("{0,-6}{1,-5}{2,-10}{3,6}{4,10}{5,10}{6,10}{7,12}{8,4}{9,8}{10,10}", "SNo", "Cnt", "Barcode", "Qty", "Gwt", "Swt", "Nwt", "Bar.Date  ", "St.Type", " Carat", "Weight"));
            sb.Append(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine();

            decimal objSumUnits = 0, objSumGwt = 0, objSumNwt = 0, objSumSwt = 0;
            decimal sumStnCarat = 0, sumDmdCarat = 0;
            for (int i = 0; i < result.Count; i++) {
                if (i == 0 || !result[i].ItemName.ToString().Equals(result[i - 1].ItemName.ToString())) {
                    if (i > 0) sb.AppendLine();
                    sb.AppendLine(string.Format("{0,-10}", result[i].ItemName.ToString() + " :"));
                    sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
                }
                if (i == 0 || !result[i].BarcodeNo.ToString().ToString().Equals(result[i - 1].ItemName.ToString().ToString())) {
                    sb.Append(string.Format("{0,-6}", i + 1));
                    sb.Append(string.Format("{0,-5}", result[i].CounterCode.ToString()));
                    sb.Append(string.Format("{0,-10}", result[i].BarcodeNo.ToString()));
                    sb.Append(string.Format("{0,6}", result[i].Qty.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].GrossWt.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].Swt.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].NetWt.ToString()));
                    sb.Append(string.Format("{0,12}", Convert.ToDateTime(result[i].BarcodeDate).ToString("dd/MM/yyy")));

                    objSumUnits += Convert.ToDecimal(result[i].Qty);
                    objSumGwt += Convert.ToDecimal(result[i].GrossWt);
                    objSumNwt += Convert.ToDecimal(result[i].NetWt);
                    objSumSwt += Convert.ToDecimal(result[i].Swt);
                }
                else {
                    sb.Append(string.Format("{0,69}", ""));
                }
                if (string.IsNullOrEmpty(result[i].StType)) {
                    sb.Append(string.Format("{0,35}", ""));
                }
                else {
                    if (result[i].StType.ToString().Trim().ToUpper().Equals("S"))
                        sumStnCarat += Convert.ToDecimal(result[i].Carat.ToString());
                    else
                        sumDmdCarat += Convert.ToDecimal(result[i].Carat.ToString());

                    sb.Append(string.Format("{0,4}", result[i].StType.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].Carat.ToString()));
                    sb.Append(string.Format("{0,10}", result[i].Weight.ToString()));
                }
                sb.AppendLine();
            }
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            sb.AppendLine(string.Format("{0,-15}{1,11} {2,10} {3,10} {4,10}{5,10}{6,12}{7,14}", "Total", objSumUnits, objSumGwt, objSumSwt, objSumNwt, "", "", ""));
            sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));

            if (sumStnCarat >= 0 || sumDmdCarat > 0) {
                sb.AppendLine(string.Format("{0,-15}{1,11}{2,5}{3,15}{4,-11}", "Total Stone carat  :", sumStnCarat, "", "Weight:", Math.Round(sumStnCarat / 5, 3)));
                sb.AppendLine(string.Format("{0,-15}{1,11}{2,5}{3,15}{4,-11}", "Total Diamond carat:", sumDmdCarat, "", "Weight:", Math.Round(sumDmdCarat / 5, 3)));
                sb.AppendLine(string.Format("{0}{1}", strSpaces, strTransLine));
            }
            sb.AppendLine(Convert.ToChar(12).ToString());
            sb.Append(esc + Convert.ToChar(64).ToString());
            return sb.ToString();
        }

        private StringBuilder GetHeader(int maxWidth, string companyCode, string branchCode)
        {
            string tempString = string.Empty;
            StringBuilder sb = new StringBuilder();
            string esc = Convert.ToChar(27).ToString();
            sb.Append(Convert.ToChar(18).ToString());
            sb.Append(esc + Convert.ToChar(77).ToString());
            sb.Append(esc + Convert.ToChar(120).ToString() + Convert.ToChar(48).ToString());
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(com => com.company_code == companyCode && com.branch_code == branchCode).FirstOrDefault();
            if (company != null) {
                sb.Append(esc + Convert.ToChar(69).ToString());
                sb.Append(esc + Convert.ToChar(14).ToString() + Convert.ToChar(14).ToString());
                sb.Append(esc + Convert.ToChar(52).ToString());
                sb.Append(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.company_name.ToString()));
                sb.Append(esc + Convert.ToChar(53).ToString());
                sb.Append(Convert.ToChar(20).ToString());
                sb.Append(esc + Convert.ToChar(70).ToString());
                sb.AppendLine();
                if (!string.IsNullOrEmpty(company.address1.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address1.ToString()));
                if (!string.IsNullOrEmpty(company.address2.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address2.ToString()));
                if (!string.IsNullOrEmpty(company.address3.ToString()))
                    sb.AppendLine(string.Format("{0}{1}", esc + Convert.ToChar(97).ToString() + Convert.ToChar(1).ToString(), company.address3.ToString()));
            }
            return sb;
        }
        #endregion
    }

    #region Report Related Classes
    public class StockTakingReport
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int? BatchNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal? Dcts { get; set; }
        public string SalesmanCode { get; set; }
        public string ItemName { get; set; }

        public string CounterCode { get; set; }

        public DateTime? UpdateOn { get; set; }
        public string BarcodeNo { get; set; }
    }

    public class StockStoneDetailsReport
    {
        public int? BatchNo { get; set; }
        public string BranchCode { get; set; }
        public string CompanyCode { get; set; }
        public string CounterCode { get; set; }
        public string BarcodeNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal? Swt { get; set; }
        public DateTime? BarcodeDate { get; set; }
        public string ItemName { get; set; }
        public string SalCode { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string StType { get; set; }
        public decimal? Carat { get; set; }
        public decimal? Weight { get; set; }
    }
    #endregion
}
