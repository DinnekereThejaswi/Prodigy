using System;
using System.Linq;
using System.Web.Http;
using System.Net.Http;
using System.Web.Http.OData.Query;
using ProdigyAPI.BL.ViewModel.Master;
using System.Security.Claims;
using ProdigyAPI.BL.ViewModel.Error;
using System.Web.Http.ModelBinding;

namespace ProdigyAPI.Handlers
{
    public interface IBaseMasterActionController<ViewModelClass, ModelClass>
        where ViewModelClass : class
        where ModelClass : class
    {
        IQueryable<ViewModelClass> List();
        IHttpActionResult Count(ODataQueryOptions<ModelClass> oDataOptions);
        IHttpActionResult Get(int id);
        IHttpActionResult Post(ViewModelClass t);
        IHttpActionResult Put(int id, ViewModelClass t);
        IHttpActionResult Delete([FromBody] int id);
    }

    public interface IBaseTransactionActionController<ViewModelClass, ModelClass>
        where ViewModelClass : class
        where ModelClass : class
    {
        IQueryable<ViewModelClass> List();
        IHttpActionResult Count(ODataQueryOptions<ModelClass> oDataOptions);
        IHttpActionResult Get(int id);
        IHttpActionResult Post(ViewModelClass t);
        IHttpActionResult Put(int id, ViewModelClass t);
        IHttpActionResult Delete([FromBody] int id);
        IHttpActionResult PostDocument(int id);
        IHttpActionResult Print(object id);
    }


    public abstract class SIBaseApiController<T> : ApiController
        where T : class
    {
        private IQueryable<T> _t;
        /// <summary>
        /// CompanyCode that is sent in the request header
        /// </summary>
        protected string CompanyCode
        {
            get { return GetHeaderProperty("CompanyCode"); }
        }
        /// <summary>
        /// BranchCode that is sent in the request header
        /// </summary>
        protected string BranchCode
        {
            get { return GetHeaderProperty("BranchCode"); }
        }
        public SIBaseApiController(IQueryable<T> t)
        {
            _t = t;
        }

        public SIBaseApiController()
        {
        }

        protected IHttpActionResult GetCount(ODataQueryOptions<T> oDataOptions, IQueryable<T> t)
        {
            var rowCount = (oDataOptions.ApplyTo(t).AsQueryable()
                as IQueryable<T>).Count();
            return Ok(new { RecordCount = rowCount });
        }

        protected IHttpActionResult GetCount(ODataQueryOptions<T> oDataOptions)
        {
            var rowCount = (oDataOptions.ApplyTo(_t).AsQueryable()
                as IQueryable<T>).Count();
            return Ok(new { RecordCount = rowCount });
        }

        //internal IHttpActionResult GetCount(ODataQueryOptions<T> oDataOptions, IQueryable<T> queryable)
        //{
        //    var rowCount = (oDataOptions.ApplyTo(_t).AsQueryable()
        //        as IQueryable<T>).Count();
        //    return Ok(new { RecordCount = rowCount });
        //}

        protected string GetUserId()
        {
            try {
                ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
                string userId = principal.Claims.Where(c => c.Type == "UserID").FirstOrDefault().Value;
                return userId;
            }
            catch (Exception) {
                return string.Empty;
            }
        }

        protected string GetClaimProperty(string property)
        {
            try {
                ClaimsPrincipal principal = Request.GetRequestContext().Principal as ClaimsPrincipal;
                string claimValue = principal.Claims.Where(c => c.Type == property).FirstOrDefault().Value;
                return claimValue;
            }
            catch (Exception) {
                return "Not found";
            }
        }

        private string GetHeaderProperty(string propName)
        {
            if (Request == null)
                return null;
            var headers = Request.Headers;
            if (headers.Contains(propName)) {
                return headers.GetValues(propName).First();
            }
            else
                return null;
        }

        protected ErrorVM ParseModelErrors(ModelStateDictionary modelState)
        {
            var modelErrors = modelState.Keys
               .SelectMany(key => modelState[key].Errors.Select(x => new ModelErrorVM { Field = key, Message = x.ErrorMessage }))
               .ToList();
            return new ErrorVM
            {
                ErrorStatusCode = System.Net.HttpStatusCode.BadRequest,
                field = "ModelErrors",
                description = "Model errors occured. Please see ModelErrors attribute.",
                ModelErrors = modelErrors
            };
        }
    }
}