using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Barcoding
{
    public class CounterToCounterTransferVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TransferToCounter { get; set; }
        public string Remarks { get; set; }
        public List<TransferBarcodeLine> BarcodeLines { get; set; }
    }

    public class TransferBarcodeLine
    {
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public string CounterCode { get; set; }
        public string BarcodeNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public int? OrderNo { get; set; }
    }
}
