using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace eform
{
    public class dbHelper
    {
        private string conStr;
        public dbHelper()
        {
            conStr= WebConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        }

        public DataTable sql2tb(string sql)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            SqlDataReader rd = cmd.ExecuteReader();
            DataTable tb = new DataTable();
            tb.Load(rd);
            con.Close();
            return tb;
        }

        public int sql2count(string sql)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            try
            {
                return (int)cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public void execSql(string sql)
        {
            SqlConnection con = new SqlConnection(conStr);
            con.Open();
            SqlCommand cmd = new SqlCommand(sql, con);
            cmd.ExecuteNonQuery();
        }
    }
}