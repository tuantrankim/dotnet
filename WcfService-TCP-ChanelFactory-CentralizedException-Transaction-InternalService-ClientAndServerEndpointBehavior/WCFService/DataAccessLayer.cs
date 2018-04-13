using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    public struct EventType
    {
        public const string Error = "Error";
        public const string Information = "Information";
        public const string Performance = "Performance";
    }


    public class DataAccessLayer
    {
        //"Data Source=ServerName\InstanceName;Initial Catalog=DatabaseName;Integrated Security=True;MultipleActiveResultSets=True"
        private static String connectionString = "Data Source=CRP-DEV-02;Initial Catalog=LA_Fitness;Integrated Security=True;MultipleActiveResultSets=True";
        private static object _objLock = new object();
        public static object ToDBType(object value)
        {
            return value ?? DBNull.Value;
        }

        public static int Insert(int ID, string Name, DateTime? DOB)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@ID", ToDBType(ID));
                cmd.Parameters.AddWithValue("@Name", ToDBType(Name));
                cmd.Parameters.AddWithValue("@DOB", ToDBType(DOB));
                cmd.CommandText = @"INSERT INTO TestEMP(ID,Name,DOB)
                                values(@ID,@Name,@DOB)";
                return cmd.ExecuteNonQuery();
            }
        }
    }
}
