using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdigyAPI.BL.BusinessLayer.Accounts
{
    public class ChequeBL
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Methods

        #region Cheque Entry Module
        public dynamic List(string companyCode, string branchCode)
        {
            var data = (from cq in db.KSTU_ACC_CHEQUE_MASTER
                        join am in db.KSTU_ACC_LEDGER_MASTER
                        on new { Company = cq.company_code, Branch = cq.branch_code, AccCode = cq.acc_code }
                        equals new { Company = am.company_code, Branch = am.branch_code, AccCode = am.acc_code }
                        where cq.company_code == companyCode && cq.branch_code == branchCode
                        select new
                        {
                            AccCode = am.acc_code,
                            AccName = am.acc_name,
                            Nos = cq.no_of_chqs,
                            MaxAmt = cq.Max_amount,
                            StartNo = DbFunctions.Right("000000" + cq.chq_start_no, 6),
                            EndNo = DbFunctions.Right("000000" + cq.chq_end_no, 6)
                        }).ToList();
            return data.OrderBy(ac => ac.AccName).ThenBy(ac => ac.Nos).ThenBy(ac => ac.StartNo).ThenBy(ac => ac.EndNo).Distinct();
        }
        public bool Save(ChequeVM cheque, out ErrorVM error)
        {
            error = null;
            if (cheque == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            KSTU_ACC_CHEQUE_MASTER chequeExist = db.KSTU_ACC_CHEQUE_MASTER.Where(c => c.company_code == cheque.CompanyCode
                                                                                && c.branch_code == cheque.BranchCode
                                                                                && c.chq_no == cheque.ChqStartNo
                                                                                && c.acc_code == cheque.AccCode).FirstOrDefault();
            if (chequeExist != null) {
                error = new ErrorVM()
                {
                    description = "Cheque No already exist. Try with other number",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                for (int i = 0; i < cheque.NoOfChqs; i++) {
                    KSTU_ACC_CHEQUE_MASTER ch = new KSTU_ACC_CHEQUE_MASTER();
                    ch.obj_id = SIGlobals.Globals.GetNewGUID();
                    ch.company_code = cheque.CompanyCode;
                    ch.branch_code = cheque.BranchCode;
                    ch.acc_code = cheque.AccCode;
                    ch.chq_no = cheque.ChqStartNo + i;
                    ch.no_of_chqs = cheque.NoOfChqs;
                    ch.chq_start_no = cheque.ChqStartNo;
                    ch.chq_end_no = cheque.ChqEndNo;
                    ch.chq_issue_date = SIGlobals.Globals.GetApplicationDate(cheque.CompanyCode, cheque.BranchCode);
                    ch.chq_issued = "N";
                    ch.chq_closed = "N";
                    ch.chq_closed_by = null;
                    ch.UpdateOn = SIGlobals.Globals.GetDateTime();
                    ch.chq_closed_remarks = null;
                    ch.Max_amount = cheque.MaxAmount;
                    ch.UniqRowID = Guid.NewGuid();
                    db.KSTU_ACC_CHEQUE_MASTER.Add(ch);
                }
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }

        public bool Delete(ChequeVM cheque, out ErrorVM error)
        {
            error = null;
            if (cheque == null) {
                error = new ErrorVM()
                {
                    description = "Invalid Data",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {

                var data = db.KSTU_ACC_CHEQUE_MASTER.Where(cq => cq.company_code == cheque.CompanyCode
                                                                                       && cq.branch_code == cheque.BranchCode
                                                                                       && cq.chq_start_no >= cheque.ChqStartNo
                                                                                       && cq.chq_end_no == cheque.ChqEndNo
                                                                                       && cq.acc_code == cheque.AccCode
                                                                                       && cq.chq_issued == "Y");
                if (data.ToList().Count > 0) {
                    error = new ErrorVM()
                    {
                        description = "Some of the cheques has been used hence cannot be deleted.",
                        ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                    };
                    return false;
                }

                List<KSTU_ACC_CHEQUE_MASTER> chequeDet = db.KSTU_ACC_CHEQUE_MASTER.Where(ch => ch.company_code == cheque.CompanyCode
                                                            && ch.branch_code == cheque.BranchCode
                                                            && ch.acc_code == cheque.AccCode
                                                            && ch.chq_start_no == cheque.ChqStartNo
                                                            && ch.no_of_chqs == cheque.NoOfChqs).ToList();
                db.KSTU_ACC_CHEQUE_MASTER.RemoveRange(chequeDet);
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #region Cheque Closing Module
        public dynamic GetCheckClosingBank(string companyCode, string branchCode)
        {
            return (from cq in db.KSTU_ACC_CHEQUE_MASTER
                    join am in db.KSTU_ACC_LEDGER_MASTER
                    on new { Company = cq.company_code, Branch = cq.branch_code, AccCode = cq.acc_code }
                    equals new { Company = am.company_code, Branch = am.branch_code, AccCode = am.acc_code }
                    where cq.company_code == companyCode && cq.branch_code == branchCode
                    group new { cq } by new
                    {
                        AccCode = am.acc_code,
                        AccName = am.acc_name,
                    });
        }

        public dynamic GetCheckList(string companyCode, string branchCode, int accCode)
        {
            return (from ch in db.KSTU_ACC_CHEQUE_MASTER
                    where ch.company_code == companyCode && ch.branch_code == branchCode && ch.acc_code == accCode && ch.chq_issued != "Y"
                    select new
                    {
                        ChequeNo = DbFunctions.Right("000000" + ch.chq_no, 6),
                        ChqStatus = ch.chq_closed == "Y" ? "C" : "O",
                        ClosedRemarks = ch.chq_closed_remarks
                    }).OrderBy(s => s.ChequeNo);
        }

        public bool OpenOrCloseCheque(ChequeVM cheque, out ErrorVM error)
        {
            error = null;
            try {
                KSTU_ACC_CHEQUE_MASTER chequeDet = db.KSTU_ACC_CHEQUE_MASTER.Where(c => c.company_code == cheque.CompanyCode
                                                                                && c.branch_code == cheque.BranchCode
                                                                                && c.chq_no == cheque.ChqNo).FirstOrDefault();
                chequeDet.chq_closed = cheque.ChqClosed;
                chequeDet.chq_closed_by = cheque.ChqClosedBy;
                chequeDet.chq_closed_remarks = cheque.ChqClosedRemarks;
                chequeDet.UpdateOn = SIGlobals.Globals.GetDateTime();
                db.Entry(chequeDet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }
        }
        #endregion

        #endregion
    }
}
