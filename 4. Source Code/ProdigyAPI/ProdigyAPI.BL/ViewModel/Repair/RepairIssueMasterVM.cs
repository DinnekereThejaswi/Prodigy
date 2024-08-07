﻿using ProdigyAPI.BL.ViewModel.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Repair
{
    public class RepairIssueMasterVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int IssueNo { get; set; }
        public int ReceiptNo { get; set; }
        public Nullable<System.DateTime> IssueDate { get; set; }
        public string SalCode { get; set; }
        public string OperatorCode { get; set; }
        public decimal TotalRepairAmount { get; set; }
        public string CFlag { get; set; }
        public string ItemType { get; set; }
        public string Remark { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<decimal> TaxPercentage { get; set; }
        public Nullable<decimal> TotalTaxAmount { get; set; }
        public Nullable<decimal> TotalAmountBeforeTax { get; set; }
        public Nullable<int> ShiftID { get; set; }
        public string NewBillNo { get; set; }
        public List<RepairIssueDetailsVM> lstOfRepairIssueDetails { get; set; }
        public List<PaymentVM> lstOfPayment { get; set; }


        //public string ObjID { get; set; }
        //public string CompanyCode { get; set; }
        //public string BranchCode { get; set; }
        public int RepairNo { get; set; }
        public int CustID { get; set; }
        public string CustName { get; set; }
        public Nullable<System.DateTime> RepairDate { get; set; }
        //public string SalCode { get; set; }
        //public string OperatorCode { get; set; }
        public System.DateTime DueDate { get; set; }
        public decimal TagWt { get; set; }
        //public string CFlag { get; set; }
        //public Nullable<int> IssueNo { get; set; }
        public string RepairItems { get; set; }
        public string CancelledBy { get; set; }
        public string Remarks { get; set; }
        public Nullable<System.DateTime> CustDueDate { get; set; }
        //public Nullable<System.DateTime> UpdateOn { get; set; }
        //public Nullable<int> ShiftID { get; set; }
        //public string NewBillNo { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string City { get; set; }
        public string PinCode { get; set; }
        public string MobileNo { get; set; }
        public string State { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string TIN { get; set; }
        public string PANNo { get; set; }
    }
}
