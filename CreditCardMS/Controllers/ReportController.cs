using CreditCardMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreditCardMS.Controllers
{
    public class ReportController : Controller
    {
        private CreditCardMSEntities db = new CreditCardMSEntities();

        public ActionResult Index()
        {
            if (Session["UserType"].ToString() == "Admin") {
                var list = db.CustomerMonthlyPaids.ToList();
                return View(list);
                }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        [HttpPost]
        public ActionResult Index(int? CustomerId, int? month)
        {
            if (Session["UserType"].ToString() == "Admin")
            {
                if (CustomerId == null && month == null)
                {
                    var list = db.CustomerMonthlyPaids.ToList();
                    return View(list);
                }

                if (CustomerId == null && month > 0)
                {
                    var list = (from d in db.CustomerMonthlyPaids
                                where d.Month == month
                                select d).ToList();
                    return View(list);
                }

                if (CustomerId > 0 && month > 0)
                {
                    var list = (from d in db.CustomerMonthlyPaids
                                where d.CustomerId == CustomerId && d.Month == month
                                select d).ToList();
                    return View(list);
                }
                if (CustomerId > 0 && month == null)
                {
                    var list = (from d in db.CustomerMonthlyPaids
                                where d.CustomerId == CustomerId
                                select d).ToList();
                    return View(list);
                }

                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }



    }
}