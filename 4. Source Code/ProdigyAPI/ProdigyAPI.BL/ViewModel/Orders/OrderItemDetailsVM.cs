using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class OrderItemDetailsVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public int SlNo { get; set; }
        public string ItemName { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public decimal FromGrossWt { get; set; }
        public decimal ToGrossWt { get; set; }
        public decimal MCPer { get; set; }
        public decimal WastagePercent { get; set; }
        public decimal Amount { get; set; }
        public string SFlag { get; set; }
        public string SParty { get; set; }
        public int BillNo { get; set; }
        public string ImgID { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public decimal VAPercent { get; set; }
        public decimal ItemAmount { get; set; }
        public string GSCode { get; set; }
        public Nullable<int> FinYear { get; set; }
        public Nullable<decimal> ItemwiseTaxPercentage { get; set; }
        public Nullable<decimal> ItemwiseTaxAmount { get; set; }
        public Nullable<decimal> MCPerPiece { get; set; }
        public string ProductID { get; set; }
        public string IsIssued { get; set; }
        public string CounterCode { get; set; }
        public string SalCode { get; set; }
        public Nullable<decimal> MCPercent { get; set; }
        public Nullable<int> MCType { get; set; }
        public Nullable<int> EstNo { get; set; }
        public string ItemType { get; set; }
        public Nullable<decimal> AppSwt { get; set; }
    }
}
