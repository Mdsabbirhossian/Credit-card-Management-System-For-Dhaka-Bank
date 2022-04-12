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
    public class CardStatusController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CardStatus
        public ActionResult Index()
        {
            var cardStatus = db.CardStatus.Include(c => c.CardType1).Include(c => c.Customer);
            return View(cardStatus.ToList());
        }

        // GET: CardStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardStatu cardStatu = db.CardStatus.Find(id);
            if (cardStatu == null)
            {
                return HttpNotFound();
            }
            return View(cardStatu);
        }

        // GET: CardStatus/Create
        public ActionResult Create()
        {
            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name");
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
            return View();
        }

        // POST: CardStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CardStatusId,CustomerId,CardNumber,CardType,CardStatus")] CardStatu cardStatu)
        {
            if (ModelState.IsValid)
            {
                db.CardStatus.Add(cardStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", cardStatu.CardType);
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", cardStatu.CustomerId);
            return View(cardStatu);
        }

        // GET: CardStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardStatu cardStatu = db.CardStatus.Find(id);
            if (cardStatu == null)
            {
                return HttpNotFound();
            }
            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", cardStatu.CardType);
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", cardStatu.CustomerId);
            return View(cardStatu);
        }

        // POST: CardStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CardStatusId,CustomerId,CardNumber,CardType,CardStatus")] CardStatu cardStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cardStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", cardStatu.CardType);
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", cardStatu.CustomerId);
            return View(cardStatu);
        }

        // GET: CardStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CardStatu cardStatu = db.CardStatus.Find(id);
            if (cardStatu == null)
            {
                return HttpNotFound();
            }
            return View(cardStatu);
        }

        // POST: CardStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CardStatu cardStatu = db.CardStatus.Find(id);
            db.CardStatus.Remove(cardStatu);
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
