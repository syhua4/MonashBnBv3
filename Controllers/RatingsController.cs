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
    public class RatingsController : Controller
    {
        private MonashBnB_db db = new MonashBnB_db();

        // GET: Ratings
        
        public ActionResult Index()
        {
            String userId = User.Identity.GetUserId();
            var userIsExist = db.Ratings.Any(x => x.userId == userId);
            var ratings = db.Ratings.Include(r => r.AspNetUser).Include(r => r.Hotel);
            
            if (userIsExist)
            {
                ratings = ratings.Where(x => x.userId.Equals(userId));
                return View(ratings.ToList());
            }
            else
            {
                return View("NoRating");
            }
        }
            
        
        // GET: Ratings/CreateRating
        
        public ActionResult CreateRating(int hotelId, String hotelName, int reserveId)
        {
            ViewBag.ratingHotel = hotelId;
            ViewBag.ratingHotelName = hotelName;
            ViewBag.reserveId = reserveId;
           
            var ratingIsExist = db.Ratings.Any(x=>x.reserveId == reserveId);
            if (ratingIsExist)
            {
                return View("Rated");
            }
            else {
                return View();
            }
            
        }

        public ActionResult SetRating(int hotelId, decimal rank, int reserveId)
        {
            Rating rating = new Rating
            {
                ratingNum = rank,
                hotelId = hotelId,
                reserveId = reserveId,
                userId = User.Identity.GetUserId()
            };

            db.Ratings.Add(rating);
            db.SaveChanges();
            return RedirectToAction("Index") ;
        }

        // GET: Ratings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: Ratings/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName");
            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ratingId,ratingNum,userId,hotelId")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Ratings.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", rating.userId);
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName", rating.hotelId);
            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", rating.userId);
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName", rating.hotelId);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ratingId,ratingNum,userId,hotelId")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "Email", rating.userId);
            ViewBag.hotelId = new SelectList(db.Hotels, "hotelId", "hotelName", rating.hotelId);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.Ratings.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rating rating = db.Ratings.Find(id);
            db.Ratings.Remove(rating);
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
