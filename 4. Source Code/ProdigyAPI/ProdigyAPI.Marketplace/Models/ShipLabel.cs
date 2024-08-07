using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class ShipLabel
    {
        public ShipLabelMetadata shipLabelMetadata { get; set; }
        public InvoiceInfo shipLabel { get; set; }
        public InvoiceInfo fileData { get; set; }
    }

    public class ShipLabelMetadata
    {
        public string carrierName { get; set; }
        public string trackingId { get; set; }
    }
}
