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
    public class ProductTreeBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public List<ProductTreeVM> GetAllProductTreeDetails(string CompanyCode, string BranchCode)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            //List<KSTS_GS_ITEM_ENTRY> lstOfItems = db.KSTS_GS_ITEM_ENTRY.Where(item => item.item_level1_id == 2).ToList();
            List<KSTS_GS_ITEM_ENTRY> lstOfItems = db.KSTS_GS_ITEM_ENTRY.Where(item => item.measure_type != "C"
                                                                        && item.company_code == CompanyCode
                                                                        && item.branch_code == BranchCode).ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = lstOfItems[i].obj_id;
                    pt.ItemLevelID = lstOfItems[i].item_level1_id;
                    pt.ItemLevelName = lstOfItems[i].item_level1_name;
                    pt.GSCode = lstOfItems[i].gs_code;
                    pt.CompanyCode = CompanyCode;
                    pt.BranchCode = BranchCode;
                    pt.innerLevelItems = GetInnerLevel2Items(lstOfItems[i].item_level1_id);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        public List<ProductTreeVM> GetAllProductTreeDetails2(string CompanyCode, string BranchCode)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<KSTS_GS_ITEM_ENTRY> lstOfItems = db.KSTS_GS_ITEM_ENTRY.Where(item => item.measure_type != "C"
                                                                        && item.company_code == CompanyCode
                                                                        && item.branch_code == BranchCode).ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = lstOfItems[i].obj_id;
                    pt.ItemLevel1ID = lstOfItems[i].item_level1_id;
                    pt.ItemLevel1Name = lstOfItems[i].item_level1_name;
                    pt.GSCode = lstOfItems[i].gs_code;
                    pt.CompanyCode = CompanyCode;
                    pt.BranchCode = BranchCode;
                    pt.innerLevelItems = GetInnerLevel2Items2(lstOfItems[i].item_level1_id, CompanyCode, BranchCode);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        public bool SaveProduct(ProductTreeVM product, out ErrorVM error)
        {
            error = null;
            string itemName = string.Empty;
            int level = 0;
            int parentID = 0;
            string parentName = string.Empty;

            KSTU_ITEM_LIST_GROUP_MASTER itemList = new KSTU_ITEM_LIST_GROUP_MASTER();
            itemList.company_code = product.CompanyCode;
            itemList.branch_code = product.BranchCode;
            itemList.item_level1_id = product.ItemLevel1ID;
            itemList.item_level1_name = product.ItemLevel1Name;
            itemList.gs_code = product.GSCode;

            if (product.ItemLevel2Id == 0) {
                if (CheckItemExist(2, product.ItemLevel1ID, product.ItemLevel2Name, product.GSCode)) {
                    error = new ErrorVM()
                    {
                        description = "Item Code already Exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                itemList.item_level2_id = db.KSTU_ITEM_LIST_GROUP_MASTER.Max(item => item.item_level2_id) + 1;
                itemList.item_level2_name = product.ItemLevel2Name;
                itemName = product.ItemLevel2Name;
                itemList.item_level3_id = 0;
                itemList.item_level3_name = "NA";
                itemList.item_level4_id = 0;
                itemList.item_level4_name = "NA";
                itemList.item_level5_id = 0;
                itemList.item_level5_name = "NA";
                itemList.item_level6_id = 0;
                itemList.item_level6_name = "NA";

                level = 1;
                parentID = product.ItemLevel1ID;
                parentName = product.ItemLevel1Name;
            }
            else if (product.ItemLevel3Id == 0) {
                if (CheckItemExist(3, product.ItemLevel2Id, product.ItemLevel3Name, product.GSCode)) {
                    error = new ErrorVM()
                    {
                        description = "Item Code already Exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                itemList.item_level2_id = product.ItemLevel2Id;
                itemList.item_level2_name = product.ItemLevel2Name;
                itemList.item_level3_id = db.KSTU_ITEM_LIST_GROUP_MASTER.Max(item => item.item_level3_id) + 1;
                itemList.item_level3_name = product.ItemLevel3Name;
                itemName = product.ItemLevel3Name;
                itemList.item_level4_id = 0;
                itemList.item_level4_name = "NA";
                itemList.item_level5_id = 0;
                itemList.item_level5_name = "NA";
                itemList.item_level6_id = 0;
                itemList.item_level6_name = "NA";

                level = 2;
                parentID = product.ItemLevel2Id;
                parentName = product.ItemLevel2Name;
            }
            else if (product.ItemLevel4Id == 0) {
                if (CheckItemExist(4, product.ItemLevel3Id, product.ItemLevel4Name, product.GSCode)) {
                    error = new ErrorVM()
                    {
                        description = "Item Code already Exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                itemList.item_level2_id = product.ItemLevel2Id;
                itemList.item_level2_name = product.ItemLevel2Name;
                itemList.item_level3_id = product.ItemLevel3Id;
                itemList.item_level3_name = product.ItemLevel3Name;
                itemList.item_level4_id = db.KSTU_ITEM_LIST_GROUP_MASTER.Max(item => item.item_level4_id) + 1;
                itemList.item_level4_name = product.ItemLevel4Name;
                itemName = product.ItemLevel4Name;
                itemList.item_level5_id = 0;
                itemList.item_level5_name = "NA";
                itemList.item_level6_id = 0;
                itemList.item_level6_name = "NA";

                level = 3;
                parentID = product.ItemLevel3Id;
                parentName = product.ItemLevel3Name;
            }
            else if (product.ItemLevel5Id == 0) {
                if (CheckItemExist(5, product.ItemLevel4Id, product.ItemLevel5Name, product.GSCode)) {
                    error = new ErrorVM()
                    {
                        description = "Item Code already Exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                itemList.item_level2_id = product.ItemLevel2Id;
                itemList.item_level2_name = product.ItemLevel2Name;
                itemList.item_level3_id = product.ItemLevel3Id;
                itemList.item_level3_name = product.ItemLevel3Name;
                itemList.item_level4_id = product.ItemLevel4Id;
                itemList.item_level4_name = product.ItemLevel4Name;
                itemList.item_level5_id = db.KSTU_ITEM_LIST_GROUP_MASTER.Max(item => item.item_level5_id) + 1;
                itemList.item_level5_name = product.ItemLevel5Name;
                itemName = product.ItemLevel5Name;
                itemList.item_level6_id = 0;
                itemList.item_level6_name = "NA";

                level = 4;
                parentID = product.ItemLevel4Id;
                parentName = product.ItemLevel4Name;
            }
            else if (product.ItemLevel6Id == 0) {
                if (CheckItemExist(6, product.ItemLevel5Id, product.ItemLevel6Name, product.GSCode)) {
                    error = new ErrorVM()
                    {
                        description = "Item Code already Exist.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                itemList.item_level2_id = product.ItemLevel2Id;
                itemList.item_level2_name = product.ItemLevel2Name;
                itemList.item_level3_id = product.ItemLevel3Id;
                itemList.item_level3_name = product.ItemLevel3Name;
                itemList.item_level4_id = product.ItemLevel4Id;
                itemList.item_level4_name = product.ItemLevel4Name;
                itemList.item_level5_id = product.ItemLevel5Id;
                itemList.item_level5_name = product.ItemLevel5Name;
                itemList.item_level6_id = db.KSTU_ITEM_LIST_GROUP_MASTER.Max(item => item.item_level6_id) + 1;
                itemList.item_level6_name = product.ItemLevel6Name;
                itemName = product.ItemLevel6Name;

                level = 5;
                parentID = product.ItemLevel5Id;
                parentName = product.ItemLevel5Name;
            }
            itemList.short_description = product.ShortDescription;
            itemList.ischild = product.IsChild == null ? "Y" : product.IsChild;
            itemList.min_units = product.MinUnits;
            itemList.min_stock_level = product.MinStockLevel;
            itemList.catalogId = product.CatalogId;
            itemList.grade = product.Grade;
            itemList.tagged = product.Tagged;
            itemList.stone = product.Stone;
            itemList.diamond = product.Diamond;
            itemList.counter_code = product.CounterCode;
            itemList.karat = product.karat;
            itemList.piece_item = product.PieceItem;
            itemList.obj_status = product.ObjStatus;
            itemList.alias_name = product.AliasName;
            itemList.UpdateOn = SIGlobals.Globals.GetDateTime();
            itemList.min_profit_percent = 0;
            itemList.QTY_LOCK = product.QtyLock;
            itemList.hallmark = product.Hallmark;
            itemList.certification = product.Certification;
            itemList.tcs_perc = 0;
            itemList.GSTGoodsGroupCode = product.GSTGoodsGroupCode;
            itemList.GSTServicesGroupCode = product.GSTServicesGroupCode;
            itemList.HSN = product.HSN;
            itemList.UniqRowID = Guid.NewGuid();
            itemList.obj_id = SIGlobals.Globals.GetHashcode("KSTU_ITEM_LIST_GROUP_MASTER" + SIGlobals.Globals.Separator + itemList.item_level1_id + SIGlobals.Globals.Separator + itemList.item_level2_id + SIGlobals.Globals.Separator + itemList.item_level3_id + SIGlobals.Globals.Separator + itemList.item_level4_id + SIGlobals.Globals.Separator + itemList.item_level5_id + SIGlobals.Globals.Separator + itemList.item_level6_id + SIGlobals.Globals.Separator + itemList.company_code + SIGlobals.Globals.Separator + itemList.branch_code);
            db.KSTU_ITEM_LIST_GROUP_MASTER.Add(itemList);

            KSTU_ITEM_LIST_GROUP_MASTER primary = null;
            switch (level) {
                case 1:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level1_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
                case 2:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
                case 3:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
                case 4:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level4_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
                case 5:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level5_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
                case 6:
                    primary = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level6_id == parentID
                                                                                       && item.company_code == product.CompanyCode
                                                                                       && item.branch_code == product.BranchCode).FirstOrDefault();
                    break;
            }


            primary.ischild = "N";
            db.Entry(primary).State = System.Data.Entity.EntityState.Modified;

            if (product.CounterList.Count > 0) {
                if (product.CounterList.Count > 0) {
                    foreach (ItemCounterListVM counter in product.CounterList) {
                        KSTS_ITEM_COUNTER_LIST itemCounterList = new KSTS_ITEM_COUNTER_LIST();
                        itemCounterList.obj_id = SIGlobals.Globals.GetHashcode("KSTS_ITEM_COUNTER_LIST" + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode + product.GSCode + SIGlobals.Globals.Separator + itemName + SIGlobals.Globals.Separator + counter.CounterCode);
                        itemCounterList.company_code = product.CompanyCode;
                        itemCounterList.branch_code = product.BranchCode;
                        itemCounterList.gs_code = product.GSCode;
                        itemCounterList.item_name = itemName;
                        itemCounterList.counter_code = counter.CounterCode;
                        db.KSTS_ITEM_COUNTER_LIST.Add(itemCounterList);
                    }
                }
            }
            else {
                KSTS_ITEM_COUNTER_LIST itemCounterList = new KSTS_ITEM_COUNTER_LIST();
                itemCounterList.obj_id = SIGlobals.Globals.GetHashcode("KSTS_ITEM_COUNTER_LIST" + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode + product.GSCode + SIGlobals.Globals.Separator + itemName + SIGlobals.Globals.Separator + "NA");
                itemCounterList.company_code = product.CompanyCode;
                itemCounterList.branch_code = product.BranchCode;
                itemCounterList.gs_code = product.GSCode;
                itemCounterList.item_name = itemName;
                itemCounterList.counter_code = "NA";
                db.KSTS_ITEM_COUNTER_LIST.Add(itemCounterList);
            }

            KTTU_COUNTER_STOCK counterStock = new KTTU_COUNTER_STOCK();
            counterStock.item_name = itemName;
            counterStock.gs_code = product.GSCode;
            counterStock.company_code = product.CompanyCode;
            counterStock.branch_code = product.BranchCode;
            counterStock.op_units = 0;
            counterStock.date = SIGlobals.Globals.GetDateTime();
            counterStock.op_gwt = 0.000M;
            counterStock.op_nwt = 0.000M;
            counterStock.op_swt = 0.000M;
            counterStock.barcoded_units = 0;
            counterStock.barcoded_gwt = 0.000M;
            counterStock.barcoded_swt = 0.000M;
            counterStock.barcoded_nwt = 0.000M;
            counterStock.sales_units = 0;
            counterStock.sales_gwt = 0.000M;
            counterStock.sales_swt = 0.000M;
            counterStock.sales_nwt = 0.000M;
            counterStock.receipt_units = 0;
            counterStock.receipt_gwt = 0.000M;
            counterStock.receipt_swt = 0.000M;
            counterStock.receipt_nwt = 0.000M;
            counterStock.issues_units = 0;
            counterStock.issues_gwt = 0.000M;
            counterStock.issues_swt = 0.000M;
            counterStock.issues_nwt = 0.000M;
            counterStock.closing_units = 0;
            counterStock.closing_gwt = 0.000M;
            counterStock.closing_swt = 0.000M;
            counterStock.closing_nwt = 0.000M;
            counterStock.UniqRowID = Guid.NewGuid();

            if (product.CounterList.Count > 0) {
                for (int i = 0; i < product.CounterList.Count; i++) {
                    counterStock.counter_code = product.CounterList[i].CounterCode;
                    string temp = "KTTU_COUNTER_STOCK" + SIGlobals.Globals.Separator + product.GSCode + SIGlobals.Globals.Separator + itemName + SIGlobals.Globals.Separator + counterStock.counter_code + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode;
                    counterStock.obj_id = SIGlobals.Globals.GetHashcode(temp);
                    db.KTTU_COUNTER_STOCK.Add(counterStock);
                }
            }
            else {
                counterStock.counter_code = "NA";
                string temp = "KTTU_COUNTER_STOCK" + SIGlobals.Globals.Separator + product.GSCode + SIGlobals.Globals.Separator + itemName + SIGlobals.Globals.Separator + counterStock.counter_code + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode;
                counterStock.obj_id = SIGlobals.Globals.GetHashcode(temp);
                db.KTTU_COUNTER_STOCK.Add(counterStock);
            }

            KTTU_ITEM_STOCK itemStock = new KTTU_ITEM_STOCK();
            itemStock.item_name = itemName;
            itemStock.gs_code = product.GSCode;
            itemStock.company_code = product.CompanyCode;
            itemStock.branch_code = product.BranchCode;
            itemStock.op_units = 0;
            itemStock.op_gwt = 0.000M;
            itemStock.op_nwt = 0.000M;
            itemStock.op_swt = 0.000M;
            itemStock.sales_units = 0;
            itemStock.sales_gwt = 0.000M;
            itemStock.sales_swt = 0.000M;
            itemStock.sales_nwt = 0.000M;
            itemStock.barcoded_units = 0;
            itemStock.barcoded_gwt = 0.000M;
            itemStock.barcoded_swt = 0.000M;
            itemStock.barcoded_nwt = 0.000M;
            itemStock.receipt_units = 0;
            itemStock.receipt_gwt = 0.000M;
            itemStock.receipt_swt = 0.000M;
            itemStock.receipt_nwt = 0.000M;
            itemStock.issues_units = 0;
            itemStock.issues_gwt = 0.000M;
            itemStock.issues_swt = 0.000M;
            itemStock.issues_nwt = 0.000M;
            itemStock.closing_units = 0;
            itemStock.closing_gwt = 0.000M;
            itemStock.closing_swt = 0.000M;
            itemStock.closing_nwt = 0.000M;
            string itemStockObjID = "KTTU_ITEM_STOCK" + SIGlobals.Globals.Separator + itemStock.gs_code + SIGlobals.Globals.Separator + itemStock.item_name + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode;
            itemStock.obj_id = SIGlobals.Globals.GetHashcode(itemStockObjID);
            db.KTTU_ITEM_STOCK.Add(itemStock);

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

        public bool Update(string objID, ProductTreeVM product, out ErrorVM error)
        {
            error = null;
            KSTU_ITEM_LIST_GROUP_MASTER itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.obj_id == objID
                                                                            && item.company_code == product.CompanyCode
                                                                            && item.branch_code == product.BranchCode).FirstOrDefault();
            itemList.company_code = product.CompanyCode;
            itemList.branch_code = product.BranchCode;
            itemList.item_level1_name = product.ItemLevel1Name == null ? "NA" : product.ItemLevel1Name;
            itemList.gs_code = product.GSCode;
            itemList.item_level2_name = product.ItemLevel2Name == null ? "NA" : product.ItemLevel2Name;
            itemList.item_level3_name = product.ItemLevel3Name == null ? "NA" : product.ItemLevel3Name;
            itemList.item_level4_name = product.ItemLevel4Name == null ? "NA" : product.ItemLevel4Name;
            itemList.item_level5_name = product.ItemLevel5Name == null ? "NA" : product.ItemLevel5Name;
            itemList.item_level6_name = product.ItemLevel6Name == null ? "NA" : product.ItemLevel6Name;
            itemList.short_description = product.ShortDescription;
            itemList.ischild = product.IsChild == null ? "Y" : product.IsChild;
            itemList.min_units = product.MinUnits;
            itemList.min_stock_level = product.MinStockLevel;
            itemList.catalogId = product.CatalogId;
            itemList.grade = product.Grade;
            itemList.tagged = product.Tagged;
            itemList.stone = product.Stone;
            itemList.diamond = product.Diamond;
            itemList.counter_code = product.CounterCode;
            itemList.karat = product.karat;
            itemList.piece_item = product.PieceItem;
            itemList.obj_status = product.ObjStatus;
            itemList.alias_name = product.AliasName;
            itemList.UpdateOn = SIGlobals.Globals.GetDateTime();
            itemList.min_profit_percent = 0;
            itemList.QTY_LOCK = product.QtyLock;
            itemList.hallmark = product.Hallmark;
            itemList.certification = product.Certification;
            itemList.tcs_perc = product.TcsPerc;
            itemList.GSTGoodsGroupCode = product.GSTGoodsGroupCode;
            itemList.GSTServicesGroupCode = product.GSTServicesGroupCode;
            itemList.HSN = product.HSN;
            db.Entry(itemList).State = System.Data.Entity.EntityState.Modified;

            if (product.CounterList.Count > 0) {
                string itemName = GetItemName(itemList);
                List<KSTS_ITEM_COUNTER_LIST> lstOfItemCounterList = db.KSTS_ITEM_COUNTER_LIST.Where(i => i.company_code == product.CompanyCode
                                                                                             && i.branch_code == product.BranchCode
                                                                                             && i.gs_code == product.GSCode
                                                                                             && i.item_name == itemName).ToList();
                db.KSTS_ITEM_COUNTER_LIST.RemoveRange(lstOfItemCounterList);
                db.SaveChanges();
                foreach (ItemCounterListVM counter in product.CounterList) {
                    KSTS_ITEM_COUNTER_LIST itemCounterList = new KSTS_ITEM_COUNTER_LIST();
                    itemCounterList.obj_id = SIGlobals.Globals.GetHashcode("KSTS_ITEM_COUNTER_LIST" + SIGlobals.Globals.Separator + product.CompanyCode + SIGlobals.Globals.Separator + product.BranchCode + product.GSCode + SIGlobals.Globals.Separator + itemName + SIGlobals.Globals.Separator + counter.CounterCode);
                    itemCounterList.company_code = product.CompanyCode;
                    itemCounterList.branch_code = product.BranchCode;
                    itemCounterList.gs_code = product.GSCode;
                    itemCounterList.item_name = GetItemName(itemList);
                    itemCounterList.counter_code = counter.CounterCode;
                    db.KSTS_ITEM_COUNTER_LIST.Add(itemCounterList);
                }
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

        public bool Delete(string objID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            int level = 0;
            int id = 0;
            KSTU_ITEM_LIST_GROUP_MASTER itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.obj_id == objID
                                                                            && item.company_code == companyCode
                                                                            && item.branch_code == branchCode).FirstOrDefault();
            if (itemList == null) {
                error = new ErrorVM()
                {
                    description = "No Item to Delete",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            if (itemList.item_level6_id == 0 && itemList.item_level6_name == "NA") {
                level = 5;
                id = itemList.item_level5_id;
            }
            if (itemList.item_level5_id == 0 && itemList.item_level5_name == "NA") {
                level = 4;
                id = itemList.item_level4_id;
            }
            if (itemList.item_level4_id == 0 && itemList.item_level4_name == "NA") {
                level = 3;
                id = itemList.item_level3_id;
            }
            if (itemList.item_level3_id == 0 && itemList.item_level3_name == "NA") {
                level = 2;
                id = itemList.item_level2_id;
            }
            if (itemList.item_level2_id == 0 && itemList.item_level2_name == "NA") {
                level = 1;
                id = itemList.item_level1_id;
            }
            ErrorVM errorDel = new ErrorVM();
            bool deleted = Delete(level, id, companyCode, branchCode, out errorDel);
            if (deleted) {
                return true;
            }
            else {
                error = errorDel;
                return false;
            }
        }

        public bool Delete(int level, int ID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            KSTU_ITEM_LIST_GROUP_MASTER itemList = null;
            string itemName = string.Empty;
            switch (level) {
                case 2:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == ID
                                                                && item.company_code == companyCode
                                                                && item.branch_code == branchCode).FirstOrDefault();

                    if (itemList == null) {
                        return false;
                    }
                    itemName = itemList.item_level2_name;
                    break;
                case 3:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == ID
                                                                 && item.company_code == companyCode
                                                                 && item.branch_code == branchCode).FirstOrDefault();
                    if (itemList == null) {
                        return false;
                    }
                    itemName = itemList.item_level3_name;
                    break;
                case 4:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level4_id == ID
                                                                && item.company_code == companyCode
                                                                && item.branch_code == branchCode).FirstOrDefault();
                    if (itemList == null) {
                        return false;
                    }
                    itemName = itemList.item_level4_name;
                    break;
                case 5:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level5_id == ID
                                                                 && item.company_code == companyCode
                                                                 && item.branch_code == branchCode).FirstOrDefault();
                    if (itemList == null) {
                        return false;
                    }
                    itemName = itemList.item_level5_name;
                    break;
                case 6:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level6_id == ID
                                                                 && item.company_code == companyCode
                                                                 && item.branch_code == branchCode).FirstOrDefault();
                    if (itemList == null) {
                        return false;
                    }
                    itemName = itemList.item_level6_name;
                    break;
            }

            // Checking Barcoded Items for the Current GS, If exists we should not delete.
            List<KTTU_BARCODE_MASTER> barcodes = db.KTTU_BARCODE_MASTER.Where(b => b.company_code == companyCode
                                                                                           && b.branch_code == branchCode
                                                                                           && b.gs_code == itemList.item_level2_name).ToList();
            if (barcodes.Count > 0) {
                error = new ErrorVM()
                {
                    description = "Cannot delete this node.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            // Checking Counter Stock and Item Stock, If exists We should not delete.
            KTTU_COUNTER_STOCK counterStock = db.KTTU_COUNTER_STOCK.Where(s => s.company_code == companyCode && s.branch_code == branchCode && s.gs_code == itemList.gs_code && s.item_name == itemName).FirstOrDefault();
            KTTU_ITEM_STOCK itemStock = db.KTTU_ITEM_STOCK.Where(i => i.company_code == companyCode && i.branch_code == branchCode && i.gs_code == itemList.gs_code && i.item_name == itemName).FirstOrDefault();
            KSTS_GS_ITEM_ENTRY gs = db.KSTS_GS_ITEM_ENTRY.Where(g => g.company_code == companyCode && g.branch_code == branchCode && g.gs_code == itemList.gs_code).FirstOrDefault();
            if (gs.measure_type == "P") {
                if (counterStock != null) {
                    if (counterStock.op_units > 0 || counterStock.closing_units > 0 || counterStock.sales_units > 0
                   || counterStock.issues_units > 0 || counterStock.receipt_units > 0 || counterStock.barcoded_units > 0) {
                        error = new ErrorVM()
                        {
                            description = "Item has Stock,Can't delete this item.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                if (itemStock != null) {
                    if (itemStock.op_units > 0 || itemStock.closing_units > 0 || itemStock.sales_units > 0
                    || itemStock.issues_units > 0 || itemStock.receipt_units > 0 || itemStock.barcoded_units > 0) {
                        error = new ErrorVM()
                        {
                            description = "Item has Stock,Can't delete this item.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }
            else {
                if (counterStock != null) {
                    if (counterStock.receipt_gwt > 0 || counterStock.closing_gwt > 0 || counterStock.sales_gwt > 0 || counterStock.issues_gwt > 0
                    || counterStock.receipt_gwt > 0 || counterStock.closing_gwt > 0) {
                        error = new ErrorVM()
                        {
                            description = "Item has Stock,Can't delete this item.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
                if (itemStock != null) {
                    if (itemStock.receipt_gwt > 0 || itemStock.closing_gwt > 0 || itemStock.sales_gwt > 0 || itemStock.issues_gwt > 0
                    || itemStock.receipt_gwt > 0 || itemStock.closing_gwt > 0) {
                        error = new ErrorVM()
                        {
                            description = "Item has Stock,Can't delete this item.",
                            ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                        };
                        return false;
                    }
                }
            }

            KTTU_COUNTER_STOCK delCounterStock = db.KTTU_COUNTER_STOCK.Where(i => i.company_code == companyCode
                                                                    && i.branch_code == branchCode
                                                                    && i.gs_code == itemList.gs_code
                                                                    && i.item_name == itemName).FirstOrDefault();
            if (delCounterStock != null) {
                db.KTTU_COUNTER_STOCK.Remove(delCounterStock);
            }

            KTTU_ITEM_STOCK delItemStock = db.KTTU_ITEM_STOCK.Where(i => i.company_code == companyCode
                                                                     && i.branch_code == branchCode
                                                                     && i.gs_code == itemList.gs_code
                                                                     && i.item_name == itemName).FirstOrDefault();
            if (delCounterStock != null) {
                db.KTTU_ITEM_STOCK.Remove(delItemStock);
            }

            List<KSTS_ITEM_COUNTER_LIST> lstOfDelCounterList = db.KSTS_ITEM_COUNTER_LIST.Where(i => i.company_code == companyCode
                                                                    && i.branch_code == branchCode
                                                                    && i.gs_code == itemList.gs_code
                                                                    && i.item_name == itemName).ToList();
            if (lstOfDelCounterList != null && lstOfDelCounterList.Count > 0) {
                db.KSTS_ITEM_COUNTER_LIST.RemoveRange(lstOfDelCounterList);
            }
            db.KSTU_ITEM_LIST_GROUP_MASTER.Remove(itemList);

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
        private List<ProductTreeVM> GetInnerLevel2Items(int Level1ID)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level1_id == Level1ID && item.item_level2_id != 0).Select(item => item.item_level2_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level2ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == level2ID).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.ItemLevel1ID = kilgm.item_level2_id;
                    pt.ItemLevel1Name = kilgm.item_level2_name + "-" + kilgm.alias_name;
                    pt.innerLevelItems = GetInnerLevel3Items(level2ID);
                    lstOfProductTree.Add(pt);
                }

            }
            return lstOfProductTree;
        }

        private List<ProductTreeVM> GetInnerLevel3Items(int Level2ID)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == Level2ID && item.item_level3_id != 0).Select(item => item.item_level3_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level3ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == level3ID).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.ItemLevelID = kilgm.item_level3_id;
                    pt.ItemLevelName = kilgm.item_level3_name + "-" + kilgm.alias_name;
                    pt.innerLevelItems = GetInnerLevel4Items(level3ID);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        private List<ProductTreeVM> GetInnerLevel4Items(int level3ID)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == level3ID && item.item_level4_id != 0).Select(item => item.item_level4_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level4ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level4_id == level4ID).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.ItemLevelID = kilgm.item_level3_id;
                    pt.ItemLevelName = kilgm.item_level3_name + "-" + kilgm.alias_name;
                    //pt.innerLevelItems = GetInnerLevel5Items(level4ID);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        private List<ProductTreeVM> GetInnerLevel2Items2(int Level1ID, string companyCode, string branchCode)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level1_id == Level1ID && item.item_level2_id != 0).Select(item => item.item_level2_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level2ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == level2ID
                                                                                    && item.company_code == companyCode
                                                                                    && item.branch_code == branchCode).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.CompanyCode = companyCode;
                    pt.BranchCode = branchCode;
                    pt.ItemLevel1ID = kilgm.item_level1_id;
                    pt.ItemLevel1Name = kilgm.item_level1_name;
                    pt.GSCode = kilgm.gs_code;
                    pt.ItemLevel2Id = kilgm.item_level2_id;
                    pt.ItemLevel2Name = kilgm.item_level2_name;
                    pt.ItemLevel3Id = kilgm.item_level3_id;
                    pt.ItemLevel3Name = kilgm.item_level3_name;
                    pt.ItemLevel4Id = kilgm.item_level4_id;
                    pt.ItemLevel4Name = kilgm.item_level4_name;
                    pt.ItemLevel5Id = kilgm.item_level5_id;
                    pt.ItemLevel5Name = kilgm.item_level5_name;
                    pt.ItemLevel6Id = kilgm.item_level6_id;
                    pt.ItemLevel6Name = kilgm.item_level6_name;
                    pt.ShortDescription = kilgm.short_description;
                    pt.IsChild = kilgm.ischild;
                    pt.MinUnits = kilgm.min_units;
                    pt.MinStockLevel = kilgm.min_stock_level;
                    pt.CatalogId = kilgm.catalogId;
                    pt.Grade = kilgm.grade;
                    pt.Tagged = kilgm.tagged;
                    pt.Stone = kilgm.stone;
                    pt.Diamond = kilgm.diamond;
                    pt.CounterCode = kilgm.counter_code;
                    pt.karat = kilgm.karat;
                    pt.PieceItem = kilgm.piece_item;
                    pt.ObjStatus = kilgm.obj_status;
                    pt.AliasName = kilgm.alias_name;
                    pt.MinProfitPercent = kilgm.min_profit_percent;
                    pt.QtyLock = kilgm.QTY_LOCK;
                    pt.Hallmark = kilgm.hallmark;
                    pt.Certification = kilgm.certification;
                    pt.TcsPerc = kilgm.tcs_perc;
                    pt.GSTGoodsGroupCode = kilgm.GSTGoodsGroupCode;
                    pt.GSTServicesGroupCode = kilgm.GSTServicesGroupCode;
                    pt.HSN = kilgm.HSN;

                    pt.innerLevelItems = GetInnerLevel3Items3(level2ID, companyCode, branchCode);
                    pt.CounterList = GetCounterList(pt.GSCode, pt.ItemLevel2Name, companyCode, branchCode);
                    lstOfProductTree.Add(pt);
                }

            }
            return lstOfProductTree;
        }

        private List<ProductTreeVM> GetInnerLevel3Items3(int Level2ID, string companyCode, string branchCode)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_id == Level2ID && item.item_level3_id != 0).Select(item => item.item_level3_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level3ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == level3ID
                                                                                    && item.company_code == companyCode
                                                                                    && item.branch_code == branchCode).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.CompanyCode = companyCode;
                    pt.BranchCode = branchCode;
                    pt.ItemLevel1ID = kilgm.item_level1_id;
                    pt.ItemLevel1Name = kilgm.item_level1_name;
                    pt.GSCode = kilgm.gs_code;
                    pt.ItemLevel2Id = kilgm.item_level2_id;
                    pt.ItemLevel2Name = kilgm.item_level2_name;
                    pt.ItemLevel3Id = kilgm.item_level3_id;
                    pt.ItemLevel3Name = kilgm.item_level3_name;
                    pt.ItemLevel4Id = kilgm.item_level4_id;
                    pt.ItemLevel4Name = kilgm.item_level4_name;
                    pt.ItemLevel5Id = kilgm.item_level5_id;
                    pt.ItemLevel5Name = kilgm.item_level5_name;
                    pt.ItemLevel6Id = kilgm.item_level6_id;
                    pt.ItemLevel6Name = kilgm.item_level6_name;
                    pt.ShortDescription = kilgm.short_description;
                    pt.IsChild = kilgm.ischild;
                    pt.MinUnits = kilgm.min_units;
                    pt.MinStockLevel = kilgm.min_stock_level;
                    pt.CatalogId = kilgm.catalogId;
                    pt.Grade = kilgm.grade;
                    pt.Tagged = kilgm.tagged;
                    pt.Stone = kilgm.stone;
                    pt.Diamond = kilgm.diamond;
                    pt.CounterCode = kilgm.counter_code;
                    pt.karat = kilgm.karat;
                    pt.PieceItem = kilgm.piece_item;
                    pt.ObjStatus = kilgm.obj_status;
                    pt.AliasName = kilgm.alias_name;
                    pt.MinProfitPercent = kilgm.min_profit_percent;
                    pt.QtyLock = kilgm.QTY_LOCK;
                    pt.Hallmark = kilgm.hallmark;
                    pt.Certification = kilgm.certification;
                    pt.TcsPerc = kilgm.tcs_perc;
                    pt.GSTGoodsGroupCode = kilgm.GSTGoodsGroupCode;
                    pt.GSTServicesGroupCode = kilgm.GSTServicesGroupCode;
                    pt.HSN = kilgm.HSN;

                    pt.innerLevelItems = GetInnerLevel4Items4(level3ID, companyCode, branchCode);
                    pt.CounterList = GetCounterList(pt.GSCode, pt.ItemLevel3Name, companyCode, branchCode);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        private List<ProductTreeVM> GetInnerLevel4Items4(int level3ID, string companyCode, string branchCode)
        {
            List<ProductTreeVM> lstOfProductTree = new List<ProductTreeVM>();
            List<int> lstOfItems = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_id == level3ID && item.item_level4_id != 0).Select(item => item.item_level4_id).Distinct().ToList();
            if (lstOfItems.Count > 0) {
                for (int i = 0; i < lstOfItems.Count; i++) {
                    int level4ID = Convert.ToInt32(lstOfItems[i]);
                    KSTU_ITEM_LIST_GROUP_MASTER kilgm = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level4_id == level4ID
                                                                                    && item.company_code == companyCode
                                                                                    && item.branch_code == branchCode).FirstOrDefault();
                    ProductTreeVM pt = new ProductTreeVM();
                    pt.ObjID = kilgm.obj_id;
                    pt.CompanyCode = companyCode;
                    pt.BranchCode = branchCode;
                    pt.ItemLevel1ID = kilgm.item_level1_id;
                    pt.ItemLevel1Name = kilgm.item_level1_name;
                    pt.GSCode = kilgm.gs_code;
                    pt.ItemLevel2Id = kilgm.item_level2_id;
                    pt.ItemLevel2Name = kilgm.item_level2_name;
                    pt.ItemLevel3Id = kilgm.item_level3_id;
                    pt.ItemLevel3Name = kilgm.item_level3_name;
                    pt.ItemLevel4Id = kilgm.item_level4_id;
                    pt.ItemLevel4Name = kilgm.item_level4_name;
                    pt.ItemLevel5Id = kilgm.item_level5_id;
                    pt.ItemLevel5Name = kilgm.item_level5_name;
                    pt.ItemLevel6Id = kilgm.item_level6_id;
                    pt.ItemLevel6Name = kilgm.item_level6_name;
                    pt.ShortDescription = kilgm.short_description;
                    pt.IsChild = kilgm.ischild;
                    pt.MinUnits = kilgm.min_units;
                    pt.MinStockLevel = kilgm.min_stock_level;
                    pt.CatalogId = kilgm.catalogId;
                    pt.Grade = kilgm.grade;
                    pt.Tagged = kilgm.tagged;
                    pt.Stone = kilgm.stone;
                    pt.Diamond = kilgm.diamond;
                    pt.CounterCode = kilgm.counter_code;
                    pt.karat = kilgm.karat;
                    pt.PieceItem = kilgm.piece_item;
                    pt.ObjStatus = kilgm.obj_status;
                    pt.AliasName = kilgm.alias_name;
                    pt.MinProfitPercent = kilgm.min_profit_percent;
                    pt.QtyLock = kilgm.QTY_LOCK;
                    pt.Hallmark = kilgm.hallmark;
                    pt.Certification = kilgm.certification;
                    pt.TcsPerc = kilgm.tcs_perc;
                    pt.GSTGoodsGroupCode = kilgm.GSTGoodsGroupCode;
                    pt.GSTServicesGroupCode = kilgm.GSTServicesGroupCode;
                    pt.HSN = kilgm.HSN;

                    pt.CounterList = GetCounterList(pt.GSCode, pt.ItemLevel4Name, companyCode, branchCode);
                    lstOfProductTree.Add(pt);
                }
            }
            return lstOfProductTree;
        }

        private bool CheckItemExist(int itemLevel, int parentLevelID, string itemName, string gsCode)
        {
            KSTU_ITEM_LIST_GROUP_MASTER itemList = null;
            switch (itemLevel) {
                case 2:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level2_name == itemName
                                                                                       && item.gs_code == gsCode
                                                                                       && item.item_level1_id == parentLevelID).FirstOrDefault();
                    if (itemList != null) {
                        return true;
                    }
                    break;
                case 3:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level3_name == itemName
                                                                                       && item.gs_code == gsCode
                                                                                        && item.item_level2_id == parentLevelID).FirstOrDefault();
                    if (itemList != null) {
                        return true;
                    }
                    break;
                case 4:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level4_name == itemName
                                                                                       && item.gs_code == gsCode
                                                                                        && item.item_level3_id == parentLevelID).FirstOrDefault();
                    if (itemList != null) {
                        return true;
                    }
                    break;
                case 5:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level5_name == itemName
                                                                                       && item.gs_code == gsCode
                                                                                        && item.item_level4_id == parentLevelID).FirstOrDefault();
                    if (itemList != null) {
                        return true;
                    }
                    break;
                case 6:
                    itemList = db.KSTU_ITEM_LIST_GROUP_MASTER.Where(item => item.item_level6_name == itemName
                                                                                       && item.gs_code == gsCode
                                                                                        && item.item_level5_id == parentLevelID).FirstOrDefault();
                    if (itemList != null) {
                        return true;
                    }
                    break;
            }
            return false;
        }

        private List<ItemCounterListVM> GetCounterList(string gsCode, string itemName, string companyCode, string branchCode)
        {
            var data = (from cm in db.KSTU_COUNTER_MASTER
                        join pcm in db.KSTS_ITEM_COUNTER_LIST on cm.counter_code equals pcm.counter_code
                        where cm.obj_status == "O" && pcm.gs_code == gsCode && pcm.item_name == itemName
                        select new ItemCounterListVM()
                        {
                            ObjID = pcm.obj_id,
                            CompanyCode = pcm.company_code,
                            BranchCode = pcm.branch_code,
                            GSCode = pcm.gs_code,
                            ItemName = pcm.item_name,
                            CounterCode = pcm.counter_code,
                            CounterName = cm.counter_name

                        }).ToList();
            return data;
        }

        private string GetItemName(KSTU_ITEM_LIST_GROUP_MASTER product)
        {
            string itemName = string.Empty;
            if (product.item_level6_id == 0 && product.item_level6_name == "NA") {
                itemName = product.item_level5_name;
            }
            if (product.item_level5_id == 0 && product.item_level5_name == "NA") {
                itemName = product.item_level4_name;
            }
            if (product.item_level4_id == 0 && product.item_level4_name == "NA") {
                itemName = product.item_level3_name;
            }
            if (product.item_level3_id == 0 && product.item_level3_name == "NA") {
                itemName = product.item_level2_name;
            }
            if (product.item_level2_id == 0 && product.item_level2_name == "NA") {
                itemName = product.item_level1_name;
            }
            return itemName;
        }
        #endregion
    }
}
