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
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Accounts
{
    /// <summary>
    /// This provides API's for Bank Voucher Entry in Accounts.
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Accounts/BankVoucherEntry")]
    public class BankVoucherEntryController : SIBaseApiController<AccVoucherTransactionsVM>, IBaseMasterActionController<AccVoucherTransactionsVM, AccVoucherTransactionsVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region Controller Methods

        /// <summary>
        /// Get Types of Transacton Receipt/Payment
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Type")]
        public IHttpActionResult Type()
        {
            return Ok(new BankVoucherEntryBL().Type());
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
            int lastVoucherNo = new BankVoucherEntryBL().GetLastVoucherNo(companyCode, branchCode);
            return Ok(new { LastVoucherNo = lastVoucherNo });
        }


        /// <summary>
        /// Get List of AccountName
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}")]
        [Route("AccountName")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode)
        {
            ErrorVM error = new ErrorVM();
            var accountName = new BankVoucherEntryBL().GetListOfAccountName(companyCode, branchCode, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(accountName);
        }

        /// <summary>
        /// Get Bank Voucher Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="tranType"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}/{accCodeMaster}/{tranType}/{date}")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, int accCodeMaster, string tranType, DateTime date)
        {
            //return new BankVoucherEntryBL().List(companyCode, branchCode, accCodeMaster, tranType, date);
            ErrorVM error = new ErrorVM();
            var data = new BankVoucherEntryBL().List(companyCode, branchCode, accCodeMaster, tranType, date);
            return Ok(data);
        }

        /// <summary>
        /// Get Bank Voucher Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="tranType"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("PrintList/{companyCode}/{branchCode}/{tranType}/{date}")]
        public IQueryable GetVoucherDetailsForPrint(string companyCode, string branchCode, string tranType, DateTime date)
        {
            return new BankVoucherEntryBL().PrintList(companyCode, branchCode, tranType, date);
        }

        /// <summary>
        /// post all the details
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult Post(List<AccVoucherTransactionsVM> v)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new BankVoucherEntryBL().SaveBankVoucherEntry(v, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Get Master Ledger Details By Transaction Type
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="voucherNo"></param>
        /// <param name="accCodeMaster"></param>
        /// <param name="tranType"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Voucher/{companyCode}/{branchCode}/{voucherNo}/{accCodeMaster}/{tranType}")]
        [Route("Voucher")]
        public IHttpActionResult GetVoucherDetails(string companyCode, string branchCode, int voucherNo, int accCodeMaster, string tranType)
        {
            return Ok(new BankVoucherEntryBL().GetVoucherDetails(companyCode, branchCode, voucherNo, accCodeMaster, tranType));
        }

        /// <summary>
        /// Update Bank Voucher Details
        /// </summary>
        /// <param name="voucherNo"></param>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put")]
        public IHttpActionResult Put(int voucherNo, [FromBody] List<AccVoucherTransactionsVM> voucherDet)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            ErrorVM error = new ErrorVM();
            AccVoucherTransactionsVM createdAccVM = new AccountsCommonBL().UpdateVourcherDetails(voucherNo, voucherDet, out error);
            if (error != null) {
                return Content(error.ErrorStatusCode, error);
            }
            return Ok(createdAccVM);
        }

        /// <summary>
        /// Delete the Voucher Details
        /// </summary>
        /// <param name="voucherDet"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public IHttpActionResult DeleteVoucherDetails([FromBody] List<AccVoucherTransactionsVM> voucherDet)
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

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Post(AccVoucherTransactionsVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, AccVoucherTransactionsVM t)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
