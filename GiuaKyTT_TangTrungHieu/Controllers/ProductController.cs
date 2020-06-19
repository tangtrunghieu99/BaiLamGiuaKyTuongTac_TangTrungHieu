using System;
using GiuaKyTT_TangTrungHieu.EF;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using System.Data.Entity;

namespace GiuaKyTT_TangTrungHieu.Controllers
{
    public class ProductController : Controller
    {
        // GET: Product
        QLSPEntities db = new QLSPEntities();
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;
            var products = from s in db.Products
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s =>
               s.Name.ToUpper().Contains(searchString.ToUpper())
                ||
               s.Short_Name.ToUpper().Contains(searchString.ToUpper()));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    products = products.OrderByDescending(s => s.Name);
                    break;

                default: // Name ascending
                    products = products.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 5;
            int pageNumber = (page ?? 1);
            return View(products.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Create()
        {
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Code,Name,Short_name,Note,Category_ID")] Product p)
        {
            if (ModelState.IsValid)
            {
             
                db.Products.Add(p);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Category_ID = new SelectList(db.Categories, "ID", "Name",p.Category_ID);
            return View(p);
        }

        public ActionResult Edit(int id)
        {
            return View(db.Products.Find(id));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Code,Name,Short_name,Note")] Product p)
        {
            if (ModelState.IsValid)
            {
                p.Category_ID = 1;
                db.Entry(p).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(p);
        }

        public ActionResult Delete(int id)
        {
            var user = db.Products.Find(id);
            db.Products.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}