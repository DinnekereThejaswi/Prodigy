using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.OldPurchase
{
    public class PurchaseEstStoneDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int BillNO { get; set; }
        public int SlNo { get; set; }
        public int EstNo { get; set; }
        public int ItemSlNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int Qty { get; set; }
        public decimal Carrat { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public int PurchaseDealerBillNo { get; set; }
        public string GSCode { get; set; }
        public Nullable<int> PurReturnNo { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> FinYear { get; set; }
    }
}
