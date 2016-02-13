using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using WebApplication2.Helper;

namespace ZeeSoft.ClassRoomJson
{
    public enum Status{
        NonActive=0,
        Active=1
    }
    public enum Gender
    {
        Male = 0,
        Female = 1
    }
    public class Academy
    {
        
        public static Academy Current { get; set; }
        static Academy()
        {
            JsonSerializer serializer = new JsonSerializer();
            // Set the datafile path relative to the application's path.


            using (StreamReader sw = new StreamReader(HostingEnvironment.ApplicationPhysicalPath + "\\academy.json"))
            using (JsonReader reader = new JsonTextReader(sw))
            {
                try
                {

              
                Current = (Academy)serializer.Deserialize(reader, typeof(Academy));
                Current.Courses = Current.Courses ?? new List<Course>();
                Current.Students = Current.Students ?? new List<Student>();
                Current.Teachers = Current.Teachers ?? new List<Teacher>();
                Current.Classes = Current.Classes ?? new List<Class>();
                Current.ClassSessions = Current.ClassSessions ?? new List<ClassSession>();
                Current.ClassSchedules = Current.ClassSchedules ?? new List<ClassSchedule>();
                var newYear=new ActivityCalendar { Name = "fahm-e-islam.com - Year # 1" };
                if (Current.CalendarYear == null)
                {
                    newYear.Init();
                    Current.CalendarYear = newYear;
                }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Current.MessageBoard = Current.MessageBoard ?? new MessageBoard() { Messages=new List<Message>()};

                

            }
        }

        public List<Course> Courses { get; set; }
        public List<Student> Students { get; set; }
        public List<Teacher> Teachers { get; set; }

        public List<Class> Classes { get; set; }
        public List<ClassSession> ClassSessions { get; set; }

        public List<ClassSchedule> ClassSchedules { get; set; }

        public ActivityCalendar CalendarYear { get; set; }

        /// <summary>
        /// Central DB of all student-2-student and teacher-2-teacher and student-2-teacher communication:
        /// . News & Events
        /// . Announcement
        /// . Messages
        /// . Alerts/Reminders
        /// . To-Do's
        /// </summary>
        public MessageBoard MessageBoard { get; set; }

        #region Methods
        #region Add/Update Methods
        public Guid AddOrUpdate(Course course)
        {
            course.Topics = course.Topics ?? new List<CourseTopic>();
            course.Topics=course.Topics.Where(t => !string.IsNullOrEmpty(t.Description)).ToList();
            var old = this.Courses.Where(c => c.Id == course.Id).SingleOrDefault();
        
        
            if (old == null)//add
                this.Courses.Add(course);
            else//update
            {              
                this.Courses.Remove(old);
                this.Courses.Add(course);
            }
            SaveChanges();
            return course.Id;
        }
        public Guid AddOrUpdate(Teacher obj)
        {
           var old = this.Teachers.Where(c => c.Id == obj.Id).SingleOrDefault();
        
        
            if (old == null)//add
                this.Teachers.Add(obj);

            else
            {
                this.Teachers.Remove(old);
                this.Teachers.Add(obj);
            }
            SaveChanges();
            return obj.Id;
        }
        public Guid AddOrUpdate(Student obj)
        {
            var old = this.Students.Where(c => c.Id == obj.Id).SingleOrDefault();


            if (old == null)//add
                this.Students.Add(obj);

            else
            {
                this.Students.Remove(old);
                this.Students.Add(obj);
            }
            SaveChanges();
            return obj.Id;
      
        }
        public Guid AddOrUpdate(Class obj)
        {
            var old = this.Classes.Where(c => c.Id == obj.Id).SingleOrDefault();


            if (old == null)//add
                this.Classes.Add(obj);

            else
            {
                this.Classes.Remove(old);
                this.Classes.Add(obj);
            }
            SaveChanges();
            return obj.Id;
        }

        public Guid AddOrUpdate(ClassSession obj)
        {
            var old = this.ClassSessions.Where(c => c.Id == obj.Id).SingleOrDefault();


            if (old == null)//add
                this.ClassSessions.Add(obj);

            else
            {
                this.ClassSessions.Remove(old);
                this.ClassSessions.Add(obj);
            }
            SaveChanges();
            return obj.Id;
        }

        public Guid AddOrUpdate(ClassSchedule obj)
        {
            var old = this.ClassSchedules.Where(c => c.Id == obj.Id).SingleOrDefault();


            if (old == null)//add
                this.ClassSchedules.Add(obj);

            else
            {
                this.ClassSchedules.Remove(old);
                this.ClassSchedules.Add(obj);
            }
            SaveChanges();
            return obj.Id;
        }

        public Guid AddOrUpdate(ActivityDay obj)
        {
            var date = this.CalendarYear.Days.Where(day=>day.Date==obj.Date).SingleOrDefault();
            AutoMapper.Mapper.CreateMap<ActivityDay, ActivityDay>();
            AutoMapper.Mapper.Map(obj, date);
           
            SaveChanges();
            return obj.Id;
        }

        public bool AddOrUpdate(Message obj)
        {
            var old = this.MessageBoard.Messages.Where(m => m.Id == obj.Id).SingleOrDefault();
            if (old == null)//add
                this.MessageBoard.Messages.Add(obj);

            else
            {
                this.MessageBoard.Messages.Remove(old);
                this.MessageBoard.Messages.Add(obj);
            }
            SaveChanges();
           

            return true;
        }
     
        #endregion
        #region Get Methods
        

        public List<Student> GetStudentsByCourseId(Guid courseId)
        {
            
            return this.Students.Where(student => student.Courses.Contains(courseId)).ToList();
        }

       
        public List<Student> GetStudentsByClassId(Guid classId)
        {
               //var result = from s in this.Students
               //          join c in this.Courses on s.Cou equals  s.
               //          join c in this.Classes on  s.ClassId equals c.Id 
               //          where c.TeacherId == teacherId
               //          select cs;
            var classStudentIds= this.ClassSchedules.Where(cs => cs.ClassId==classId).SingleOrDefault().Students.ToList();
            var studentsPerCR = this.Students.Where(s => classStudentIds.Contains(s.Id)).ToList();
            return studentsPerCR;
        }
        
        public List<Teacher> GetTeachersByCourseId(Guid courseId)
        {
            
            return this.Teachers.Where(teacher => teacher.Courses.Contains(courseId)).ToList();
        }
        public List<Course> GetCoursesByTeacherId(Guid teacherId)
        {
            
            var teacher= this.Teachers.Where(t=>t.Id==teacherId).SingleOrDefault();
            var courses = this.Courses.Where(c => teacher.Courses.Contains(c.Id)).ToList();
            return courses;
        }
        public Course GetCourseById(Guid courseId)
        {
            
            var course = this.Courses.Where(c => c.Id == courseId).FirstOrDefault();
            course.Topics = course.Topics ?? new List<CourseTopic>();
            return course;

        }
        public string NewStudentId()
        {
            return string.Format("S{0}{1}", DateTime.Now.Minute, DateTime.Now.Second);
        }
        public string NewTeacherId()
        {
            return string.Format("T{0}{1}", DateTime.Now.Minute, DateTime.Now.Second);
        }
        public User GetUserById(Guid userId)
        {

            User user = this.Students.Where(c => c.Id == userId).SingleOrDefault();
            if (user == null)
            {
                user = this.Teachers.Where(t => t.Id == userId).SingleOrDefault();
            }
            return user;
        }
        public User GetUserByUserId(string userId)
        {

            User user = this.Students.Where(c => c.UserId == userId).SingleOrDefault();
            if (user == null)
            {
                user = this.Teachers.Where(t => t.UserId == userId).SingleOrDefault();
            }
            return user;
        }
        public Student GetStudentById(Guid id)
        {
            
            var student= this.Students.Where(c => c.Id == id).FirstOrDefault();
            if (student!=null)
            student.Courses = student.Courses ?? new List<Guid>();
            return student;
        }
        public Student GetStudentByEmailId(string email)
        {

            var student = this.Students.Where(c => c.Email == email).FirstOrDefault();
            
            return student;
        }
        public Teacher GetTeacherByEmailId(string email)
        {

            var teacher = this.Teachers.Where(c => c.Email == email).FirstOrDefault();

            return teacher;
        }
        public Teacher GetTeacherById(Guid id)
        {
            
            var teacher= this.Teachers.Where(c => c.Id == id).FirstOrDefault();
            teacher.Courses = teacher.Courses ?? new List<Guid>();
            return teacher;
        }
        public Class GetClassById(Guid classId)
        {
            
            var cls = this.Classes.Where(c => c.Id == classId).FirstOrDefault();
           
            return cls;

        }
        public List<Class> GetClassesByTeacherId(Guid teacherId)
        {
            
            var cls = this.Classes.Where(c => c.TeacherId == teacherId).ToList();

            return cls;

        }
        /// <summary>
        /// Normally a student will have a single class;hence this List should only have 1 record
        /// </summary>
        /// <param name="studentId"></param>
        /// <returns></returns>
        public List<Class> GetStudentClasses(Guid studentId)
        {
            var student = GetStudentById(studentId);

                var result = from cls in this.Classes
                             join c in this.ClassSchedules on cls.Id equals c.ClassId                             
                             where c.Students.Contains(student.Id)
                             select cls;

                return result.ToList();

        }
        public List<User> GetTeachersContacts(Guid teacherId)
        {
            var teacher = GetTeacherById(teacherId);
            var students = GetStudentsByTeacherId(teacherId);
            var courses=GetCoursesByTeacherId(teacherId);

            var toReturn = new List<User>();

            foreach (var c in courses)
            {
                var teachers=GetTeachersByCourseId(c.Id);
                foreach (var t in teachers)
	            {
                    if(!toReturn.Contains(t))
                    toReturn.Add(t);   
	            }                
            }

            toReturn.AddRange(students);
            toReturn.Remove(teacher);


            return toReturn;
        }
        public List<User> GetClassMates(Guid studentId)
        {
            var student = GetStudentById(studentId);

            var toReturn = new List<User>();
            //get Id's of his classmates as per his class schedules
            var result = from cls in this.Classes
                         join c in this.ClassSchedules   on cls.Id equals c.ClassId   
                         where c.Students.Contains(student.Id)
                         select c.Students;
            //finally, get students matching selected students Id's above
            toReturn = this.Students.Where(s => result.SelectMany(r=>r).Contains(s.Id)).ToList<User>();
            //exclude himself
            toReturn.Remove(student);

            //this is his classmates only
            return toReturn;
        }
        public List<Class> GetClassesByCourseId(Guid courseId)
        {

            var cls = this.Classes.Where(c => c.CourseId == courseId).ToList();

            return cls;

        }
        //public List<ClassSession> GetStudentsByClassId(int classId)
        //{

        //    var result = from stu in this.Students
        //                 join c in this.Courses on
        //                 stu.C equals c.
        //                 where c.TeacherId == teacherId
        //                 select cs;

        //    return result.ToList();

        //}
        public List<ClassSession> GetClassSessionsByTeacherId(Guid teacherId)
        {
            var result2 = this.ClassSessions;

            var result = from cs in this.ClassSessions
                         join s in this.ClassSchedules on cs.ClassScheduleId equals  s.Id
                         join c in this.Classes on  s.ClassId equals c.Id 
                         where c.TeacherId == teacherId
                         select cs;
          
            return result.ToList();

        }
        public List<Student> GetStudentsByTeacherId(Guid teacherId)
        {
            var classes = GetClassesByTeacherId(teacherId);
            var classSchedules=GetClassSchedulesByTeacherId(teacherId);
            var students = classSchedules.SelectMany(cs => cs.Students).ToList();
            var studentsPerCR = this.Students.Where(s => students.Contains(s.Id)).ToList();
            return studentsPerCR;
        }
        public List<ClassSchedule> GetClassSchedulesByTeacherId(Guid teacherId)
        {

            var result = from cs in this.ClassSchedules
                         join c in this.Classes on
                         cs.ClassId equals c.Id
                         where c.TeacherId == teacherId
                         select cs;

            return result.ToList();

        }
        public Course GetCourseByClassId(Guid clsId)
        {

            var cls = this.Classes.SingleOrDefault(c => c.Id== clsId);
            var toReturn = this.Courses.SingleOrDefault(c => c.Id == cls.CourseId);
            return toReturn;

        }
        public Course GetClassBySClassId(Guid clsId)
        {

            var cls = this.Classes.SingleOrDefault(c => c.Id == clsId);
            var toReturn = this.Courses.SingleOrDefault(c => c.Id == cls.CourseId);
            return toReturn;

        }
        public ClassSchedule GetClassScheduleById(Guid id)
        {


            //var toReturn = this.Courses.SingleOrDefault(c => c.Id == cls.CourseId);
            return this.ClassSchedules.SingleOrDefault(c => c.Id == id);

        }
        public List<Student> GetStudentsByScheduleId(Guid scheduleId)
        {


            var schedule = this.GetClassScheduleById(scheduleId);
            return this.Students.Where(s => schedule.Students.Contains(s.Id)).ToList();

        }
        public ClassSchedule GetClassScheduleByClassId(Guid clsId)
        {

           
            //var toReturn = this.Courses.SingleOrDefault(c => c.Id == cls.CourseId);
            return this.ClassSchedules.SingleOrDefault(c => c.ClassId == clsId);

        }

        public ResourceFile GetFileByTeacherId(Guid teacherId, Guid fileId)
        {
            return this.Teachers.SingleOrDefault(t => t.Id == teacherId).Files.SingleOrDefault(f => f.Id == fileId);
        }
        public ResourceFile GetFileByStudentId(Guid studentId, Guid fileId)
        {
            return this.Students.SingleOrDefault(t => t.Id == studentId).Files.SingleOrDefault(f => f.Id == fileId);
        }
        public List<ResourceFile> GetFilesByStudentCourse(Guid studentId)
        {
            var student = GetStudentById(studentId);
            var courses = student.Courses;
            var sharedfilesPerStudentsCourse = new List<ResourceFile>();
            foreach (var t in this.Teachers)
            {
                var perTeacher=t.Files.Where(f => courses.Contains(f.Links.CourseIds[0])).ToList();
                sharedfilesPerStudentsCourse.AddRange(perTeacher);
            }
            return sharedfilesPerStudentsCourse;
        }
        public List<ResourceFile> GetFilesForStudent(Guid studentId)
        {
            //var student = GetStudentById(studentId);
            //var courses = student.Courses;
            var sharedfilesPerStudentsCourse = new List<ResourceFile>();
            foreach (var t in this.Teachers)
            {
                var perTeacher=t.Files.Where(f =>f.Links.StudentIds!=null&& f.Links.StudentIds.Contains(studentId)).ToList();
                sharedfilesPerStudentsCourse.AddRange(perTeacher);
            }
            return sharedfilesPerStudentsCourse;
        }
        
        #endregion
        #region Delete Methods
        public bool DeleteTeacher(Guid id)
        {
            var old = this.Teachers.Where(c => c.Id == id).SingleOrDefault();
            this.Teachers.Remove(old);
            SaveChanges();
            return true;
        }
        public bool DeleteStudent(Guid id)
        {
            var old = this.Students.Where(c => c.Id == id).SingleOrDefault();
            this.Students.Remove(old);
            SaveChanges();
            return true;
        }
        public bool DeleteCourse(Guid id)
        {
            var old = this.Courses.Where(c => c.Id == id).SingleOrDefault();
            this.Courses.Remove(old);
            SaveChanges();
            return true;
        }

        public bool DeleteClass(Guid id)
        {
            var old = this.Classes.Where(c => c.Id == id).SingleOrDefault();
            this.Classes.Remove(old);
            SaveChanges();
            return true;
        }
        #endregion
        #region User Security Methods
        public bool IsStudent(Guid userId)
        {
            var student = this.Students.SingleOrDefault(c => c.Id == userId);

            return student != null;
            
        }
        public bool IsTeacher(Guid userId)
        {
            var teacher = this.Teachers.SingleOrDefault(c => c.Id == userId);

            return teacher != null;

        }
        public bool IsStudent(string userId, string pw)
        {
            var student = this.Students.SingleOrDefault(c => c.UserId == userId && c.Password==pw );
            return student != null;
          
        }
        public BaseEntity GetStudent(string userId, string pw)
        {
            var student = this.Students.SingleOrDefault(c => c.UserId == userId && c.Password == pw);
            return student;

        }
        public bool IsTeacher(string userId, string pw)
        {
            var teacher = this.Teachers.SingleOrDefault(c => c.UserId == userId && c.Password==pw);
            return teacher != null;
        }
        public BaseEntity GetTeacher(string userId, string pw)
        {
            var teacher = this.Teachers.SingleOrDefault(c => c.UserId == userId && c.Password == pw);
            return teacher;

        }

        #endregion

        private void SaveChanges()
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(HostingEnvironment.ApplicationPhysicalPath + "\\academy.json"))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, Academy.Current);
            }

        }
        #endregion

    }
    public abstract class BaseEntity
    {
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Display(Name="Active")]
        public bool Status { get; set; }

        public string Remarks { get; set; }
    }
    /// <summary>
    /// For Only Student or Teacher
    /// </summary>
    public abstract class User : BaseEntity
    {
        public virtual void Init()
        {
            Id = Guid.NewGuid();
            Files = new List<ResourceFile>();
            Courses = new List<Guid>();
            CmbCourse = new List<SelectListItem>();
        }
        public string NIC { get; set; }
        [Required]
        public Gender Sex { get; set; }
        [Display(Name = "Date of Birth")]
        public DateTime DOB { get; set; }
        [Display(Name = "Contact No")]
        public string ContactNo { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Display(Name = "Postal Address")]
        public string PostalAddress { get; set; }
        [Required(ErrorMessage="Please Enter A Email Address!")]

        [EmailAddress(ErrorMessage="Enter A Valid Email Address")]

        [Remote("IsUniqueEmail"/*ActionMethod*/, "User" /*Controller*/, AdditionalFields = "Email",
            ErrorMessage = "Email is already registered")]

        public string Email { get; set; }
        [Required]
        [Display(Name = "Login ID")]

        [Remote("IsUniqueCode", "User", AdditionalFields = "UserId", 
            ErrorMessage = "UserID already exists")]

        public string UserId { get; set; }
        [Required]
        public string Password { get; set; }
       
        [Required]
        public List<Guid> Courses { get; set; }
        [Display(Name = "Courses")]
        public List<SelectListItem> CmbCourse { get; set; }

        public List<ResourceFile> Files { get; set; }
        /// <summary>
        /// Used to Show Only Messages To this user
        /// </summary>
        public MessageBox Inbox { get; set; }
        /// <summary>
        /// Used to Show Only Messages From this user
        /// </summary>
        public MessageBox Sent { get; set; }

        public static bool IsUniqueCode(string code)
        {
            var uniqueInTeachers = Academy.Current.Teachers.Where(m => m.UserId.ToLower().Equals(code.ToLower())).SingleOrDefault() == null;
            var uniqueInStudents = Academy.Current.Students.Where(m => m.UserId.ToLower().Equals(code.ToLower())).SingleOrDefault() == null;
            return uniqueInTeachers && uniqueInStudents;
        }
        public static bool IsUniqueEmail(string email)
        {
            var uniqueInTeachers = Academy.Current.Teachers.Where(m => m.Email.ToLower().Equals(email.ToLower())).SingleOrDefault() == null;
            var uniqueInStudents = Academy.Current.Students.Where(m => m.Email.ToLower().Equals(email.ToLower())).SingleOrDefault() == null;
            return uniqueInTeachers && uniqueInStudents;
        }
       
    }
    public class CourseTopic
    {
        public int Id { get; set; }

        public string Description { get; set; }
    }
    public class Course : BaseEntity
    {
        public Course()
        {
            this.Topics = new List<CourseTopic>();
        }
     [Required]
        public List<CourseTopic> Topics { get; set; }
         [Display(Name="No of Classes")]
         [Required]
        public int NoOfClasses { get; set; }

        public Course AddTopic(CourseTopic topic)
        {
            this.Topics.Add(topic);
            return this;
        }

    }
    public class Teacher : User
    {
        public Teacher()
        {
            
         
            CmbCourse = new List<SelectListItem>();
        }

      
        public Teacher AddCourse(Guid courseId)
        {
            this.Courses.Add(courseId);
            return this;
        }


        public override void Init()
        {
            base.Init();
            var uid = Academy.Current.NewTeacherId();//here inside cons, Current will always be null as Deserialization has just started to create obj of type Student
            UserId = uid;
            Password = uid;
 
        }
    }

    #region MessageBoard object Model
    
    public class MessageBoard : BaseEntity
    {
        /// <summary>
        /// Central Messageboard where all messages will be saved
        /// </summary>
        public List<Message> Messages { get; set; }
    }

    public class MessageBox : BaseEntity
    {
 
       
        /// <summary>
        /// every User's Inbox will have messages 'FROM him or TO him' popultaed on load from Main MB
        /// </summary>
        public List<Message> Messages { get; set; }
       

    }
    public enum MessageType
    {
        Message,
        News,
        Announcement,
        Reminder,
        Alert,
        Warning

    }
    public enum MessageStatus
    {
        Unread,
        Read,
        Deleted,
        Archived,
        Bookmarked,
        etc

    }
    /// <summary>
    /// UNUSED for now; will use User
    /// </summary>
    public class MessageRecipient : BaseEntity {

        public Guid UserId { get; set; }
    
    }
    public class Message : BaseEntity
    {
        public Message()
        {
            this.RecipientIds = new List<Guid>();
            this.CmbRecipients = new List<SelectListItem>();
        }
        public Guid SenderId { get; set; }
        [Display(Name="Message Type")]
        public MessageType MessageType { get; set; }
        /// <summary>
        /// used for sorting
        /// </summary>
        public int DisplayOrder { get; set; }

        public DateTime DateCreated { get; set; }
        /// <summary>
        /// Message body is actual message string cntent
        /// </summary>
        public string Body { get; set; }

       public List<User> Recipients { get; set; }
        public List<Guid> RecipientIds { get; set; }
        [Display(Name = "To")]

        public List<SelectListItem> CmbRecipients { get; set; }

        public void FillRecipientsDropdown(){
            foreach (var user in this.Recipients)
            {
                this.RecipientIds.Add(user.Id);
                this.CmbRecipients.Add(new SelectListItem { Text=user.Name,Value=user.Id.ToString()});
            }
        }
        public MessageStatus MessageStatus { get; set; }
     
        public string GetFrom() { 
       
                return Academy.Current.GetUserById(this.SenderId).Name;

            
        }
        public string GetTo()
        {
       
                string rc = string.Empty;
                if (this.RecipientIds != null)
                {
                    foreach (var to in this.RecipientIds)
                    {
                        var user = Academy.Current.GetUserById(to);
                        rc += user.Name + ",";
                    }
                    //remoe last comma
                    if (this.RecipientIds.Count > 0)
                        rc = rc.Substring(0, rc.Length - 1);
                }
                return rc;

            
        }

    }

    #endregion


    public class Student : User
    {
        public Student()
        {
          
        
        }
      

        public Student AddCourse(Guid courseId)
        {
            this.Courses.Add(courseId);
            return this;
        }


        public override void Init()
        {
            base.Init();
            var uid = Academy.Current.NewStudentId();//here inside cons, Current will always be null as Deserialization has just started to create obj of type Student
            UserId = uid;
            Password = uid;

        }
    }
    /// <summary>
    /// Classess running in the Academy
    /// </summary>
    public class Class:
        BaseEntity
    {
        public Class()
        {
            Id = Guid.NewGuid();

            CmbTeachers = new List<SelectListItem>();
            CmbCourse = new List<SelectListItem>();
        }
        public double TotalNoOfMinutes
        {
            get
            {
                return (this.Duration.TotalMinutes * Academy.Current.GetCourseById(this.CourseId).NoOfClasses);
            }
        }


        public double TotalNoOfHours
        {
            get
            {
                return (this.Duration.TotalHours * Academy.Current.GetCourseById(this.CourseId).NoOfClasses);
            }
        }
        [Required]
        public string ClassDisplayName { get; set; }

        /// <summary>
        /// Course which is being taught in this class
        /// </summary>
        public Guid CourseId { get; set; }
        [Display(Name = "Course")]
        [Required]
        public List<SelectListItem> CmbCourse { get; set; }
        /// <summary>
        /// every class is taught under a teacher
        /// </summary>
        public Guid TeacherId { get; set; }
        [Display(Name = "Teacher")]
        [Required]
        public List<SelectListItem> CmbTeachers { get; set; }
        [Required]
        /// <summary>
        /// Duration of Class in hh:mm
        /// </summary>
        /// 
        public TimeSpan Duration { get; set; }

    }
    /// <summary>
    /// Reralworld classroom concept; currently it shows a class taken in a single session
    /// </summary>
    public class ClassSession:
        BaseEntity
    {
        public ClassSession()
        {
            Id = Guid.NewGuid();

            StartTime = DateTime.Now;
            EndTime = DateTime.Now;
            //CmbClasses = new List<SelectListItem>();
        }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Guid ClassScheduleId { get; set; }

        //public int ClassId { get; set; }
        //[Display(Name = "Class")]
        //[Required]
        //public List<SelectListItem> CmbClasses { get; set; }
        [Required]
        public Attendance Attendance { get; set; }

        public string TimeElapsed
        {
            get
            {
                var elapsed = EndTime - StartTime;
                var timeOfClassElapsed = String.Format(System.Globalization.CultureInfo.CurrentCulture, "{0}:{1}:{2}", elapsed.Hours.ToString().PadLeft(2,'0'),
                    elapsed.Minutes.ToString().PadLeft(2, '0'),
                    elapsed.Seconds.ToString().PadLeft(2, '0'));
                return timeOfClassElapsed;
   
            }
        }
    }

    public class Attendance : BaseEntity
    {
        public Attendance()
        {
            Id = Guid.NewGuid();

            Date = DateTime.Today;
            Students = new List<Guid>();
            CmbStudents = new List<SelectListItem>();
        }
        public DateTime Date { get; set; }
        [Required(ErrorMessage="Select one or more students.")]
        public List<Guid> Students { get; set; }
        [Display(Name = "Students")]
     
        public List<SelectListItem> CmbStudents { get; set; }
    }

    public enum DayStatus
    {
        Active,Holiday,Cancelled
    }
    public class ActivityDay:
        BaseEntity
    {
        public int DayOfYear { get; set; }
        public DateTime Date { get; set; }
        public DayStatus ActivityStatus { get; set; }
    }
    /// <summary>
    /// Calendar for a class (weekly/monthly/full)
    /// </summary>
    public class ClassSchedule:
        BaseEntity
    {
        public ClassSchedule()
        {
            Id = Guid.NewGuid();

             Students = new List<Guid>();
             CmbStudents = new List<SelectListItem>();
        }
        
        public List<Guid> Students { get; set; }
        [Display(Name = "Students")]

        public List<SelectListItem> CmbStudents { get; set; }
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public Guid ClassId { get; set; }
        public List<ActivityDay> Days { get; set; }
        public void Init()
        {
            var course = Academy.Current.GetCourseByClassId(ClassId);
            var classesWithOff = (int)(course.NoOfClasses + Math.Round(((double)course.NoOfClasses / 7)));
            End = Start.AddDays(classesWithOff);
            Days = Academy.Current.CalendarYear.GetPeriod(Start, End);
        
        }
        public void DateRange(string from,string  to)
        {
            var fromDt = DateTime.Parse(from);
            var toDt = DateTime.Parse(to);
            
            Start = fromDt;
            End = toDt;
            
            Days = Academy.Current.CalendarYear.GetPeriod(Start, End);

        }
    }
    public class ActivityCalendar : BaseEntity
    {
        static GregorianCalendar calendarYear = new GregorianCalendar();
           
        public ActivityCalendar()
        {

           
                      
        }
        public void Init()
        {
            Id = Guid.NewGuid();

            this.Days = new List<ActivityDay>();

            var daysInYr = calendarYear.GetDaysInYear(DateTime.Now.Year);
            var startOfYr = new DateTime(DateTime.Now.Year, 1, 1);
            for (int day = 0; day < daysInYr; day++)
            {
                var dateOfYr = new ActivityDay {Id=Guid.NewGuid(), DayOfYear = day, Date = startOfYr.AddDays(day) };
                dateOfYr.Name = calendarYear.GetDayOfWeek(dateOfYr.Date).ToString();
                dateOfYr.Status = true;
                dateOfYr.ActivityStatus = calendarYear.GetDayOfWeek(dateOfYr.Date) == DayOfWeek.Friday ? DayStatus.Holiday : DayStatus.Active;
                Days.Add(dateOfYr);

            }
        }
        public List<ActivityDay> Days { get; set; }
        public List<ActivityDay> CurrentWeek { 
            get {
                var days = new List<ActivityDay>();
                var dayNumber = (int)DateTime.Now.DayOfWeek;
                var weekDay = DateTime.Now.AddDays(-dayNumber).Date;
                for (int i = 0; i < 7; i++)
                {
                    days.Add(this.Days.SingleOrDefault(d => d.Date == weekDay));
                    weekDay = weekDay.AddDays(1);
                }
                return days;
            }
        }
        public List<ActivityDay> GetPeriod(DateTime start,DateTime end)
        {
            
            var days = new List<ActivityDay>();
            DateTime endDate = start;
            do
            {
                endDate = endDate.AddDays(1);
                days.Add(this.Days.SingleOrDefault(d => d.Date == endDate));
            } while (endDate.CompareTo(end) != 0);
                days.RemoveAll(d => d == null);
                return days;
            
        }
        public List<ActivityDay> CurrentMonth {
            get
            {
                var days = new List<ActivityDay>();
                var daysInMonth = calendarYear.GetDaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                for (int day = 1; day <= daysInMonth; day++)
                {
                    var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                    days.Add(this.Days.SingleOrDefault(d=>d.Date==date));
                  
                }
                 return days;
            }
        }
    }

    public enum FileType
    {
        AudioLecture,
        AudioChat,
        EBook,
        Help,
        Assignment,
        Quiz,
        Test,
        Image,
        Unknown
    }
    public class ResourceFile:
        BaseEntity, IEquatable<ResourceFile>
    
    {
      
        public override string ToString()
        {
            return FileName;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            ResourceFile objAsPart = obj as ResourceFile;
            if (objAsPart == null) return false;
            else return Equals(objAsPart);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public bool Equals(ResourceFile other)
        {
            if (other == null) return false;
            return (this.UniqueId.Equals(other.UniqueId));
        }

        /// <summary>
        /// used to group multiple files upload/download
        /// </summary>
        public Guid UniqueId { get; set; }
        public string FileName { get; set; }
        /// <summary>
        /// Id of Teacher or Student
        /// </summary>
        //public int SharedBy { get; set; }
        public ResourceFile()
        {
            
        }
        public void Init()
        {
            DateCreated = DateTime.Now;
            DateModified = DateTime.Now;
            Type = FileType.Unknown;
            Extention = string.Empty;
            Path = string.Empty;
            UniqueId = Guid.NewGuid();
            FileName = string.Empty;
            Id = Guid.NewGuid();
          
            
        }
        public string SharedBy { get; set; }

        public FileLink Links { get; set; }
        [Display(Name="Last Modified")]
        public DateTime DateModified { get; set; }
        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
        public FileType Type { get; set; }
        public string Path { get; set; }
        public int Size { get; set; }

        public string Extention { get; set; }
        [Display(Name = "Compressed")]
        public bool IsCompressed { get; set; }

    }

    public class FileLink:BaseEntity
    {
        public FileLink()
        {
            Id = Guid.NewGuid();

            CmbCourse = new List<SelectListItem>();
            CmbClass = new List<SelectListItem>();
            CmbStudent = new List<SelectListItem>();
            CmbTeacher = new List<SelectListItem>();
        }
        public Guid FileId { get; set; }
                [Display(Name = "Course")]

        public List<SelectListItem> CmbCourse { get; set; }
                [Display(Name = "Class")]

        public List<SelectListItem> CmbClass { get; set; }
                [Display(Name = "Student")]

        public List<SelectListItem> CmbStudent { get; set; }
                [Display(Name = "Teacher")]

        public List<SelectListItem> CmbTeacher { get; set; }
        /// <summary>
        /// if assignee=Student/Teacher, only selected class's courseId
        /// if assignee=Admin, all courses selected
        /// </summary>
        public List<Guid> CourseIds { get; set; }
        /// <summary>
        /// if assignee=Teacher, all classes assigned
        /// if assignee=Student, only selected/his class Id
        /// </summary>
        public List<Guid> ClassIds { get; set; }
        /// <summary>
        /// if assignee=Teacher, all students' id's assigned
        /// if assignee=Student, only his Id
        /// </summary>
        public List<Guid> StudentIds { get; set; }
        /// <summary>
        /// if assignee=Teacher, only his Id
        /// if assignee=Student, only selected class teacher Id
        /// </summary>
        public List<Guid> TeacherIds { get; set; }
    }
}
