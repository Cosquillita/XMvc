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
using $nameSpaceRoot$.Models;

namespace $nameSpace$.Controllers
{
    public class $modelNamePlural$ApiController : ApiController
    {
        private $dbContextName$ db = new $dbContextName$();

        // GET: api/$modelNamePlural$
        public IQueryable<$modelName$> Get$modelNamePlural$()
        {
            return db.$modelNamePlural$;
        }

        // GET: api/$modelNamePlural$/5
        [ResponseType(typeof($modelName$))]
        public IHttpActionResult Get$modelName$($pkDataType$ id)
        {
            $modelName$ $modelVariable$ = db.$modelNamePlural$.Find(id);
            if ($modelVariable$ == null)
            {
                return NotFound();
            }

            return Ok($modelVariable$);
        }

        // PUT: api/$modelNamePlural$/5
        [ResponseType(typeof(void))]
        public IHttpActionResult Put$modelName$($pkDataType$ id, $modelName$ $modelVariable$)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != $modelVariable$.$pkName$)
            {
                return BadRequest();
            }

            db.Entry($modelVariable$).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!$modelName$Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/$modelNamePlural$/5
        [ResponseType(typeof($modelName$))]
        public IHttpActionResult Post$modelName$($modelName$ $modelVariable$)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.$modelNamePlural$.Add($modelVariable$);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = $modelVariable$.$pkName$ }, $modelVariable$);
        }

        // DELETE: api/$modelNamePlural$/5
        [ResponseType(typeof($modelName$))]
        public IHttpActionResult Delete$modelName$(int id)
        {
            $modelName$ $modelVariable$ = db.$modelNamePlural$.Find(id);
            if ($modelVariable$ == null)
            {
                return NotFound();
            }

            db.$modelNamePlural$.Remove($modelVariable$);
            db.SaveChanges();

            return Ok($modelVariable$);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool $modelName$Exists($pkDataType$ id)
        {
            return db.$modelNamePlural$.Count(e => e.$pkName$ == id) > 0;
        }
    }
}