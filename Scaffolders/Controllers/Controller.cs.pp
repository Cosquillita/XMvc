using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Net;
using $nameSpaceRoot$.Models;
using $nameSpaceRoot$.Helpers;

namespace $nameSpace$.Controllers
{   
    public class $modelNamePlural$Controller : Controller
    {
		private $dbContextName$ db = new $dbContextName$();
		
        //
        // GET: /$modelName$/
        [HttpGet]
        public ActionResult Index(string filters = null, string sorts = null, int pageNum = 1, int pageSize = 10)
        {
            // gather parameters
            ViewBag.filters = filters;
            ViewBag.sorts = sorts;
            ViewBag.pageNum = pageNum;
            ViewBag.pageSize = pageSize;

            // set the table
            IQueryable<$modelName$> $modelNamePluralLower$ = db.$modelNamePlural$;

            // apply filtering
            if (!string.IsNullOrWhiteSpace(filters))
            {
                Dictionary<string, string> dFilters = new Dictionary<string, string>();
                List<string> asdf = filters.Split(Generics.ampersandd).ToList();
                foreach (string a in asdf)
                {
                    if (string.IsNullOrWhiteSpace(a)) continue;
                    List<string> aa = a.Split(Generics.equalSignn).ToList();
                    dFilters.Add(aa[0], aa[1]);
                }
                $modelNamePluralLower$ = LinqFilter.ApplyFilters($modelNamePluralLower$, dFilters);
            }

            // apply sorting
            if (!string.IsNullOrWhiteSpace(sorts))
            {
                Dictionary<string, string> dSorts = new Dictionary<string, string>();
                List<string> asdf = sorts.Split(Generics.ampersandd).ToList();
                foreach (string a in asdf)
                {
                    if (string.IsNullOrWhiteSpace(a)) continue;
                    List<string> aa = a.Split(Generics.equalSignn).ToList();
                    dSorts.Add(aa[0], aa[1]);
                }
                $modelNamePluralLower$ = LinqSorter.ApplySorts($modelNamePluralLower$, dSorts);
            }

            // apply paging
            int rowCount = $modelNamePluralLower$.ToList().Count();
            ViewBag.rowCount = rowCount;
            ViewBag.pageCount = (int)Math.Floor((decimal)(rowCount / pageSize)) + (rowCount % pageSize > 0 ? 1 : 0);
            ViewBag.paramz = null;
            if (string.IsNullOrWhiteSpace(sorts))
            {
                $modelNamePluralLower$ = LinqPager.GetPageByPageNumber($modelNamePluralLower$.OrderBy(m => m.$pkName$), pageNum, pageSize, rowCount);
            }
            else
            {
                $modelNamePluralLower$ = LinqPager.GetPageByPageNumber($modelNamePluralLower$, pageNum, pageSize, rowCount);
            }
            return View($modelNamePluralLower$.ToList());
        }

        // GET: $modelNamePlural$/Edit/5
        [HttpGet]
        public ActionResult Edit(int id)
        {
            if (id.GetType() == typeof(int))
            {
				if (id == 0)
					return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            $modelName$ $modelVariable$ = db.$modelNamePlural$.Find(id);
            if ($modelVariable$ == null)
            {
                return HttpNotFound();
            }

            var jsonEditForm = RenderRazor.RenderRazorViewToString
                    (ControllerContext, "Edit", $modelVariable$);

            return Json(jsonEditForm, JsonRequestBehavior.AllowGet);
        }

        // POST: $modelNamePlural$/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit($modelName$ $modelVariable$)
        {
            if (ModelState.IsValid)
            {
                db.Entry($modelVariable$).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View($modelVariable$);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) 
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}

