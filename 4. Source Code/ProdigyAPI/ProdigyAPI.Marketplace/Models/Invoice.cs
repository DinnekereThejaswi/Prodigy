using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class Invoice
    {
        public InvoiceInfo invoice { get; set; }
        public InvoiceInfo fileData { get; set; }
    }

    public class InvoiceInfo
    {
        public string format { get; set; } //Enum: ZPL, PDF
        public EncryptedContent encryptedContent { get; set; }
    }
    

    public class EncryptedContent
    {
        public string value { get; set; }
        public EncryptionInfo encryptionInfo { get; set; }
    }
}
