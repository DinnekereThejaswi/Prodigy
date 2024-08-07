using ProdigyAPI.BL.BusinessLayer;
using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using System.Web.Http;

/// <summary>
/// Author: Mustureswara M M
/// Date: 28th June 2021
/// </summary>
namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This Provides API's related to Card Charges Module in Masters
    /// </summary>
    [Authorize]
    [RoutePrefix("api/CardCharges")]
    public class CardChargesController : ApiController
    {
        #region Declaration
        CardChargesBL cardCharges = CardChargesBL.GetInstance;
        ErrorVM error = new ErrorVM();
        #endregion

        #region Controller Methods
        /// <summary>
        /// This API returns all the Account Names
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("AccountName/{companyCode}/{branchCode}")]
        public IHttpActionResult GetAccountName(string companyCode, string branchCode)
        {
            var data = cardCharges.GetAccountNames(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API return all the Bank Names
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("BankName/{companyCode}/{branchCode}")]
        public IHttpActionResult GetBankName(string companyCode, string branchCode)
        {
            var data = cardCharges.GetBankNames(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API return all the Bank Names
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("CardCharges/{companyCode}/{branchCode}")]
        public IHttpActionResult GetCardCharges(string companyCode, string branchCode)
        {
            var data = cardCharges.GetCardCharges(companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API provides Save API Card Charges
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        public IHttpActionResult SaveCardCharges([FromBody] CardCommissionVM card)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var data = cardCharges.Save(card, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API provides Save API Card Charges
        /// </summary>
        /// <param name="objID"></param>
        /// <param name="card"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{objID}")]
        public IHttpActionResult UpdateCardCharges(string objID, [FromBody] CardCommissionVM card)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var data = cardCharges.Update(objID, card, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }

        /// <summary>
        /// This API provides Save API Card Charges
        /// </summary> 
        /// <param name="objID"></param>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("Delete/{objID}/{companyCode}/{branchCode}")]
        public IHttpActionResult DeleteCardCharges(string objID, string companyCode, string branchCode)
        {
            var data = cardCharges.Delete(objID, companyCode, branchCode, out error);
            if (error == null) {
                return Ok(data);
            }
            else {
                return Content(error.ErrorStatusCode, error);
            }
        }
        #endregion
    }
}
