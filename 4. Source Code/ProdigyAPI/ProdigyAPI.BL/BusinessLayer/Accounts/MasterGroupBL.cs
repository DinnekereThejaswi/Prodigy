using ProdigyAPI.BL.BusinessLayer.Masters;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Orders;
using ProdigyAPI.BL.ViewModel.Payment;
using ProdigyAPI.BL.ViewModel.Repair;
using ProdigyAPI.BL.ViewModel.Sales;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.SIGlobals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class MasterGroupBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string ACC_MODULE_SEQ_ID = "01";
        private const string TABLE_NAME = "KSTU_ACC_GROUP_MASTER";
        #endregion

        #region Controller Methods
        public List<OrderItemVM> GetGroupType(string companyCode, string branchCode)
        {
            List<OrderItemVM> groupType = new List<OrderItemVM>() {
                new OrderItemVM()
                {
                    GSCode = "A",
                    ItemName = "Asset"
                }, new OrderItemVM()
                {
                    GSCode = "L",
                    ItemName = "Liabality"
                }, new OrderItemVM()
                {
                    GSCode = "I",
                    ItemName = "Income"
                }, new OrderItemVM()
                {
                    GSCode = "E",
                    ItemName = "Expense"
                }
                };
            return groupType;
        }

        public IQueryable GetMasterGroupList(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_GROUP_MASTER.Where(g => g.company_code == companyCode && g.branch_code == branchCode && g.parent_group_id == 0)
                .Select(g => new MasterGroupVM()
                {
                    ObjID = g.obj_id,
                    CompanyCode = g.company_code,
                    BranchCode = g.branch_code,
                    GroupID = g.group_id,
                    GroupName = g.group_name,
                    Under = g.group_type == "A" ? "Asset" : g.group_type == "L" ? "Liabality" : g.group_type == "I" ? "Income" : "Expense",
                    GroupType = g.group_type,
                    GroupDescription = g.group_description,
                    IsTrading = g.is_trading,
                    ObjStatus = g.obj_status,
                    ParentGroupID = g.parent_group_id,
                    NewGroupCode = g.NewGroupCode,
                    NewSubGroupCode = g.NewSubGroupCode
                });
        }

        public bool SaveMasterGroupDetails(MasterGroupVM masterGroup, out ErrorVM error)
        {
            error = null;
            KSTU_ACC_GROUP_MASTER groupMaster = db.KSTU_ACC_GROUP_MASTER.Where(gp => gp.group_name == masterGroup.GroupName
                                                                              && gp.company_code == masterGroup.CompanyCode
                                                                              && gp.branch_code == masterGroup.BranchCode).FirstOrDefault();
            if (groupMaster != null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Specified Group Name already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                int groupID = db.KSTS_ACC_SEQ_NOS.Where(acc => acc.obj_id == ACC_MODULE_SEQ_ID
                                                        && acc.company_code == masterGroup.CompanyCode
                                                        && acc.branch_code == masterGroup.BranchCode).FirstOrDefault().nextno;
                string objID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, groupID, masterGroup.CompanyCode, masterGroup.BranchCode);
                KSTU_ACC_GROUP_MASTER mg = new KSTU_ACC_GROUP_MASTER();
                mg.obj_id = objID;
                mg.company_code = masterGroup.CompanyCode;
                mg.branch_code = masterGroup.BranchCode;
                mg.group_id = groupID;
                mg.group_name = masterGroup.GroupName;
                mg.group_type = masterGroup.GroupType;
                mg.group_description = masterGroup.GroupDescription;
                mg.is_trading = masterGroup.IsTrading;
                mg.obj_status = "O";
                mg.UpdateOn = SIGlobals.Globals.GetDateTime();
                mg.parent_group_id = masterGroup.ParentGroupID;
                mg.UniqRowID = Guid.NewGuid();
                mg.NewGroupCode = masterGroup.NewGroupCode;
                mg.NewSubGroupCode = masterGroup.NewSubGroupCode;
                db.KSTU_ACC_GROUP_MASTER.Add(mg);
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, ACC_MODULE_SEQ_ID, masterGroup.CompanyCode, masterGroup.BranchCode);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool UpdateMasterGroupDetails(string objID, MasterGroupVM masterGroup, out ErrorVM error)
        {
            error = null;
            if (objID == "") {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            if (masterGroup == null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                KSTU_ACC_GROUP_MASTER mg = db.KSTU_ACC_GROUP_MASTER.Where(g => g.obj_id == objID
                                                                        && g.company_code == masterGroup.CompanyCode
                                                                        && g.branch_code == masterGroup.BranchCode).FirstOrDefault();
                if (mg == null) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Group Master",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }
                if (masterGroup.GroupID != mg.group_id) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Group Master",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                mg.obj_id = masterGroup.ObjID;
                mg.company_code = masterGroup.CompanyCode;
                mg.branch_code = masterGroup.BranchCode;
                mg.group_name = masterGroup.GroupName;
                mg.group_type = masterGroup.GroupType;
                mg.group_description = masterGroup.GroupDescription;
                mg.is_trading = masterGroup.IsTrading;
                mg.obj_status = masterGroup.ObjStatus;
                mg.UpdateOn = SIGlobals.Globals.GetDateTime();
                mg.parent_group_id = masterGroup.ParentGroupID;
                mg.UniqRowID = Guid.NewGuid();
                mg.NewGroupCode = masterGroup.NewGroupCode;
                mg.NewSubGroupCode = masterGroup.NewSubGroupCode;
                db.Entry(mg).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion
    }
}
