using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
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

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// This provides API's related to GST Posting setup
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/GSTPostingSetUp")]
    public class GSTPostingSetUpController : SIBaseApiController<GSTPostingSetUpMasterVM>, IBaseMasterActionController<GSTPostingSetUpMasterVM, GSTPostingSetUpMasterVM>
    {

        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion

        #region all controller methods
        /// <summary>
        /// List Of GSTPostingSetUpMaster Details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("List/{companyCode}/{branchCode}")]
        public IQueryable<GSTPostingSetUpMasterVM> List(string companyCode, string branchCode)
        {
            var data = db.GSTPostingSetups.Where(g => g.company_code == companyCode && g.branch_code == branchCode)
                                      .Select(s => new GSTPostingSetUpMasterVM
                                      {
                                          ID = s.ID,
                                          GSTGroupCode = s.GSTGroupCode,
                                          GSTComponentCode = s.GSTComponentCode,
                                          GSTPercent = s.GSTPercent,
                                          CalculationOrder = s.CalculationOrder,
                                          EffectiveDate = s.EffectiveDate,
                                          ExpenseAccount = s.ExpenseAccount,
                                          IsRegistered = s.IsRegistered,
                                          LastModifiedBy = s.LastModifiedBy,
                                          LastModifiedOn = s.LastModifiedOn,
                                          PayableAccount = s.PayableAccount,
                                          ReceivableAccount = s.ReceivableAccount,
                                          RefundAccount = s.RefundAccount,
                                          CompanyCode = s.company_code,
                                          BranchCode = s.branch_code
                                      }).OrderByDescending(c => c.ID).ToList();

            if (data.Count > 0) {
                foreach (GSTPostingSetUpMasterVM gst in data) {
                    gst.PaybaleAccountName = GetAccountName(gst.PayableAccount, gst.CompanyCode, gst.BranchCode);
                    gst.ReceivableAccountName = GetAccountName(gst.ReceivableAccount, gst.CompanyCode, gst.BranchCode);
                }
            }
            return data.AsQueryable();
        }

        /// <summary>
        /// Get GST Group Code
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]

        [Route("GroupCode/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGroupCode(string companyCode, string branchCode)
        {
            var groupCode = db.GSTGroups.Where(gst => gst.company_code == companyCode && gst.branch_code == branchCode)
                                        .Select(g => new
                                        {
                                            Code = g.Code,
                                            Name = g.Code
                                        })
                                        .ToList();
            return Ok(groupCode);
        }

        /// <summary>
        /// Get GST Component and Calculation Order.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ComponentCode/{companyCode}/{branchCode}")]
        public IHttpActionResult GetGSTComponent(string companyCode, string branchCode)
        {
            var groupCode = db.GSTComponents.Where(gst => gst.company_code == companyCode && gst.branch_code == branchCode)
                                        .Select(g => new
                                        {
                                            Code = g.Code,
                                            Name = g.Code,
                                            CalculationOrder = g.CalculationOrder
                                        })
                                        .ToList();
            return Ok(groupCode);
        }

        /// <summary>
        /// Input and Output Account
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("InputOutputAccount/{companyCode}/{branchCode}")]
        public IHttpActionResult GeInputOutputAccount(string companyCode, string branchCode)
        {
            string[] notIn = { "B", "R", "C" };
            var accLeder = db.KSTU_ACC_LEDGER_MASTER.Where(l => l.company_code == companyCode
                                                                            && l.branch_code == branchCode
                                                                            && !notIn.Contains(l.acc_type))
                                                    .Select(g => new
                                                    {
                                                        AccCode = g.acc_code,
                                                        AccName = g.acc_name
                                                    })
                                                    .OrderBy(ord => ord.AccName)
                                                    .ToList();
            return Ok(accLeder);
        }

        /// <summary>
        /// Save GSTPostingSetUpMaster Details
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Post")]
        [ResponseType(typeof(GSTPostingSetUpMasterVM))]
        public IHttpActionResult Post(GSTPostingSetUpMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            GSTPostingSetup gstPostExist = db.GSTPostingSetups.Where(g => g.company_code == s.CompanyCode
                                                                    && g.branch_code == s.BranchCode
                                                                    && g.GSTGroupCode == s.GSTGroupCode
                                                                    && g.GSTComponentCode == s.GSTComponentCode).FirstOrDefault();
            if (gstPostExist != null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM()
                {
                    description = s.GSTGroupCode + "-" + s.GSTComponentCode + " Exist.",
                    ErrorStatusCode = HttpStatusCode.BadRequest
                });
            }

            GSTPostingSetup GSTpostsetup = new GSTPostingSetup();
            GSTpostsetup.GSTGroupCode = s.GSTGroupCode;
            GSTpostsetup.GSTComponentCode = s.GSTComponentCode;
            GSTpostsetup.GSTPercent = s.GSTPercent;
            GSTpostsetup.CalculationOrder = s.CalculationOrder;
            GSTpostsetup.EffectiveDate = s.EffectiveDate;
            GSTpostsetup.ExpenseAccount = s.ExpenseAccount;
            GSTpostsetup.IsRegistered = s.IsRegistered;
            GSTpostsetup.LastModifiedBy = s.LastModifiedBy;
            GSTpostsetup.LastModifiedOn = SIGlobals.Globals.GetDateTime();
            GSTpostsetup.PayableAccount = s.PayableAccount;
            GSTpostsetup.ReceivableAccount = s.ReceivableAccount;
            GSTpostsetup.RefundAccount = s.RefundAccount;
            GSTpostsetup.company_code = s.CompanyCode;
            GSTpostsetup.branch_code = s.BranchCode;
            GSTpostsetup.UniqRowID = new Guid();
            db.GSTPostingSetups.Add(GSTpostsetup);
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                throw excp;
            }
            return Ok();
        }


        /// <summary>
        /// Save GSTPostingSetUpMaster Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("Put/{id}")]
        [ResponseType(typeof(GSTPostingSetUpMasterVM))]
        public IHttpActionResult Update(int id, GSTPostingSetUpMasterVM s)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            GSTPostingSetup GSTpostsetup = db.GSTPostingSetups.Where(g => g.company_code == s.CompanyCode
                                                                    && g.branch_code == g.branch_code
                                                                    && g.ID == id).FirstOrDefault();
            GSTpostsetup.GSTGroupCode = s.GSTGroupCode;
            GSTpostsetup.GSTComponentCode = s.GSTComponentCode;
            GSTpostsetup.GSTPercent = s.GSTPercent;
            GSTpostsetup.CalculationOrder = s.CalculationOrder;
            GSTpostsetup.EffectiveDate = s.EffectiveDate;
            GSTpostsetup.ExpenseAccount = s.ExpenseAccount;
            GSTpostsetup.IsRegistered = s.IsRegistered;
            GSTpostsetup.LastModifiedBy = s.LastModifiedBy;
            GSTpostsetup.LastModifiedOn = SIGlobals.Globals.GetDateTime();
            GSTpostsetup.PayableAccount = s.PayableAccount;
            GSTpostsetup.ReceivableAccount = s.ReceivableAccount;
            GSTpostsetup.RefundAccount = s.RefundAccount;
            GSTpostsetup.company_code = s.CompanyCode;
            GSTpostsetup.branch_code = s.BranchCode;
            db.Entry(GSTpostsetup).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception excp) {
                throw excp;
            }
            return Ok();
        }

        public IHttpActionResult Count(ODataQueryOptions<GSTPostingSetUpMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, GSTPostingSetUpMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IQueryable<GSTPostingSetUpMasterVM> List()
        {
            throw new NotImplementedException();
        }

        private string GetAccountName(int? accCode, string companyCode, string branchCode)
        {
            KSTU_ACC_LEDGER_MASTER accLedger = db.KSTU_ACC_LEDGER_MASTER.Where(acc => acc.acc_code == accCode
                                                                                && acc.company_code == companyCode
                                                                                && acc.branch_code == branchCode).FirstOrDefault();
            return accLedger == null ? "" : accLedger.acc_name;
        }

        #endregion
    }
}
