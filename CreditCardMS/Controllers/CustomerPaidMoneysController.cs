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
    public class CustomerPaidMoneysController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CustomerPaidMoneys
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (Session["UserType"].ToString() == "Customer")
                {
                    var id = Convert.ToInt32(Session["UserId"].ToString());

                    var det = (from d in db.CustomerPaidMoneys
                               where d.CustomerId == id
                               select d).ToList();
                    return View(det);
                }
                else
                {
                    var customerPaidMoneys = db.CustomerPaidMoneys.Include(c => c.Customer);
                    return View(customerPaidMoneys.ToList());
                }
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerPaidMoneys/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerPaidMoney customerPaidMoney = db.CustomerPaidMoneys.Find(id);
                if (customerPaidMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerPaidMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerPaidMoneys/Create
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
        public ActionResult Create(CustomerPaidMoney customerPaidMoney)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (ModelState.IsValid)
                {
                    ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerPaidMoney.Month);
                    ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerPaidMoney.CustomerId);

                    CustomerConsumingMoney CCM = new CustomerConsumingMoney();
                    var customerMoneyInfoList = db.CustomerMonthlyPaids.Where(x => x.Month == customerPaidMoney.Month && x.CustomerId == customerPaidMoney.CustomerId).FirstOrDefault();

                    if (customerMoneyInfoList.ConsumeMoney > 0)
                    {
                        var cashBack = 0.0;
                        var consumeMoney = 0.0;
                        var serviceCharge = 0.0;
                        var totalNeedToPaid = 0.0;
                        var totalPaid = 0.0;

                        consumeMoney = (double)customerMoneyInfoList.ConsumeMoney;
                        cashBack = (double)customerMoneyInfoList.CashBack;
                        serviceCharge = (double)customerMoneyInfoList.ServiceCharge;
                        totalNeedToPaid = (consumeMoney + ((consumeMoney * serviceCharge) / 100)) - cashBack;

                        var paidInfo = db.CustomerPaidMoneys.Where(x => x.Month == customerPaidMoney.Month && x.CustomerId == customerPaidMoney.CustomerId).ToList();
                        totalPaid = paidInfo.Select(x => x.PaidMoney).Sum();
                        var TotalwithcurrentPaid = customerPaidMoney.PaidMoney + totalPaid;

                        if (customerMoneyInfoList.PaidStatus == "Paid")
                        {
                            ViewBag.error = "Dear Sir, you already Paid the bills";
                            return View();
                        }
                        else
                        {
                            if (TotalwithcurrentPaid > totalNeedToPaid)
                            {
                                ViewBag.error = "Dear Sir, you already Paid +" + TotalwithcurrentPaid + " taka. Now your available bill is " + (totalNeedToPaid - TotalwithcurrentPaid) + " taka.";
                                return View();
                            }
                            else
                            {

                                db.CustomerPaidMoneys.Add(customerPaidMoney);
                                db.SaveChanges();

                                //monthly paid table 
                                customerMoneyInfoList.PaidMoney = TotalwithcurrentPaid;
                                if (TotalwithcurrentPaid == totalNeedToPaid)
                                {
                                    customerMoneyInfoList.PaidStatus = "Paid";
                                }
                                else
                                {
                                    customerMoneyInfoList.PaidStatus = "Running";
                                }
                                db.Entry(customerMoneyInfoList).State = EntityState.Modified;
                                db.SaveChanges();

                                return RedirectToAction("Index");


                            }
                        }
                    }
                    else
                    {
                        ViewBag.error = "Dear Sir, You have no bills";
                        return View();
                    }

                }

               
                return View(customerPaidMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerPaidMoneys/Edit/5
        public ActionResult Edit(int? id)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerPaidMoney customerPaidMoney = db.CustomerPaidMoneys.Find(id);
                if (customerPaidMoney == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerPaidMoney.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerPaidMoney.CustomerId);
                return View(customerPaidMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( CustomerPaidMoney customerPaidMoney)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customerPaidMoney).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerPaidMoney.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerPaidMoney.CustomerId);
                return View(customerPaidMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }

        // GET: CustomerPaidMoneys/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerPaidMoney customerPaidMoney = db.CustomerPaidMoneys.Find(id);
                if (customerPaidMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerPaidMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }

        // POST: CustomerPaidMoneys/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                CustomerPaidMoney customerPaidMoney = db.CustomerPaidMoneys.Find(id);
                db.CustomerPaidMoneys.Remove(customerPaidMoney);
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
