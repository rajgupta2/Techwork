using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using Techwork.App_Code;
using System.Data.SqlClient;
using Techwork.Models;
using System.Runtime.Serialization;

namespace Techwork.Controllers
{
    public class DeveloperController : Controller
    {
        // GET: Developer
        Techwork_DBsEntities db = new Techwork_DBsEntities();
        bool IsValidUser()
        {
            if (Session["uid"] != null)
            {
                DisplayUserPicAndName();
                return true;
            }
            else
                return false;
        }
        void DisplayUserPicAndName()
        {
            string uid = Session["uid"].ToString();
            Developer dv = db.Developers.Find(uid);
            ViewBag.Name="Hi, "+dv.Name;
            ViewBag.Pic = dv.Photo;
        }
        public ActionResult Welcome()
        {
            if(IsValidUser()==false)
            {
                return RedirectToAction("Login", "Home");
            }
            return View();
        }
        public ActionResult SearchProjects()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var pp= db.PostedProjects.OrderByDescending(x => x.Posted_On).ToList();
            return View(pp);
        }
        [HttpPost]
        public ActionResult SearchProjects(string searchedString)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var spn = db.PostedProjects.Where(s => s.ProjectTitle.Contains(searchedString) || s.PostedBy.Contains(searchedString));
            var sp = spn.OrderByDescending(x=>x.Posted_On).ToList();
            return View(sp);
        }
        public ActionResult RequestForProjects(int pid)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var client = db.PostedProjects.Find(pid);
            ViewBag.cid = client.PostedBy.ToString();
                return View();
        }
        [HttpPost]
        public ActionResult RequestForProjects(RequestedProject rp)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            try
            {
                rp.Requested_On = DateTime.Now;
                rp.RequestedBy = Session["uid"].ToString();
                db.RequestedProjects.Add(rp);
                db.SaveChanges();
                msg = "Project requested successfully";
            }
            catch(excepction ex)
            {
                msg = "Sorry! we are unable to request for your project.";
            }
            ViewBag.msg = msg;
            return View();
        }
        public ActionResult MyProjects()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string ub = Session["uid"].ToString();
            var mp = db.UploadProjects.Where(x => x.UploadedBy.Contains(ub));
            return View(mp);
        }

        [HttpPost]
        public ActionResult MyProjects(string searchedString)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string ub = Session["uid"].ToString();
            var id = db.UploadProjects.Where(x => x.UploadedBy.Contains(ub));
            var spn = id.Where(s => s.Title.Contains(searchedString)
                           || (s.Uploaded_On.ToString()).Contains(searchedString)
                           || s.FileName.Contains(searchedString));
            return View(spn);
        }
        public ActionResult MyProfile()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            //load Profile of current user from database
            Developer dv = db.Developers.Find(Session["uid"].ToString());
            return View(dv);
        }
        [HttpPost]
        public ActionResult MyProfile(Developer d)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            bool status = true;
            Developer dev = db.Developers.Find(Session["uid"].ToString());
            //To upload file
            HttpPostedFileBase file = Request.Files["Photo"];
            if (file.ContentLength > 0)
            {
                string fname = file.FileName;
                string FinalFileName = Path.GetRandomFileName() + "_" + fname;
                //To get the extension of photo
                string FileExt = fname.Substring(fname.LastIndexOf(".")).ToUpper();
                string[] AllowedExt = new string[] { ".JPG", ".PNG", ".JPEG", "JFIF" };
                int x = Array.IndexOf(AllowedExt, FileExt);
                if (x >= 0)
                {
                    string FilePath = Server.MapPath("/Content/DeveloperPics");
                    if (Directory.Exists(FilePath) == false)
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    file.SaveAs(FilePath + "/" + FinalFileName);
                    dev.Photo = FinalFileName;
                }
                else
                {
                    status = false;
                    msg = "Please choose a valid image file for your photo";
                }
            }
            if (status)
            {
                try {
                    //To save data in database
                    dev.Name = d.Name;
                    dev.Gender = d.Gender;
                    dev.Highest_Qualification = d.Highest_Qualification;
                    dev.Experience_Year = d.Experience_Year;
                    dev.Address = d.Address;
                    dev.Phone = d.Phone;
                    dev.Skills=d.Skills;
                    db.Entry(dev);
                    db.SaveChanges();
                    msg = "Your Profile updated successfully";
                }catch(Exception ex)
                {
                    msg = "Sorry! unable to update your profile";
                }
            }
            ViewBag.msg=msg;
            Developer dv = db.Developers.Find(Session["uid"].ToString());
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
        public ActionResult ChangePassword(string CurPass,string NewPass,string ConfPass)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            string msg = "";
            try
            {
                Login lg = db.Logins.Find(Session["uid"].ToString());
                Cryptography cg = new Cryptography();
                string epass=cg.EncryptMyData(CurPass);
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
            catch(Exception ex)
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
            return RedirectToAction("Login","Home");
        }
        [HttpGet]
        public ActionResult UploadProject()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            List<Client_Table> Ltsp = db.Client_Table.ToList();
            return View(Ltsp);
        }

        [HttpPost]
        public ActionResult UploadProject(UploadProject up)
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            HttpPostedFileBase file = Request.Files["upfile"];
            string msg = "";
            if (file.ContentLength > 0)
            {
                string fname=file.FileName;
                string ext=fname.Substring(fname.LastIndexOf('.'));
                string FinalName = Path.GetRandomFileName() + "_" + fname;
                string FolderPath = Server.MapPath("/Content/DeveloperUploadedProject");
                if (Directory.Exists(FolderPath) == false)
                {
                    Directory.CreateDirectory(FolderPath);
                }
                    file.SaveAs(FolderPath + "/" +FinalName);
                //database Start
                try
                {
                    up.FileName = FinalName;
                    up.UploadedBy = Session["uid"].ToString();
                    up.Uploaded_On = DateTime.Now;
                    db.UploadProjects.Add(up);
                    db.SaveChanges();
                   msg = "Project uploaded successfully.";
                }catch(Exception ex)
                {
                    msg = "Sorry! Failed to Upload Project.";
                }
            }
            else
            {
                msg = "Please choose a file to upload";
            }
            ViewBag.msg = msg;
            string uid = Session["uid"].ToString();
            var c = db.RequestedProjects.Where(x => x.RequestedBy == uid).ToList();
            List<Client_Table> Ltsp = db.Client_Table.ToList();
            return View(Ltsp);

        }
        public ActionResult EnquiryManagement()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var viewEnquiry = db.Enquiries.OrderByDescending(x => x.EnquiryDate).ToList();
            return View(viewEnquiry);
        }

        public ActionResult ViewFeedback()
        {
            if (IsValidUser() == false)
            {
                return RedirectToAction("Login", "Home");
            }
            var feedbacklist = db.Feedbacks.OrderByDescending(x => x.Feedback_DT).ToList();
            return View(feedbacklist);
        }
        [Serializable]
        private class excepction : Exception
        {
            public excepction()
            {
            }

            public excepction(string message) : base(message)
            {
            }

            public excepction(string message, Exception innerException) : base(message, innerException)
            {
            }

            protected excepction(SerializationInfo info, StreamingContext context) : base(info, context)
            {
            }
        }
    }
}