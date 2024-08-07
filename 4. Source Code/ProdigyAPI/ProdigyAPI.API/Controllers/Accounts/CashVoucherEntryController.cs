using ProdigyAPI.BL.BusinessLayer.Accounts;
using ProdigyAPI.BL.ViewModel.Accounts;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

/// <summary>
/// Author:  Eshwar 
/// Date: 27/01/2020
/// </summary>
namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// Provides API's related to Cash Voucher Entry in Accounts
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/CashVoucherEntry")]
    public class CashVoucherEntryController : SIBaseApiController<AccVoucherTransactionsVM>, IBaseMasterActionController<AccVoucherTransactionsVM, AccVoucherTransactionsVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// Get List of MasterLedger
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MasterLedger/{companyCode}/{branchCode}")]
        [Route("MasterLedger")]
        public IHttpActionResult GetMasterLedger(string companyCode, string branchCode)
        {

            //var itemList = from i in db.KSTU_ACC_LEDGER_MASTER
            //               where i.company_code == companyCode && i.branch_code == branchCode
            //               && i.acc_type == "c" && i.obj_status != "C" && i.acc_code == 1 || i.acc_code == 2000117
            //               orderby i.acc_name
            //               select new
            //               {
            //                   AccountName = i.acc_name,
            //                   AccountCode = i.acc_code
            //               };
            //return Ok(itemList.ToList());

            var itemList = from i in db.KSTU_ACC_LEDGER_MASTER
                           where i.company_code == companyCode && i.branch_code == branchCode
                           && i.acc_type == "C" && i.obj_status != "C"
                           orderby i.acc_name
                           select new
                           {
                               AccountName = i.acc_name,
                               AccountCode = i.acc_code
                           };
            return Ok(itemList.ToList());
        }

        /// <summary>
        /// Get List of AccountName
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="accountCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}/{accountCode}")]
        [Route("AccountName")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode, string accountCode)
        {
            return Ok(db.Usp_CashVoucherAccName(companyCode, branchCode, accountCode));
        }

        /// <summary>
        /// Get List of Narration
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Narration/{companyCode}/{branchCode}")]
        [Route("Narration")]
        public IHttpActionResult GetNarration(string companyCode, string branchCode)
        {
            var result = from a in db.KSTU_ACC_NARRATION_MASTER
                         where a.company_code == companyCode && a.branch_code == branchCode
                         orderby a.narr_id
                         select new
                         {
                             NarrationID = a.narr_id,
                             Narration = a.narration
                         };
            return Ok(result.ToList());
        }

        /// <summary>
        /// Get List of TransactionType
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("TransactionType")]
        public IHttpActionResult GetVoucherType()
        {
            List<VoucherTypesVM> voucherTypes = new List<VoucherTypesVM>();
            voucherTypes.Add(new VoucherTypesVM()
            {
                Code = "PAY",
                Name = "Payment"
            });
            voucherTypes.Add(new VoucherTypesVM()
            {
                Code = "REC",
                Name = "Receipt"
            });

            return Ok(voucherTypes.ToList());
        }

        /// <summary>
        /// Get Last Voucher Number
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("LastVoucher/{companyCode}/{branchCode}")]
        [Route("LastVoucher")]
        public IHttpActionResult GetLastVoucherNo(string companyCode, string branchCode)
        {
            int lastVoucherNo = new CashVoucherEntryBL().GetLastVoucherNo(companyCode, branchCode);
            return Ok(new { LastVoucherNo = lastVoucherNo });
        }

        /// <summary>
        /// Get List of VoucherDetails
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tranType"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("VoucherDetails/{companyCode}/{branchCode}/{tranType}/{accCodeMaster}/{date}")]
        [Route("VoucherDetails")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, string tranType, int accCodeMaster, DateTime date)
        {

            //return Ok(new CashVoucherEntryBL().GetListOfEntry(companyCode, branchCode, tranType, accCodeMaster, date));

            var data = new CashVoucherEntryBL().GetListOfEntry(companyCode, branchCode, tranType, accCodeMaster, date);
            return Ok(data);
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(AccVoucherTransactionsVM v)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new CashVoucherEntryBL().SaveCashVourcherDetails(v, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Get Voucher Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCode"></param>
        /// <param name="accCodeMaster"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Voucher/{companyCode}/{branchCode}/{voucherNo}/{accCode}/{accCodeMaster}")]
        [Route("Voucher")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCode, int accCodeMaster)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new CashVoucherEntryBL().GetCashVoucherDetails(companyCode, branchCode, voucherNo, accCode, accCodeMaster, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Get Voucher Details.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="transType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}/{voucherNo}/{accCodeMaster}/{transType}")]
        [Route("Get")]
        public IHttpActionResult GetVoucher(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string transType)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new CashVoucherEntryBL().GetCashVoucherDetails(companyCode, branchCode, voucherNo, accCodeMaster, transType, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Update Voucher Details
        /// </summary>
        /// <param name="voucherNo"></param>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult UpdateVoucherDetails(int voucherNo, [FromBody] AccVoucherTransactionsVM voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new AccountsCommonBL().UpdateVourcherDetails(voucherNo, voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Delete the Voucher Details
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult DeleteVoucherDetails([FromBody] AccVoucherTransactionsVM voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            bool created = new AccountsCommonBL().DeleteVourcherDetails(voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok();
        }

        /// <summary>
        /// Print Contra Voucher
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="tranType"></param>
        /// <param name="accType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Print/{companyCode}/{branchCode}/{voucherNo}/{accCodeMaster}/{transType}/{accType}")]
        [Route("Print")]
        public IHttpActionResult Print(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string tranType, string accType)
        {
            ErrorVM error = new ErrorVM();
            string report = new AccountsCommonBL().Print(companyCode, branchCode, voucherNo, accCodeMaster, accType, tranType, out error);
            if (error != null) {
                return Content(HttpStatusCode.NotFound, error);
            }
            return Ok(report);
        }
        public IQueryable<AccVoucherTransactionsVM> List()
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Count(ODataQueryOptions<AccVoucherTransactionsVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, AccVoucherTransactionsVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
