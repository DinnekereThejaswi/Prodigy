using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models
{
    public class Packaging
    {
        public List<Package> packages { get; set; }
    }
    public class Package
    {
        public string id { get; set; }
        public PackageDimension length { get; set; }
        public PackageDimension width { get; set; }
        public PackageDimension height { get; set; }
        public PackageWeight weight { get; set; }
        public List<string> hazmatLabels { get; set; }
        public List<PackagedLineItem> packagedLineItems { get; set; }
    }

    public class PackageDimension
    {
        public decimal value { get; set; }
        public string dimensionUnit { get; set; } //Enum: CM, M
    }

    public class PackageWeight
    {
        public decimal value { get; set; }
        public string weightUnit { get; set; } //Enum: grams, kilograms
    }

    public class PackagedLineItem
    {
        public OrderLineItem lineItem { get; set; }
        public int quantity { get; set; }
        public List<SerialNumber> serialNumbers { get; set; }
    }
    public class OrderLineItem
    {
        public string id { get; set; }
    }

    public class SerialNumber
    {
        public string value { get; set; }
        public List<EncryptionInfo> encryptionInfo { get; set; }
    }

    public partial class EncryptionInfo
    {
        public string type { get; set; } //Enum: AWS_KMS
        public string context { get; set; } //Enum: giftMessage, invoice, serialNumber, shipLabel, shipToAddress
    }


}
