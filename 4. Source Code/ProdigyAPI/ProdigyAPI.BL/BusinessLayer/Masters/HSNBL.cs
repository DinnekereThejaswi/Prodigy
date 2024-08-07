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
    public class HSNBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public dynamic GetHSNDetails(string companyCode, string branchCode)
        {
            return db.HSNSUCs.Where(s => s.company_code == companyCode && s.branch_code == branchCode).Select(s => new HSNUCVM()
            {
                Code = s.Code,
                GSTGroupCode = s.GSTGroupCode,
                Type = s.Type,
                Description = s.Description,
                IsActive = s.IsActive,
                ComapnyCode = s.branch_code,
                BranchCode = s.company_code
            }).ToList();
        }

        public bool Save(HSNUCVM hsn, out ErrorVM error)
        {
            error = null;
            HSNSUC hsnCheck = db.HSNSUCs.Where(h => h.Code == hsn.Code
                                                && h.company_code == hsn.ComapnyCode
                                                && h.branch_code == hsn.BranchCode).FirstOrDefault();
            if (hsnCheck != null) {
                error = new ErrorVM()
                {
                    description = string.Format("{0} already Exist.", hsn.Code),
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            HSNSUC hsnMaster = new HSNSUC();
            hsnMaster.Code = hsn.Code;
            hsnMaster.GSTGroupCode = hsn.GSTGroupCode;
            hsnMaster.Type = hsn.Type;
            hsnMaster.Description = hsn.Description;
            hsnMaster.IsActive = hsn.IsActive;
            hsnMaster.company_code = hsn.ComapnyCode;
            hsnMaster.branch_code = hsn.BranchCode;
            db.HSNSUCs.Add(hsnMaster);
            db.SaveChanges();
            return true;
        }

        public bool Update(HSNUCVM hsn, out ErrorVM error)
        {
            error = null;
            try {
                HSNSUC hsnMaster = db.HSNSUCs.Where(h => h.Code == hsn.Code
                                                && h.company_code == hsn.ComapnyCode
                                                && h.branch_code == hsn.BranchCode).FirstOrDefault();
                if (hsnMaster == null) {
                    error = new ErrorVM()
                    {
                        description = "Invalid Input Data",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }
                hsnMaster.Code = hsn.Code;
                hsnMaster.GSTGroupCode = hsn.GSTGroupCode;
                hsnMaster.Type = hsn.Type;
                hsnMaster.Description = hsn.Description;
                hsnMaster.IsActive = hsn.IsActive;
                hsnMaster.company_code = hsn.ComapnyCode;
                hsnMaster.branch_code = hsn.BranchCode;
                db.Entry(hsnMaster).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception excp) {
                error = new ErrorVM()
                {
                    description = excp.Message,
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
            }
            return true;
        }
        #endregion
    }
}
