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
    public class StockGroupBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods
        public List<StockGroupVM> GetAllStockGroup(string CompanyCode, string BranchCode)
        {
            return db.KSTU_METALS_MASTER.Select(s => new StockGroupVM
            {
                ObjID = s.obj_id,
                CompanyCode = s.company_code,
                BranchCode = s.branch_code,
                MetalCode = s.metal_code,
                MetalName = s.metal_name,
                ObjectStatus = s.obj_status,
                UpdateOn = s.UpdateOn
            }).OrderBy(c => c.MetalCode).ToList();
        }

        public bool Save(StockGroupVM stockGroup, out ErrorVM error)
        {
            error = null;
            KSTU_METALS_MASTER metalMaster = db.KSTU_METALS_MASTER.Where(mm => mm.metal_code == stockGroup.MetalCode).FirstOrDefault();
            if (metalMaster != null) {
                error = new ErrorVM()
                {
                    description = "Metal Code already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            KSTU_METALS_MASTER kmm = new KSTU_METALS_MASTER();
            kmm.obj_id = SIGlobals.Globals.GetNewGUID();
            kmm.company_code = stockGroup.CompanyCode;
            kmm.branch_code = stockGroup.BranchCode;
            kmm.metal_name = stockGroup.MetalName;
            kmm.metal_code = stockGroup.MetalCode;
            kmm.obj_status = "O";
            kmm.UpdateOn = SIGlobals.Globals.GetApplicationDate(stockGroup.CompanyCode, stockGroup.BranchCode);
            kmm.UniqRowID = Guid.NewGuid();
            db.KSTU_METALS_MASTER.Add(kmm);
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

        public bool Update(string objID, StockGroupVM stockGroup, out ErrorVM error)
        {
            error = null;
            KSTU_METALS_MASTER kmm = db.KSTU_METALS_MASTER.Where(d => d.obj_id == objID).FirstOrDefault();
            if (kmm == null) {
                return false;
            }
            kmm.company_code = stockGroup.CompanyCode;
            kmm.branch_code = stockGroup.BranchCode;
            kmm.metal_name = stockGroup.MetalName;
            kmm.obj_status = stockGroup.ObjectStatus;
            kmm.UpdateOn = SIGlobals.Globals.GetApplicationDate(stockGroup.CompanyCode, stockGroup.BranchCode);
            db.Entry(kmm).State = System.Data.Entity.EntityState.Modified;
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

    }
}
