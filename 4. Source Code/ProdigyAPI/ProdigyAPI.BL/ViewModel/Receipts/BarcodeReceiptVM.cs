using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Receipts
{
    public class BarcodeReceiptVM
    {
        public string CompanyCode{get; set; }
        public string BranchCode{get; set; }
        public string IssueBranchCode{get; set; }
        public int IssueNo{get; set; }
        public DateTime IssueDate { get; set; }
        public int TotalLineCount{get; set; }
        public int TotalQty{get; set; }
        public decimal TotalGrossWt{get; set; }
        public decimal TotalStoneWt{get; set; }
        public decimal TotalNetWt{get; set; }
        public decimal TotalDcts{get; set; }
        public decimal TotalAmount{get; set; }
        public List<BarcodeReceiptDetailVM> ReceiptDetails { get; set; }
        public List<BarcodeReceiptDetailVM> ScannedBarcodes { get; set; }
    }

    public class BarcodeReceiptDetailVM
    {
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public string BarcodeNo { get; set; }
        public string CounterCode { get; set; }
        public int SlNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
        #region Used in Non-Tag Receipts
        public decimal PurityPercent { get; set; }
        public decimal PureWeight { get; set; }
        public decimal Rate { get; set; } 
        #endregion
        public decimal Amount { get; set; }
        public List<BarcodeReceiptStoneDetailVM> BarcodeReceiptStoneDetails { get; set; }
    }

    public class BarcodeReceiptStoneDetailVM
    {
        public string BarcodeNo { get; set; }
        public int SlNo { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public int? Qty { get; set; }
        public decimal? Carrat { get; set; }
        public decimal? Rate { get; set; }
        public decimal? Amount { get; set; }
    }
    public class ScannedBarcodeVM
    {
        public string GSCode { get; set; }
        public string ItemCode { get; set; }
        public string BarcodeNo { get; set; }
        public string CounterCode { get; set; }
        public int SlNo { get; set; }
        public int Qty { get; set; }
        public decimal GrossWt { get; set; }
        public decimal StoneWt { get; set; }
        public decimal NetWt { get; set; }
        public decimal Dcts { get; set; }
        public decimal Amount { get; set; }
    }
    

    #region Barcode Receipt Output from API
    public class BarcodeIssueHeaderOutputVM
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public string issue_type { get; set; }
        public DateTime issue_date { get; set; }
        public string issue_to { get; set; }
        public string sal_code { get; set; }
        public string gs_type { get; set; }
        public string operator_code { get; set; }
        public string obj_status { get; set; }
        public string cflag { get; set; }
        public string cancelled_by { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string is_approved { get; set; }
        public string approved_by { get; set; }
        public string approver_name { get; set; }
        public DateTime? approved_date { get; set; }
        public decimal? total_amount { get; set; }
        public decimal? tolerance_percent { get; set; }
        public decimal? final_amount { get; set; }
        public string IsConfirmed { get; set; }
        public string remarks { get; set; }
        public int? ShiftID { get; set; }
        public int New_Bill_No { get; set; }
        public decimal bmu_charges { get; set; }
        public decimal total_wastage { get; set; }
        public decimal? hallmark_charges { get; set; }
        public string Transfer_Type { get; set; }
        public decimal? SGST_Percent { get; set; }
        public decimal? IGST_Percent { get; set; }
        public decimal? CGST_Percent { get; set; }
        public decimal? IGST_Amount { get; set; }
        public decimal? CGST_Amount { get; set; }
        public decimal? SGST_Amount { get; set; }
        public string GSTGroupCode { get; set; }
        public string HSN { get; set; }
        public decimal? total_mc { get; set; }
        public decimal? TDSPerc { get; set; }
        public decimal? TDS_Amount { get; set; }
        public decimal? Net_Amount { get; set; }
        public decimal? stone_charges { get; set; }
        public decimal? diamond_charges { get; set; }
        public decimal? grand_total { get; set; }
        public string material_type { get; set; }
        public decimal? other_charges { get; set; }
        public Guid UniqRowID { get; set; }
        public decimal? round_off { get; set; }
        public string isReservedIssue { get; set; }
        public string Cancelled_Remarks { get; set; }
        public int? store_location_id { get; set; }
        public List<BarcodeIssueLineVM> BarcodeIssueLines { get; set; }
        public List<BarcodeDetailVM> BarcodeDetails { get; set; }
        public List<BarcodeImageUrlVM> BarcodeImageUrls { get; set; }
        public List<BarcodeStoneDetailVM> BarcodeStoneDetails { get; set; }
        public List<OnlineInventoryDetailVM> OnlineInventoryDetails { get; set; }
        public List<BarcodeIssueLineStoneVM> BarcodeIssueLineStones { get; set; }
    }

    public class BarcodeIssueLineVM
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public int sno { get; set; }
        public string barcode_no { get; set; }
        public string item_name { get; set; }
        public int quantity { get; set; }
        public decimal gwt { get; set; }
        public decimal swt { get; set; }
        public decimal nwt { get; set; }
        public string counter_code { get; set; }
        public string cflag { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string gs_code { get; set; }
        public decimal? amount { get; set; }
        public decimal? rate { get; set; }
        public decimal? dcts { get; set; }
        public string batch_id { get; set; }
        public string supplier_code { get; set; }
        public int? Fin_Year { get; set; }
        public int? Batch_no { get; set; }
        public string BReceiptNo { get; set; }
        public int? pur_mc_type { get; set; }
        public decimal? pur_mc_rate { get; set; }
        public decimal? pur_rate { get; set; }
        public DateTime? barcode_date { get; set; }
        public int? BSno { get; set; }
        public string design_code { get; set; }
        public decimal? purity_perc { get; set; }
        public decimal? pure_wt { get; set; }
        public decimal baud_rate { get; set; }
        public decimal pur_wastage_perc { get; set; }
        public decimal pur_wastage_wt { get; set; }
        public decimal? pur_making_charges { get; set; }
        public decimal? CGST_Amount { get; set; }
        public decimal? CGST_Percent { get; set; }
        public decimal? IGST_Amount { get; set; }
        public decimal? IGST_Percent { get; set; }
        public decimal? SGST_Amount { get; set; }
        public decimal? SGST_Percent { get; set; }
        public decimal? stone_charges { get; set; }
        public decimal? diamond_charges { get; set; }
        public decimal? hallmark_charges { get; set; }
        public Guid UniqRowID { get; set; }
        public int? ReservedEstNo { get; set; }
        public string Reserved_Salcode { get; set; }
        public decimal? Reserved_VA { get; set; }
        public int? bin_no { get; set; }
        public decimal? tolerance_amount { get; set; }
    }

    public class BarcodeDetailVM
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string barcode_no { get; set; }
        public int? batch_no { get; set; }
        public string sal_code { get; set; }
        public string operator_code { get; set; }
        public DateTime? date { get; set; }
        public string counter_code { get; set; }
        public string gs_code { get; set; }
        public string item_name { get; set; }
        public decimal? gwt { get; set; }
        public decimal? swt { get; set; }
        public decimal? nwt { get; set; }
        public string grade { get; set; }
        public string catalog_id { get; set; }
        public decimal? making_charge_per_rs { get; set; }
        public decimal? wast_percent { get; set; }
        public int? qty { get; set; }
        public string item_size { get; set; }
        public string design_no { get; set; }
        public decimal? piece_rate { get; set; }
        public decimal? daimond_amount { get; set; }
        public decimal? stone_amount { get; set; }
        public int? order_no { get; set; }
        public string sold_flag { get; set; }
        public string product_code { get; set; }
        public decimal? hallmark_charges { get; set; }
        public string remarks { get; set; }
        public string supplier_code { get; set; }
        public string ordered_company_code { get; set; }
        public string ordered_branch_code { get; set; }
        public string karat { get; set; }
        public decimal? mc_amount { get; set; }
        public decimal? wastage_grms { get; set; }
        public decimal? mc_percent { get; set; }
        public string mc_type { get; set; }
        public string old_barcode_no { get; set; }
        public string prod_ida { get; set; }
        public string prod_tagno { get; set; }
        public DateTime? UpdateOn { get; set; }
        public int? Lot_No { get; set; }
        public decimal? tag_wt { get; set; }
        public string isConfirmed { get; set; }
        public string confirmedBy { get; set; }
        public DateTime? confirmedDate { get; set; }
        public decimal? current_wt { get; set; }
        public string MC_For { get; set; }
        public int? diamond_no { get; set; }
        public string batch_id { get; set; }
        public decimal? add_wt { get; set; }
        public string weightRead { get; set; }
        public string confirmedweightRead { get; set; }
        public string party_name { get; set; }
        public string design_name { get; set; }
        public string item_size_name { get; set; }
        public string master_design_code { get; set; }
        public string master_design_name { get; set; }
        public string vendor_model_no { get; set; }
        public decimal? pur_mc_gram { get; set; }
        public decimal? mc_per_piece { get; set; }
        public string Tagging_Type { get; set; }
        public string BReceiptNo { get; set; }
        public int? BSNo { get; set; }
        public string Issue_To { get; set; }
        public decimal? pur_mc_amount { get; set; }
        public int? pur_mc_type { get; set; }
        public decimal? pur_rate { get; set; }
        public string sr_batch_id { get; set; }
        public decimal? total_selling_mc { get; set; }
        public decimal? pur_diamond_amount { get; set; }
        public decimal? total_purchase_mc { get; set; }
        public decimal? pur_stone_amount { get; set; }
        public decimal pur_purity_percentage { get; set; }
        public int pur_wastage_type { get; set; }
        public decimal pur_wastage_type_value { get; set; }
        public Guid UniqRowID { get; set; }
        public string certification_no { get; set; }
        public string ref_no { get; set; }
        public string receipt_type { get; set; }
        public string EntryDocType { get; set; }
        public DateTime? EntryDate { get; set; }
        public string EntryDocNo { get; set; }
        public string ExitDocType { get; set; }
        public DateTime? ExitDate { get; set; }
        public string ExitDocNo { get; set; }
        public string OnlineStock { get; set; }
        public string is_shuffled { get; set; }
        public DateTime? shuffled_date { get; set; }
        public string Collections { get; set; }
        public string isActive { get; set; }
        public string product_description { get; set; }
    }

    public class BarcodeStoneDetailVM
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int sl_no { get; set; }
        public string barcode_no { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int? qty { get; set; }
        public decimal? carrat { get; set; }
        public decimal? rate { get; set; }
        public decimal? amount { get; set; }
        public string clarity { get; set; }
        public string color { get; set; }
        public string prod_ida { get; set; }
        public string prod_tagno { get; set; }
        public string old_barcode_no { get; set; }
        public DateTime? UpdateOn { get; set; }
        public string stone_type { get; set; }
        public string stone_gs_type { get; set; }
        public int? Fin_Year { get; set; }
        public string uom { get; set; }
        public decimal? pur_cost { get; set; }
        public string stone_code { get; set; }
        public string shape { get; set; }
        public string cut { get; set; }
        public string polish { get; set; }
        public string symmetry { get; set; }
        public string fluorescence { get; set; }
        public string certificate { get; set; }
        public Guid UniqRowID { get; set; }
        public decimal? pur_rate { get; set; }
        public string Size { get; set; }
    }

    public class BarcodeImageUrlVM
    {
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string barcode_no { get; set; }
        public int sl_no { get; set; }
        public string URL { get; set; }
    }

    public partial class OnlineInventoryDetailVM
    {
        public int ID { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public string barcode_no { get; set; }
        public int portal_id { get; set; }
        public string isActive { get; set; }
        public string gs_code { get; set; }
        public string item_name { get; set; }
        public int? qty { get; set; }
        public decimal? gwt { get; set; }
        public decimal? nwt { get; set; }
    }

    public class BarcodeIssueLineStoneVM
    {
        public string obj_id { get; set; }
        public string company_code { get; set; }
        public string branch_code { get; set; }
        public int issue_no { get; set; }
        public int sno { get; set; }
        public int receipt_no { get; set; }
        public int item_sno { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public int qty { get; set; }
        public decimal carrat { get; set; }
        public decimal rate { get; set; }
        public decimal amount { get; set; }
        public string gs_code { get; set; }
        public Nullable<System.DateTime> UpdateOn { get; set; }
        public Nullable<int> Fin_Year { get; set; }
        public string IR_Type { get; set; }
        public string party_code { get; set; }
        public Nullable<decimal> swt { get; set; }
        public Nullable<decimal> stone_damage_carat { get; set; }
        public Nullable<decimal> stone_damage_Gms { get; set; }
        public Nullable<int> sub_Lot_No { get; set; }
        public System.Guid UniqRowID { get; set; }
        public string stonetype { get; set; }
        public string color { get; set; }
        public string cut { get; set; }
        public string symmetry { get; set; }
        public string fluorescence { get; set; }
        public string CertificateNo { get; set; }
        public string seiveSize { get; set; }
        public string clarity { get; set; }
        public string polish { get; set; }
        public string shape { get; set; }
    }
    #endregion
}
