using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ZeeSoft.ClassRoomJson;

namespace ZeeSoft.Web.MVC.FI.Helper
{
    public static class ExtensionMethods
    {
        public static string Truncate(this string value, int maxChars)
        {
            return value.Length <= maxChars ? value : value.Substring(0, maxChars) + "...";
        }
        public static string GetUserNameById(this Guid id)
        {
            var student=Academy.Current.GetStudentById(id);
            var teacher=Academy.Current.GetTeacherById(id);
            if (student != null)
                return student.Name;
            else if (teacher != null)
                return teacher.Name;
            else
                return "Unknown User";
        }
        //public static Student GetStudentNameById(this int id)
        //{
        //    return Academy.Current.GetStudentById(id).Name;
        //}
        public static List<SelectListItem> ToSelectList<T>(this List<T> items,string selectedValue) where T : BaseEntity
        {
            var Cmb = new List<SelectListItem>();
            foreach (var item in items)
            {
                if(!string.IsNullOrEmpty(selectedValue) && selectedValue==item.Id.ToString())
                    Cmb.Add(new SelectListItem() {Selected=true, Text = item.Name, Value = item.Id + "" });
                else
                    Cmb.Add(new SelectListItem() { Text = item.Name, Value = item.Id + "" });
            }
            return Cmb;
        }
        public static List<SelectListItem> ToSelectList<T>(this List<T> items, List<Guid> selectedValues) where T : BaseEntity
        {
            var Cmb = new List<SelectListItem>();
            foreach (var item in items)
            {
                if (selectedValues!=null)
                    Cmb.Add(new SelectListItem() { Selected = selectedValues.Contains(item.Id), Text = item.Name, Value = item.Id + "" });
                else
                    Cmb.Add(new SelectListItem() { Text = item.Name, Value = item.Id + "" });
            }
            return Cmb;
        }
        public static string ToJSDate(this DateTime datetime)
        {
            var jsDt= string.Format("{0}/{1}/{2} {3}:{4}:{5}", datetime.Year,
                datetime.Month, datetime.Date.Day,
                datetime.Hour, datetime.Minute, datetime.Second);
            return jsDt;
        }
        public static string ToYesNo(this bool val)
        {
            return val ? "Yes" : "No";
        }
    }
}