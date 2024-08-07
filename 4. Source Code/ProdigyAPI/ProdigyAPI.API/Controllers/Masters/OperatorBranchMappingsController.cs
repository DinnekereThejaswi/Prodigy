using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using ProdigyAPI.Model.MagnaDb;

namespace ProdigyAPI.Controllers.Masters
{
    [RoutePrefix("api/Masters/OperatorBranchMapping")]
    public class OperatorBranchMappingsController : ApiController
    {
        private MagnaDbEntities db = new MagnaDbEntities();

        // GET: api/OperatorBranchMappings
        public IQueryable<OperatorBranchMapping> GetOperatorBranchMappings()
        {
            return db.OperatorBranchMappings;
        }

        // GET: api/OperatorBranchMappings/5
        [ResponseType(typeof(OperatorBranchMapping))]
        public IHttpActionResult Get(string operatorCode)
        {
            var operatorMapping = db.OperatorBranchMappings.Where(op => op.OperatorCode == operatorCode);
            if (operatorMapping == null)
                return NotFound();
            return Ok(operatorMapping.ToList());
        }

        // GET: api/OperatorBranchMappings/5
        [ResponseType(typeof(OperatorBranchMapping))]
        public IHttpActionResult Get(int id)
        {
            var operatorMapping = db.OperatorBranchMappings.Find(id);
            if (operatorMapping == null)
                return NotFound();
            return Ok(operatorMapping);
        }

        // PUT: api/OperatorBranchMappings/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Put(int id, OperatorBranchMapping operatorBranchMapping)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            if (id != operatorBranchMapping.ID) {
                return BadRequest();
            }
            operatorBranchMapping.UpdatedOn = ProdigyAPI.SIGlobals.Globals.GetDateTime();
            db.Entry(operatorBranchMapping).State = EntityState.Modified;

            try {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException) {
                if (!OperatorBranchMappingExists(id)) {
                    return NotFound();
                }
                else {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/OperatorBranchMappings
        [ResponseType(typeof(OperatorBranchMapping))]
        public IHttpActionResult Post(OperatorBranchMapping operatorBranchMapping)
        {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            operatorBranchMapping.UpdatedOn = ProdigyAPI.SIGlobals.Globals.GetDateTime();
            db.OperatorBranchMappings.Add(operatorBranchMapping);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = operatorBranchMapping.ID }, operatorBranchMapping);
        }

        // DELETE: api/OperatorBranchMappings/5
        [ResponseType(typeof(OperatorBranchMapping))]
        public IHttpActionResult Delete(int id)
        {
            OperatorBranchMapping operatorBranchMapping = db.OperatorBranchMappings.Find(id);
            if (operatorBranchMapping == null) {
                return NotFound();
            }

            db.OperatorBranchMappings.Remove(operatorBranchMapping);
            db.SaveChanges();

            return Ok(operatorBranchMapping);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OperatorBranchMappingExists(int id)
        {
            return db.OperatorBranchMappings.Count(e => e.ID == id) > 0;
        }
    }
}