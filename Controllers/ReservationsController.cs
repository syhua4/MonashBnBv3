using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using MonashBnBv3.Models;

namespace MonashBnBv3.Controllers
{
    [Authorize]
    public class ReservationsController : Controller
    {
        private MonashBnB_db db = new MonashBnB_db();

        // GET: Reservations
        public ActionResult Index()
        {
            String userId = User.Identity.GetUserId();
            var reservations = db.Reservations.Include(r => r.AspNetUser).Include(r => r.Hotel).Include(r => r.Room);
            reservations = reservations.Where(x => x.userId.Equals(userId));
            if (reservations.Count() > 0)
            {
                return View(reservations.ToList());
            }
            else
            {
                return View("Error");
            }
            
        }
        public ActionResult Error()
        {
            return View();
        }
        // GET: Reservations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }
        
        // GET: Reservations/Create
        public ActionResult Create(string name, string type, int roomId, float price, int hotelId)
        {
            if (TempData["inDate"]!= null & TempData["outDate"]!= null)
            {//{17/10/2019 2:00:00 PM}
                DateTime d1 = (DateTime)TempData["inDate"];
                DateTime d2 = (DateTime)TempData["outDate"];
                TimeSpan ts1 = new TimeSpan(14, 0, 0);
                TimeSpan ts2 = new TimeSpan(11, 0, 0);
                d1 = d1.Date + ts1;
                d2 = d2.Date + ts2;
                
                TimeSpan diff = d2.Subtract(d1);
                string format = "yyyy-MM-dd HH:mm:ss";
                String f1 = d1.ToString(format);
                string f2 = d2.ToString(format);
                var days = 0;
                if (diff.TotalDays<1)
                {
                    days = 1;
                }
                else
                {
                    days = (int)Math.Round(diff.TotalDays);
                }
                float pricePerDay = price;
                double totalPrice = Math.Round((days * pricePerDay), 2);
                ViewBag.hotelName = name;
                ViewBag.roomType = type;
                ViewBag.roomId = roomId;
                ViewBag.day = days;
                ViewBag.price = pricePerDay;
                ViewBag.totalPrice = totalPrice;
                ViewBag.d1 = f1;
                ViewBag.d2 = f2;
                ViewBag.hotelId = hotelId;
                ViewBag.roomId = new SelectList(db.Rooms, "roomId", "roomId");
                return View();
            }
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "reserveId,reserveCheckIn,reserveCheckOut,reservePrice,userId,hotelId,roomId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                //DateTime d1Date = ViewBag.d1;
                //DateTime d2Date = ViewBag.d2;

                var user = User.Identity.GetUserId();
                //reservation.reserveCheckIn = d1Date;
                //reservation.reserveCheckOut = d2Date;
                //reservation.hotelId = ViewBag.hotelId;
                //reservation.roomId = ViewBag.roomId;
                reservation.userId = user;
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", reservation.userId);
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName", reservation.hotelId);
            ViewBag.roomId = new SelectList(db.Rooms, "roomId", "roomId", reservation.roomId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "reserveId,reserveCheckIn,reserveCheckOut,reservePrice,userId,hotelId,roomId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", reservation.userId);
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName", reservation.hotelId);
            ViewBag.roomId = new SelectList(db.Rooms, "roomId", "roomId", reservation.roomId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
            db.Reservations.Remove(reservation);
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
