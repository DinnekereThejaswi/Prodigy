using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Orders
{
    public class OrderCPCIssuesVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string GSCode { get; set; }
        public string Karat { get; set; }
        public string Counter { get; set; }
        public string OrderType { get; set; }
        public string IssuedUser { get; set; }
        public List<OrderCPCIssueLineVM> issueLines { get; set; }
    }
    public class OrderCPCIssueLineVM
    {
        public int OrderNo { get; set; }
        public string GSCode { get; set; }
        public string Karat { get; set; }
        public decimal Rate { get; set; }
        public string Salesman { get; set; }
        public string Counter { get; set; }
        public int SubOrderNo { get; set; }
        public string Item { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public int Qty { get; set; }
        public decimal FromWeight { get; set; }
        public decimal ToWeight { get; set; }
        public decimal StoneWeight { get; set; }
        public decimal DiaCarets { get; set; }
        public decimal NetWeight { get; set; }
        public string Remarks { get; set; }
        public DateTime OrderDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal AdvanceAmount { get; set; }
        public string CustomerName { get; set; }
        public string MobileNo { get; set; }
    }
}
