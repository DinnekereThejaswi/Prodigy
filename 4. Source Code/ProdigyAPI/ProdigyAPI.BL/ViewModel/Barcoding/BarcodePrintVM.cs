using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Barcoding
{
    public class BarcodePrintVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string[] Barcodes { get; set; }
        public string PrintType { get; set; }
    }
}
