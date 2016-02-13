using Fahm_e_Islam.Helpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Zeesoft.MVC.Utils;
using ZeeSoft.ClassRoomJson;
using ZeeSoft.Web.MVC.FI.Models;
using ZeeSoft.Web.MVC.FI.Helper;
using WebApplication2.Models;
using System.IO;

namespace ZeeSoft.Web.MVC.FI.Controllers
{
    public class TeacherController : Controller
    {
        #region Teacher Panel methods

        public ActionResult Home()
        {
            
            return View(SessionCache.CurrentUser_Dashboard as TeacherDashboard);
        }

        #endregion

        #region Admin Panel methods
        //
        // GET: /Teacher/
        public ActionResult Index()
        {
            return View(Academy.Current.Teachers);
        }
        
        //
        // GET: /NewSession/
        public ActionResult NewSession(Guid scheduleId)
        {
            SessionCache.Teacher_CurrentClassSession = null;
            SessionCache.Teacher_CurrentSessionCalc = null;

            var newSession = new ClassSession();
            var schedule = Academy.Current.GetClassScheduleById(scheduleId);
            var cls = Academy.Current.GetClassById(schedule.ClassId);

                var attendance = new Attendance();

                newSession.ClassScheduleId = schedule.Id;
                var students = Academy.Current.GetStudentsByScheduleId(scheduleId);
                foreach (var student in students)
                {
                    attendance.CmbStudents.Add(new SelectListItem() { Text = student.Name, Value = student.Id + "" });
                }

                newSession.Attendance = attendance;
                SessionCache.Teacher_CurrentClassSession = newSession;
                SessionCache.Teacher_CurrentSessionCalc = SessionCache.Teacher_CurrentSessionCalc ?? new NewClassSessionCalc() { Duration = cls.Duration.ToString() };
         

            
            return View(newSession);
        }
        //
        // GET: /Teacher/StartStopSession/0 or 1
        public ActionResult StartStopSession(ClassSession form)
        {
              
              SessionCache.Teacher_CurrentSessionCalc.ClickCount += 1;
              var duration=int.Parse(SessionCache.Teacher_CurrentSessionCalc.Duration.Split(':')[1]) * 60 * 1000;
               
             
             if (SessionCache.Teacher_CurrentSessionCalc.ClickCount == 1)
             {
                 SessionCache.Teacher_CurrentClassSession = form;
                 SessionCache.Teacher_CurrentClassSession.StartTime = DateTime.Now;
                 SessionCache.Teacher_CurrentClassSession.EndTime = SessionCache.Teacher_CurrentClassSession.StartTime.AddMilliseconds(duration);
                 SessionCache.Teacher_CurrentSessionCalc.EndTime = SessionCache.Teacher_CurrentClassSession.EndTime.ToJSDate();
             }
             if (SessionCache.Teacher_CurrentSessionCalc.Mode == 1)//Start
            {
                var lastMinUsed = SessionCache.Teacher_CurrentSessionCalc.TimeElapsed;
                SessionCache.Teacher_CurrentSessionCalc.EndTime = DateTime.Now.AddMilliseconds(duration - lastMinUsed.TotalMilliseconds).ToJSDate();
               

            }
            else//Stop
            {
                var timeRemaining = DateTime.Parse(SessionCache.Teacher_CurrentSessionCalc.EndTime) - DateTime.Now;
                var remainingMin = timeRemaining.TotalMilliseconds;
                 SessionCache.Teacher_CurrentSessionCalc.TimeElapsed = TimeSpan.FromMilliseconds((duration - remainingMin));
                
            }
             SessionCache.Teacher_CurrentSessionCalc.Mode = SessionCache.Teacher_CurrentSessionCalc.Mode == 1 ? 0 : 1;
          
            var data =JsonConvert.SerializeObject(SessionCache.Teacher_CurrentSessionCalc);
            return Content(data);
            //return Json(SessionCache.Teacher_CurrentSessionCalc, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ChangeClass(Guid id)
        {
            var students = new List<SelectListItem>();
            var cls = ZeeSoft.ClassRoomJson.Academy.Current.GetClassById(id);
            var studentsByfirstCls = Academy.Current.GetStudentsByCourseId(cls.CourseId);
            foreach (var student in studentsByfirstCls)
            {
                students.Add(new SelectListItem() { Text = student.Name, Value = student.Id + "" });
            }
            return Json(students, JsonRequestBehavior.AllowGet);
               
        }
         [HttpPost]
        public string EndSession(){
            var timeRemaining = DateTime.Parse(SessionCache.Teacher_CurrentSessionCalc.EndTime) - DateTime.Now;
            var remainingMin = timeRemaining.TotalMilliseconds;
            var duration = int.Parse(SessionCache.Teacher_CurrentSessionCalc.Duration.Split(':')[1]) * 60 * 1000;
          
            SessionCache.Teacher_CurrentSessionCalc.TimeElapsed = TimeSpan.FromMilliseconds((duration - remainingMin));
         
            var currentSession = SessionCache.Teacher_CurrentClassSession;
            var currentSessionCalc = SessionCache.Teacher_CurrentSessionCalc;
             //save in db
            currentSession.EndTime =new DateTime(currentSession.StartTime.AddMilliseconds(SessionCache.Teacher_CurrentSessionCalc.TimeElapsed.TotalMilliseconds).Ticks);
            Academy.Current.AddOrUpdate(currentSession);

            SessionCache.Teacher_CurrentClassSession = null;
            SessionCache.Teacher_CurrentSessionCalc = null;
             
            return "OK";
        }

         public ActionResult Files()
         {
            var teacher=Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
            teacher.Files = teacher.Files ?? new List<ResourceFile>();
            return View(teacher.Files);
         }
         public ActionResult EditFile(string fileId)
         {
             var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
             var resFile = Academy.Current.GetFileByTeacherId(teacher.Id, Guid.Parse(fileId));
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

                 var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
                 model.SharedBy = teacher.Id.GetUserNameById();

                 var fileToEdit_IsDeleted = teacher.Files.Remove(model);
                 //model.Id = lastFile.Id + 1;
                 teacher.Files.Add(model);
                 Academy.Current.AddOrUpdate(teacher);
                 return RedirectToAction("Files");

             }
             catch (Exception e)
             {

                 return View(model);
             }
         }
         public ActionResult DeleteFile(string fileId)
         {
            var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
            var resFile = Academy.Current.GetFileByTeacherId(teacher.Id, Guid.Parse(fileId));

            var fileToEdit_IsDeleted = teacher.Files.Remove(resFile);
           
             Academy.Current.AddOrUpdate(teacher);
             return RedirectToAction("Files");
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
                 model.DateCreated =model.DateModified= DateTime.Now;
                 
                 var fileName =string.Format("{0}.{1}", model.UniqueId,
                     model.FileName.Substring(model.FileName.LastIndexOf(".") + 1));// Path.GetFileName(file.FileName);
                      
                 model.Path=Path.Combine(Server.MapPath("~/App_Data"), fileName);

                 var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
                 model.SharedBy = teacher.Id.GetUserNameById();

                 teacher.Files.Add(model);
                 Academy.Current.AddOrUpdate(teacher);
                 return RedirectToAction("Files");

             }
             catch (Exception e)
             {

                 return View(model);
             }
         }
        public ActionResult FileDetails(string fileId)
        {
            var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
            var resFile = Academy.Current.GetFileByTeacherId(teacher.Id, Guid.Parse(fileId));
            return View(resFile);
        }
        public JsonResult GetClassesByCourseIds(Guid courseId) // its a GET, not a POST
        {
            var cmbClasses=Academy.Current.GetClassesByCourseId(courseId);
            return Json(cmbClasses.ToSelectList(new List<Guid>()), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStudentsByClassId(Guid classId) // its a GET, not a POST
        {
            var cmbStudent = Academy.Current.GetStudentsByClassId(classId);
            return Json(cmbStudent.ToSelectList(new List<Guid>()), JsonRequestBehavior.AllowGet);
        }
        
         public ActionResult FileLink(string fileId)
         {
             var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
             var myCourses = Academy.Current.GetCoursesByTeacherId(teacher.Id);
             var myClasses = Academy.Current.GetClassesByTeacherId(teacher.Id);
             var myStudents = Academy.Current.GetStudentsByTeacherId(teacher.Id);

             var resFile = Academy.Current.GetFileByTeacherId(teacher.Id, Guid.Parse(fileId));
             if(resFile.Links==null)//Add new
             resFile.Links = new FileLink
             {
                 FileId=resFile.Id,
                 CmbCourse = myCourses.ToSelectList(new List<Guid>()),
                 //CmbClass = myClasses.ToSelectList(null),
                 //CmbTeacher = new List<SelectListItem> { new SelectListItem { Selected = true, Text = teacher.Name, Value = teacher.Id.ToString() } }


             };
             else//edit
             {
                 resFile.Links.CmbCourse = myCourses.ToSelectList(resFile.Links.CourseIds);
                 resFile.Links.CmbClass = myClasses.ToSelectList(resFile.Links.ClassIds);
                 resFile.Links.CmbStudent = myStudents.ToSelectList(resFile.Links.StudentIds);
                 
             }
             return View(resFile.Links);
         }
        [HttpPost]
         public ActionResult FileLink(FileLink model)
         {
             var teacher = Academy.Current.GetTeacherById(SessionCache.CurrentUser.DbId);
             var resFile = Academy.Current.GetFileByTeacherId(teacher.Id, model.FileId);
             model.TeacherIds = new List<Guid> { teacher.Id };
             resFile.Links = model;
             Academy.Current.AddOrUpdate(teacher);
             //var file=teacher.Files.Where(f => f.Id == model.FileId).SingleOrDefault();
             return RedirectToAction("Files");
         }
        
        //
        // GET: /Teacher/Details/5
        public ActionResult Details(Guid id)
        {
            return View(Academy.Current.GetTeacherById(id));
        }

        //
        // GET: /Teacher/Create
        public ActionResult Create()
        {
            var teacher = new Teacher();
            teacher.Init();
            foreach (var course in Academy.Current.Courses)
            {
                teacher.CmbCourse.Add(new SelectListItem() { Text = course.Name, Value = course.Id + "" });
            }
            return View(teacher);
        }

        //
        // POST: /Teacher/Create
        [HttpPost]
        public ActionResult Create(Teacher teacher)
        {
            try
            {
                teacher.Files = new List<ResourceFile>();
                Academy.Current.AddOrUpdate(teacher);
                var body = MVCUtils.GetRazorViewAsString(teacher, "~/Views/Shared/Templates/tpl_teacher_create.cshtml");
                Constants.mailer.SendMail(Constants.mailer.USER, teacher.Email, Constants.Email_Fahm_e_Islam, Constants.Email_BCC,
                "Your Account Has Been Created.", body, Constants.App_Title + " Team");

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Teacher/Edit/5
        public ActionResult Edit(Guid id)
        {
            var teacher = Academy.Current.GetTeacherById(id);
            teacher.CmbCourse = new List<SelectListItem>();
            foreach (var course in Academy.Current.Courses)
            {
                if (teacher.Courses.Contains(course.Id))
                    teacher.CmbCourse.Add(new SelectListItem() { Selected = true, Text = course.Name, Value = course.Id + "" });
                else teacher.CmbCourse.Add(new SelectListItem() { Text = course.Name, Value = course.Id + "" });
            }
            return View(teacher);
        }

        //
        // POST: /Teacher/Edit/5
        [HttpPost]
        public ActionResult Edit(Teacher teacher)
        {
            try
            {
                var teacherold = Academy.Current.GetTeacherById(teacher.Id);

                teacher.Files = teacherold.Files;

                Academy.Current.AddOrUpdate(teacher);
             
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

       
        //
        // GET: /Teacher/Delete/5
        public ActionResult Delete(Guid id)
        {
            Academy.Current.DeleteTeacher(id);
            return RedirectToAction("Index");
        }

        #endregion
    }
}
