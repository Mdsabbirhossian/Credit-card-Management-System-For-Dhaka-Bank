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
    public class CustomerAssignMoneysController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CustomerAssignMoneys
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (Session["UserType"].ToString() == "Customer")
                {
                    var id = Convert.ToInt32(Session["UserId"].ToString());

                    var det = (from d in db.CustomerAssignMoneys
                               where d.CustomerId == id
                               select d).ToList();
                    return View(det);
                }
                else
                {
                    var customerAssignMoneys = db.CustomerAssignMoneys.Include(c => c.CardType1).Include(c => c.Customer);
                    return View(customerAssignMoneys.ToList());
                }
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerAssignMoneys/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerAssignMoney customerAssignMoney = db.CustomerAssignMoneys.Find(id);
                if (customerAssignMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerAssignMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerAssignMoneys/Create
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Customer")
            {

                ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name");
                ViewBag.ApplyingMonth = new SelectList(db.MonthTbls, "MonthId", "Name");
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }


        [HttpPost]
        public ActionResult Create(CustomerAssignMoney customerAssignMoney)
        {
            if (Session["UserType"].ToString() == "Customer" )
            {
                ViewBag.ApplyingMonth = new SelectList(db.MonthTbls, "MonthId", "Name", customerAssignMoney.ApplyingMonth);
                ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customerAssignMoney.CardType);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerAssignMoney.CustomerId);

                if (ModelState.IsValid)
                {
                    if (customerAssignMoney.CardType == Convert.ToInt32(Session["CardType"].ToString()))
                    {
                        CardType ct = new CardType();
                        CustomerMonthlyPaid cmp = new CustomerMonthlyPaid();
                        var TotalMonthlyApply = 0;
                        var li = db.CardTypes.Find(customerAssignMoney.CardType);
                        var MaxAmount = li.LimitAmount;
                        var cusAg = db.CustomerAssignMoneys.Where(x => x.ApplyingMonth == customerAssignMoney.ApplyingMonth && x.CustomerId == customerAssignMoney.CustomerId).ToList();
                        TotalMonthlyApply = cusAg.Select(x => x.Amount).Sum();
                        var bothMoney = TotalMonthlyApply + customerAssignMoney.Amount;
                        var eligibleMoney = MaxAmount - TotalMonthlyApply;


                        if (customerAssignMoney.Amount > MaxAmount)
                        {
                            ViewBag.error = "Applying amount is not valid. Out of Range";
                            return View();
                        }
                        if (TotalMonthlyApply > MaxAmount)
                        {
                            ViewBag.error = "Dear Sir, Sorry your application for money is rejected.You already uses " + TotalMonthlyApply + " taka in this current month";
                            return View();
                        }

                        if (bothMoney > MaxAmount)
                        {
                            ViewBag.error = "Dear Sir, Sorry your application is rejected.You already uses " + TotalMonthlyApply + " taka in this current month. So you eligible for " + eligibleMoney + " taka";
                            return View();
                        }


                        else
                        {
                            db.CustomerAssignMoneys.Add(customerAssignMoney);
                            db.SaveChanges();

                            cmp.CustomerId = customerAssignMoney.CustomerId;
                            cmp.CardNo = customerAssignMoney.CardNo;
                            cmp.Month = customerAssignMoney.ApplyingMonth;
                            cmp.ApplyingMoney = customerAssignMoney.Amount;
                            cmp.ConsumeMoney = 0;
                            cmp.PaidMoney = 0;
                            cmp.ServiceCharge = 0;
                            cmp.Fine = 0;
                            cmp.PaidStatus = "Running";
                            db.CustomerMonthlyPaids.Add(cmp);
                            db.SaveChanges();

                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.error = "Sorry, You selected wrong Card.Please check your Card Type";
                        return View();
                    }

                }


                return View(customerAssignMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerAssignMoneys/Edit/5
        public ActionResult Edit(int? id)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerAssignMoney customerAssignMoney = db.CustomerAssignMoneys.Find(id);
                if (customerAssignMoney == null)
                {
                    return HttpNotFound();
                }
                ViewBag.ApplyingMonth = new SelectList(db.MonthTbls, "MonthId", "Name", customerAssignMoney.ApplyingMonth);
                ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customerAssignMoney.CardType);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerAssignMoney.CustomerId);
                return View(customerAssignMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost]
        public ActionResult Edit( CustomerAssignMoney customerAssignMoney)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customerAssignMoney).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.ApplyingMonth = new SelectList(db.MonthTbls, "MonthId", "Name", customerAssignMoney.ApplyingMonth);
                ViewBag.CardType = new SelectList(db.CardTypes, "CardId", "Name", customerAssignMoney.CardType);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerAssignMoney.CustomerId);
                return View(customerAssignMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerAssignMoneys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerAssignMoney customerAssignMoney = db.CustomerAssignMoneys.Find(id);
                if (customerAssignMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerAssignMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // POST: CustomerAssignMoneys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                CustomerAssignMoney customerAssignMoney = db.CustomerAssignMoneys.Find(id);
                db.CustomerAssignMoneys.Remove(customerAssignMoney);
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
