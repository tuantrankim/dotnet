using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Sample.Hello.Common.Data;
using System.Data.SqlClient;
using Sample.Helper;
using Sample.Hello.BusinessFacade;

namespace Sample.Hello.DataAccess
{
    public class HelloCache
    {
        public static List<Configuration> Configuration_Select_By_Application(SqlConnection connection, SqlTransaction transaction, int applicationID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@ApplicationID", SqlDbType.Int)
            };
            DatabaseHelper.AssignParameterValue(parameters[0], applicationID);

            List<Configuration> collection = new List<Configuration>();
            using (SqlDataReader reader = DatabaseHelper.ExecuteReader(connection, transaction, "LA_Fitness.dbo.Configuration_Select_By_Application", parameters))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        Configuration pc = new Configuration();
                        pc.TypeID = reader["ConfigurationTypeID"] as int? ?? 0;
                        pc.ConfigValue = reader["Value"] as String;
                        collection.Add(pc);
                    }
                }
            }
            return collection;
        }

        public static List<ConfigurationExt> ConfigurationExt_Select_By_Application(SqlConnection connection, SqlTransaction transaction, int applicationID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@ApplicationID", SqlDbType.Int)
            };
            DatabaseHelper.AssignParameterValue(parameters[0], applicationID);

            List<ConfigurationExt> collection = new List<ConfigurationExt>();
            using (SqlDataReader reader = DatabaseHelper.ExecuteReader(connection, transaction, "LA_Fitness.dbo.ConfigurationExt_Select_By_Application",  parameters))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        ConfigurationExt pc = new ConfigurationExt();
                        pc.TypeID = reader["ConfigurationTypeID"] as int? ?? 0;
                        pc.ConfigValue = reader["Value"] as String;
                        pc.StartDate = reader["StartDate"] as DateTime? ?? DateTime.MinValue;
                        pc.EndDate = reader["EndDate"] as DateTime? ?? DateTime.MinValue;
                        collection.Add(pc);
                    }
                }
            }
            return collection;
        }


        public static Dictionary<int, ClubConfiguration> ClubConfiguration_Select_By_Application(
                                                                                    SqlConnection connection,
                                                                                    SqlTransaction transaction,
                                                                                    int applicationID)
        {

            SqlParameter[] parameters = 
                {
				    new SqlParameter("@ApplicationID", SqlDbType.Int)
                };

            DatabaseHelper.AssignParameterValue(parameters[0], applicationID);

            Dictionary<int, ClubConfiguration> dict = new Dictionary<int, ClubConfiguration>();
            using (SqlDataReader reader = DatabaseHelper.ExecuteReader(connection, transaction, "LA_Fitness.dbo.ClubConfiguration_Select_By_Application", parameters))
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        int configurationTypeID = reader["ConfigurationTypeID"] as int? ?? 0;
                        string value = reader["Value"].ToString();
                        int clubAphelionRowID = reader["ClubAphelionRowID"] as int? ?? 0;

                        if (clubAphelionRowID <= 0) continue;

                        ClubConfiguration clubConfig = null;
                        if (dict.ContainsKey(clubAphelionRowID)) clubConfig = dict[clubAphelionRowID];
                        else
                        {
                            clubConfig = new ClubConfiguration();
                            dict.Add(clubAphelionRowID, clubConfig);
                        }

                        if (configurationTypeID == (int)Common.Enumeration.ConfigurationTypes.Allow_Manual_Credit_Card_Input)
                        {
                            bool bValue = false;
                            if (bool.TryParse(value, out bValue)) clubConfig.AllowManualCreditCardInput = bValue;
                        }
                    }
                }
            }
            return dict;
        }
    }
}
