
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    /// <summary>
    /// Provides API's for Company Master
    /// </summary>
    [Authorize]
    [RoutePrefix("api/Master/CompanyMaster")]
    public class CompanyMasterController : SIBaseApiController<CompanyVM>, IBaseMasterActionController<CompanyVM, CompanyVM>
    {

        #region Declaration
        ProdigyAPI.Model.MagnaDb.MagnaDbEntities db = new Model.MagnaDb.MagnaDbEntities();
        #endregion

        #region Controller Methods
        /// <summary>
        /// Get Company details
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("Get/{companyCode}/{branchCode}")]
        [ResponseType(typeof(CompanyVM))]
        public IHttpActionResult list(string companyCode, string branchCode)
        {
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(c => c.company_code == companyCode && c.branch_code == branchCode).FirstOrDefault();
            CompanyVM com = new CompanyVM();
            com.ObjID = company.obj_id;
            com.CompanyCode = company.company_code;
            com.BranchCode = company.branch_code;
            com.CompanyName = company.company_name;
            com.ShortName = company.short_name;
            com.Address1 = company.address1;
            com.Address2 = company.address2;
            com.Address3 = company.address3;
            com.city = company.city;
            com.State = company.state;
            com.PhoneNo = company.phone_no;
            com.FAX = company.fax_no;
            com.EMailID = company.email_id;
            com.MobileNo = company.mobile_no;
            com.PAN = company.pan_no;
            com.TinNo = company.tin_no;
            com.CSTNo = company.cst_no;
            com.Website = company.website;
            com.CompanyFooter = company.company_footer;
            com.DisplayNo = company.display_no;
            com.ObjectStatus = company.object_status;
            com.BranchName = company.branch_code;
            com.UpdateOn = company.UpdateOn;
            com.HOCODE = company.HOCODE;
            com.EDRegNo = company.ED_Reg_No;
            com.Header1 = company.Header1;
            com.Header2 = company.Header2;
            com.Header3 = company.Header3;
            com.Header4 = company.Header4;
            com.Header5 = company.Header5;
            com.Header6 = company.Header6;
            com.Header7 = company.Header7;
            com.Footer1 = company.Footer1;
            com.Footer2 = company.Footer2;
            com.DefaultCurrencyCode = company.default_currency_code;
            com.StateCode = company.state_code;
            com.CountryName = company.Country_name;
            com.CurrencyCode = company.Currency_code;
            return Ok(com);
        }

        /// <summary>
        /// Save company details.
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        [ResponseType(typeof(CompanyVM))]
        public IHttpActionResult Post(CompanyVM c)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            KSTU_COMPANY_MASTER company = new KSTU_COMPANY_MASTER();
            company.obj_id = Common.GetNewGUID();
            company.company_code = c.CompanyCode;
            company.branch_code = c.BranchCode;
            //  company.cust_id = db.KSTS_SEQ_NOS.Where(sq => sq.obj_id == ModuleSeqNo).First().nextno;
            company.Country_name = c.CountryName;
            company.short_name = c.ShortName;
            company.address1 = c.Address1;
            company.address2 = c.Address2;
            company.address3 = c.Address3;
            company.city = c.city;
            company.state = c.State;
            company.phone_no = c.PhoneNo;
            company.fax_no = c.FAX;
            company.email_id = c.EMailID;
            company.mobile_no = c.MobileNo;
            company.pan_no = c.PAN;
            company.tin_no = c.TinNo;
            company.cst_no = c.CSTNo;
            company.website = c.Website;
            company.company_footer = c.CompanyFooter;
            company.display_no = c.DisplayNo;
            company.object_status = c.ObjectStatus;
            company.branch_code = c.BranchCode;
            company.UpdateOn = c.UpdateOn;
            company.HOCODE = c.HOCODE;
            company.ED_Reg_No = c.EDRegNo;
            company.Header1 = c.Header1;
            company.Header2 = c.Header2;
            company.Header3 = c.Header3;
            company.Header4 = c.Header4;
            company.Header5 = c.Header5;
            company.Header6 = c.Header6;
            company.Header7 = c.Header7;
            company.Footer1 = c.Footer1;
            company.Footer2 = c.Footer2;
            company.default_currency_code = c.DefaultCurrencyCode;
            company.state_code = c.StateCode;
            company.Country_name = c.CountryName;
            company.Currency_code = c.CurrencyCode;
            db.KSTU_COMPANY_MASTER.Add(company);
            try {
                db.SaveChanges();
                //Framework.Common.UpdateSeqenceNumber(ModuleSeqNo);
                //c.ID = customer.cust_id;
                return CreatedAtRoute("DefaultApi", new
                {
                    id = company.company_code
                }, c);
                //var cust = Get(customer.cust_id);
                //return Content(HttpStatusCode.Accepted, cust);
            }
            catch (Exception excp) {
                throw excp;
            }
        }

        public IHttpActionResult Put(int id, CompanyVM t)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Update company details.
        /// </summary>
        /// <param name="ObjID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("put/{ObjID}")]
        [ResponseType(typeof(CompanyVM))]
        public IHttpActionResult Put(string ObjID, CompanyVM c)
        {
            //KSTU_CUSTOMER_MASTER customer = db.KSTU_CUSTOMER_MASTER.Find(id);
            KSTU_COMPANY_MASTER company = db.KSTU_COMPANY_MASTER.Where(cust => cust.obj_id == ObjID).FirstOrDefault();
            if (company == null) {
                return NotFound();
            }

            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (c.CompanyCode != company.company_code) {
                return BadRequest();
            }

            company.obj_id = company.obj_id;
            company.company_code = c.CompanyCode;
            company.branch_code = c.BranchCode;
            company.Country_name = c.CountryName;
            company.short_name = c.ShortName;
            company.address1 = c.Address1;
            company.address2 = c.Address2;
            company.address3 = c.Address3;
            company.city = c.city;
            company.state = c.State;
            company.phone_no = c.PhoneNo;
            company.fax_no = c.FAX;
            company.email_id = c.EMailID;
            company.mobile_no = c.MobileNo;
            company.pan_no = c.PAN;
            company.tin_no = c.TinNo;
            company.cst_no = c.CSTNo;
            company.website = c.Website;
            company.company_footer = c.CompanyFooter;
            company.display_no = c.DisplayNo;
            company.object_status = c.ObjectStatus;
            company.branch_code = c.BranchCode;
            company.UpdateOn = c.UpdateOn;
            company.HOCODE = c.HOCODE;
            company.ED_Reg_No = c.EDRegNo;
            company.Header1 = c.Header1;
            company.Header2 = c.Header2;
            company.Header3 = c.Header3;
            company.Header4 = c.Header4;
            company.Header5 = c.Header5;
            company.Header6 = c.Header6;
            company.Header7 = c.Header7;
            company.Footer1 = c.Footer1;
            company.Footer2 = c.Footer2;
            company.default_currency_code = c.DefaultCurrencyCode;
            company.state_code = c.StateCode;
            company.Country_name = c.CountryName;
            company.Currency_code = c.CurrencyCode;
            db.Entry(company).State = System.Data.Entity.EntityState.Modified;
            try {
                db.SaveChanges();
            }
            catch (Exception ex) {
                throw ex;
            }
            return Content(HttpStatusCode.Accepted, company);
        }

        public IHttpActionResult Count(ODataQueryOptions<CompanyVM> oDataOptions)
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

        public IQueryable<CompanyVM> List()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get Footer Information
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Footer/{companyCode}/{branchCode}")]
        public IHttpActionResult GetFooterDetails(string companyCode, string branchCode)
        {
            DateTime applicationDate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.gs_code == "NGO" && r.karat == "22K").FirstOrDefault().ondate;
            decimal goldRate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.gs_code == "NGO" && r.karat == "22K").FirstOrDefault().rate;
            decimal silverRate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.gs_code == "SL" && r.karat == "NA").FirstOrDefault().rate;

            FooterVM footer = new FooterVM();
            footer.CompanyName = "Sharaan Info Systems";
            footer.Place = "Bangalore";
            footer.GoldRate = goldRate;
            footer.SilverRate = silverRate;
            footer.ApplicationDate = applicationDate;
            return Ok(footer);
        }

        /// <summary>
        /// Get Dashboard Information
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DashBoard/{companyCode}/{branchCode}")]
        public IHttpActionResult GetDaashboardInformation(string companyCode, string branchCode)
        {
            decimal goldRate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.gs_code == "NGO" && r.karat == "22K").FirstOrDefault().rate;
            decimal silverRate = db.KSTU_RATE_MASTER.Where(r => r.company_code == companyCode && r.branch_code == branchCode && r.gs_code == "SL" && r.karat == "NA").FirstOrDefault().rate;
            DateTime applicationDate = SIGlobals.Globals.GetApplicationDate(companyCode, branchCode);
            DashboardLineItem sales = (from ksm in db.KTTU_SALES_MASTER
                                       join ksd in db.KTTU_SALES_DETAILS on
                                       new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.bill_no }
                                       equals new { Company = ksd.company_code, Branch = ksd.branch_code, BillNo = ksd.bill_no }
                                       where ksm.company_code == companyCode && ksm.branch_code == branchCode
                                                                             && System.Data.Entity.DbFunctions.TruncateTime(ksm.bill_date) == System.Data.Entity.DbFunctions.TruncateTime(applicationDate)
                                       group new { ksm, ksd }
                                                   by new
                                                   {
                                                       CompanyCode = ksm.company_code,
                                                       BranchCode = ksm.branch_code
                                                   } into g
                                       select new DashboardLineItem()
                                       {
                                           NetWt = g.Sum(e => e.ksd.nwt),
                                           StoneWt = g.Sum(e => e.ksd.swt),
                                           Amount = g.Sum(e => e.ksm.total_sale_amount)
                                       }).FirstOrDefault();

            DashboardLineItem orders = (from ksm in db.KTTU_ORDER_MASTER
                                        join ksd in db.KTTU_ORDER_DETAILS on
                                        new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.order_no }
                                        equals new { Company = ksd.company_code, Branch = ksd.branch_code, BillNo = ksd.order_no }
                                        where ksm.company_code == companyCode && ksm.branch_code == branchCode
                                                                              && System.Data.Entity.DbFunctions.TruncateTime(ksm.order_date) == System.Data.Entity.DbFunctions.TruncateTime(applicationDate)
                                        group new { ksm, ksd }
                                                    by new
                                                    {
                                                        CompanyCode = ksm.company_code,
                                                        BranchCode = ksm.branch_code
                                                    } into g
                                        select new DashboardLineItem()
                                        {
                                            NetWt = g.Sum(e => e.ksd.from_gwt),
                                            StoneWt = g.Sum(e => e.ksd.app_swt),
                                            Amount = g.Sum(e => e.ksm.advance_ord_amount)
                                        }).FirstOrDefault();

            DashboardLineItem purchase = (from ksm in db.KTTU_PURCHASE_MASTER
                                          join ksd in db.KTTU_PURCHASE_DETAILS on
                                          new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.bill_no }
                                          equals new { Company = ksd.company_code, Branch = ksd.branch_code, BillNo = ksd.bill_no }
                                          where ksm.company_code == companyCode && ksm.branch_code == branchCode
                                                                                && System.Data.Entity.DbFunctions.TruncateTime(ksm.p_date) == System.Data.Entity.DbFunctions.TruncateTime(applicationDate)
                                          group new { ksm, ksd }
                                                      by new
                                                      {
                                                          CompanyCode = ksm.company_code,
                                                          BranchCode = ksm.branch_code
                                                      } into g
                                          select new DashboardLineItem()
                                          {
                                              NetWt = g.Sum(e => e.ksd.nwt),
                                              StoneWt = g.Sum(e => e.ksd.swt),
                                              Amount = g.Sum(e => e.ksm.purchase_amount)
                                          }).FirstOrDefault();
            DashboardLineItem repair = (from ksm in db.KTTU_REPAIR_RECEIPT_MASTER
                                        join ksd in db.KTTU_REPAIR_RECEIPT_DETAILS on
                                        new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.Repair_no }
                                        equals new { Company = ksd.company_code, Branch = ksd.branch_code, BillNo = ksd.Repair_no }
                                        where ksm.company_code == companyCode && ksm.branch_code == branchCode
                                                                              && System.Data.Entity.DbFunctions.TruncateTime(ksm.repair_date) == System.Data.Entity.DbFunctions.TruncateTime(applicationDate)
                                        group new { ksm, ksd }
                                                    by new
                                                    {
                                                        CompanyCode = ksm.company_code,
                                                        BranchCode = ksm.branch_code
                                                    } into g
                                        select new DashboardLineItem()
                                        {
                                            NetWt = g.Sum(e => e.ksd.nwt),
                                            StoneWt = g.Sum(e => e.ksd.swt)
                                        }).FirstOrDefault();
            DashboardLineItem repairDelivery = (from ksm in db.KTTU_REPAIR_ISSUE_MASTER
                                                join ksd in db.KTTU_REPAIR_ISSUE_DETAILS on
                                                new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.issue_no }
                                                equals new { Company = ksd.company_code, Branch = ksd.branch_code, BillNo = ksd.issue_no }
                                                join kpd in db.KTTU_PAYMENT_DETAILS on
                                                new { Company = ksm.company_code, Branch = ksm.branch_code, BillNo = ksm.issue_no }
                                                equals new { Company = kpd.company_code, Branch = kpd.branch_code, BillNo = kpd.series_no }
                                                where ksm.company_code == companyCode && ksm.branch_code == branchCode
                                                                                      && System.Data.Entity.DbFunctions.TruncateTime(ksm.issue_date) == System.Data.Entity.DbFunctions.TruncateTime(applicationDate)
                                                                                      && kpd.trans_type == "RO"
                                                group new { ksm, ksd, kpd }
                                                            by new
                                                            {
                                                                CompanyCode = ksm.company_code,
                                                                BranchCode = ksm.branch_code
                                                            } into g
                                                select new DashboardLineItem()
                                                {
                                                    NetWt = g.Sum(e => e.ksd.nwt),
                                                    StoneWt = g.Sum(e => e.ksd.swt),
                                                    Amount = g.Sum(e => e.kpd.pay_amt)
                                                }).FirstOrDefault();
            return Ok(new Dashboard()
            {
                Sales = sales,
                Purchase = purchase,
                Orders = orders,
                Repair = repair,
                RepairDelivery = repairDelivery
            });
        }
        #endregion
    }
}


