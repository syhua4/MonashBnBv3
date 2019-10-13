using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using MonashBnBv3.Models;

namespace MonashBnBv3.Controllers
{
    public class MailController : Controller
    {
        private MonashBnB_db db = new MonashBnB_db();

        // GET: Mail
        public ActionResult Index()
        {
            var aspNetUsers = db.AspNetUsers.Include(a => a.Mail);
            return View(aspNetUsers.ToList());
        }

        // GET: Mail/Details/5
        public ActionResult SendMail(string id, string email )
        {
            var receipient = from r in db.AspNetUsers select r;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
           
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.Email = email;
            return View(new MailTemplate());
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
                    if (postedFile != null)
                    {
                        string filename = Path.GetFileName(postedFile.FileName);
                        mail.Attachments.Add(new Attachment(postedFile.InputStream, filename));
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
        public ActionResult Create()
        {
            ViewBag.mailId = new SelectList(db.Mails, "mailId", "mailSentTo");
            return View();
        }

        // POST: Mail/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirstName,LastName,mailId")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.AspNetUsers.Add(aspNetUser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.mailId = new SelectList(db.Mails, "mailId", "mailSentTo", aspNetUser.mailId);
            return View(aspNetUser);
        }

        // GET: Mail/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            ViewBag.mailId = new SelectList(db.Mails, "mailId", "mailSentTo", aspNetUser.mailId);
            return View(aspNetUser);
        }

        // POST: Mail/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,EmailConfirmed,PasswordHash,SecurityStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEndDateUtc,LockoutEnabled,AccessFailedCount,UserName,FirstName,LastName,mailId")] AspNetUser aspNetUser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(aspNetUser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.mailId = new SelectList(db.Mails, "mailId", "mailSentTo", aspNetUser.mailId);
            return View(aspNetUser);
        }

        // GET: Mail/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            if (aspNetUser == null)
            {
                return HttpNotFound();
            }
            return View(aspNetUser);
        }

        // POST: Mail/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            AspNetUser aspNetUser = db.AspNetUsers.Find(id);
            db.AspNetUsers.Remove(aspNetUser);
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
