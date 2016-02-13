using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeeSoft.ClassRoomJson;
using WebApplication2.Helper;

namespace WebApplication2.Controllers
{
    public class ActivityCalendarController : Controller
    {
        //
        // GET: /ActivityCalendar/
        public ActionResult Index(int? period /*0=Weekly, 1=Monthly or 2=Yearly*/)
        {
            ViewBag.SelectedPeriod = period.GetValueOrDefault();
            var days = new List<ActivityDay>();
            switch (period.GetValueOrDefault())
            {
                case 0:
                    days = Academy.Current.CalendarYear.CurrentWeek;
                     break;
                case 1:
                    days = Academy.Current.CalendarYear.CurrentMonth;
                     break;
                default:
                    
                    days = Academy.Current.CalendarYear.Days;
                    break;
            }
            return View(days);
        }

        //
        // GET: /ActivityCalendar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

      

        //
        // POST: /ActivityCalendar/Create

        [HttpPost]
        public string UpdateCalendar(List<ActivityDay> days)
        {
            foreach (var day in days)
            {
                Academy.Current.AddOrUpdate(day);
            }
            return "OK";
        }
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ActivityCalendar/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ActivityCalendar/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ActivityCalendar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ActivityCalendar/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
