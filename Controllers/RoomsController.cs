using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MonashBnBv3.Models;

namespace MonashBnBv3.Controllers
{
    public class RoomsController : Controller
    {
        private MonashBnB_db db = new MonashBnB_db();

        // GET: Rooms/Index
        public ActionResult Index()
        {
            var rooms = db.Rooms.Include(r => r.Hotel).Include(r => r.RoomType);
            return View(rooms.ToList());
        }

        // GET: Rooms/Search
        public ActionResult Search(string destination, DateTime? checkInDate, DateTime? checkOutDate, int? guestNumber)
        {
            var destResult = from r in db.Rooms
                             select r;
            var availability = from r in db.Rooms
                               where (!r.Reservations.Any(b => b.reserveCheckOut >= checkOutDate && b.reserveCheckIn <= checkInDate))
                               select r;


            if (!string.IsNullOrEmpty(destination))
                destResult = destResult.Where(x => x.Hotel.hotelAddress.Contains(destination));
            if (checkInDate.HasValue && checkOutDate.HasValue)
                destResult = availability;
            if (guestNumber.HasValue)
                destResult = destResult.Where(x => x.RoomType.roomTypeAccomodates == guestNumber);

            int count = destResult.Count();
            if (count == 0)
            {
                return RedirectToAction("SearchError");
            }
            else
            {
                ViewBag.FoundResults = count + " places to stay";
                return View(destResult);
            }

        }


        // GET: Rooms/SearchError
        public ActionResult SearchError()
        {
            return View();
        }


        // GET: Rooms/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        public ActionResult Create()
        {
            ViewBag.roomTypeId = new SelectList(db.RoomTypes, "roomTypeId", "roomTypeName");
            return View();
        }

        // POST: Rooms/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "roomId,roomPricePerNight,hotelId,roomTypeId")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Rooms.Add(room);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.roomTypeId = new SelectList(db.RoomTypes, "roomTypeId", "roomTypeName", room.roomTypeId);
            return View(room);
        }

        // GET: Rooms/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            ViewBag.roomTypeId = new SelectList(db.RoomTypes, "roomTypeId", "roomTypeName", room.roomTypeId);
            return View(room);
        }

        // POST: Rooms/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "roomId,roomPricePerNight,hotelId,roomTypeId")] Room room)
        {
            if (ModelState.IsValid)
            {
                db.Entry(room).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.roomTypeId = new SelectList(db.RoomTypes, "roomTypeId", "roomTypeName", room.roomTypeId);
            return View(room);
        }

        // GET: Rooms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // POST: Rooms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
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
