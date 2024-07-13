using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using Techwork.App_Code;
using Techwork.Models;
using System.IO;
using System.Configuration;
using System.Web.Configuration;
using System.Web.Helpers;
using System.Runtime.Serialization.Json;

namespace Techwork.Controllers
{
    public class HomeController : Controller
    {
        Techwork_DBsEntities db=new Techwork_DBsEntities();
        // GET: Home

        [HttpGet]
        public ActionResult Index()
        {
            var n = db.Notifications.OrderByDescending(x => x.Notification_Date).Take(5).ToList();
            return View(n);
        }
        [HttpGet]
        public ActionResult Trending_Projects()
        {
            List<PostedProject> pp = db.PostedProjects.OrderByDescending(x => x.Posted_On).ToList();
            return View(pp);
        }
        [HttpGet]
        public ActionResult Sign_Developer()
        {
            var config = WebConfigurationManager.AppSettings;
            ViewBag.PublicSecret = config["Recaptcha_Public_Key"].ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Sign_Developer(Developer d)
        {
            string msg = "";
            //Start captcha verification
            string UserResponse = Request.Form["g-recaptcha-response"];
            WebClient wc = new WebClient();
            //Asking for environment variables/keys
            var config = WebConfigurationManager.AppSettings;
            var secret = config["RecaptchaPrivateKey"].ToString();
            string url = "https://www.google.com/recaptcha/api/siteverify?secret="+secret+"&response=" + UserResponse;
            string captchaResult = wc.DownloadString(url);
            int n = captchaResult.IndexOf("true");
            //End :captcha verification
            if (n >= 0)
            {//Password Encryption Start from here
                Cryptography cg = new Cryptography();
                string Pass = Request["Password"];
                string EncrPass = cg.EncryptMyData(Pass);
                //To upload Photo
                HttpPostedFileBase file = Request.Files["Photo"];
                if (file.ContentLength > 0)
                {
                    string fname = file.FileName;
                    string FinalFileName = Path.GetRandomFileName() + "_" + fname;
                    //To get the extension of photo
                    string FileExt = fname.Substring(fname.LastIndexOf(".")).ToUpper();
                    string[] AllowedExt = new string[] { ".JPG", ".PNG", ".JPEG", "JFIF" };
                    int x=Array.IndexOf(AllowedExt, FileExt);
                    if (x >= 0)
                    {
                        string FilePath = Server.MapPath("/Content/DeveloperPics");
                        if(Directory.Exists(FilePath)==false)
                        {
                            Directory.CreateDirectory(FilePath);
                        }
                        file.SaveAs(FilePath + "/" + FinalFileName);
                        //Saving Records in Developer table(Database)
                        d.Photo = FinalFileName;
                        d.Saving_Date=DateTime.Now;
                        try
                        {
                            //Adding record in developer table
                            db.Developers.Add(d);
                            Login lg=new Login();
                            lg.UserType = "Developer";
                            lg.UserId = d.Email;
                            lg.Pass = EncrPass;
                            //Adding record in login table
                            db.Logins.Add(lg);
                            db.SaveChanges();
                            msg = "Congratulations! you are registerd successfully";
                        }
                        catch
                        {
                            msg = "Sorry! Due to some technical error; we are unable to registerd you ";
                        }

                    }
                    else
                        msg = "Please choose a valid image file for your photo";
                }
                else
                    msg = "Please choose your Photo.";
            }
            else
                msg = "Please use google recaptcha";
            ViewBag.msg = msg;
            return View();
        }
        [HttpGet]
        public ActionResult Sign_Client()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Sign_Client(Client_Table c)
        {   
            string msg = "";
            //Start captcha verification
            string UserResponse = Request.Form["g-recaptcha-response"];
            WebClient wc=new WebClient();

            string url = "https://www.google.com/recaptcha/api/siteverify?secret=6LdGL7EhAAAAAG-BbEiqTrdA9KmfDkmtoC4Qhqa-&response=" +UserResponse;
            string captchaResult=wc.DownloadString(url);
            int n = captchaResult.IndexOf("true");
            //End :captcha verification
            if (n >= 0)
            {//Password Encryption Start from here
                Cryptography cg=new Cryptography();
                string Pass = Request["Password"];
                string EncrPass = cg.EncryptMyData(Pass);
                c.Saving_Date = DateTime.Now;
                try
                {
                 //Adding record in Client table
                db.Client_Table.Add(c);
                    Login lg = new Login();
                    lg.UserType = "Client";
                    lg.UserId = c.Email;
                    lg.Pass = EncrPass;
                    //Adding record in login table
                    db.Logins.Add(lg);
                    db.SaveChanges();
                    msg = "Congratulations! you are registerd successfully";
                }
                catch
                {
                    msg = "Sorry! Due to some technical error; we are unable to registerd you ";
                }
            }
            else
                msg = "Please use google recaptcha";
            ViewBag.msg = msg;
            return View();
        }

        [HttpGet]
        public ActionResult Developer_Page()
        {
            return View();
        }
        [HttpPost]
        public ActionResult saveenquiry( string Name,string Email,string Phone,string Message)
        {
            var msg = "";
            try
            {
                Enquiry eq = new Enquiry();
                eq.Name = Name;
                eq.Email = Email;
                eq.Phone = Phone;
                eq.EnquiryDate = DateTime.Now;
                eq.Query = Message;
                db.Enquiries.Add(eq);
                db.SaveChanges();
                msg = "SUCCESS";
            }
            catch(Exception e)
            {
                msg = "FAIL";
            }
            return Json(msg,JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult SendOTP(string userid,string usertype)
        {
            Login lg=db.Logins.SingleOrDefault(x=>x.UserId== userid &&x.UserType==usertype);
            string res = "";
            if (lg != null)
            {
              try{ 

                EmailSender es = new EmailSender();
                int otp = es.GenerateOTP();
                es.SendMyEmail(userid, "OTP to recover password", "Hello" +
                    ", Someone tried to recover password of your Techwork account.OTP is" + otp +
                    "Please don't share this OTP with anyone");
                ForgotPassword fp = new ForgotPassword();
                fp.UserId = userid;
                fp.Code = otp;
                fp.Date_TIme = DateTime.Now;
                fp.Issued = false;
                db.ForgotPasswords.Add(fp);
                db.SaveChanges();
                res = "SUCCESS";
            }catch(Exception ex)
            {
                    res="FAIL";
            }
            }
            else
            {
                res = "FAIL";
            }
            return Json(res, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult changepass(string userid,string newpass,string confpass)
        {
            string res ="";
            try
            {
                Login lg = db.Logins.Find(userid);
                Cryptography cg=new Cryptography();
                string epass=cg.EncryptMyData(newpass);
                lg.Pass=epass;
                List<ForgotPassword> lstp=db.ForgotPasswords.Where(x => x.UserId == userid).
                    OrderByDescending(x=>x.ForgotId).Take(1).ToList();
                ForgotPassword fp = lstp[0];
                fp.Issued = true;
                db.Entry(lg);
                db.Entry(fp);
                db.SaveChanges();
                    res = "SUCCESS";
            }catch(Exception  ex)
            {
                res = "FAIL";
            }
            return Json(res, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public ActionResult verifyOTP(string userid,int OTP)
        {
            string res = "";
            List<ForgotPassword> lst = db.ForgotPasswords.Where(x => x.UserId == userid && x.Code == OTP && x.Issued == false).OrderByDescending(x=>x.ForgotId).
                Take(1).ToList();
            if (lst.Count > 0)
                res = "SUCCESS";
            else
                res = "FAIL";
            return Json(res,JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult Login()
        {   
            return View();
        }
        [HttpPost]
        public ActionResult Login(Login l)
        {
            string msg = "";
            Cryptography cg = new Cryptography();
            l.Pass=cg.EncryptMyData(l.Pass);
            Login lgdb = db.Logins.Find(l.UserId);
            if (lgdb != null)
            {
                if (l.Pass == lgdb.Pass && l.UserType == lgdb.UserType)
                {
                    if (l.UserType == "Developer")
                    {
                        //Creating Session
                        Session["uid"] = l.UserId;
                        return RedirectToAction("Welcome", "Developer");
                    }
                    else if (l.UserType == "Client")
                    {
                        //Creating Session
                        Session["aid"] = l.UserId;
                        return RedirectToAction("Welcome", "Client");
                    }
                }
                else
                {
                    msg = "Invalid Password or User type.Please try again.";
                }
            }
            else
                msg = "Invalid User Id.Please try again.";
            ViewBag.msg = msg;
            return View();
        }
    }
}