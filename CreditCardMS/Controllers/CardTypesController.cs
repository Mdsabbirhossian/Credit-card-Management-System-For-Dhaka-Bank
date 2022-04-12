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
    public class CardTypesController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CardTypes
        public ActionResult Index()
        {
            return View(db.CardTypes.ToList());
        }

        // GET: CardTypes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // GET: CardTypes/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CardTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CardId,Name,LimitAmount,ServiceCharge")] CardType cardType)
        {
            if (ModelState.IsValid)
            {
                db.CardTypes.Add(cardType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(cardType);
        }

        // GET: CardTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // POST: CardTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CardId,Name,LimitAmount,ServiceCharge")] CardType cardType)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cardType).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(cardType);
        }

        // GET: CardTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardType cardType = db.CardTypes.Find(id);
            if (cardType == null)
            {
                return HttpNotFound();
            }
            return View(cardType);
        }

        // POST: CardTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CardType cardType = db.CardTypes.Find(id);
            db.CardTypes.Remove(cardType);
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
