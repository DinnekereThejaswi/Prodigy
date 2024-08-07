using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// Author: Mustureswara M M
/// Date: 18th June 2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    /// <summary>
    /// This Provides API's Related to IR Setup
    /// </summary>
    public sealed class IRSetupBL
    {
        #region Declaration
        private readonly MagnaDbEntities dbContext = null;
        public static readonly Lazy<IRSetupBL> irInstance = new Lazy<IRSetupBL>(() => new IRSetupBL());
        public static IRSetupBL GetInstance
        {
            get
            {
                return irInstance.Value;
            }
        }
        #endregion

        #region Constructor
        private IRSetupBL()
        {
            dbContext = new MagnaDbEntities();
        }
        #endregion

        #region Methods
        public dynamic GetIRTypes(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = dbContext.KSTU_VOUCHER_MASTER.Where(vou => vou.company_code == companyCode
                                                                && vou.branch_code == branchCode
                                                                && vou.obj_status == "O").Select(ir => new
                                                                {
                                                                    Code = ir.voucher_code,
                                                                    Name = ir.voucher_name
                                                                }).ToList();
                return data;

            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                throw;
            }
        }

        public dynamic GetIRSetup(string companyCode, string branchCode, string Type, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from irt in dbContext.KSTU_ISSUERECEIPTS_TYPES
                            join acm in dbContext.KSTU_ACC_LEDGER_MASTER on new
                            {
                                CompanyCode = irt.company_code,
                                BranchCode = irt.branch_code,
                                AccCode = irt.ac_code ?? 0
                            }
                            equals new
                            {
                                CompanyCode = acm.company_code,
                                BranchCode = acm.branch_code,
                                AccCode = acm.acc_code
                            }
                             into Details
                            from det in Details.DefaultIfEmpty()
                            where irt.company_code == companyCode && irt.branch_code == branchCode && irt.voucher_code == Type && irt.obj_status=="O"
                            select new IRSetupVM()
                            {
                                ObjID = irt.obj_id,
                                VoucherCode = irt.voucher_code,
                                IRCode = irt.ir_code,
                                IRName = irt.ir_name,
                                ACCode = irt.ac_code,
                                AccName = det.acc_name,
                                ObjStatus = irt.obj_status
                            }).ToList();
                return data;

            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                throw;
            }
        }

        public bool Save(IRSetupVM irSetup, out ErrorVM error)
        {
            error = null;
            KSTU_ISSUERECEIPTS_TYPES issueReceipt = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.company_code == irSetup.CompanyCode
                                                                                            && ir.branch_code == irSetup.BranchCode
                                                                                            && ir.ir_code == irSetup.IRCode).FirstOrDefault();
            if (issueReceipt != null) {
                error = new ErrorVM()
                {
                    description = "Details alredy Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                issueReceipt = new KSTU_ISSUERECEIPTS_TYPES();
                string temp = "KSTU_ISSUERECEIPTS_TYPES" + SIGlobals.Globals.Separator
                              + irSetup.VoucherCode + SIGlobals.Globals.Separator
                              + irSetup.IRCode + SIGlobals.Globals.Separator
                              + irSetup.CompanyCode + SIGlobals.Globals.Separator
                              + irSetup.BranchCode;
                issueReceipt.obj_id = SIGlobals.Globals.GetHashcode(temp);
                issueReceipt.company_code = irSetup.CompanyCode;
                issueReceipt.branch_code = irSetup.BranchCode;
                issueReceipt.voucher_code = irSetup.VoucherCode;
                issueReceipt.ir_code = irSetup.IRCode;
                issueReceipt.ir_name = irSetup.IRName;
                issueReceipt.ac_code = irSetup.ACCode;
                issueReceipt.obj_status = "O";
                issueReceipt.UpdateOn = SIGlobals.Globals.GetDateTime();
                issueReceipt.UniqRowID = Guid.NewGuid();
                dbContext.KSTU_ISSUERECEIPTS_TYPES.Add(issueReceipt);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool Update(string objID, IRSetupVM irSetup, out ErrorVM error)
        {
            error = null;
            KSTU_ISSUERECEIPTS_TYPES issueReceipt = dbContext.KSTU_ISSUERECEIPTS_TYPES.Where(ir => ir.company_code == irSetup.CompanyCode
                                                                                            && ir.branch_code == irSetup.BranchCode
                                                                                            && ir.ir_code == irSetup.IRCode
                                                                                            && ir.obj_id == objID).FirstOrDefault();
            if (issueReceipt == null) {
                error = new ErrorVM()
                {
                    description = "Details Not Exist.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                issueReceipt.obj_id = irSetup.ObjID;
                issueReceipt.company_code = irSetup.CompanyCode;
                issueReceipt.branch_code = irSetup.BranchCode;
                issueReceipt.voucher_code = irSetup.VoucherCode;
                issueReceipt.ir_code = irSetup.IRCode;
                issueReceipt.ir_name = irSetup.IRName;
                issueReceipt.ac_code = irSetup.ACCode;
                issueReceipt.obj_status = "O";
                issueReceipt.UpdateOn = SIGlobals.Globals.GetDateTime();
                dbContext.Entry(issueReceipt).State = System.Data.Entity.EntityState.Modified;
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
