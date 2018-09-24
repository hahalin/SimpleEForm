using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eform
{
    public static class dateUtils
    {
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
    }
}