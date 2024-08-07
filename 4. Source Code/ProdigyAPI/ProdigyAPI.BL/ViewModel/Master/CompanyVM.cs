using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class CompanyVM
    {
        public string ObjID { get; set; }
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public string CompanyName { get; set; }
        public string ShortName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Address3 { get; set; }
        public string city { get; set; }
        public string State { get; set; }
        public string PhoneNo { get; set; }
        public string FAX { get; set; }
        public string EMailID { get; set; }
        public string MobileNo { get; set; }
        public string PAN { get; set; }
        public string TinNo { get; set; }
        public string CSTNo { get; set; }
        public string Website { get; set; }
        public string CompanyFooter { get; set; }
        public Nullable<int> DisplayNo { get; set; }
        public string ObjectStatus { get; set; }
        public string BranchName { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string HOCODE { get; set; }
        public string EDRegNo { get; set; }
        public string Header1 { get; set; }
        public string Header2 { get; set; }
        public string Header3 { get; set; }
        public string Header4 { get; set; }
        public string Header5 { get; set; }
        public string Header6 { get; set; }
        public string Header7 { get; set; }
        public string Footer1 { get; set; }
        public string Footer2 { get; set; }
        public string DefaultCurrencyCode { get; set; }
        public Nullable<int> StateCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryName { get; set; }


    }

    public class FooterVM
    {
        public string CompanyName { get; set; }
        public string Place { get; set; }
        public decimal Version { get; set; }
        public decimal GoldRate { get; set; }
        public decimal SilverRate { get; set; }
        public string Operator { get; set; }
        public DateTime ApplicationDate { get; set; }
    }

    public class Dashboard
    {
        public DashboardLineItem Sales { get; set; }
        public DashboardLineItem Purchase { set; get; }
        public DashboardLineItem Orders { set; get; }
        public DashboardLineItem Repair { set; get; }
        public DashboardLineItem RepairDelivery { set; get; }
    }

    public class DashboardLineItem
    {
        public decimal? NetWt { get; set; }
        public decimal? StoneWt { get; set; }
        public decimal? Amount { get; set; }
    }
}

