using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeeSoft.ClassRoomJson;

namespace ZeeSoft.Web.MVC.FI.Controllers
{
    public class CourseController : Controller
    {
        //
        // GET: /Course/
        public ActionResult Index()
        {
            
            return View(Academy.Current.Courses);
        }

        //
        // GET: /Course/Details/5
        public ActionResult Details(Guid id)
        {
            return View(Academy.Current.GetCourseById(id));
        }

        //
        // GET: /Course/Create
        public ActionResult Create()
        {
            var course = new Course();
            course.Id = Guid.NewGuid();
            for (var i = 0; i < 10; i++)
            {
                course.AddTopic(new CourseTopic { });
            }
                return View(course);
        }

        //
        // POST: /Course/Create
        [HttpPost]
        public ActionResult Create(Course course)
        {
            try
            {
                Academy.Current.AddOrUpdate(course);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Course/Edit/5
        public ActionResult Edit(Guid id)
        {
            return View(Academy.Current.GetCourseById(id));
        }

        //
        // POST: /Course/Edit/5
        [HttpPost]
        public ActionResult Edit(ZeeSoft.ClassRoomJson.Course course)
        {
            try
            {
                Academy.Current.AddOrUpdate(course);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Course/Delete/5
        public ActionResult Delete(Guid id)
        {
            Academy.Current.DeleteCourse(id);
            return RedirectToAction("Index");
        }

    }
}
