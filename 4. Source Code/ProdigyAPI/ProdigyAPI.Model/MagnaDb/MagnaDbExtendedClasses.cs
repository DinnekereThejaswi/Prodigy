using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Model.MagnaDb
{
    public partial class KTTU_SALES_EST_DETAILS
    {
        public decimal LineItemContribution { get; set; }
        public decimal NewSGST_Amount { get; set; }
        public decimal NewCGST_Amount { get; set; }
        public decimal NewIGST_Amount { get; set; }
        public decimal NewPiece_Rate { get; set; }
        public decimal NewCESSAmount { get; set; }
        public decimal NewItemDiscountAmt { get; set; }
        public decimal NewItemTotalAfterDiscExclGST { get; set; }
        public decimal NewItemTotalInclGST { get; set; }

    }


    public partial class KTTU_SALES_EST_MASTER
    {
        public string RowVersionString
        {
            get { return Convert.ToBase64String(RowRevision); }
        }

        private decimal TotalLineAmount;

        public decimal TotalLineAmt
        {
            get { return GetLineAmt(est_no); }
        }

        private decimal GetLineAmt(int est_no)
        {
            return 0;
        }
    }
}
