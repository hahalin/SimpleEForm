using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace eform.Repo
{
    public static class numberUtils
    {
        public static int getInt(object v)
        {
            int r = 0;
            try
            {
                r = int.Parse(v.ToString());
            }
            catch(Exception ex)
            {
                r = 0;
            }
            return r;
        }

        public static decimal getDecimal(object v)
        {
            decimal r = 0;
            try
            {
                r = Convert.ToDecimal(v.ToString());
            }
            catch (Exception ex)
            {
                r = 0;
            }
            return r;
        }
    }
}