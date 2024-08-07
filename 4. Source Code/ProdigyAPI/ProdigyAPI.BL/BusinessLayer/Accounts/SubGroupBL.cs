using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class SubGroupBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string ACC_MODULE_SEQ_ID = "01";
        private const string TABLE_NAME = "KSTU_ACC_GROUP_MASTER";
        #endregion

        #region Methods
        public List<SubGroupVM> GetAllMasterGroup(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_GROUP_MASTER.Where(g => g.company_code == companyCode && g.branch_code == branchCode && g.parent_group_id == 0)
                .Select(g => new SubGroupVM()
                {
                    GroupID = g.group_id,
                    GroupName = g.group_name
                }).ToList();
        }

        public IQueryable GetSubGroupList(string companyCode, string branchCode)
        {
            return (from pg in db.KSTU_ACC_GROUP_MASTER
                    join sg in db.KSTU_ACC_GROUP_MASTER
                    on new { Company = pg.company_code, Branch = pg.branch_code }
                    equals new { Company = sg.company_code, Branch = sg.branch_code }
                    where sg.company_code == companyCode && sg.branch_code == branchCode && sg.parent_group_id != 0 && sg.parent_group_id == pg.group_id
                    select new SubGroupVM()
                    {
                        ObjID = sg.obj_id,
                        CompanyCode = sg.company_code,
                        BranchCode = sg.branch_code,
                        GroupID = sg.group_id,
                        GroupName = sg.group_name,
                        ParentGroupName = pg.group_name,
                        Under = sg.group_type == "A" ? "Asset" : sg.group_type == "L" ? "Liabality" : sg.group_type == "I" ? "Income" : "Expnase",
                        GroupType = sg.group_type,
                        GroupDescription = sg.group_description,
                        IsTrading = pg.is_trading,
                        ObjStatus = sg.obj_status,
                        ParentGroupID = sg.parent_group_id,
                        NewGroupCode = sg.NewGroupCode,
                        NewSubGroupCode = sg.NewSubGroupCode
                    });
        }

        public bool SaveSubGroupDetails(SubGroupVM subGroup, out ErrorVM error)
        {
            error = null;
            KSTU_ACC_GROUP_MASTER groupMaster = db.KSTU_ACC_GROUP_MASTER.Where(gp => gp.group_name == subGroup.GroupName
                                                                              && gp.company_code == subGroup.CompanyCode
                                                                              && gp.branch_code == subGroup.BranchCode).FirstOrDefault();
            if (groupMaster != null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Specified Sub Group Name already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                int groupID = db.KSTS_ACC_SEQ_NOS.Where(acc => acc.obj_id == ACC_MODULE_SEQ_ID
                                                        && acc.company_code == subGroup.CompanyCode
                                                        && acc.branch_code == subGroup.BranchCode).FirstOrDefault().nextno;
                string objID = SIGlobals.Globals.GetMagnaGUID(TABLE_NAME, groupID, subGroup.CompanyCode, subGroup.BranchCode);
                KSTU_ACC_GROUP_MASTER mg = new KSTU_ACC_GROUP_MASTER();
                mg.obj_id = objID;
                mg.company_code = subGroup.CompanyCode;
                mg.branch_code = subGroup.BranchCode;
                mg.group_id = groupID;
                mg.group_name = subGroup.GroupName;
                mg.group_type = subGroup.GroupType;
                mg.group_description = subGroup.GroupDescription;
                mg.is_trading = subGroup.IsTrading;
                mg.obj_status = "O";
                mg.UpdateOn = SIGlobals.Globals.GetDateTime();
                mg.parent_group_id = subGroup.ParentGroupID;
                mg.UniqRowID = Guid.NewGuid();
                mg.NewGroupCode = subGroup.NewGroupCode;
                mg.NewSubGroupCode = subGroup.NewSubGroupCode;
                db.KSTU_ACC_GROUP_MASTER.Add(mg);
                SIGlobals.Globals.UpdateAccountSeqenceNumber(db, ACC_MODULE_SEQ_ID, mg.company_code, mg.branch_code);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool UpdateSubGroupDetails(string objID, SubGroupVM subGroup, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_ACC_GROUP_MASTER mg = db.KSTU_ACC_GROUP_MASTER.Where(g => g.obj_id == objID
                                                                        && g.company_code == subGroup.CompanyCode
                                                                        && g.branch_code == subGroup.BranchCode).FirstOrDefault();
                if (mg == null) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Sub Group Master",
                        ErrorStatusCode = System.Net.HttpStatusCode.NotFound
                    };
                    return false;
                }
                if (subGroup.ObjID != mg.obj_id) {
                    error = new ErrorVM()
                    {
                        index = 0,
                        description = "Invalid Sub Group Master",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                mg.obj_id = subGroup.ObjID;
                mg.company_code = subGroup.CompanyCode;
                mg.branch_code = subGroup.BranchCode;
                mg.group_name = subGroup.GroupName;
                mg.group_type = subGroup.GroupType;
                mg.group_description = subGroup.GroupDescription;
                mg.is_trading = subGroup.IsTrading;
                mg.obj_status = subGroup.ObjStatus;
                mg.UpdateOn = SIGlobals.Globals.GetDateTime();
                mg.parent_group_id = subGroup.ParentGroupID;
                mg.UniqRowID = Guid.NewGuid();
                mg.NewGroupCode = subGroup.NewGroupCode;
                mg.NewSubGroupCode = subGroup.NewSubGroupCode;
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
