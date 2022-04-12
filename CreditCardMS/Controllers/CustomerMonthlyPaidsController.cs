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
    public class CustomerMonthlyPaidsController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CustomerMonthlyPaids
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (Session["UserType"].ToString() == "Customer")
                {
                    var id = Convert.ToInt32(Session["UserId"].ToString());

                    var det = (from d in db.CustomerMonthlyPaids
                               where d.CustomerId == id
                               select d).ToList();
                    return View(det);
                }
                else
                {
                    var customerMonthlyPaids = db.CustomerMonthlyPaids.Include(c => c.Customer);
                    return View(customerMonthlyPaids.ToList());
                }

            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerMonthlyPaids/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerMonthlyPaid customerMonthlyPaid = db.CustomerMonthlyPaids.Find(id);
                if (customerMonthlyPaid == null)
                {
                    return HttpNotFound();
                }
                return View(customerMonthlyPaid);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }


        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name");
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }

        [HttpPost]

        public ActionResult Create(CustomerMonthlyPaid customerMonthlyPaid)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (ModelState.IsValid)
                {
                    db.CustomerMonthlyPaids.Add(customerMonthlyPaid);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerMonthlyPaid.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerMonthlyPaid.CustomerId);
              
                return View(customerMonthlyPaid);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerMonthlyPaids/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerMonthlyPaid customerMonthlyPaid = db.CustomerMonthlyPaids.Find(id);
                if (customerMonthlyPaid == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerMonthlyPaid.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerMonthlyPaid.CustomerId);
           
                return View(customerMonthlyPaid);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }


        [HttpPost]
        public ActionResult Edit(CustomerMonthlyPaid customerMonthlyPaid)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customerMonthlyPaid).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerMonthlyPaid.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerMonthlyPaid.CustomerId);
             
                return View(customerMonthlyPaid);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }

        // GET: CustomerMonthlyPaids/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerMonthlyPaid customerMonthlyPaid = db.CustomerMonthlyPaids.Find(id);
                if (customerMonthlyPaid == null)
                {
                    return HttpNotFound();
                }
                return View(customerMonthlyPaid);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // POST: CustomerMonthlyPaids/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                CustomerMonthlyPaid customerMonthlyPaid = db.CustomerMonthlyPaids.Find(id);
                db.CustomerMonthlyPaids.Remove(customerMonthlyPaid);
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
