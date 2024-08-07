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
/// Date: 28th June 2021
/// </summary>
namespace ProdigyAPI.BL.BusinessLayer
{
    /// <summary>
    /// This Business Layer provides Business Logic related to Card Charges Module.
    /// </summary>
    public sealed class CardChargesBL
    {
        #region Declaration
        private readonly MagnaDbEntities dbContext = new MagnaDbEntities();
        private static readonly Lazy<CardChargesBL> cardCharges = new Lazy<CardChargesBL>(() => new CardChargesBL());
        public static CardChargesBL GetInstance
        {
            get
            {
                return cardCharges.Value;
            }
        }
        #endregion

        #region Constructors
        private CardChargesBL()
        {

        }
        #endregion

        #region Public Methods
        public dynamic GetAccountNames(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.acc_type == "R"
                                                         && led.company_code == companyCode
                                                         && led.branch_code == branchCode).Select(led => new
                                                         {
                                                             Code = led.acc_code,
                                                             Name = led.acc_name
                                                         }).OrderBy(led => led.Code).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }

        }

        public dynamic GetBankNames(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                return dbContext.KSTU_ACC_LEDGER_MASTER.Where(led => led.acc_type == "B"
                                                         && led.obj_status == "O"
                                                         && led.company_code == companyCode
                                                         && led.branch_code == branchCode).Select(led => new
                                                         {
                                                             Code = led.acc_code,
                                                             Name = led.acc_name
                                                         }).OrderBy(led => led.Code).ToList();
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }
        }

        public dynamic GetCardCharges(string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            try {
                var data = (from kcc in dbContext.KTTS_CARD_COMMISSION
                            join kal in dbContext.KSTU_ACC_LEDGER_MASTER
                            on new
                            {
                                CompanyCode = kcc.company_code,
                                BranchCode = kcc.branch_code,
                                AccCode = kcc.acc_code
                            }
                            equals new
                            {
                                CompanyCode = kal.company_code,
                                BranchCode = kal.branch_code,
                                AccCode = kal.acc_code
                            }
                            where kcc.company_code == companyCode && kcc.branch_code == branchCode
                            select new CardCommissionVM()
                            {
                                ObjID = kcc.obj_id,
                                CompanyCode = kcc.company_code,
                                BranchCode = kcc.branch_code,
                                Bank = kcc.bank,
                                Charge = kcc.charge,
                                ServiceTax = kcc.service_tax,
                                AccCode = kcc.acc_code,
                                BankAccCode = kcc.bank_acc_code,
                                DispSeq = kcc.disp_seq,
                                CustCharge = kcc.CustCharge
                            }).ToList();
                return data;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return null;
            }

        }

        public bool Save(CardCommissionVM card, out ErrorVM error)
        {
            error = null;
            KTTS_CARD_COMMISSION cc = dbContext.KTTS_CARD_COMMISSION.Where(c => c.company_code == card.CompanyCode
                                                                            && c.branch_code == card.BranchCode
                                                                            && c.acc_code == card.AccCode
                                                                            && c.bank_acc_code == card.BankAccCode).FirstOrDefault();
            if (cc != null) {
                error = new ErrorVM()
                {
                    description = "Bank Details already found.",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                cc = new KTTS_CARD_COMMISSION();
                string temp = "KTTS_CARD_COMMISSION" + card.AccCode;
                cc.obj_id = SIGlobals.Globals.GetHashcode(temp);
                cc.company_code = card.CompanyCode;
                cc.branch_code = card.BranchCode;
                cc.bank = card.Bank;
                cc.charge = card.Charge;
                cc.service_tax = card.ServiceTax;
                cc.acc_code = card.AccCode;
                cc.bank_acc_code = card.BankAccCode;
                cc.disp_seq = card.DispSeq;
                cc.UpdateOn = card.UpdateOn;
                cc.CustCharge = card.CustCharge;
                dbContext.KTTS_CARD_COMMISSION.Add(cc);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }

        }

        public bool Update(string objID, CardCommissionVM card, out ErrorVM error)
        {
            error = null;
            KTTS_CARD_COMMISSION cc = dbContext.KTTS_CARD_COMMISSION.Where(c => c.company_code == card.CompanyCode
                                                                            && c.branch_code == card.BranchCode
                                                                            && c.obj_id == objID).FirstOrDefault();
            if (cc == null) {
                error = new ErrorVM()
                {
                    description = "Details not found",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                cc.obj_id = objID;
                cc.company_code = card.CompanyCode;
                cc.branch_code = card.BranchCode;
                cc.bank = card.Bank;
                cc.charge = card.Charge;
                cc.service_tax = card.ServiceTax;
                cc.acc_code = card.AccCode;
                cc.bank_acc_code = card.BankAccCode;
                cc.disp_seq = card.DispSeq;
                cc.UpdateOn = card.UpdateOn;
                cc.CustCharge = card.CustCharge;
                dbContext.Entry(cc).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }

        }

        public bool Delete(string objID, string companyCode, string branchCode, out ErrorVM error)
        {
            error = null;
            KTTS_CARD_COMMISSION cc = dbContext.KTTS_CARD_COMMISSION.Where(c => c.company_code == companyCode
                                                                            && c.branch_code == branchCode
                                                                            && c.obj_id == objID).FirstOrDefault();
            if (cc == null) {
                error = new ErrorVM()
                {
                    description = "Details not found",
                    ErrorStatusCode = System.Net.HttpStatusCode.BadRequest
                };
                return false;
            }
            try {
                dbContext.KTTS_CARD_COMMISSION.Remove(cc);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception excp) {
                error = new ErrorVM().GetErrorDetails(excp);
                return false;
            }

        }
        #endregion

        #region Private Methods
        #endregion
    }
}