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
    public class CustomersController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: Customers
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                var customers = db.Customers.Include(c => c.CardType1);
                return View(customers.ToList());
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Customer customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                return View(customer);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Admin")
            {

                ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }


        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    CardStatu ob = new CardStatu();
                    Reward reward = new Reward();
                    customer.CustomerStatus = "Active";

                    db.Customers.Add(customer);
                    db.SaveChanges();

                    ob.CardNumber = customer.CardNumber;
                    ob.CardType = (int)customer.CardType;
                    ob.CustomerId = customer.CustomerId;
                    ob.CardStatus = "Active";
                    db.CardStatus.Add(ob);
                    db.SaveChanges();

                    reward.CustomerId = customer.CustomerId;
                    reward.RewardsPoint = 0;
                    reward.TotalAmount = 0;
                    reward.RewardDate = DateTime.Now.Date;
                    db.Rewards.Add(reward);
                    db.SaveChanges();

                    return RedirectToAction("Index");

            }

            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customer.CardType);
            return View(customer);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customer.CardType);
            return View(customer);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,Name,Present_Address,Parmanent_Address,Phone,Email,CardNumber,CardType,CustomerStatus")] Customer customer)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customer.CardType);
            return View(customer);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
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
