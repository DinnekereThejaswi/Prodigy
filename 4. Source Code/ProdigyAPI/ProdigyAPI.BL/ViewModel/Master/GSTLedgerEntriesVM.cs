using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.ViewModel.Master
{
    public class GSTLedgerEntriesVM
    {
        public int EntryNo { get; set; }
        public string DocumentNo { get; set; }
        public int DocumentLineNo { get; set; }
        public DateTime PostingDate { get; set; }
        public int DocumentType { get; set; }
        public int TransactionType { get; set; }
        public string SourceType { get; set; }
        public string SourceNo { get; set; }
        public string ExternalDocNo { get; set; }
        public int? GSTGroupType { get; set; }
        public string GSTComponentCode { get; set; }
        public string HSNSACCode { get; set; }
        public decimal GSTBaseAmount { get; set; }
        public decimal GSTPercent { get; set; }
        public decimal GSTAmount { get; set; }
        public string ItemCode { get; set; }
        public int Qty { get; set; }
        public decimal GrossWeight { get; set; }
        public string IsGSTOnAdvanceAmt { get; set; }
        public string IsReversed { get; set; }
        public string ReversedEntryNo { get; set; }
        public string ReversedByEntryNo { get; set; }
        public string IstInterstate { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public string LastModifiedBy { get; set; }

    }
}
