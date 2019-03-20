using Sample.Hello.Common.Data;
using Sample.Hello.Common.Interface;
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
    public partial class HelloSystem : MarshalByRefObject, IHelloSystem
    {

        #region Global cache
        public static ConfigurationCollection _configurations;
        private static Dictionary<int, ClubConfiguration> _clubConfigurations;
        public static ConfigurationCollection configurations
        {
            get
            {
                if (_configurations == null) _configurations = new ConfigurationCollection();
                return _configurations;
            }
            set
            {
                _configurations = value;
            }
        }

        
        #endregion Global cache


        /// <summary>
        /// Retrieves application configurations
        /// </summary>
        public ConfigurationCollection LoadApplicationConfigurations()
        {
           try{
               ConfigurationCollection collection = new ConfigurationCollection();
               collection.AddRange(HelloCache.Instance.Configurations);
               return collection;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod());
                throw ex;
            }
        }

        public void ClusterCacheRefresh()
        {
            try
            {
                Sample.Hello.Common.Data.AppConfig.ClusterCacheRefresh();
                LogHelper.Log("Sample.Hello.Cache", "Cluster Hello Cache refreshing at " + DateTime.Now.ToString("G"));
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Sample.Hello.Cache", ex, false, MethodInfo.GetCurrentMethod());
            }
        }
        public void CacheRefresh()
        {
            HelloCache.Refresh();
        }
        /// <summary>
        /// Retrieves the datetime of when the configurations were last refreshed.
        /// </summary>
        /// <returns></returns>
        public DateTime ConfigurationLastRefresh()
        {
            return HelloCache.Instance.LastRefreshed;
        }

        /// <summary>
        /// Operation Contract
        /// Retrieves application configurations - return AppConfig object instead of a Collection
        /// </summary>
        public AppConfig GetAppConfig()
        {
            return HelloCache.Instance.AppConfig;
        }

        public AppConfig GetAppConfigByClub(int clubAphelionRowID)
        {
            return HelloCache.Instance.GetAppConfigByClub(clubAphelionRowID);
        }
        //public ConfigurationCollection Configurations
        //{
        //    get
        //    {
        //        if (
        //            configurations == null
        //            || configurations.Count == 0
        //            || DateTime.Now - configurations.LastRefreshed > Properties.Settings.Default.ReloadConfigurationInterval)
        //        {
        //            LoadApplicationConfigurations();
        //        }
        //        return configurations;
        //    }
        //}

        /// <summary>
        /// Retrieves application configurations
        /// </summary>
        public Dictionary<int, string> GetApplicationConfigurations()
        {
            try
            {
                return HelloCache.Instance.ConfigurationDictionary;
            }
            catch (Exception ex)
            {
                LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod());
                throw ex;
            }
        }
    }
}
