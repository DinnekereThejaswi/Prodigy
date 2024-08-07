using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class ToleranceMasterBL
    {
        MagnaDbEntities db = null;

        public ToleranceMasterBL()
        {
            db = new MagnaDbEntities(true);
        }

        public ToleranceMasterBL(MagnaDbEntities _dbContext)
        {
            db = _dbContext;
        }

        public List<ToleranceMasterVM> List(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var smm = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode).OrderBy(y => y.obj_id).ToList();
               
                if (smm == null || smm.Count <= 0) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }

                var menuVM = smm.Select(p => new ToleranceMasterVM
                {
                    ID = p.obj_id,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Description = p.Details,
                    MinValue = p.Min_val,
                    MaxValue = p.Max_Val
                }).ToList();
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public ToleranceMasterVM Get(string companyCode, string branchCode, int id, out ErrorVM error)
        {
            error = null;
            try {
                string subModuleId = id.ToString();
                var p = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == companyCode
                        && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (p == null) {
                    error = new ErrorVM { description = "No details found" };
                    return null;
                }
                var menuVM = new ToleranceMasterVM
                {
                    ID = p.obj_id,
                    CompanyCode = p.company_code,
                    BranchCode = p.branch_code,
                    Description = p.Details,
                    MinValue = p.Min_val,
                    MaxValue = p.Max_Val
                };
                return menuVM;
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return null;
            }
        }

        public bool Add(ToleranceMasterVM vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                if(vm.ID <= 0) {
                    error = new ErrorVM { description = "Tolerance Id must be a poistive number." };
                    return false;
                }

                var oldRecord = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.obj_id == vm.ID).FirstOrDefault();
                if (oldRecord != null) {
                    error = new ErrorVM { description = "The record with ID " + vm.ID + " already exists." };
                    return false;
                }
                #endregion

                #region Master
                KSTU_TOLERANCE_MASTER rm = new KSTU_TOLERANCE_MASTER
                {
                    obj_id = vm.ID,
                    company_code = vm.CompanyCode,
                    branch_code = vm.BranchCode,
                    Details = vm.Description,
                    Min_val = vm.MinValue,
                    Max_Val = vm.MaxValue,
                    UniqRowID = Guid.NewGuid()
                };
                db.KSTU_TOLERANCE_MASTER.Add(rm);
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Modify(ToleranceMasterVM vm, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validation
                var recordToUpdate = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == vm.CompanyCode && x.branch_code == vm.BranchCode
                    && x.obj_id == vm.ID).FirstOrDefault();
                if (recordToUpdate == null) {
                    error = new ErrorVM { description = "Tolerance Id " + vm.ID + " doesn't exist exist." };
                    return false;
                }
                #endregion

                #region Master
                
                recordToUpdate.Details = vm.Description;
                recordToUpdate.Min_val = vm.MinValue;
                recordToUpdate.Max_Val = vm.MaxValue;
                recordToUpdate.UniqRowID = Guid.NewGuid();
                db.Entry(recordToUpdate).State = System.Data.Entity.EntityState.Modified;
                #endregion

                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }

        public bool Delete(string companyCode, string branchCode, int id, string userID, out ErrorVM error)
        {
            error = null;
            try {
                #region Validations
                var record = db.KSTU_TOLERANCE_MASTER.Where(x => x.company_code == companyCode
                       && x.branch_code == branchCode && x.obj_id == id).FirstOrDefault();
                if (record == null) {
                    error = new ErrorVM { description = "No details found for the ID: " + id };
                    return false;
                }
                #endregion
                
                db.Entry(record).State = System.Data.Entity.EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM().GetErrorDetails(ex);
                return false;
            }
            return true;
        }
    }
}

