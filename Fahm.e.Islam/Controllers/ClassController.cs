using Fahm_e_Islam.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zeesoft.MVC.Utils;
using ZeeSoft.ClassRoomJson;

namespace ZeeSoft.Web.MVC.FI.Controllers
{
    public class ClassController : Controller
    {
       

        public ActionResult Home()
        {
            return View();
        }

        // GET: /Class/
        public ActionResult Index()
        {
            return View(Academy.Current.Classes);
        }

        //
        // GET: /Class/Details/5
        public ActionResult Details(Guid id)
        {
            return View(Academy.Current.GetClassById(id));
        }
        
        [HttpPost]
        public ActionResult EditSchedule(ClassSchedule model)
        {
            Academy.Current.AddOrUpdate(model);
            return Content("Class Schedule Updated");
        }
        public ActionResult EditSchedule(Guid classId, string dtFrom, string dtTo)
        {
            ClassSchedule schedule = Academy.Current.GetClassScheduleByClassId(classId);
            var veryFirst = schedule == null || schedule.Days == null;
            if (veryFirst)
            {
                schedule = new ClassSchedule() { ClassId = classId, Start = DateTime.Today };
               
            }
            if (veryFirst && string.IsNullOrEmpty(dtFrom) && string.IsNullOrEmpty(dtTo))
                schedule.Init();
            else if ( !string.IsNullOrEmpty(dtFrom) && !string.IsNullOrEmpty(dtTo))           
                schedule.DateRange(dtFrom, dtTo);
            var cls = Academy.Current.GetClassById(classId);
            schedule.CmbStudents.Clear();
            //schedule.Students.Clear();
            var studentsByCourse = Academy.Current.GetStudentsByCourseId(cls.CourseId);
            foreach (var student in studentsByCourse)
            {
                if(schedule.Students.Contains(student.Id))
                    schedule.CmbStudents.Add(new SelectListItem() { Selected=true, Text = student.Name, Value = student.Id + "" });
                else
                 schedule.CmbStudents.Add(new SelectListItem() { Text = student.Name, Value = student.Id + "" });
            }

           
            return View(schedule);
        }
        //
        // GET: /Class/Create
        public ActionResult Create()
        {
            var Class = new Class();
            foreach (var course in Academy.Current.Courses)
            {
                Class.CmbCourse.Add(new SelectListItem() { Text = course.Name, Value = course.Id + "" });
            }
            foreach (var teacher in Academy.Current.Teachers)
            {
                Class.CmbTeachers.Add(new SelectListItem() { Text = teacher.Name, Value = teacher.Id + "" });
            }
            return View(Class);
        }

        //
        // POST: /Class/Create
        [HttpPost]
        public ActionResult Create(Class Class)
        {
            try
            {
                Academy.Current.AddOrUpdate(Class);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Class/Edit/5
        public ActionResult Edit(Guid id)
        {
            var Class = Academy.Current.GetClassById(id);
            Class.CmbCourse = new List<SelectListItem>();
            foreach (var course in Academy.Current.Courses)
            {
                if (Class.CourseId==course.Id)
                    Class.CmbCourse.Add(new SelectListItem() { Selected = true, Text = course.Name, Value = course.Id + "" });
                else Class.CmbCourse.Add(new SelectListItem() { Text = course.Name, Value = course.Id + "" });
            }
            Class.CmbTeachers = new List<SelectListItem>();
            foreach (var teacher in Academy.Current.Teachers)
            {
                if (Class.TeacherId==teacher.Id)
                    Class.CmbTeachers.Add(new SelectListItem() { Selected = true, Text = teacher.Name, Value = teacher.Id + "" });
                else Class.CmbTeachers.Add(new SelectListItem() { Text = teacher.Name, Value = teacher.Id + "" });
            }
            return View(Class);
        }

        //
        // POST: /Class/Edit/5
        [HttpPost]
        public ActionResult Edit(Class Class)
        {
            try
            {
                Academy.Current.AddOrUpdate(Class);
               

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Class/Delete/5
        public ActionResult Delete(Guid id)
        {
            Academy.Current.DeleteClass(id);
            return RedirectToAction("Index");
        }

       
    }
}
