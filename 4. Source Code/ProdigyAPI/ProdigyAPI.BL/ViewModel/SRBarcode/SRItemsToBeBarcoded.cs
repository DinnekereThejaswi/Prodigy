using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.SRBarcode
{
    public class SRItemToBeBarcodedVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int SalesBillNo { get; set; }
        public int SlNo { get; set; }
        public string BarcodeNo { get; set; }
        public string GSCode { get; set; }
        public string CounterCode { get; set; }
        public string ItemCode { get; set; }
        public Nullable<decimal> GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public Nullable<decimal> NetWt { get; set; }
    }

}
