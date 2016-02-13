using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZeeSoft.ClassRoomJson;

namespace WebApplication2.Models
{
    public class MessageBoxViewModel
    {
        public MessageBoxViewModel(){
            this.Inbox=this.Sent=this.Archives=this.Outbox = new MessageBox() { Messages=new List<Message>()};
        
        }
        public MessageBox Inbox { get; set; }
        public MessageBox Sent { get; set; }
        public MessageBox Archives { get; set; }
        public MessageBox Outbox { get; set; }
    }
    public class Dashboard
    {
        public int NewMessages { get; set; }
    }

    public class TeacherDashboard : Dashboard
    {
        public TeacherDashboard()
        {
            this.Sessions = new List<ClassSession>();
            this.ClassSchedules = new List<ClassSchedule>();
            this.Classes = new List<Class>();
        }
        public List<ClassSession> Sessions { get; set; }
        public List<Class> Classes { get; set; }
        public List<ClassSchedule> ClassSchedules { get; set; }
    }
    public class StudentDashboard : Dashboard
    {
        public StudentDashboard()
        {
            this.Files = new List<ResourceFile>();
           
        }
        public List<ResourceFile> Files { get; set; }
        
    }
}