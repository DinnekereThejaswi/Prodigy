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
    public class NarrationBL
    {
        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new MagnaDbEntities();
        public const string ACC_MODULE_SEQ_ID = "07";
        #endregion

        #region Methods
        public IQueryable GetAllNarrationList(string companyCode, string branchCode)
        {
            return db.KSTU_ACC_NARRATION_MASTER.Where(n => n.company_code == companyCode && n.branch_code == branchCode)
                .Select(n => new NarrationVM()
                {
                    ObjID = n.obj_id,
                    CompanyCode = n.company_code,
                    BranchCode = n.branch_code,
                    NarrID = n.narr_id,
                    Narration = n.narration
                });
        }

        public bool SaveNarrationDetails(NarrationVM narration, out ErrorVM error)
        {
            error = null;
            if (narration == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            KSTU_ACC_NARRATION_MASTER nar = db.KSTU_ACC_NARRATION_MASTER.Where(n => n.narration == narration.Narration
                                                                              && n.company_code == narration.CompanyCode
                                                                              && n.branch_code == narration.BranchCode).FirstOrDefault();
            if (nar != null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Specified Narration already Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                
                var narrID = db.KSTU_ACC_NARRATION_MASTER.Where(n => n.company_code == narration.CompanyCode
                                                                              && n.branch_code == narration.BranchCode).Max(x => x.narr_id);
                if (narrID == 0)
                    narrID = 1001;
                else
                    narrID = narrID + 1;
                string temp = "KSTU_ACC_NARRATION_MASTER" + SIGlobals.Globals.Separator
                    + narrID + SIGlobals.Globals.Separator
                    + narration.CompanyCode + SIGlobals.Globals.Separator
                    + narration.BranchCode;

                KSTU_ACC_NARRATION_MASTER nr = new KSTU_ACC_NARRATION_MASTER();
                nr.obj_id = SIGlobals.Globals.GetHashcode(temp);
                nr.company_code = narration.CompanyCode;
                nr.branch_code = narration.BranchCode;
                nr.narr_id = narrID;
                nr.narration = narration.Narration;
                nr.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.KSTU_ACC_NARRATION_MASTER.Add(nr);
                //SIGlobals.Globals.UpdateAccountSeqenceNumber(db, ACC_MODULE_SEQ_ID, nr.company_code, nr.branch_code);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool DeleteNarrationDetails(NarrationVM narration, out ErrorVM error)
        {
            error = null;
            if (narration == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            KSTU_ACC_NARRATION_MASTER nar = db.KSTU_ACC_NARRATION_MASTER.Where(n => n.narr_id == narration.NarrID
                                                                              && n.company_code == narration.CompanyCode
                                                                              && n.branch_code == narration.BranchCode).FirstOrDefault();
            if (nar == null) {
                error = new ErrorVM()
                {
                    index = 0,
                    description = "Invalid Narration Details.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }

            try {
                db.KSTU_ACC_NARRATION_MASTER.Remove(nar);
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
