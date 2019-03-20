using Sample.Hello.Common.Data;
using Sample.Hello.Common.Enumeration;
using Sample.Hello.DataAccess;
using Sample.Helper;
using Sample.MemberManagement.Common.Enumeration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sample.Hello.BusinessFacade
{
    public class HelloCache
    {
        #region Global cache
        static readonly HelloCache instance = new HelloCache();
        private static DateTime _lastRefreshed = DateTime.MinValue;
        private static List<Configuration> _configurations;
        private static Dictionary<int, string> _configurationDictionary;
        private static List<ConfigurationExt> _configurationExts;
        private static Dictionary<int, ClubConfiguration> _clubConfigurations;
        private static AppConfig _appConfig;

        public static HelloCache Instance
        {
            get
            {
                if (_configurations == null
                   || _configurationExts == null
                   || _appConfig == null
                   || _clubConfigurations == null
                   || DateTime.Now - _lastRefreshed > Properties.Settings.Default.ReloadConfigurationInterval)
                {
                    Refresh();
                }
                return instance;
            }
        }
        public DateTime LastRefreshed
        {
            get { return _lastRefreshed; }
        }
        public AppConfig AppConfig
        {
            get
            {
                return _appConfig;
            }
        }
        public List<Configuration> Configurations
        {
            get
            {
                return _configurations;
            }
        }
        public Dictionary<int, string> ConfigurationDictionary
        {
            get
            {
                return _configurationDictionary;
            }
        }       
        public List<ConfigurationExt> ConfigurationExts
        {
            get
            {
                return _configurationExts;
            }
            
        }

        public Dictionary<int, ClubConfiguration> ClubConfigurations
        {
            get
            {
                return _clubConfigurations;
            }
        }

        #endregion Global cache

        /// <summary>
        /// Retrieves application configurations
        /// </summary>
        public static void Refresh()
        {
            SqlConnection connection = null;
            try
            {
                connection.Open();
                //Load the cached collection
                List<Configuration> newConfigurations = DataAccess.HelloCache.Configuration_Select_By_Application(connection, null, (int)Applications.Hello);
                List<ConfigurationExt> newConfigurationExts = DataAccess.HelloCache.ConfigurationExt_Select_By_Application(connection, null, (int)Applications.Hello);
                Dictionary<int, ClubConfiguration> newClubConfigurationDictionary = DataAccess.HelloCache.ClubConfiguration_Select_By_Application(connection, null, (int)Applications.Hello);

                lock (HelloSystem.lockObject)
                {
                    if (newConfigurations != null)
                    {
                        _configurations = newConfigurations;
                        _appConfig = GetAppConfig();
                        _configurationDictionary = GetConfigurationDictionary();
                    }

                    if (newConfigurationExts != null)
                    {
                        _configurationExts = newConfigurationExts;
                        _appConfig.ConfigurationExts = _configurationExts;
                    }

                    if (newClubConfigurationDictionary != null) _clubConfigurations = newClubConfigurationDictionary;

                    _lastRefreshed = DateTime.Now;
                }

                LogHelper.Log("Sample.Hello.Cache", "Refresh Hello Cache at " + _lastRefreshed.ToString("G"));
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod());
                throw ex;
            }
            finally
            {
                if (connection != null)
                    connection.Close();
            }
        }


        private static Dictionary<int, string> GetConfigurationDictionary()
        {
            Dictionary<int, string> configurationDictionary = new Dictionary<int, string>();
            foreach (Configuration c in _configurations)
                configurationDictionary.Add(c.TypeID, c.ConfigValue);
            return configurationDictionary;
        }

        /// <summary>
        /// Retrieves application configurations - return AppConfig object instead of a Collection
        /// </summary>
        private static AppConfig GetAppConfig()
        {
            var config = new AppConfig();

            try
            {
                foreach (Configuration c in _configurations)
                {
                    //TODO:   Add configuration here
                    if (c.TypeID == (int)ConfigurationTypes.Allow_Manual_Credit_Card_Input)
                    {
                        bool value = true;
                        if (bool.TryParse(c.ConfigValue, out value)) config.AllowManualCreditCardInput = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.Allow_Manual_Credit_Card_Input_From_Coporate)
                    {
                        bool value = true;
                        if (bool.TryParse(c.ConfigValue, out value)) config.AllowManualCreditCardInputFromCorporate = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.Optum_Medicare_Supplement_Program_Effective_Date)
                    {
                        DateTime value = DateTime.MaxValue;
                        if (DateTime.TryParse(c.ConfigValue, out value)) config.OptumMedicareSupplementProgramEffectiveDate = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.Optum_Medicare_Advantage_Program_Effective_Date)
                    {
                        DateTime value = DateTime.MaxValue;
                        if (DateTime.TryParse(c.ConfigValue, out value)) config.OptumMedicareAdvantageProgramEffectiveDate = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.MyZoneBand_Discount_Price)
                    {
                        Decimal value = 65;
                        if (Decimal.TryParse(c.ConfigValue, out value)) config.MyZoneBandDiscountPrice = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.Mobile_Barcode_Checkin_Timespan)
                    {
                        int value = 1; // Default value
                        if (int.TryParse(c.ConfigValue, out value)) config.MobileBarcodeCheckInTimeSpan = value;
                    }
                    else if (c.TypeID == (int)ConfigurationTypes.Command_Timeout)
                    {
                        int value = 0;
                        if (int.TryParse(c.ConfigValue, out value)) config.CommandTimeout = value;
                    }
                }
                config.ConfigurationExts = _configurationExts;

                config.ReloadTimeInterval = Properties.Settings.Default.ReloadConfigurationInterval;

            }
            catch (Exception ex)
            {
                LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod());
            }

            return config;
        }

        public AppConfig GetAppConfigByClub(int clubAphelionRowID)
        {
            var config = GetAppConfig();
            if (ClubConfigurations.ContainsKey(clubAphelionRowID))
            {
                //overwrite the configuration by club configuration
                config.AllowManualCreditCardInput = ClubConfigurations[clubAphelionRowID].AllowManualCreditCardInput;
            }
            return config;
        }
    }
}
