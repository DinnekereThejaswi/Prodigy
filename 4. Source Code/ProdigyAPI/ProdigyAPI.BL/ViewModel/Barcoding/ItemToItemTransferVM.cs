using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Barcoding
{
    public class ItemToItemTransferVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string TransferToGSCode { get; set; }
        public string TransferToItemCode { get; set; }
        public string Remarks { get; set; }
        public List<TransferBarcodeLine> BarcodeLines { get; set; }
    }
}
