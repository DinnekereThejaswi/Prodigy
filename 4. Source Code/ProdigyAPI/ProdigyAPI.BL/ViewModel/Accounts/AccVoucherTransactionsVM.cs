using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Accounts
{
    public class AccVoucherTransactionsVM
    {
        public string ObjID { get; set; }
        [Required]
        [StringLength(5, MinimumLength = 2, ErrorMessage = "Company Code Must be at least 2 characters long.")]
        public string CompanyCode { get; set; }

        [Required]
        [StringLength(5, MinimumLength = 3, ErrorMessage = "Branch Code must be Must be at least 3 characters long.")]
        public string BranchCode { get; set; }

        public int TxtSeqNo { get; set; }
        public int VoucherNo { get; set; }
        public int VoucherSeqNo { get; set; }
        public string AccountType { get; set; }
        public DateTime VoucherDate { get; set; }

        [Required]
        public int AccountCode { get; set; }

        [Required]
        public decimal DebitAmount { get; set; }

        [Required]
        public decimal CreditAmount { get; set; }

        public string ChequeNo { get; set; }
        public DateTime ChequeDate { get; set; }
        public int FinalYear { get; set; }
        public int FinalPeriod { get; set; }

        [Required]
        public int AccountCodeMaster { get; set; }
        public string Narration { get; set; }
        public int ContraSeqNo { get; set; }
        public string ReconsileFlag { get; set; }
        public DateTime ReconsileDate { get; set; }
        public string ReceiptNo { get; set; }

        public string TransType { get; set; }
        public string ApprovedBy { get; set; }
        public string CancelledRemarks { get; set; }
        public string CancelledBy { get; set; }
        public string Partyname { get; set; }
        public string VoucherType { get; set; }
        public string ReconsileBy { get; set; }
        public string CurrencyType { get; set; }
        public int NewVoucherNo { get; set; }
        public string SectionID { get; set; }
        public string VerifiedBy { get; set; }
        public string VerifiedRemarks { get; set; }
        public string AuthorizedBy { get; set; }
        public string AuthorizedRemarks { get; set; }
        public decimal? ExchangeRate { get; set; }
        public decimal? CurrencyValue { get; set; }
        public string Cflag { get; set; }
        public string IsApproved { get; set; }
        public int? SubledgerAccCode { get; set; }
    }

    public class VoucherTypesVM
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
