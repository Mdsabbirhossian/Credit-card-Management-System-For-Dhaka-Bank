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
    public class CustomerConsumingMoneysController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: CustomerConsumingMoneys
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (Session["UserType"].ToString() == "Customer")
                {
                    var id = Convert.ToInt32(Session["UserId"].ToString());

                    var det = (from d in db.CustomerConsumingMoneys
                               where d.CustomerId == id
                               select d).ToList();
                    return View(det);
                }
                else
                {
                    var customerConsumingMoneys = db.CustomerConsumingMoneys.Include(c => c.Customer);
                    return View(customerConsumingMoneys);
                }
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerConsumingMoneys/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerConsumingMoney customerConsumingMoney = db.CustomerConsumingMoneys.Find(id);
                if (customerConsumingMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerConsumingMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerConsumingMoneys/Create
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
        public ActionResult Create(CustomerConsumingMoney customerConsumingMoney)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerConsumingMoney.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerConsumingMoney.CustomerId);
                if (ModelState.IsValid)
                {

                    var monthlyPaid = db.CustomerMonthlyPaids.Where(x => x.CustomerId == customerConsumingMoney.CustomerId && x.Month == customerConsumingMoney.Month).SingleOrDefault();
                
                    var TotalConsumeMoney = 0.0;

                    var list = db.CustomerAssignMoneys.Where(x => x.ApplyingMonth == customerConsumingMoney.Month && x.CustomerId == customerConsumingMoney.CustomerId && x.ApplyingStatus == "Approved").ToList();
                    var ApplyMoney = 0;
                    ApplyMoney = list.Select(x => x.Amount).Sum(); // Apply money 
                    if(ApplyMoney > 0) { 

                    var list2 = db.CustomerConsumingMoneys.Where(x => x.Month == customerConsumingMoney.Month && x.CustomerId == customerConsumingMoney.CustomerId).ToList();
                    var ConsumeMoney = list2.Select(x => x.ConsumeMoney).Sum(); //Used money

                     TotalConsumeMoney = ConsumeMoney + customerConsumingMoney.ConsumeMoney;


                    if (TotalConsumeMoney > ApplyMoney)
                    {
                        ViewBag.error = "Sorry,Out of Range.Your balance is " + ApplyMoney + " taka and you already used  " + ConsumeMoney + " taka.";
                        return View();
                    }
                        if (customerConsumingMoney.ConsumeMoney > ApplyMoney)
                        {
                            ViewBag.error = "Sorry,Out of Range.Your Current balance is " + ApplyMoney + " taka";
                            return View();
                        }
                        else
                        {
                            //TODO
                            //var CashBack = customerConsumingMoney.CashBackMoney;
                            //var witdraw = customerConsumingMoney.ConsumeMoney;
                            //var cal = witdraw - ((witdraw* CashBack)/100);
                            //customerConsumingMoney.TotalConsumeMoney = (double)cal;

                            db.CustomerConsumingMoneys.Add(customerConsumingMoney);
                            db.SaveChanges();


                            //Update Reward
                            if (customerConsumingMoney.ConsumeMoney >= 100)
                            {
                                var reward = db.Rewards.Where(x => x.CustomerId == customerConsumingMoney.CustomerId).FirstOrDefault();
                                var Rw = db.Rewards.Where(x => x.RewardsId == reward.RewardsId).FirstOrDefault();
                                var beforRewardPoint = 0;
                                var totalReward = 0.0;
                                var beforeTotalAmount = 0.0;
                                beforRewardPoint = reward.RewardsPoint;
                                beforeTotalAmount = (double)reward.TotalAmount;
                                totalReward = (customerConsumingMoney.ConsumeMoney / 50);
                                Rw.RewardsPoint = (int)totalReward + beforRewardPoint;
                                Rw.TotalAmount = customerConsumingMoney.ConsumeMoney + beforeTotalAmount;
                                Rw.RewardDate = reward.RewardDate;

                                db.Entry(Rw).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            //End


                            //Customer Monthly paid

                            var WitdrawMoney = 0.0;
                            var TotalWitdrawMoney = 0.0;
                            var CashBackMoney = 0.0;
                            var cardType = Convert.ToInt32(Session["CardType"].ToString());
                            var monthlyData = db.CustomerMonthlyPaids.Where(x => x.Month == customerConsumingMoney.Month && x.CustomerId == customerConsumingMoney.CustomerId).ToList();

                            WitdrawMoney = (double)monthlyData.Select(x => x.ConsumeMoney).FirstOrDefault(); //Load from Monthly paid table then
                            TotalWitdrawMoney = WitdrawMoney + customerConsumingMoney.ConsumeMoney;

                            monthlyPaid.MonthlyPaidId = monthlyData.Select(x => x.MonthlyPaidId).FirstOrDefault();
                            monthlyPaid.CustomerId = customerConsumingMoney.CustomerId;
                            var cardId = monthlyData.Select(x => x.CardNo).FirstOrDefault();
                            monthlyPaid.CardNo = cardId;
                            monthlyPaid.Month = monthlyData.Select(x => x.Month).FirstOrDefault();
                            monthlyPaid.ApplyingMoney = monthlyData.Select(x => x.ApplyingMoney).FirstOrDefault();
                            monthlyPaid.ConsumeMoney = TotalWitdrawMoney;
                            monthlyPaid.PaidMoney = monthlyData.Select(x => x.PaidMoney).FirstOrDefault();
                            var servicecarge = db.CardTypes.Where(x => x.CardId == cardType).Select(y => y.ServiceCharge).FirstOrDefault();

                            monthlyPaid.ServiceCharge = servicecarge;
                            monthlyPaid.Fine = monthlyData.Select(x => x.Fine).FirstOrDefault();
                            //Rewards cash back

                            if (monthlyData.Select(x => x.CashBack).FirstOrDefault() > 0)
                            {
                                CashBackMoney = (double)monthlyData.Select(x => x.CashBack).FirstOrDefault();
                            }
                            monthlyPaid.CashBack = CashBackMoney;

                            monthlyPaid.TotalBill = TotalWitdrawMoney;
                            if (monthlyData.Select(x => x.PaidMoney).FirstOrDefault() == monthlyPaid.TotalBill)
                            {
                                var TotalBill = ((TotalWitdrawMoney) + ((TotalWitdrawMoney * servicecarge) / 100)) - CashBackMoney;
                                monthlyPaid.TotalBill = TotalBill;
                                monthlyPaid.PaidStatus = "Paid Successfully";
                            }
                            else
                            {
                                monthlyPaid.PaidStatus = "Running";
                            }
                            db.Entry(monthlyPaid).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ViewBag.error = "Sorry Sir, your balance is 0 taka. So you are not able to consume money.";
                        return View();
                    }
   
                }
               
                return View(customerConsumingMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
                
            }
        }

        // GET: CustomerConsumingMoneys/Edit/5
        public ActionResult Edit(int? id)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerConsumingMoney customerConsumingMoney = db.CustomerConsumingMoneys.Find(id);
                if (customerConsumingMoney == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerConsumingMoney.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerConsumingMoney.CustomerId);
                return View(customerConsumingMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost]
        public ActionResult Edit(CustomerConsumingMoney customerConsumingMoney)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
                {
                    db.Entry(customerConsumingMoney).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name", customerConsumingMoney.Month);
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", customerConsumingMoney.CustomerId);
                return View(customerConsumingMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: CustomerConsumingMoneys/Delete/5
        public ActionResult Delete(int? id)
        {

            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                CustomerConsumingMoney customerConsumingMoney = db.CustomerConsumingMoneys.Find(id);
                if (customerConsumingMoney == null)
                {
                    return HttpNotFound();
                }
                return View(customerConsumingMoney);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                CustomerConsumingMoney customerConsumingMoney = db.CustomerConsumingMoneys.Find(id);
                db.CustomerConsumingMoneys.Remove(customerConsumingMoney);
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
