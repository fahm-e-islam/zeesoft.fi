using Fahm_e_Islam.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication2.Models;
using ZeeSoft.Web.MVC.FI.Helper;
using Zeesoft.MVC.Utils;
using ZeeSoft.ClassRoomJson;
using ZeeSoft.Web.MVC.FI.Models;

namespace ZeeSoft.Web.MVC.FI.Controllers
{
    public class StudentController : Controller
    {
        #region Student Panel methods

        public ActionResult Home()
        {
            return View(SessionCache.CurrentUser_Dashboard as StudentDashboard);
        }

        #endregion

        #region Admin Panel methods
        //
        // GET: /Student/
        public ActionResult Index()
        {
            return View(Academy.Current.Students);
        }

        //
        // GET: /Student/Details/5
        public ActionResult Details(Guid id)
        {
            return View(Academy.Current.GetStudentById(id));
        }

        //
        // GET: /Student/Create

        public ActionResult Create()
        {
            var student = new Student();
            student.Init();
            foreach (var course in Academy.Current.Courses)
            {
                student.CmbCourse.Add(new SelectListItem() { Text= course.Name,Value = course.Id + "" });
            }
            return View(student);
        }

        public ActionResult FileDetails(string fileId)
        {
            var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);
            var resFile = Academy.Current.GetFileByStudentId(student.Id, Guid.Parse(fileId));
            return View(resFile);
        }
        public ActionResult EditFile(string fileId)
        {
            var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);
            var resFile = Academy.Current.GetFileByStudentId(student.Id, Guid.Parse(fileId));
            return View(resFile);
        }
        [HttpPost]
        public ActionResult EditFile(ResourceFile model)
        {
            try
            {
                model.Extention = model.FileName.Substring(model.FileName.LastIndexOf("."));
                model.DateModified = DateTime.Now;
                var fileName = string.Format("{0}.{1}", model.UniqueId,
                  model.FileName.Substring(model.FileName.LastIndexOf(".") + 1));// Path.GetFileName(file.FileName);

                model.Path = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);

                var fileToEdit_IsDeleted = student.Files.Remove(model);
                //model.Id = lastFile.Id + 1;
                student.Files.Add(model);
                Academy.Current.AddOrUpdate(student);
                return RedirectToAction("Files");

            }
            catch (Exception e)
            {

                return View(model);
            }
        }
        public ActionResult DeleteFile(string fileId)
        {
            var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);
            var resFile = Academy.Current.GetFileByStudentId(student.Id, Guid.Parse(fileId));

            var fileToEdit_IsDeleted = student.Files.Remove(resFile);

            Academy.Current.AddOrUpdate(student);
            return RedirectToAction("Files");
        }
        public ActionResult Files()
        {
            var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);
            student.Files = student.Files ?? new List<ResourceFile>();
            return View(student.Files);
        }

        public ActionResult CreateFile()
        {
            var newFile = new ResourceFile();
            newFile.Init();
            return View(newFile);
        }
        [HttpPost]
        public ActionResult CreateFile(ResourceFile model)
        {
            try
            {
                model.Extention = model.FileName.Substring(model.FileName.LastIndexOf("."));
                model.DateCreated = model.DateModified = DateTime.Now;

                var fileName = string.Format("{0}.{1}", model.UniqueId,
                    model.FileName.Substring(model.FileName.LastIndexOf(".") + 1));// Path.GetFileName(file.FileName);

                model.Path = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                var student = Academy.Current.GetStudentById(SessionCache.CurrentUser.DbId);

                student.Files.Add(model);
                Academy.Current.AddOrUpdate(student);
                return RedirectToAction("Files");

            }
            catch (Exception e)
            {

                return View(model);
            }
        }
        //
        // POST: /Student/Create
        [HttpPost]
        public ActionResult Create(Student student)
        {
            try
            {
                Academy.Current.AddOrUpdate(student);
                var body = MVCUtils.GetRazorViewAsString(student, "~/Views/Shared/Templates/tpl_student_create.cshtml");
                Constants.mailer.SendMail(Constants.mailer.USER, student.Email,Constants.Email_Fahm_e_Islam , Constants.Email_BCC,
                "Your Account Has Been Created.", body, Constants.App_Title + " Team");
                
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Student/Edit/5
        public ActionResult Edit(Guid id)
        {
            var student=Academy.Current.GetStudentById(id);
            student.CmbCourse = new List<SelectListItem>();
            foreach (var course in Academy.Current.Courses)
            {
                if (student.Courses.Contains(course.Id))
                student.CmbCourse.Add(new SelectListItem() { Selected=true, Text = course.Name, Value = course.Id + "" });
                else student.CmbCourse.Add(new SelectListItem() {  Text = course.Name, Value = course.Id + "" });
            }
            return View(student);
        }

        //
        // POST: /Student/Edit/5
        [HttpPost]
        public ActionResult Edit(ZeeSoft.ClassRoomJson.Student student)
        {
            try
            {
                var studentold = Academy.Current.GetStudentById(student.Id);
                student.Files = studentold.Files;
                Academy.Current.AddOrUpdate(student);

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Student/Delete/5
        public ActionResult Delete(Guid id)
        {
            Academy.Current.DeleteStudent(id);
            return RedirectToAction("Index");
        }
    
        #endregion
    }
}
 