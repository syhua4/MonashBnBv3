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
            var rooms = db.Rooms.Include(r => r.Hotel).Include(r => r.RoomType).Include(r=>r.Hotel.Images);
            return View(rooms.ToList());
        }

        // GET: Rooms/Search
        public ActionResult Search(string destination, DateTime? checkInDate, DateTime? checkOutDate, int? guestNumber)
        {

            
            if (checkInDate.HasValue & checkOutDate.HasValue)
            { 
                DateTime d1 = checkInDate.Value;
                DateTime d2 = checkOutDate.Value;
                TempData["inDate"] = d1;
                TempData["outDate"] = d2;
            }
            else
            {
                ViewBag.Message = "nodate";
                return View("Index");
            }
            ViewBag.inDate = checkInDate;
            ViewBag.outDate = checkOutDate;

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
            var hotelId = from i in destResult select i.hotelId;
            var image = from j in db.Images where j.hotelId.Equals(hotelId) select j.imagePath;
           
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
        
        [HttpPost]
        public JsonResult popularHotel()
        {
            var res = from r in db.Reservations select r;
            res.Select(x => x.hotelId).Distinct();
            var list = res.Select(x => x.Hotel.hotelName).ToList();

            //var g = list.GroupBy(i => i);

            //res.GroupBy(l => l.hotelId).Select(g => new
            //{
            //    hotelName = g.Key,
            //    Count = g.Select(l => l.Hotel).Distinct().Count()
            //});
            List<object> iData = new List<object>();
            var hotelCount = from item in list
            group item by item into g
            orderby g.Count() descending
            select new { g.Key, Count = g.Count() };
            var list2 = hotelCount.ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Hotel", System.Type.GetType("System.String"));
            dt.Columns.Add("Count", System.Type.GetType("System.Int32"));
            for (int i = 0; i < list2.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["Hotel"] = list2[i].Key;
                dr["Count"] = list2[i].Count;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }


            return Json(iData, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult cheapestHotel()
        {
            var price = from i in db.Rooms
                        where i.RoomType.roomTypeAccomodates == 1
                        orderby i.roomPricePerNight ascending
                        select new
                        {
                            hotelName = i.Hotel.hotelName,
                            price = i.roomPricePerNight
                        };
            List<object> iData = new List<object>();
          
            var list = price.ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Hotel", System.Type.GetType("System.String"));
            dt.Columns.Add("Price", System.Type.GetType("System.Int32"));
            for (int i = 0; i < list.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["Hotel"] = list[i].hotelName;
                dr["Price"] = list[i].price;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }


            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult highestRating()
        {
            var rate = from i in db.Ratings
                       select new
                       {
                           hotelName = i.Hotel.hotelName,
                           rating = i.ratingNum
                       };
            var list3 = rate.ToList();

            List<object> iData = new List<object>();
            var avgRate = from item in list3
                             group item.rating by item.hotelName into g
                             orderby g.Average() descending
                             select new { g.Key, rate = g.Average() };
            var list4 = avgRate.ToList();
            DataTable dt = new DataTable();
            dt.Columns.Add("Hotel", System.Type.GetType("System.String"));
            dt.Columns.Add("AverageRating", System.Type.GetType("System.Int32"));
            for (int i = 0; i < list4.Count(); i++)
            {
                DataRow dr = dt.NewRow();
                dr["Hotel"] = list4[i].Key;
                dr["AverageRating"] = list4[i].rate;
                dt.Rows.Add(dr);
            }

            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }


            return Json(iData, JsonRequestBehavior.AllowGet);
        }
        // GET: Rooms/SearchError
        public ActionResult SearchError()
        {
            return View();
        }


        // GET: Rooms/Details/5
        public ActionResult Details(int? value)
        {
            if (value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(value);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }
        public ActionResult RoomInfo(int? value)
        {
            if (value == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Room room = db.Rooms.Find(value);
            if (room == null)
            {
                return HttpNotFound();
            }
            return View(room);
        }

        // GET: Rooms/Create
        public ActionResult Create(int id)
        {
            ViewBag.hotelId = id;
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
                return RedirectToAction("Index","Hotels");
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
