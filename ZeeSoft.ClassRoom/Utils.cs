using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using ZeeSoft.ClassRoomJson;

namespace WebApplication2.Helper
{
    public class Utils
    {
        public static List<ActivityDay> GetCalendarDaysFor(int? period/*0=yearly, 1=monthly and 2=weekly*/)
        {
            var days = new List<ActivityDay>();
            //var calendar = new ActivityCalendar();        
                var calendarYear = new GregorianCalendar();
                switch (period)
                {
                    case 0:                         
                        
                        var dayNumber=(int)DateTime.Now.DayOfWeek;
                        var weekDay = DateTime.Now.AddDays(-dayNumber);
                        
                        for (int i=0;i<7;i++)
	                    {
                            
                            days.Add(new ActivityDay {
                                Date = weekDay,
                                Name = calendarYear.GetDayOfWeek(weekDay).ToString(),
                                Status=true,
                                ActivityStatus = calendarYear.GetDayOfWeek(weekDay) == DayOfWeek.Friday ?
                                DayStatus.Holiday
                                :DayStatus.Active});
                            weekDay=weekDay.AddDays(1);
	                    }
                        break;
                    case 1:                         
                        
                        var daysInMonth=calendarYear.GetDaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                        for (int day = 1; day <= daysInMonth;day++ )
                        {
                            var date = new DateTime(DateTime.Now.Year, DateTime.Now.Month, day);
                            days.Add(new ActivityDay
                            {
                                Date = date,
                                Name = calendarYear.GetDayOfWeek(date).ToString(),
                                Status = true,
                                ActivityStatus = calendarYear.GetDayOfWeek(date) ==
                                DayOfWeek.Friday ? DayStatus.Holiday : DayStatus.Active
                            });
                        }
                        break;
                    default:
                                          
                        
                        var daysInYr=calendarYear.GetDaysInYear(DateTime.Now.Year);
                        var startOfYr = new DateTime(DateTime.Now.Year, 1, 1);
                        for (int day = 0; day < daysInYr; day++)
                        {
                            var dateOfYr = new ActivityDay {Date=startOfYr.AddDays(day) };
                            dateOfYr.Name = calendarYear.GetDayOfWeek(dateOfYr.Date).ToString(); 
                            dateOfYr.Status = true;
                            dateOfYr.ActivityStatus = calendarYear.GetDayOfWeek(dateOfYr.Date) == DayOfWeek.Friday ? DayStatus.Holiday : DayStatus.Active;
                            days.Add(dateOfYr);
                            
	                    }
                      
                        break;
                }
                return days;
            
        }
    }
}