using ProdigyAPI.BL.BusinessLayer.PrintConfig;
using ProdigyAPI.BL.ViewModel.Barcoding;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Print;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Barcoding
{
    public class BarcodePrintBL
    {
        private bool validbarcode = true;
        private bool isGoldPrint = true;
        private string prBarcode = string.Empty, prOrderNo = string.Empty, prModelNo = string.Empty, prBarcodeNoprint = string.Empty;
        private string prGwt = string.Empty, prNwt = string.Empty, prDiamondCarat = string.Empty, prOperatorCode = string.Empty, prVa = string.Empty;
        private string prPieceRate = string.Empty, prSwt = string.Empty, prStoneNos = string.Empty;
        private string[] SettingValues = new string[11];
        MagnaDbEntities db = null;
        public BarcodePrintBL()
        {
            db = new MagnaDbEntities();
            string str = "G";
            if (string.Compare("G", str) == 0) {
                isGoldPrint = true;
            }
            else if (string.Compare("S", str) == 0) {
                isGoldPrint = false;
            }
        }
        public bool GetPrint(BarcodePrintVM vm, out ProdigyPrintVM printObject, out ErrorVM error)
        {
            printObject = null;
            error = null;
            if(vm == null) {
                error = new ErrorVM { description = "There is nothing to print." };
                return false;
            }
            if(vm.Barcodes == null || vm.Barcodes.Count() <= 0) {
                error = new ErrorVM { description = "Barcodes are not specified." };
                return false;
            }
            if (vm.PrintType == null ) {
                error = new ErrorVM { description = "PrintType must be specified." };
                return false;
            }
            string printType = vm.PrintType.ToUpper();
            if (printType == "LEFT" || printType == "RIGHT" || printType == "DOUBLE") {
                ;
            }
            else {
                error = new ErrorVM { description = "PrintType should be one of these: LEFT, RIGHT or DOUBLE" };
                return false;
            }

            foreach (var barcodeNo in vm.Barcodes) {
                if (!ValidateBarcode(vm.CompanyCode, vm.BranchCode, barcodeNo, out error))
                    return false;
            }

            bool isLeft = true;
            isLeft = printType == "RIGHT" ? false : true;
            if (printType == "DOUBLE") {
                if(vm.Barcodes.Count() <= 1) {
                    error = new ErrorVM { description = "For PrintType DOUBLE, two barcodes must be provided." };
                    return false;
                }
            }
            else {
                if (vm.Barcodes.Count() > 1) {
                    error = new ErrorVM { description = "For PrintType LEFT and RIGHT, only one must be provided." };
                    return false;
                }
            }
            StringBuilder sb = new StringBuilder();
            if (!GenerateBarcodePrintData(vm.CompanyCode, vm.BranchCode, vm.Barcodes, isLeft, out sb, out error)) {
                return false;
            }

            printObject = new ProdigyPrintVM
            {
                PrintType = "RAW",
                ContinueNextPrint = false,
                Data = new PrintConfiguration().Base64Encode(sb.ToString())
                //Data = sb.ToString()
            };
            return true;
        }
        private bool GenerateBarcodePrintData(string companyCode, string branchCode, string[] Barcodes, bool isLeft, out StringBuilder sb, out ErrorVM error)
        {
            sb = new StringBuilder();
            error = null;
            GetSettingValues();
            try {
                int i = 0;
                sb.AppendLine("m");
                sb.AppendLine("L");
                sb.AppendLine("D11");
                sb.AppendLine("H16");
                while (i < Barcodes.Length) {
                    var barcodeNo = Barcodes[i];
                    
                    GetPrintValues(companyCode, branchCode, barcodeNo);
                    if (validbarcode) {
                        if (isLeft) {
                            sb.AppendLine("m");
                            sb.AppendLine("L");
                            sb.AppendLine("D11");
                            sb.AppendLine("H16");
                        }
                        if (!isLeft) {
                            sb.AppendLine(string.Format("{0}", SettingValues[8]));
                            isLeft = true;
                        }
                        else {
                            isLeft = false;
                        }
                        sb.AppendLine(string.Format("{1}{0}", prGwt, SettingValues[0]));
                        sb.AppendLine(string.Format("{1}{0}", prBarcodeNoprint, SettingValues[1]));
                        sb.AppendLine(string.Format("{1}{0}", prBarcode, SettingValues[2]));
                        sb.AppendLine(string.Format("{1}{0}", prOperatorCode, SettingValues[3]));
                        if (isGoldPrint) {
                            sb.AppendLine(string.Format("{1}{0}", prBarcode, SettingValues[4]));
                        }
                        sb.AppendLine(string.Format("{1}{0}", prSwt, SettingValues[5]));
                        sb.AppendLine(string.Format("{1}{0}", prNwt, SettingValues[6]));
                        sb.AppendLine(string.Format("{1}{0}", prPieceRate, SettingValues[6]));
                        if (!string.IsNullOrEmpty(prPieceRate)) {
                            sb.AppendLine(string.Format("{1}{0}", prDiamondCarat, SettingValues[5]));
                            sb.AppendLine(string.Format("{1}{0}", prStoneNos, SettingValues[7]));
                        }
                        else {
                            sb.AppendLine(string.Format("{1}{0}", prDiamondCarat, SettingValues[6]));
                            sb.AppendLine(string.Format("{1}{0}", prStoneNos, SettingValues[5]));
                        }
                        sb.AppendLine(string.Format("{1}{0}", prVa, SettingValues[7]));
                        sb.AppendLine(string.Format("{1}{0}", prModelNo, SettingValues[9]));
                        sb.AppendLine(string.Format("{1}{0}", prOrderNo, SettingValues[10]));
                        if (isLeft) {
                            sb.AppendLine("Q0001");
                            sb.AppendLine("E");
                        }
                    }
                    i = i + 1;
                }
                sb.AppendLine("Q0001");
                sb.AppendLine("E"); 
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        private bool ValidateBarcode(string companyCode, string branchCode, string barcodeNo, out ErrorVM error)
        {
            error = null;
            var barcodeInfo = db.KTTU_BARCODE_MASTER.Where(x => x.company_code == companyCode && x.branch_code == branchCode
                && x.barcode_no == barcodeNo && x.sold_flag != "Y").FirstOrDefault();
            if(barcodeInfo == null) {
                error = new ErrorVM { description = "Barcode No. " + barcodeNo + " is not invalid." };
                return false;
            }
            return true;
        }

        private void GetPrintValues(string companyCode, string branchCode, string barcodeNo)
        {
            string itemquery = string.Format("SELECT barcode_no,piece_rate,gwt,nwt,swt,counter_code,mc_percent,remarks,order_no,diamond_no,item_name,old_barcode_no,certification_no,receipt_type from KTTU_BARCODE_MASTER WHERE barcode_no='{0}' AND sold_flag = 'N' AND company_code='{1}' AND branch_code='{2}'",
                barcodeNo, companyCode, branchCode);
            DataTable dtFirstBarcode = Globals.GetDataTable(itemquery);
            if (dtFirstBarcode != null && dtFirstBarcode.Rows.Count > 0) {
                validbarcode = true;
                prBarcodeNoprint = prBarcode = dtFirstBarcode.Rows[0]["barcode_no"].ToString();
                if (!string.IsNullOrEmpty(dtFirstBarcode.Rows[0]["receipt_type"].ToString()))
                    prBarcodeNoprint = dtFirstBarcode.Rows[0]["receipt_type"].ToString() + '-' + dtFirstBarcode.Rows[0]["barcode_no"].ToString();

                if (!string.IsNullOrEmpty(dtFirstBarcode.Rows[0]["old_barcode_no"].ToString())) {
                    prBarcodeNoprint = prBarcodeNoprint + "-" + dtFirstBarcode.Rows[0]["old_barcode_no"].ToString();
                }
                prOperatorCode = dtFirstBarcode.Rows[0]["counter_code"].ToString();
                prModelNo = dtFirstBarcode.Rows[0]["certification_no"].ToString();
                if (string.IsNullOrEmpty(prModelNo)) {
                    prModelNo = dtFirstBarcode.Rows[0]["item_name"].ToString();
                }
                prOrderNo = dtFirstBarcode.Rows[0]["order_no"].ToString();
                if (Convert.ToInt32(dtFirstBarcode.Rows[0]["diamond_no"].ToString()) == 0) {
                    prStoneNos = string.Empty;
                }
                else {
                    prStoneNos = dtFirstBarcode.Rows[0]["diamond_no"].ToString();
                }
                if (Convert.ToInt32(prOrderNo) == 0) {
                    prOrderNo = string.Empty;
                }
                if (Convert.ToDecimal(dtFirstBarcode.Rows[0]["gwt"].ToString()) > 0) {
                    prGwt = dtFirstBarcode.Rows[0]["gwt"].ToString();
                    prGwt = "GR: " + prGwt + "g";
                    prSwt = dtFirstBarcode.Rows[0]["swt"].ToString();
                    prSwt = "ST: " + prSwt + "g";
                    prNwt = dtFirstBarcode.Rows[0]["Nwt"].ToString();
                    prNwt = "NT: " + prNwt + "g";
                }
                else {
                    prGwt = string.Empty;
                    prSwt = string.Empty;
                    prNwt = string.Empty;
                }
                if (Convert.ToDecimal(dtFirstBarcode.Rows[0]["mc_percent"].ToString()) > 0) {
                    prVa = GetVAcodeForBarcodePrint(Convert.ToDecimal(dtFirstBarcode.Rows[0]["mc_percent"].ToString()));
                }
                else {
                    prVa = string.Empty;
                }
                if (Convert.ToDecimal(dtFirstBarcode.Rows[0]["piece_rate"].ToString()) <= 0)
                    prPieceRate = string.Empty;
                else {
                    prPieceRate = Convert.ToString(Math.Round(Convert.ToDecimal(dtFirstBarcode.Rows[0]["piece_rate"].ToString()), 0, MidpointRounding.ToEven));
                    prPieceRate = "Rs: " + prPieceRate + "/-";
                    prSwt = string.Empty;
                    prNwt = string.Empty;
                }

                string stonequery = string.Format("SELECT type,carrat,qty from KTTU_BARCODE_STONE_DETAILS WHERE barcode_no='{0}' AND company_code='{1}' AND branch_code='{2}'",
                    barcodeNo, companyCode, branchCode);
                DataTable dtStoneDetails = Globals.GetDataTable(stonequery);
                if (dtStoneDetails != null && dtStoneDetails.Rows.Count > 0) {
                    decimal DiamondCarat = 0.00M;
                    //int TotalQty = 0;
                    for (int i = 0; i < dtStoneDetails.Rows.Count; i++) {
                        if (string.Compare(dtStoneDetails.Rows[i][0].ToString(), "D") == 0) {
                            DiamondCarat = DiamondCarat + Convert.ToDecimal(dtStoneDetails.Rows[i][1].ToString());
                            // TotalQty = TotalQty + Convert.ToInt32(dtStoneDetails.Rows[i][2].ToString());
                        }
                    }
                    if (string.IsNullOrEmpty(prPieceRate)) {
                        if (!string.IsNullOrEmpty(prStoneNos)) {
                            //prStoneNos = "DNos: " + Convert.ToString(TotalQty);
                            prStoneNos = "DNoS: " + prStoneNos;
                        }
                        else {
                            prStoneNos = string.Empty;
                        }
                    }
                    else {
                        prStoneNos = string.Empty;
                        prSwt = string.Empty;
                        prNwt = string.Empty;
                    }
                    if (DiamondCarat > 0) {
                        prDiamondCarat = "DCts: " + Convert.ToString(DiamondCarat);
                        prSwt = string.Empty;
                        prNwt = string.Empty;

                    }
                    else {
                        prDiamondCarat = string.Empty;
                        prStoneNos = string.Empty;
                    }
                }
                else {
                    prDiamondCarat = string.Empty;
                    prStoneNos = string.Empty;
                }
            }
            else {
                validbarcode = false;
            }

        }
        private void GetSettingValues()
        {
            SettingValues[0] = "1911A0600120028";
            SettingValues[1] = "1911A0600340028";
            if (isGoldPrint) {
                SettingValues[2] = "1d4206000560032";
            }
            else {
                SettingValues[2] = "1d4213000560";
            }
            SettingValues[3] = "1911A0600190197";
            SettingValues[4] = "3d4206002020227";
            SettingValues[5] = "3911A0602260227";
            SettingValues[6] = "3911A0602460222";
            SettingValues[7] = "2911A0602110007";
            SettingValues[8] = "C0520";
            SettingValues[9] = "2911A0601210005";
            SettingValues[10] = "1911A0600350160";

            #region Direct assignment - so this is commented.
            //string fileName = string.Empty;
            //string rootPath = Globals.GetWebConfigValue("ImageUrl");
            //if (string.IsNullOrEmpty(rootPath))
            //    rootPath = System.Web.HttpContext.Current.Request.MapPath(@"~\App_Data\");
            //string barcodeSettings = rootPath + string.Format(@"\barcode_settings\BarcodePrintSettings.txt");

            //if (isGoldPrint) {
            //    fileName = rootPath + string.Format(@"\barcode_settings\BarcodePrintSettings.txt");
            //}
            //else {
            //    fileName = rootPath + string.Format(@"\barcode_settings\BarcodePrintSettingsSilver.txt");
            //}

            //if (File.Exists(fileName)) {
            //    StreamReader SR = new StreamReader(fileName);
            //    for (int i = 0; i < 11; i++) {
            //        SettingValues[i] = SR.ReadLine();
            //    }
            //    SR.Close();
            //}
            //else {
            //    SettingValues[0] = "1911A0600120028";
            //    SettingValues[1] = "1911A0600340028";
            //    if (isGoldPrint) {
            //        SettingValues[2] = "1d4206000560032";
            //    }
            //    else {
            //        SettingValues[2] = "1d4213000560";
            //    }
            //    SettingValues[3] = "1911A0600190197";
            //    SettingValues[4] = "3d4206002020227";
            //    SettingValues[5] = "3911A0602260227";
            //    SettingValues[6] = "3911A0602460222";
            //    SettingValues[7] = "2911A0602110007";
            //    SettingValues[8] = "C0520");
            //    SettingValues[9] = "2911A0601210005";
            //    SettingValues[10] = "1911A0600350160";
            //} 
            #endregion
        }
        private string GetVAcodeForBarcodePrint(decimal VAPercent)
        {
            string VAcode = string.Empty;
            string[] MC = { "R", "B", "H", "I", "M", "A", "T", "O", "W", "E" };
            string VA = Convert.ToString(VAPercent);
            for (int i = 0; i < VA.Length; i++) {
                string x = VA.Substring(i, 1);
                if (string.Compare(x, ".") == 0) {
                    ;
                }
                else {
                    int y = Convert.ToInt32(x);
                    VAcode = VAcode + MC[y];
                }
            }
            return VAcode;
        }
    }
}
