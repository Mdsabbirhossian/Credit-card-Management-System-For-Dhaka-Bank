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
    public class RewardsController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        // GET: Rewards
        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (Session["UserType"].ToString() == "Customer")
                {
                    var id = Convert.ToInt32(Session["UserId"].ToString());

                    var det = (from d in db.Rewards
                               where d.CustomerId == id
                               select d).ToList();
                    return View(det);
                }
                else
                {
                    var rewards = db.Rewards.Include(r => r.Customer);
                    return View(rewards.ToList());
                }
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }

        }

        public ActionResult CashBack(int? id)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                Reward reward = db.Rewards.Find(id);
                if (reward == null)
                {
                    return HttpNotFound();
                }
                ViewBag.Month = new SelectList(db.MonthTbls, "MonthId", "Name");
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", reward.CustomerId);
                return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }
        [HttpPost]
        public ActionResult CashBack(int? id, int month, int point, int cashback, int CustomerId)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (id != null)
                {
                    var CustomerPaidInfo = db.CustomerMonthlyPaids.Where(x => x.CustomerId == CustomerId && x.Month == month).FirstOrDefault();

                    Reward reward = db.Rewards.Find(id);
                    if (CustomerPaidInfo.PaidStatus == "Running")
                    {
                        CustomerPaidInfo.CashBack = cashback;
                        db.Entry(CustomerPaidInfo).State = EntityState.Modified;
                        db.SaveChanges();

                        reward.CustomerId = CustomerId;
                        reward.RewardDate = DateTime.Now.Date;
                        reward.RewardsPoint = 0;
                        reward.TotalAmount = 0;
                        db.Entry(reward).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ViewBag.error = "Dear Sir Sorry, you already paid the Bill.Please select valid Month";
                        return View();
                    }




                    return RedirectToAction("Index");
                }

                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Rewards/Details/5
        public ActionResult Details(int? id)
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reward reward = db.Rewards.Find(id);
            if (reward == null)
            {
                return HttpNotFound();
            }
            return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Rewards/Create
        public ActionResult Create()
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name");
            return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RewardsId,CustomerId,RewardDate,RewardsPoint,TotalAmount")] Reward reward)
        {
            if (Session["UserType"].ToString() == "Customer")
            {
                if (ModelState.IsValid)
            {
                db.Rewards.Add(reward);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", reward.CustomerId);
            return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Rewards/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reward reward = db.Rewards.Find(id);
            if (reward == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", reward.CustomerId);
            return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // POST: Rewards/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RewardsId,CustomerId,RewardDate,RewardsPoint,TotalAmount")] Reward reward)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (ModelState.IsValid)
            {
                db.Entry(reward).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "Name", reward.CustomerId);
            return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // GET: Rewards/Delete/5
        public ActionResult Delete(int? id)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reward reward = db.Rewards.Find(id);
            if (reward == null)
            {
                return HttpNotFound();
            }
            return View(reward);
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        // POST: Rewards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reward reward = db.Rewards.Find(id);
            db.Rewards.Remove(reward);
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
