using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class ListOfValue
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class GSTAttributeBreakup
    {
        public string GSTGroupCode { get; set; }
        public string HSN { get; set; }
        public decimal AmountBeforeTax { get; set; }
        public decimal SGSTPercent { get; set; }
        public decimal SGSTAmount { get; set; }
        public decimal CGSTPercent { get; set; }
        public decimal CGSTAmount { get; set; }
        public decimal IGSTPercent { get; set; }
        public decimal IGSTAmount { get; set; }
        public decimal CessPercent { get; set; }
        public decimal CessAmount { get; set; }
        public decimal AmountAfterTax
        {
            get { return (AmountBeforeTax + SGSTAmount + CGSTAmount + IGSTAmount + CessAmount); }
        }

    }
}
