using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class GSEntryBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public dynamic GetAllGS(string companyCode, string branchCode)
        {
            return db.KSTS_GS_ITEM_ENTRY
                   .Where(g => g.company_code == companyCode && g.branch_code == branchCode)
                   .Select(g => new GSItemEntryMasterVM
                   {
                       ObjID = g.obj_id,
                       CompanyCode = g.company_code,
                       BranchCode = g.branch_code,
                       ItemLevel1ID = g.item_level1_id,
                       GSName = g.item_level1_name,
                       GsCode = g.gs_code,
                       MeasureType = g.measure_type,
                       MetalType = g.metal_type,
                       Karat = g.karat,
                       BillType = g.bill_type,
                       Tax = g.tax,
                       OpeningGwt = g.opening_gwt,
                       OpeningGwtValue = g.opening_gwt_value,
                       OpeningNwt = g.opening_nwt,
                       OpeningNwtValue = g.opening_nwt_value,
                       OpeningUnits = g.opening_units,
                       DisplayOrder = g.display_order,
                       CommodityCode = g.commodity_code,
                       ObjectStatus = g.object_status,
                       UpdateOn = g.UpdateOn,
                       EduCess = g.edu_cess,
                       HighEle = g.high_ele,
                       Tcs = g.tcs,
                       IsStone = g.isStone,
                       Purity = g.purity,
                       TcsPerc = g.tcs_perc,
                       STax = g.S_tax,
                       CTax = g.C_tax,
                       ITax = g.I_tax,
                       GSTGoodsGroupCode = g.GSTGoodsGroupCode,
                       GSTServicesGroupCode = g.GSTServicesGroupCode,
                   }).OrderBy(c => c.GSName);
        }

        public dynamic GetGoodsServiceType(string companyCode, string branchCode, string type)
        {
            return db.GSTGroups.Where(gst => gst.GSTGroupType == type);
        }

        public bool Save(GSItemEntryMasterVM g, out ErrorVM error)
        {
            error = null;



            KSTS_GS_ITEM_ENTRY items = db.KSTS_GS_ITEM_ENTRY.Where(item => item.gs_code == g.GsCode && item.company_code == g.CompanyCode && item.branch_code == g.BranchCode).FirstOrDefault();
            if (items != null) {
                error = new ErrorVM()
                {
                    description = "GS Code already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            int itmeLevelID = db.KSTS_GS_ITEM_ENTRY.Max(item => item.item_level1_id) + 1;
            KSTS_GS_ITEM_ENTRY kgie = new KSTS_GS_ITEM_ENTRY();
            kgie.obj_id = SIGlobals.Globals.GetHashcode("KSTS_GS_ITEM_ENTRY"
                + SIGlobals.Globals.Separator + g.GsCode
                + SIGlobals.Globals.Separator + g.CompanyCode
                + SIGlobals.Globals.Separator + g.BranchCode);
            kgie.company_code = g.CompanyCode;
            kgie.branch_code = g.BranchCode;
            kgie.item_level1_id = itmeLevelID;
            kgie.item_level1_name = g.ItemLevel1Name;
            kgie.gs_code = g.GsCode;
            kgie.measure_type = g.MeasureType;
            kgie.metal_type = g.MetalType;
            kgie.karat = g.Karat;
            kgie.bill_type = g.BillType;
            kgie.tax = g.Tax;
            kgie.opening_units = g.OpeningUnits;
            kgie.opening_gwt = g.OpeningGwt;
            kgie.opening_nwt = g.OpeningNwt;
            kgie.opening_gwt_value = g.OpeningGwtValue;
            kgie.opening_nwt_value = g.OpeningNwtValue;
            kgie.object_status = "O";
            kgie.display_order = g.DisplayOrder;
            kgie.commodity_code = g.CommodityCode;
            kgie.UpdateOn = SIGlobals.Globals.GetApplicationDate(g.CompanyCode, g.BranchCode);
            kgie.excise_duty = g.ExciseDuty;
            kgie.edu_cess = g.EduCess;
            kgie.high_ele = g.HighEle;
            kgie.tcs = g.Tcs;
            kgie.isStone = g.IsStone;
            kgie.purity = g.Purity;
            kgie.tcs_perc = g.TcsPerc;
            kgie.S_tax = g.STax;
            kgie.I_tax = g.ITax;
            kgie.C_tax = g.CTax;
            kgie.GSTGoodsGroupCode = g.GSTGoodsGroupCode;
            kgie.GSTServicesGroupCode = g.GSTServicesGroupCode;
            kgie.HSN = g.HSN;
            kgie.UniqRowID = Guid.NewGuid();
            db.KSTS_GS_ITEM_ENTRY.Add(kgie);

            // One row is inserting into the Bellow Table becuase Magna Uses this row for generating header.
            KSTU_ITEM_LIST_GROUP_MASTER itemList = new KSTU_ITEM_LIST_GROUP_MASTER();
            itemList.obj_id = SIGlobals.Globals.GetHashcode("KSTU_ITEM_LIST_GROUP_MASTER"
                    + SIGlobals.Globals.Separator + itmeLevelID
                    + 0 + 0 + 0 + 0 + SIGlobals.Globals.Separator + g.CompanyCode + SIGlobals.Globals.Separator + g.BranchCode);
            itemList.company_code = g.CompanyCode;
            itemList.branch_code = g.BranchCode;
            itemList.item_level1_id = itmeLevelID;
            itemList.item_level1_name = g.ItemLevel1Name;
            itemList.gs_code = g.GsCode;
            itemList.item_level2_id = 0;
            itemList.item_level2_name = "NA";
            itemList.item_level3_id = 0;
            itemList.item_level3_name = "NA";
            itemList.item_level4_id = 0;
            itemList.item_level4_name = "NA";
            itemList.item_level5_id = 0;
            itemList.item_level5_name = "NA";
            itemList.item_level6_id = 0;
            itemList.item_level6_name = "NA";
            itemList.short_description = null;
            itemList.ischild = "Y";
            itemList.min_units = 0;
            itemList.min_stock_level = 0;
            itemList.catalogId = null;
            itemList.grade = null;
            itemList.tagged = null;
            itemList.stone = null;
            itemList.diamond = null;
            itemList.counter_code = null;
            itemList.karat = null;
            itemList.piece_item = null;
            itemList.obj_status = null;
            itemList.alias_name = null;
            itemList.UpdateOn = SIGlobals.Globals.GetDateTime();
            itemList.min_profit_percent = 0;
            itemList.QTY_LOCK = null;
            itemList.hallmark = "Y";
            itemList.certification = "N";
            itemList.tcs_perc = 0;
            itemList.GSTGoodsGroupCode = null;
            itemList.GSTServicesGroupCode = null;
            itemList.HSN = null;
            itemList.UniqRowID = Guid.NewGuid();
            db.KSTU_ITEM_LIST_GROUP_MASTER.Add(itemList);

            KTTU_GS_SALES_STOCK stockEntry = new KTTU_GS_SALES_STOCK();
            stockEntry.obj_id = SIGlobals.Globals.GetHashcode("KTTU_GS_SALES_STOCK" + SIGlobals.Globals.Separator + g.GsCode + SIGlobals.Globals.Separator + g.CompanyCode + SIGlobals.Globals.Separator + g.BranchCode);
            stockEntry.date = DateTime.Now;
            stockEntry.item_name = g.ItemLevel1Name;
            stockEntry.gs_code = g.GsCode;
            stockEntry.company_code = g.CompanyCode;
            stockEntry.branch_code = g.BranchCode;
            stockEntry.opening_units = g.OpeningUnits;
            stockEntry.opening_gwt = g.OpeningGwt;
            stockEntry.opening_nwt = g.OpeningNwt;
            stockEntry.receipt_gwt = 0;
            stockEntry.receipt_nwt = 0;
            stockEntry.receipt_units = 0;
            stockEntry.issue_gwt = 0;
            stockEntry.issue_nwt = 0;
            stockEntry.issue_units = 0;
            stockEntry.closing_units = stockEntry.opening_units + stockEntry.receipt_units - stockEntry.issue_units;
            stockEntry.closing_gwt = stockEntry.opening_gwt + stockEntry.receipt_gwt - stockEntry.issue_gwt;
            stockEntry.closing_nwt = stockEntry.opening_nwt + stockEntry.receipt_nwt - stockEntry.issue_nwt;
            db.KTTU_GS_SALES_STOCK.Add(stockEntry);

            string[] salesList = { "OPN.STK " + " " + g.ItemLevel1Name
                                 , "SALES " + g.ItemLevel1Name + " LOCAL"
                                 , "PURCHASE " + g.ItemLevel1Name + " LOCAL"
                                 , "SALES RETURN " + g.ItemLevel1Name + " LOCAL"
                                 , "CLK.STK " + " " + g.ItemLevel1Name
                                 , "PUR.RET RETURN " + g.ItemLevel1Name + " LOCAL"
                                 , "SALES " + g.ItemLevel1Name + " INTERSTATE"
                                 , "PURCHASE " + g.ItemLevel1Name + " INTERSTATE"
                                 , "SALES RETURN " + g.ItemLevel1Name + " INTERSTATE"
                                 , "PUR.RET RETURN " + g.ItemLevel1Name + " INTERSTATE"
                                 , "SALES " + g.ItemLevel1Name + " EXPORT"
                                 , "PURCHASE " + g.ItemLevel1Name + " IMPORT"
                                 , "SALES RETURN " + g.ItemLevel1Name + " IMPORT"
                                 , "PUR.RET RETURN " + g.ItemLevel1Name + " EXPORT"
                                 , "STOCK INWARD -  " + g.ItemLevel1Name + " LOCAL"
                                 , "STOCK OUTWARD - " + g.ItemLevel1Name + " LOCAL"
                                 , "STOCK INWARD -  " + g.ItemLevel1Name + " INTERSTATE"
                                 , "STOCK OUTWARD - " + g.ItemLevel1Name + " INTERSTATE"
                                 };


            for (int i = 0; i < salesList.Length; i++) {
                KSTU_ACC_LEDGER_MASTER leggerMaster = new KSTU_ACC_LEDGER_MASTER();
                leggerMaster.gs_seq_no = i + 1;
                leggerMaster.acc_name = salesList[i].ToString();
                leggerMaster.gs_code = g.GsCode;
                leggerMaster.opn_bal_type = "C";
                leggerMaster.opn_bal = 0;
                leggerMaster.IsAutomatic = "Y";

                if (i == 0) {
                    leggerMaster.group_id = GetSubGroup(30, g.CompanyCode, g.BranchCode); //30;

                    if (g.OpeningNwtValue > 0) {
                        leggerMaster.opn_bal = g.OpeningNwtValue;
                        if (leggerMaster.opn_bal > 0)
                            leggerMaster.opn_bal_type = "C";
                        else
                            leggerMaster.opn_bal_type = "D";
                    }
                }
                if (i == 1 || i == 3 || i == 6 || i == 8 || i == 10 || i == 12) {
                    leggerMaster.group_id = GetSubGroup(30, g.CompanyCode, g.BranchCode); //30;
                }
                if (i == 2 || i == 5 || i == 7 || i == 9 || i == 11 || i == 13) {
                    leggerMaster.group_id = GetSubGroup(29, g.CompanyCode, g.BranchCode); //29;
                }
                if (i == 4) {
                    leggerMaster.group_id = GetSubGroup(16, g.CompanyCode, g.BranchCode);//16;
                }

                leggerMaster.acc_type = "O";
                leggerMaster.company_code = g.CompanyCode;
                leggerMaster.branch_code = g.BranchCode;
                leggerMaster.cust_id = 0;
                leggerMaster.scheme_code = string.Empty;
                leggerMaster.vat_id = "0";
                leggerMaster.party_code = string.Empty;
                leggerMaster.obj_status = "O";

                if (leggerMaster.gs_seq_no == 2) // Sales Local
                {
                    leggerMaster.transType = "S";
                }
                else if (leggerMaster.gs_seq_no == 3) // Purchase Local
                {
                    leggerMaster.transType = "P";
                }
                else if (leggerMaster.gs_seq_no == 4) //SR Local
                {
                    leggerMaster.transType = "RS";
                }
                else if (leggerMaster.gs_seq_no == 6) //Purchase return Local
                {
                    leggerMaster.transType = "PR";
                }
                else if (leggerMaster.gs_seq_no == 7) //Sales Interstate
                {
                    leggerMaster.transType = "SI";
                }
                else if (leggerMaster.gs_seq_no == 8) //Purchase Interstate
                {
                    leggerMaster.transType = "PI";
                }
                else if (leggerMaster.gs_seq_no == 9) //SR Interstate
                {
                    leggerMaster.transType = "RSI";
                }
                else if (leggerMaster.gs_seq_no == 10) //Purchase Return Interstate
                {
                    leggerMaster.transType = "PRI";
                }
                else {
                    leggerMaster.transType = "";
                }
                leggerMaster.acc_code = Convert.ToInt32(SIGlobals.Globals.GetFinancialYear(db, g.CompanyCode, g.BranchCode).ToString().Substring(2, 2) + SIGlobals.Globals.GetNewSerialNo(db, "02", g.CompanyCode, g.BranchCode));
                string[] temp = { "KSTU_ACC_LEDGER_MASTER", leggerMaster.acc_code.ToString() };
                leggerMaster.obj_id = SIGlobals.Globals.GetHashcode(temp, g.CompanyCode, g.BranchCode);
                leggerMaster.jflag = "N";
                leggerMaster.UniqRowID = Guid.NewGuid();

                if (leggerMaster.gs_seq_no == 2 || leggerMaster.gs_seq_no == 3 || leggerMaster.gs_seq_no == 4 || leggerMaster.gs_seq_no == 6
                   || leggerMaster.gs_seq_no == 7 || leggerMaster.gs_seq_no == 8 || leggerMaster.gs_seq_no == 9 || leggerMaster.gs_seq_no == 10) {
                    KSTS_ACC_POSTING_SETUP AccPostingSetup = new KSTS_ACC_POSTING_SETUP();
                    AccPostingSetup.company_code = g.CompanyCode;
                    AccPostingSetup.branch_code = g.BranchCode;
                    AccPostingSetup.gs_code = leggerMaster.gs_code;
                    AccPostingSetup.trans_type = leggerMaster.transType;
                    AccPostingSetup.acc_code = leggerMaster.acc_code;
                    AccPostingSetup.sno = db.KSTS_ACC_POSTING_SETUP.Max(s => s.sno) + 1;
                    db.KSTS_ACC_POSTING_SETUP.Add(AccPostingSetup);
                }
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, "02", g.CompanyCode, g.BranchCode);
                db.SaveChanges();
                db.KSTU_ACC_LEDGER_MASTER.Add(leggerMaster);
            }
            try {
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return false;
            }
        }

        public bool Update(int Id, GSItemEntryMasterVM g, out ErrorVM error)
        {
            error = null;
            KSTS_GS_ITEM_ENTRY kgie = db.KSTS_GS_ITEM_ENTRY.Where(item => item.item_level1_id == Id
                                                                    && item.company_code == g.CompanyCode
                                                                    && item.branch_code == g.BranchCode).FirstOrDefault();

            db.KSTS_GS_ITEM_ENTRY.Remove(kgie);
            db.SaveChanges();

            kgie.obj_id = SIGlobals.Globals.GetHashcode("KSTS_GS_ITEM_ENTRY"
                + SIGlobals.Globals.Separator + g.GsCode
                + SIGlobals.Globals.Separator + g.CompanyCode
                + SIGlobals.Globals.Separator + g.BranchCode);
            kgie.company_code = g.CompanyCode;
            kgie.branch_code = g.BranchCode;
            kgie.item_level1_id = g.ItemLevel1ID;
            kgie.item_level1_name = g.ItemLevel1Name;
            kgie.gs_code = g.GsCode;
            kgie.measure_type = g.MeasureType;
            kgie.metal_type = g.MetalType;
            kgie.karat = g.Karat;
            kgie.bill_type = g.BillType;
            kgie.tax = g.Tax;
            kgie.opening_units = g.OpeningUnits;
            kgie.opening_gwt = g.OpeningGwt;
            kgie.opening_nwt = g.OpeningNwt;
            kgie.opening_gwt_value = g.OpeningGwtValue;
            kgie.opening_nwt_value = g.OpeningNwtValue;
            kgie.object_status = "O";
            kgie.display_order = g.DisplayOrder;
            kgie.commodity_code = g.CommodityCode;
            kgie.UpdateOn = SIGlobals.Globals.GetApplicationDate(g.CompanyCode, g.BranchCode);
            kgie.excise_duty = g.ExciseDuty;
            kgie.edu_cess = g.EduCess;
            kgie.high_ele = g.HighEle;
            kgie.tcs = g.Tcs;
            kgie.isStone = g.IsStone;
            kgie.purity = g.Purity;
            kgie.tcs_perc = g.TcsPerc;
            kgie.S_tax = g.STax;
            kgie.I_tax = g.ITax;
            kgie.C_tax = g.CTax;
            kgie.GSTGoodsGroupCode = g.GSTGoodsGroupCode;
            kgie.GSTServicesGroupCode = g.GSTServicesGroupCode;
            kgie.HSN = g.HSN;
            kgie.UniqRowID = Guid.NewGuid();
            db.KSTS_GS_ITEM_ENTRY.Add(kgie);

            KSTU_ITEM_LIST_GROUP_MASTER itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level1_id == Id
                                                                    && item.company_code == g.CompanyCode
                                                                    && item.branch_code == g.BranchCode).FirstOrDefault();
            db.KSTU_ITEM_LIST_GROUP_MASTER.Remove(itemList);
            db.SaveChanges();

            itemList.obj_id = SIGlobals.Globals.GetHashcode("KSTU_ITEM_LIST_GROUP_MASTER"
                    + SIGlobals.Globals.Separator + g.ItemLevel1ID
                    + 0 + 0 + 0 + 0 + SIGlobals.Globals.Separator + g.CompanyCode + SIGlobals.Globals.Separator + g.BranchCode);
            itemList.company_code = g.CompanyCode;
            itemList.branch_code = g.BranchCode;
            itemList.item_level1_id = g.ItemLevel1ID;
            itemList.item_level1_name = g.ItemLevel1Name;
            itemList.gs_code = g.GsCode;
            itemList.item_level2_id = 0;
            itemList.item_level2_name = "NA";
            itemList.item_level3_id = 0;
            itemList.item_level3_name = "NA";
            itemList.item_level4_id = 0;
            itemList.item_level4_name = "NA";
            itemList.item_level5_id = 0;
            itemList.item_level5_name = "NA";
            itemList.item_level6_id = 0;
            itemList.item_level6_name = "NA";
            itemList.short_description = null;
            itemList.ischild = "Y";
            itemList.min_units = 0;
            itemList.min_stock_level = 0;
            itemList.catalogId = null;
            itemList.grade = null;
            itemList.tagged = null;
            itemList.stone = null;
            itemList.diamond = null;
            itemList.counter_code = null;
            itemList.karat = null;
            itemList.piece_item = null;
            itemList.obj_status = null;
            itemList.alias_name = null;
            itemList.UpdateOn = SIGlobals.Globals.GetDateTime();
            itemList.min_profit_percent = 0;
            itemList.QTY_LOCK = null;
            itemList.hallmark = "Y";
            itemList.certification = "N";
            itemList.tcs_perc = 0;
            itemList.GSTGoodsGroupCode = null;
            itemList.GSTServicesGroupCode = null;
            itemList.HSN = null;
            itemList.UniqRowID = Guid.NewGuid();
            db.KSTU_ITEM_LIST_GROUP_MASTER.Add(itemList);

            try {
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError
                };
                return false;
            }
        }
        #endregion

        #region Private Methods
        private int GetSubGroup(int Id, string companyCode, string branchCode)
        {
            KSTS_ACC_CODE_MASTER kacm = db.KSTS_ACC_CODE_MASTER.Where(ac => ac.obj_id == Id
                                                                    && ac.company_code == companyCode
                                                                    && ac.branch_code == branchCode).FirstOrDefault();
            return kacm == null ? 0 : Convert.ToInt32(kacm.sub_group_id);
        }
        #endregion
    }
}
