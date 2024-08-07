using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class DailyRateVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public DateTime Date { get; set; }
        public List<SellingRate> SellingRates { get; set; }
        public List<PurchaseRate> PurchaseRates { get; set; }
    }

    public class SellingRate
    {
        public string GsCode { get; set; }
        public string GSName { get; set; }
        public string Karat { get; set; }
        public decimal SellingBoardRate { get; set; }

    }

    public class PurchaseRate
    {
        public string GsCode { get; set; }
        public string GSName { get; set; }
        public string Karat { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal CashRate { get; set; }

    }
}
