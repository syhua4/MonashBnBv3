using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using MonashBnBv3.Models;

namespace MonashBnBv3.Controllers
{
    [Authorize(Roles ="Admin")]
    public class MailController : Controller
    {
        private MonashBnB_db db = new MonashBnB_db();

        // GET: Mail
        public ActionResult Index()
        {
            var user = from u in db.AspNetUsers where !u.UserName.Contains("admin") select u;
            return View(user.ToList());
        }


        [HttpPost]
        public ActionResult BulkMail(String[] resp)
        {
            if (resp == null)
            {
                var user = from u in db.AspNetUsers where !u.UserName.Contains("admin") select u;
                ViewBag.Message = "Error";
                return View("Index", user.ToList());
            }
            else
            {
                string kk = "";
                for (int i = 0; i < resp.Length; i++)
                {
                    kk += resp[i] + ", ";
                }
                int test = (kk.Length - 2);


                StringBuilder builder = new StringBuilder();
                builder.Append(kk);
                builder.Remove(test, 2);
                kk = builder.ToString();
                ViewBag.Email = kk;

                return View("SendMail");
            }
        }

        [HttpPost]
        public ActionResult SendMail(MailTemplate mailModel, HttpPostedFileBase postedFile)
        {
            if (ModelState.IsValid)
            {
                string from = "MonashBnb@gmail.com";

                using (MailMessage mail = new MailMessage(from, mailModel.To))
                {
                    mail.Subject = mailModel.Subject;
                    mail.Body = mailModel.Body;
                    string extension = Path.GetExtension(postedFile.FileName);
                    if (postedFile != null)
                    {
                        if (!extension.Contains(".exe"))
                        {
                            string filename = Path.GetFileName(postedFile.FileName);
                            mail.Attachments.Add(new Attachment(postedFile.InputStream, filename));
                        }
                        else
                        {
                            ViewBag.Message = "isExe";
                            return View("SendMail", mailModel);
                        }
                        
                    }
                    mail.IsBodyHtml = true;
                    SmtpClient smtp = new SmtpClient("smtp.gmail.com", Convert.ToInt32(587));
                    NetworkCredential credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["Email"].ToString(), ConfigurationManager.AppSettings["Password"].ToString());
                    smtp.Credentials = credentials;
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                    ViewBag.Message = "Sent";
                    return View("SendMail", mailModel);
                }
            }
            else
            {
                return View("Index");
            }
        }
        // GET: Mail/Create
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
