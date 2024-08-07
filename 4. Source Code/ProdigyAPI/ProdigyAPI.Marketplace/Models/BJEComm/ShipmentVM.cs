using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.Marketplace.Models.BJEComm
{
    public class ShipmentVM
    {
        public string OrderNo { get; set; }
        public bool Error { get; set; }
        public string StatusInfo { get; set; }
        public string CCRCrdRefNo { get; set; }
        public string AWBNo { get; set; }
        public string AWBPdfPrintContent { get; set; }
        public bool ErrorInPU { get; set; }
        public string PickUpTokenNo { get; set; }
        public List<BluedartStatusInfoVM> StatusInformation { get; set; }
    }
    public class BluedartStatusInfoVM
    {
        private string StatusCode;
        private string StatusMessage;
        //public string StatusCode { get; set; }
        //public string StatusMessage { get; set; }
        public BluedartStatusInfoVM(string _statusCode, string _statusMessage)
        {
            StatusCode = _statusCode;
            StatusMessage = _statusMessage;
        }
        public override string ToString()
        {
            return string.Format($"Status Code: {StatusCode} | Information: {StatusMessage} {Environment.NewLine}");
        }
    }

    public class PickupRegistrationInputVM
    {
        public string CompanyCode { get; set; }
        public string BranchCode { get; set; }
        public int OrderNo { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonMobileNo { get; set; }
        public DateTime PickupDate { get; set; }
    }

}
