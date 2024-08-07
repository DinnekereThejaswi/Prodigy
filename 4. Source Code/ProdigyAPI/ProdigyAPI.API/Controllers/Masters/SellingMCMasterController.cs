using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.BL.ViewModel.Master;
using ProdigyAPI.Framework;
using ProdigyAPI.Handlers;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.OData.Query;

namespace ProdigyAPI.Controllers.Masters
{
    [RoutePrefix("api/Master/SellingMCMaster")]
    public class SellingMCMasterController : SIBaseApiController<SellingMCMasterVM>, IBaseMasterActionController<SellingMCMasterVM, SellingMCMasterVM>
    {
        #region Declaration
        MagnaDbEntities db = new MagnaDbEntities();
        #endregion
        private int GetAndUpdateNextNo(string companyCode, string branchCode)
        {
            var kstsSeqNos = db.KSTS_SEQ_NOS.Where(
                                rs => rs.company_code == companyCode
                                && rs.branch_code == branchCode
                                && rs.obj_id == "34").FirstOrDefault();
            if (kstsSeqNos == null)
                return 0;

            int nextBatchNo = kstsSeqNos.nextno;
            kstsSeqNos.nextno = nextBatchNo + 1;
            db.Entry(kstsSeqNos).State = System.Data.Entity.EntityState.Modified;

            return nextBatchNo;
        }

        #region all controller methods
        /// <summary>
        /// Get List Of SupplierNames
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SupplierNames/{companyCode}/{branchCode}")]
        [Route("SupplierNames")]
        public IHttpActionResult GetSupplier(string branchCode, string companyCode)
        {
            var result = db.KSTU_SUPPLIER_MASTER
                .Where(k => k.company_code == companyCode && k.branch_code == branchCode)
                  .Select(s => new SupplerVAmasterVM
                  {
                      SupplierCode = s.party_code,
                      SupplierName = (s.party_code + "-" + s.party_name)
                  }).OrderBy(r => r.SupplierName).ToList();
            return Ok(result);
        }
        /// <summary>
        /// Get List Of GSNames
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GSNames/{companyCode}/{branchCode}")]
        [Route("GSNames")]
        public IHttpActionResult GetGSNames(string branchCode, string companyCode)
        {
            var result = db.KSTS_GS_ITEM_ENTRY
                .Where(k => k.company_code == companyCode && k.branch_code == branchCode
                && k.bill_type == "S" && k.measure_type == "W" && k.object_status != "C")
                  .Select(s => new GSVAmasterVM
                  {
                      GSCode = s.gs_code,
                      GSName = s.item_level1_name
                  }).OrderBy(r => r.GSName).ToList();
            return Ok(result.ToList());
        }
        /// <summary>
        /// Get List Of ItemNames
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <param name="gsCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("ItemNames/{companyCode}/{branchCode}/{gsCode}")]
        [Route("ItemNames")]
        public IHttpActionResult GetDesignNames(string branchCode, string companyCode, string gsCode)
        {
            var result = db.ITEM_MASTER
                .Where(k => k.company_code == companyCode && k.branch_code == branchCode && k.gs_code == gsCode)
                  .Select(s => new ItemVAmasterVM
                  {
                      ItemCode = s.Item_code,
                      ItemName = (s.Item_code + "-" + s.Item_Name)
                  }).OrderBy(r => r.ItemCode).ToList();
            return Ok(result);
        }
        /// <summary>
        /// Get List Of DesignNames
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("DesignNames/{companyCode}/{branchCode}")]
        [Route("DesignNames")]
        public IHttpActionResult GetDesignNames(string branchCode, string companyCode)
        {
            var result = db.KTTU_DESIGN_MASTER
                .Where(k => k.company_code == companyCode && k.branch_code == branchCode)
                  .Select(s => new DesignVAmasterVM
                  {
                      DesignCode = s.design_code,
                      DesignName = (s.design_code + "-" + s.design_name)
                  }).OrderBy(r => r.DesignName).ToList();
            return Ok(result);
        }
        /// <summary>
        /// Get List Of MCTypes
        /// </summary>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("MCTypes/{companyCode}/{branchCode}")]
        [Route("MCTypes")]
        public IHttpActionResult GetMCTypes(string branchCode, string companyCode)
        {
            var result = db.KSTS_MC_TYPES
                .Where(k => k.company_code == companyCode && k.branch_code == branchCode)
                  .Select(s => new MCTypesVM
                  {
                      MCTypeID = s.mc_type_id,
                      MCTypeName = s.mc_type_name
                  }).OrderBy(r => r.MCTypeName).ToList();
            return Ok(result);
        }
        /// <summary>
        /// get SellingMC Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="supplierCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="designCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SellingMCMasterDetails/{companyCode}/{branchCode}/{supplierCode?}/{gsCode?}/{itemCode?}/{designCode?}")]
        [Route("SellingMCMasterDetails")]
        public IQueryable<SupplierDetails> GetSellingMCMasterDetails(string companyCode, string branchCode, string supplierCode = null, string gsCode = null, string itemCode = null, string designCode = null)
        {
            return SellingMCMasterDetails(companyCode, branchCode, supplierCode, gsCode, itemCode, designCode, true).AsQueryable();
        }

