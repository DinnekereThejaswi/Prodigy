using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProdigyAPI.Model.MagnaDb;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.BL.ViewModel.Error;

namespace ProdigyAPI.BL.BusinessLayer.Masters
{
    public class PaymentMasterBL
    {
        MagnaDbEntities db = null;

        public PaymentMasterBL()
        {
            db = new MagnaDbEntities();
        }

        public List<PaymentMasterVM> List(string companyCode, string branchCode)
        {
            var data = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode 
                && p.branch_code == branchCode).OrderBy(x => x.seq_no).ToList();
            List<PaymentMasterVM> paymentList = new List<PaymentMasterVM>();
            foreach(var record in data) {
                PaymentMasterVM pymt = new PaymentMasterVM
                {
                    PaymentCode = record.payment_code,
                    PaymentName = record.payment_name,
                    SeqNo = Convert.ToInt32(record.seq_no),
                    Active = record.obj_status == "O" ? true : false
                };
                paymentList.Add(pymt);
            }
            return paymentList;
        }

        public PaymentMasterVM Get(string companyCode, string branchCode, string paymentCode)
        {
            var record = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode
                && p.branch_code == branchCode && p.payment_code == paymentCode).FirstOrDefault();
            if (record != null) {
                PaymentMasterVM pymt = new PaymentMasterVM
                {
                    PaymentCode = record.payment_code,
                    PaymentName = record.payment_name,
                    SeqNo = Convert.ToInt32(record.seq_no),
                    Active = record.obj_status == "O" ? true : false
                };
                return pymt;
            }
            else
                return null;
        }

        public bool Add(string companyCode, string branchCode, PaymentMasterVM payment, out ErrorVM error)
        {
            error = null;
            if (payment == null) {
                error = new ErrorVM { description = "There is nothing to post. Please send valid data", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (string.IsNullOrEmpty(payment.PaymentCode)) {
                error = new ErrorVM { description = "PaymentCode cannot be null or empty.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            var existingPayment = Get(companyCode, branchCode, payment.PaymentCode);
            if(existingPayment != null) {
                error = new ErrorVM { description = $"The payment with payment code {payment.PaymentCode} already exists.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }

            if (string.IsNullOrEmpty(payment.PaymentName)) {
                error = new ErrorVM { description = "PaymentName cannot be null or empty.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if(payment.PaymentCode.Length > 5) {
                error = new ErrorVM { description = "Maximum length allowed for PaymentCode is 5.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (payment.PaymentName.Length > 30) {
                error = new ErrorVM { description = "Maximum length allowed for PaymentCode is 30.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }

            try {
                DateTime dateNow = DateTime.Now;
                KTTS_PAYMENT_MASTER newPayment = new KTTS_PAYMENT_MASTER
                {
                    company_code = companyCode, 
                    branch_code = branchCode,
                    payment_code = payment.PaymentCode,
                    payment_name = payment.PaymentName,
                    seq_no = payment.SeqNo,
                    obj_status = "O",
                    UpdateOn = dateNow,
                    obj_id = companyCode + "$" + branchCode + "$" + payment.PaymentCode,
                    UniqRowID = Guid.NewGuid()
                };
                db.KTTS_PAYMENT_MASTER.Add(newPayment);
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = ex.Message };
                return false;
            }
            return true;
        }

        public bool Modify(string companyCode, string branchCode, PaymentMasterVM payment, out ErrorVM error)
        {
            error = null;
            if (payment == null) {
                error = new ErrorVM { description = "There is nothing to post. Please send valid data", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (string.IsNullOrEmpty(payment.PaymentCode)) {
                error = new ErrorVM { description = "PaymentCode cannot be null or empty.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (string.IsNullOrEmpty(payment.PaymentName)) {
                error = new ErrorVM { description = "PaymentName cannot be null or empty.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (payment.PaymentCode.Length > 5) {
                error = new ErrorVM { description = "Maximum length allowed for PaymentCode is 5.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }
            if (payment.PaymentName.Length > 30) {
                error = new ErrorVM { description = "Maximum length allowed for PaymentCode is 30.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                return false;
            }

            try {
                DateTime dateNow = DateTime.Now;
                var existingPayment = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode
                    && p.branch_code == branchCode && p.payment_code == payment.PaymentCode).FirstOrDefault();
                if(existingPayment == null) {
                    error = new ErrorVM { description = $"The payment code {payment.PaymentCode} you are trying to modify does not exist.", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                existingPayment.payment_name = payment.PaymentName;
                existingPayment.seq_no = payment.SeqNo;
                existingPayment.obj_status = payment.Active == true ? "O" : null;
                db.Entry(existingPayment).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = ex.Message };
                return false;
            }
            return true;
        }

        public bool Delete(string companyCode, string branchCode, string paymentCode, out ErrorVM error)
        {
            error = null;
            try {
                var record = db.KTTS_PAYMENT_MASTER.Where(p => p.company_code == companyCode
                        && p.branch_code == branchCode && p.payment_code == paymentCode).FirstOrDefault();
                if (record == null) {
                    error = new ErrorVM { description = $"The {paymentCode} you are trying to delete does not exist", ErrorStatusCode = System.Net.HttpStatusCode.BadRequest };
                    return false;
                }
                db.KTTS_PAYMENT_MASTER.Remove(record);
                db.SaveChanges();
            }
            catch (Exception ex) {
                error = new ErrorVM { ErrorStatusCode = System.Net.HttpStatusCode.BadRequest, description = ex.Message };
                return false;
            }
            return true;
        }
    }
}
