using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.PrintConfig
{
    public class PrintConfiguration
    {

        public string GetPrintConfigurationForDraftDocuments(string companyCode, string branchCode, string documentType)
        {
            string printType = "HTML";
            string configuredPrintType = "RAW";
            switch (documentType) {
                case "SAL_EST":
                case "PUR_EST":
                case "SRT_EST":
                    using (MagnaDbEntities db = new MagnaDbEntities(true)) {
                        var printCnfg = db.KSTU_COMPANY_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode)
                            .Select(y => y.cst_no).FirstOrDefault();
                        if (string.IsNullOrEmpty(printCnfg))
                            configuredPrintType = "RAW";
                        else
                            configuredPrintType = "HTML";
                    }
                    printType = configuredPrintType;
                    break;
                case "ORD_FRM":
                case "ORD_REC":
                case "ORD_CLS":
                case "REP_REC":
                case "REP_ISS":
                case "CRD_REC":
                case "SRT_INV":
                case "PUR_INV":
                case "SAL_INV":
                    printType = "HTML";
                    break;
                default:
                    printType = "HTML";
                    break;
            }

            return printType;
        }        

        public string Base64Encode(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return string.Empty;
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public string Base64Decode(string base64EncodedData)
        {
            string decodedString = string.Empty;
            try {
                var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
                decodedString = Encoding.UTF8.GetString(base64EncodedBytes);
            }
            catch (Exception) {
            }
            return decodedString;
        }
    }
}
