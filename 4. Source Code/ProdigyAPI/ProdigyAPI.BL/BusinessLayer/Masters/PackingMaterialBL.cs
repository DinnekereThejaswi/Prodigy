using ProdigyAPI.BL.BusinessLayer.Masters;
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
    public class PackingMaterialBL
    {
        MagnaDbEntities db = new MagnaDbEntities();

        public List<PackingMaterialVM> GetAllPackingMaterial(string companyCode, string branchCode)
        {
            return db.KSTU_PACKING_DETAILS.Where(p => p.company_code == companyCode
                                                 && p.branch_code == branchCode).Select(p => new PackingMaterialVM()
                                                 {
                                                     ObjID = p.obj_id,
                                                     CompanyCode = p.company_code,
                                                     BranchCode = p.branch_code,
                                                     PCode = p.p_code,
                                                     PName = p.p_name,
                                                     MLength = p.m_length,
                                                     MLengthUOM = p.m_length_uom,
                                                     MHeight = p.m_height,
                                                     MHeightUOM = p.m_height_uom,
                                                     MWidth = p.m_width,
                                                     MWidthUOM = p.m_width_uom,
                                                     Color = p.color,
                                                     MWeight = p.m_weight,
                                                     MWeightUOM = p.m_weight_uom,
                                                     Remarks = p.remarks,
                                                     ObjStatus = p.obj_status
                                                 }).ToList();
        }

        public bool SavePackingMaterial(PackingMaterialVM pm, out ErrorVM error)
        {
            error = new ErrorVM();

            KSTU_PACKING_DETAILS packing = db.KSTU_PACKING_DETAILS.Where(p => p.p_code == pm.PCode
                                                            && p.company_code == pm.CompanyCode
                                                            && p.branch_code == pm.BranchCode).FirstOrDefault();
            if (packing != null) {
                error.ErrorStatusCode = System.Net.HttpStatusCode.BadRequest;
                error.description = "Packing code already Exist.";
                return false;
            }
            else {
                packing = new KSTU_PACKING_DETAILS();
            }
            packing.obj_id = SIGlobals.Globals.GetNewGUID();
            packing.company_code = pm.CompanyCode;
            packing.branch_code = pm.BranchCode;
            packing.p_code = pm.PCode;
            packing.p_name = pm.PName;
            packing.m_length = pm.MLength;
            packing.m_length_uom = pm.MLengthUOM;
            packing.m_height = pm.MHeight;
            packing.m_height_uom = pm.MHeightUOM;
            packing.m_width = pm.MWidth;
            packing.m_width_uom = pm.MWidthUOM;
            packing.color = pm.Color;
            packing.m_weight = pm.MWeight;
            packing.m_weight_uom = pm.MWeightUOM;
            packing.remarks = pm.Remarks;
            packing.obj_status = pm.ObjStatus;
            db.KSTU_PACKING_DETAILS.Add(packing);
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                error.description = excp.Message;
                return false;
            }
            return true;
        }

        public bool UpdatePackingMaterial(string objID, PackingMaterialVM pm, out ErrorVM error)
        {
            error = new ErrorVM();
            KSTU_PACKING_DETAILS packing = db.KSTU_PACKING_DETAILS.Where(p => p.obj_id == objID
                                                                    && p.company_code == pm.CompanyCode
                                                                    && p.branch_code == pm.BranchCode).FirstOrDefault();
            packing.obj_id = pm.ObjID;
            packing.company_code = pm.CompanyCode;
            packing.branch_code = pm.BranchCode;
            packing.p_name = pm.PName;
            packing.m_length = pm.MLength;
            packing.m_length_uom = pm.MLengthUOM;
            packing.m_width = pm.MWidth;
            packing.m_width_uom = pm.MWidthUOM;
            packing.color = pm.Color;
            packing.m_weight = pm.MWeight;
            packing.m_weight_uom = pm.MWeightUOM;
            packing.remarks = pm.Remarks;
            packing.obj_status = pm.ObjStatus;
            db.Entry(packing).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                error.ErrorStatusCode = System.Net.HttpStatusCode.InternalServerError;
                error.description = excp.Message;
                return false;
            }
            return true;
        }
    }
}
