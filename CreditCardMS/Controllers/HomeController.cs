using CreditCardMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CreditCardMS.Controllers
{
    public class HomeController : Controller
    {

        private CreditCardMSEntities db = new CreditCardMSEntities();


        public ActionResult Profile()
        {
            if (Session["UserType"].ToString() == "Customer" || Session["UserType"].ToString() == "Admin")
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }
            else
            {
                return RedirectToAction("../Home/LogIn");
            }
        }

        //--- Log In -----

        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string userName, String Password)
        {
            try
            {
                if (userName != "" && Password != "")
                {
                    if (userName == "Admin" && Password == "1234")
                    {
                        Session["UserType"] = "Admin";
                        Session["Active"] = "Active";
                        return RedirectToAction("Profile", "Home");
                    }
                    else
                    {
                        var user = (from users in db.Customers
                                    where users.Email.Equals(userName) && users.Password.Equals(Password)
                                    select new
                                    {
                                        users.Name,
                                        users.CardNumber,
                                        users.CardType,
                                        users.CustomerStatus,
                                        users.Email,
                                        users.CustomerId,

                                    }).ToList();

                        if (user.FirstOrDefault() != null)
                        {

                            if (user.FirstOrDefault().CustomerStatus != "Block")
                            {

                                var id = user.FirstOrDefault().CustomerId;
                                Session["UserName"] = user.FirstOrDefault().Name;
                                Session["UserType"] = "Customer";
                                Session["CardType"] = user.FirstOrDefault().CardType;
                                Session["CardNumber"] = user.FirstOrDefault().CardNumber;
                                Session["UserId"] = user.FirstOrDefault().CustomerId;
                                Session["Active"] = "Active";
                                ViewBag.error = "okay";
                                return RedirectToAction("Profile", "Home");

                            }
                            else
                            {
                                ViewBag.error = "Dear " + user.FirstOrDefault().Name + " your account is Blocked ";

                            }
                        }
                        else
                        {
                            ViewBag.error = "Not Matched";
                            return View();
                        }
                    }
                }
                else
                {

                    ViewBag.error = "Please Fill up";
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.error = ex.Message;
            }
            return View();
        }

        ////--- End Log In -----
        ///Log Out
        public ActionResult LogOut()
        {
            Session["Active"] = "Not Active";
            ViewBag.error = "Successfully Log Out Done";
            return View();
        }
    }
}