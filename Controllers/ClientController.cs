using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Techwork.Models;
using System.IO;
using Techwork.App_Code;


namespace Techwork.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        Techwork_DBsEntities db = new Techwork_DBsEntities();

        bool IsValidUser()
        {
            if (Session["aid"] != null)
            {
                DisplayUserPicAndName();
                return true;
            }
            else
                return false;
        }
        void DisplayUserPicAndName()
        {
            string uid = Session["aid"].ToString();
            Client_Table  cl = db.Client_Table.Find(uid);
            ViewBag.Name = "Hi, " + cl.Name;
        }
        public ActionResult Welcome()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult sendEmail()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult PostAWork()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult PostAWork(PostedProject pp)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            HttpPostedFileBase file = Request.Files["Attachment"];
                if (file.ContentLength > 0)
                {
                    string fname = file.FileName;
                    string ext = fname.Substring(fname.LastIndexOf('.'));
                    string FinalName = Path.GetRandomFileName() + "_" + fname;
                    string FolderPath = Server.MapPath("/Content/PostedProject");
                    if (Directory.Exists(FolderPath) == false)
                    {
                        Directory.CreateDirectory(FolderPath);
                    }
                    file.SaveAs(FolderPath + "/"+ FinalName);
                    //database Start
                    try
                    {
                        pp.Attachment = FinalName;
                        pp.PostedBy = Session["aid"].ToString();
                        pp.Posted_On = DateTime.Now;
                        db.PostedProjects.Add(pp);
                        db.SaveChanges();
                        msg = "Project posted successfully.";
                    }
                    catch (Exception ex)
                    {
                        msg = "Sorry! Failed to post your Project/Work.";
                    }
                }
                else
                {
                    msg = "Please choose a file to upload";
                }
                ViewBag.msg = msg;
            return View();
        }
        public ActionResult WorkRequest()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string uid = Session["aid"].ToString();
            var wr = db.RequestedProjects.Where(x => x.postedby == uid).ToList();
            List<RequestedProject> r=wr;
            return View(r);

        }
        public ActionResult AddNotification()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult AddNotification(string notification)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            Notification nt = new Notification();
            nt.Notification_Date = DateTime.Now;
            nt.Message = notification;
            nt.PostedBy = Session["aid"].ToString();
            db.Notifications.Add(nt);
            db.SaveChanges();
            msg = "Notification added successfully.";
            ViewBag.msg = msg;
            return View();
        }
        public ActionResult ManageNotification()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string uid = Session["aid"].ToString();
            var n = db.Notifications.Where(x=>x.PostedBy==uid).OrderBy(x=>x.Notification_Date).ToList();
            List<Notification> n1= n;
            return View(n1);
        }
        public ActionResult UpdateNotification(int id)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var n = db.Notifications.Find(id);
            return View(n);
        }

        [HttpPost]
        public ActionResult UpdateNotification(Notification n)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            try
            {
                var on = db.Notifications.Find(n.NotificationsId);
                on.Message = n.Message;
                db.Entry(on);
                db.SaveChanges();
                ViewBag.msg = "Notification Updated Successfully.";
            }catch(Exception e)
            {
                ViewBag.msg = "Sorry! we are unable to update your notification.";
            }
            Notification nt = n;
            return View(nt);

        }
        public ActionResult DeleteNotification(int id)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var n = db.Notifications.Find(id);
            db.Notifications.Remove(n);
            db.SaveChanges();
            ViewBag.msg = "Notifcation Deleted Successfully.";
            return RedirectToAction("ManageNotification");
        }

        public ActionResult SearchDeveloper()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var dv = db.Developers.OrderBy(x=>x.Experience_Year).ToList();
            return View(dv);
        }
        [HttpPost]
        public ActionResult SearchDeveloper(string searchedString)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var dv = db.Developers.Where(s => s.Email.Contains(Session["aid"].ToString())
                              || s.Name.Contains(searchedString)
                           || (s.Phone.ToString()).Contains(searchedString)
                           || s.Highest_Qualification.Contains(searchedString));
            return View(dv);
        }
        public ActionResult MyWorks()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string uid = Session["aid"].ToString();
            var  mw = db.PostedProjects.Where(x => x.PostedBy.Contains(uid));
            List<PostedProject> ow = mw.ToList();
            return View(ow);
        }
        public ActionResult SendMail()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult SendMail(string SendTo,string Subject,string Message)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            EmailSender es=new EmailSender();
            bool b=es.SendMyEmail(SendTo, Subject, Message);
            if (b)
                ViewBag.Message = "Email Sent Successfully";
            else
                ViewBag.Message = "Sorry! Unable to send email.";
            return View();
        }


        public ActionResult Feedback()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult Feedback(Feedback fb)
        {   if (IsValidUser() == false)
                return RedirectToAction("Login", "Home");
            try
            {
                fb.UserId = Session["aid"].ToString();
                Client_Table cv = db.Client_Table.Find(Session["aid"].ToString());
                fb.Name = cv.Name;
                fb.Feedback_DT = DateTime.Now;
                db.Feedbacks.Add(fb);
                db.SaveChanges();
                ViewBag.msg = "Thanks for your valuable feedback.";
            }
            catch(Exception ex)
            {
                ViewBag.msg = "Sorry! we are unable to save your feedback.";

            }
            return View();
        }
        public ActionResult MyProfile()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            //load Profile of current user from database
            Client_Table dv = db.Client_Table.Find(Session["aid"].ToString());
            return View(dv);
        }
        public ActionResult ChangePassword()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        [HttpPost]
        public ActionResult ChangePassword(string CurPass, string NewPass, string ConfPass)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            try
            {
                Login lg = db.Logins.Find(Session["aid"].ToString());
                Cryptography cg = new Cryptography();
                string epass = cg.EncryptMyData(CurPass);
                if (lg.Pass == epass)
                {
                    if (NewPass == ConfPass)
                    {
                        string encrPass = cg.EncryptMyData(NewPass);
                        lg.Pass = encrPass;
                        db.Entry(lg);
                        db.SaveChanges();
                        msg = "Your Password Updated successfully";
                    }
                    else
                        msg = "Confirm Password doesn't match with new password";
                }
                else
                    msg = "Invalid Current Password;please try again";

            }
            catch (Exception ex)
            {
                msg = "Sorry! unable to update your password.";
            }

            ViewBag.msg = msg;
            return View();
        }
        public ActionResult LogOut()
        {
            Session.Abandon();
            Session.Clear();
            return RedirectToAction("Login", "Home");
        }
        public ActionResult SubmittedProjectsByDeveloper()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string cid = Session["aid"].ToString();
            List<UploadProject> lstp =db.UploadProjects.Where(x => x.thisclient ==cid).ToList();
            return View(lstp);
        }
    }
}