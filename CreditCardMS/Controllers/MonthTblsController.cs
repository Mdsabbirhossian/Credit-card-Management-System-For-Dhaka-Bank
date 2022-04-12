using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CreditCardMS.Models;

namespace CreditCardMS.Controllers
{
    public class MonthTblsController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: MonthTbls
        public ActionResult Index()
        {
            return View(db.MonthTbls.ToList());
        }

        // GET: MonthTbls/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonthTbl monthTbl = db.MonthTbls.Find(id);
            if (monthTbl == null)
            {
                return HttpNotFound();
            }
            return View(monthTbl);
        }

        // GET: MonthTbls/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MonthTbls/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "MonthId,Name,ShortOrder")] MonthTbl monthTbl)
        {
            if (ModelState.IsValid)
            {
                db.MonthTbls.Add(monthTbl);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(monthTbl);
        }

        // GET: MonthTbls/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonthTbl monthTbl = db.MonthTbls.Find(id);
            if (monthTbl == null)
            {
                return HttpNotFound();
            }
            return View(monthTbl);
        }

        // POST: MonthTbls/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "MonthId,Name,ShortOrder")] MonthTbl monthTbl)
        {
            if (ModelState.IsValid)
            {
                db.Entry(monthTbl).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(monthTbl);
        }

        // GET: MonthTbls/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MonthTbl monthTbl = db.MonthTbls.Find(id);
            if (monthTbl == null)
            {
                return HttpNotFound();
            }
            return View(monthTbl);
        }

        // POST: MonthTbls/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MonthTbl monthTbl = db.MonthTbls.Find(id);
            db.MonthTbls.Remove(monthTbl);
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
