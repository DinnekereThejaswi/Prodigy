using System.Linq;
using System.Web.Http;
using System.Web.Http.OData.Query;

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
    }
}