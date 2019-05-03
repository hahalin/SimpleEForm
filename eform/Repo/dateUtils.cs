using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace eform
{
    public static class dateUtils
    {
        public static Boolean parseDate(object sdt)
        {
            Boolean r = true;
            DateTime dt;

            try
            {
                dt = Convert.ToDateTime(sdt.ToString());
            }
            catch (Exception ex)
            {
                r = false;
            }

            return r;
        }

        public static DateTime checkDate(object sdt)
        {
            DateTime dt;

            try
            {
                dt = Convert.ToDateTime(sdt.ToString());
            }
            catch(Exception ex)
            {
                dt = DateTime.Today;
            }

            return dt;
        }

        public static int GetIso8601WeekOfYear(DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }

    }
}