        /// <summary>
        /// Get Count Of Records based on Supplier code, gs Code, item Code and Design Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="supplierCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="designCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RecordsCount/{companyCode}/{branchCode}/{supplierCode?}/{gsCode?}/{itemCode?}/{designCode?}")]
        [Route("RecordsCount")]
        public IHttpActionResult GetCount(string companyCode, string branchCode, string supplierCode = null, string gsCode = null, string itemCode = null, string designCode = null)
        {
            var result = SellingMCMasterDetails(companyCode, branchCode, supplierCode, gsCode, itemCode, designCode, false);
            return Ok(new { RecordCount = result == null ? 0 : result.ToList().Count });
        }

        /// <summary>
        /// get SellingMC Details
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("SellingMCMasterDetailsByGSAndItem/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        [Route("SellingMCMasterDetailsByGSAndItem")]
        public IQueryable<SupplierDetails> GetSellingMCMasterDetailsByGSAndItem(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            return SellingMCMasterDetails(companyCode, branchCode, null, gsCode, itemCode, null, true).AsQueryable();
        }

        /// <summary>
        /// Get Count Of Records based on Supplier code, gs Code, item Code and Design Code.
        /// </summary>
        /// <param name="companyCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("RecordsCountByGs/{companyCode}/{branchCode}/{gsCode}/{itemCode}")]
        [Route("RecordsCountByGs")]
        public IHttpActionResult GetCountByGsAndItem(string companyCode, string branchCode, string gsCode, string itemCode)
        {
            var result = SellingMCMasterDetails(companyCode, branchCode, null, gsCode, itemCode, null, false);
            return Ok(new { RecordCount = result == null ? 0 : result.ToList().Count });
        }

        /// <summary>
        /// Get Fromweight
        /// </summary>
        /// <param name="supplierCode"></param>
        /// <param name="gsCode"></param>
        /// <param name="itemCode"></param>
        /// <param name="designCode"></param>
        /// <param name="branchCode"></param>
        /// <param name="companyCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Fromwt/{companyCode}/{branchCode}/{supplierCode}/{gsCode}/{itemCode}/{designCode}")]
        public IHttpActionResult GetFromwt(string supplierCode, string gsCode, string itemCode, string designCode, string branchCode, string companyCode)
        {
            decimal Fromweight = Convert.ToDecimal(0.001);
            var result = db.KSTU_VA_MASTER.Where(a => a.company_code == companyCode && a.branch_code == branchCode
                            && a.party_code == supplierCode && a.gs_code == gsCode && a.category_code == itemCode
                            && a.design_code == designCode).OrderByDescending(s => s.to_wt)
                .Select(s => new FromweightVM
                {
                    FromWt = s.to_wt + Fromweight
                }).FirstOrDefault();

            if (result == null) {
                return Ok(new FromweightVM() { FromWt = 0.001M });
            }
            return Ok(result);
        }
        /// <summary>
        /// save sellingMCDetails
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("post")]
        public IHttpActionResult Post([FromBody] SellingMCMasterVM v)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var validMCType = db.KSTS_MC_TYPES.Where(mc => mc.company_code == v.CompanyCode
                                && mc.branch_code == v.BranchCode && mc.mc_type_id.ToString() == v.TypeID).FirstOrDefault();
            if (validMCType == null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "TypeID", description = "MC Type ID is invalid or does not exist.", ErrorStatusCode = HttpStatusCode.BadRequest });
            }
            decimal Range = Convert.ToDecimal((v.FromWt + v.ToWt) / 2);
            var vm = db.KSTU_VA_MASTER.Where(a => a.company_code == v.CompanyCode && a.branch_code == v.BranchCode && a.gs_code == v.GSCode && a.category_code == v.ItemName && a.design_code == v.DesignCode && a.party_code == v.PartyCode && Range >= a.from_wt && Range < a.to_wt).OrderBy(s => s.from_wt).FirstOrDefault();
            if (vm != null) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "This range already exists!", ErrorStatusCode = HttpStatusCode.BadRequest });
            }
            int seqNo = 0;
            seqNo = GetAndUpdateNextNo(v.CompanyCode, v.BranchCode);
            if (seqNo == 0) {
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Failed to get number series.", ErrorStatusCode = HttpStatusCode.BadRequest });
            }
            KSTU_VA_MASTER kvm = new KSTU_VA_MASTER();
            kvm.obj_id = Common.GetNewGUID();
            kvm.company_code = v.CompanyCode;
            kvm.branch_code = v.BranchCode;
            kvm.sno = seqNo;
            kvm.gs_code = v.GSCode;
            kvm.category_code = v.ItemName;
            kvm.from_wt = v.FromWt;
            kvm.to_wt = v.ToWt;
            kvm.mc_amount = v.McAmount;
            kvm.mc_percent = v.McPercent;
            kvm.mc_per_gram = v.McPerGram;
            kvm.mc_per_piece = v.McPerPiece;
            kvm.wastage_grms = v.WastageGrms;
            kvm.wast_percentage = v.WastPercentage;
            kvm.type_id = v.TypeID;
            kvm.range = Range;
            kvm.party_code = v.PartyCode;
            kvm.design_code = v.DesignCode;
            kvm.obj_status = "O";
            kvm.valueadded = v.ValueAdded;
            db.KSTU_VA_MASTER.Add(kvm);
            try {
                db.SaveChanges();
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage, ErrorStatusCode = HttpStatusCode.InternalServerError });
            }

            return Ok();
        }

        public IQueryable<SellingMCMasterVM> List()
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Get Count of all VA Details
        /// </summary>
        /// <param name="oDataOptions"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Count")]
        public IHttpActionResult Count(ODataQueryOptions<SellingMCMasterVM> oDataOptions)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Get(int id)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Put(int id, SellingMCMasterVM t)
        {
            throw new NotImplementedException();
        }

        public IHttpActionResult Delete([FromBody] int id)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Private Methods

        private List<SupplierDetails> SellingMCMasterDetails(string companyCode, string branchCode, string supplierCode, string gsCode, string itemCode, string designCode, bool isData)
        {

            if (supplierCode != null && gsCode != null && itemCode != null && designCode != null) {
                var result = (from k in db.KSTU_VA_MASTER
                              where k.company_code.ToUpper() == companyCode && k.branch_code.ToUpper() == branchCode
                                 && k.party_code == (supplierCode.ToUpper() == "ALL" ? k.party_code : supplierCode)
                                 && k.gs_code.ToUpper() == gsCode
                                 && k.category_code.ToUpper() == itemCode
                                 && k.design_code.ToUpper() == (designCode.ToUpper() == "ALL" ? k.design_code : designCode)
                              orderby k.to_wt
                              select new SupplierDetails
                              {
                                  ObjID = k.obj_id,
                                  ItemName = k.category_code,
                                  DesignName = k.design_code,
                                  FromWt = k.from_wt,
                                  ToWt = k.to_wt,
                                  McPerPiece = k.mc_per_piece,
                                  McPerGram = k.mc_per_gram,
                                  McPercentage = k.mc_percent,
                                  ValueAdded = k.valueadded
                              });
                return result.ToList();

            }
            else if (itemCode != null && gsCode != null && supplierCode == null && designCode == null) {
                var result = (from k in db.KSTU_VA_MASTER
                              where k.company_code.ToUpper() == companyCode && k.branch_code.ToUpper() == branchCode
                                 && k.gs_code.ToUpper() == gsCode
                                 && k.category_code.ToUpper() == itemCode
                              orderby k.to_wt
                              select new SupplierDetails
                              {
                                  ObjID = k.obj_id,
                                  ItemName = k.category_code,
                                  DesignName = k.design_code,
                                  FromWt = k.from_wt,
                                  ToWt = k.to_wt,
                                  McPerPiece = k.mc_per_piece,
                                  McPerGram = k.mc_per_gram,
                                  McPercentage = k.mc_percent,
                                  ValueAdded = k.valueadded
                              });
                return result.ToList();
            }
            else if (supplierCode != null && itemCode == null && gsCode == null && designCode == null) {
                var result = (from k in db.KSTU_VA_MASTER
                              where k.company_code.ToUpper() == companyCode && k.branch_code.ToUpper() == branchCode
                                 && k.party_code == (supplierCode.ToUpper() == "ALL" ? k.party_code : supplierCode)
                              orderby k.to_wt
                              select new SupplierDetails
                              {
                                  ObjID = k.obj_id,
                                  ItemName = k.category_code,
                                  DesignName = k.design_code,
                                  FromWt = k.from_wt,
                                  ToWt = k.to_wt,
                                  McPerPiece = k.mc_per_piece,
                                  McPerGram = k.mc_per_gram,
                                  McPercentage = k.mc_percent,
                                  ValueAdded = k.valueadded
                              });
                return result.ToList();
            }
            return null;
        }
        #endregion
    }
    public class SupplierDetails
    {
        public string ObjID { get; set; }
        public string ItemName { get; set; }
        public string DesignName { get; set; }
        public decimal? FromWt { get; set; }
        public decimal? ToWt { get; set; }
        public decimal? McPerPiece { get; set; }
        public decimal? McPerGram { get; set; }
        public decimal? McPercentage { get; set; }
        public decimal? ValueAdded { get; set; }
    }

}
