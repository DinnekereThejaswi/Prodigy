using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Print
{
    public class ProdigyPrintVM
    {
        public string PrintType { get; set; }
        public string Data { get; set; }
        public string PrinterName { get; set; }
        public bool ContinueNextPrint { get; set; }
    }
}
