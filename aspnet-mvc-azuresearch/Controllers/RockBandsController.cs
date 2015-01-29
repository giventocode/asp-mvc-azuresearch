using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using aspnet_mvc_azuresearch.Models;
using Giventocode.AzureSearchEntityManager;
using System.Configuration;

namespace aspnet_mvc_azuresearch.Controllers
{
    public class RockBandsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();        
        private IndexQueueManager ixMan = new IndexQueueManager();


        // GET: RockBands
        public ActionResult Index()
        {
            return View(db.RockBands.ToList());
        }

        // GET: RockBands/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RockBand rockBand = db.RockBands.Find(id);
            if (rockBand == null)
            {
                return HttpNotFound();
            }
            return View(rockBand);
        }

        // GET: RockBands/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RockBands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Genre,Description")] RockBand rockBand)
        {
            if (ModelState.IsValid)
            {
                db.RockBands.Add(rockBand);
                db.SaveChanges();
                ixMan.EnqueueEntity<RockBand>(rockBand, "upload");
                return RedirectToAction("Index");
            }

            return View(rockBand);
        }

        // GET: RockBands/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RockBand rockBand = db.RockBands.Find(id);
            if (rockBand == null)
            {
                return HttpNotFound();
            }
            return View(rockBand);
        }

        // POST: RockBands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Genre,Description")] RockBand rockBand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rockBand).State = EntityState.Modified;
                db.SaveChanges();
                ixMan.EnqueueEntity<RockBand>(rockBand, "merge");
                return RedirectToAction("Index");
            }
            return View(rockBand);
        }

        // GET: RockBands/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RockBand rockBand = db.RockBands.Find(id);
            if (rockBand == null)
            {
                return HttpNotFound();
            }
            
            return View(rockBand);
        }

        // POST: RockBands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            RockBand rockBand = db.RockBands.Find(id);
            db.RockBands.Remove(rockBand);
            db.SaveChanges();
            ixMan.EnqueueEntity<RockBand>(rockBand, "delete");
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
