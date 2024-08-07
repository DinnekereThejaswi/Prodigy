using ProdigyAPI.BL.ViewModel.Error;
using ProdigyAPI.Model.MagnaDb;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace ProdigyAPI.Controllers.Masters
{
    [RoutePrefix("api/Masters/Operator")]
    public class OperatorController : ApiController
    {
        MagnaDbEntities db = new MagnaDbEntities();

        [HttpGet]
        [Route("DefaultBranch/{operatorCode}")]
        [ResponseType(typeof(OperatorDefaultBranch))]
        public IHttpActionResult GetDefaultBranch(string operatorCode)
        {
            var assignedBranch = db.OperatorDefaultBranches.Where(op => op.OperatorCode == operatorCode).FirstOrDefault();
            return Ok(assignedBranch);
        }
        
        [HttpPut]
        [Route("DefaultBranch/{id}")]
        [ResponseType(typeof(void))]
        public IHttpActionResult PutDefaultBranch(int id, OperatorDefaultBranch operatorDefaultBranch)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != operatorDefaultBranch.ID) {
                return BadRequest();
            }
            operatorDefaultBranch.UpdatedOn = ProdigyAPI.SIGlobals.Globals.GetDateTime();
            db.Entry(operatorDefaultBranch).State = EntityState.Modified;

            try {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) {
                if (!OperatorDefaultBranchExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }

            return StatusCode(HttpStatusCode.NoContent);
        }


        [HttpPost]
        [Route("DefaultBranch")]
        [ResponseType(typeof(OperatorDefaultBranch))]
        public IHttpActionResult Post(OperatorDefaultBranch operatorDefaultBranch)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            operatorDefaultBranch.UpdatedOn = ProdigyAPI.SIGlobals.Globals.GetDateTime();
            db.OperatorDefaultBranches.Add(operatorDefaultBranch);
            try {
                db.SaveChanges();
            }
            catch (Exception exp) {
                string errorMessage = string.Empty;
                if (exp.InnerException.InnerException != null)
                    errorMessage = exp.InnerException.InnerException.Message;
                else
                    errorMessage = exp.Message;
                return Content(HttpStatusCode.BadRequest, new ErrorVM { index = 0, field = "", description = "Exception occurred: " + errorMessage });
            }
            
            return Ok(operatorDefaultBranch);
        }

        private bool OperatorDefaultBranchExists(int id)
        {
            return db.OperatorDefaultBranches.Count(e => e.ID == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
