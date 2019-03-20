using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Data.SqlClient;
using Sample.Hello.Common.Interface;
using Sample.WCF;

namespace Sample.Hello.Common.Data
{
    [DataContract]
    public class AppConfig
    {
        #region Property
        public DateTime ReloadTime = DateTime.Now;
        
        [DataMember]
        public bool AllowManualCreditCardInput = true;
        [DataMember]
        public bool AllowManualCreditCardInputFromCorporate = true;
        [DataMember]
        public DateTime OptumMedicareSupplementProgramEffectiveDate = DateTime.MaxValue;
        [DataMember]
        public DateTime OptumMedicareAdvantageProgramEffectiveDate = DateTime.MaxValue;
        [DataMember]
        public Decimal MyZoneBandDiscountPrice = 65;
        [DataMember]
        public int MobileBarcodeCheckInTimeSpan = 1;
        [DataMember]
        public TimeSpan ReloadTimeInterval = new TimeSpan(1,0,0);
        
        public int CommandTimeout = 0;
        [DataMember]
        public List<ConfigurationExt> ConfigurationExts;
        #endregion

        #region Methods
        public ConfigurationExt GetConfigurationExt(int configurationTypeID, DateTime compareDate)
        {
            foreach (ConfigurationExt conf in ConfigurationExts)
            {
                if (conf.TypeID == configurationTypeID 
                    && conf.StartDate <= compareDate 
                    && conf.EndDate >= compareDate) return conf;
            }

            return null;
        }

        public decimal GetTermFitnessMaxAmount(Customer cust)
        {
            decimal amount = 0;
            if (cust == null || cust.MembershipInfo == null) return 0;

            DateTime joinDate = cust.MembershipInfo.JoinedDate.Date;
            ConfigurationExt conf = GetConfigurationExt((int)cust.TermedFitnessMaxAmountType, joinDate);

            if (conf != null) decimal.TryParse(conf.ConfigValue, out amount);
            return amount;
        }

         public static void ClusterCacheRefresh()
        {
             String systemHost = Sample.WCF.Factory.Properties.Settings.Default.SystemHost;
             try
             {
                 String systemHost1 = Sample.WCF.Factory.Properties.Settings.Default.SystemHost1;
                 String systemHost2 = Sample.WCF.Factory.Properties.Settings.Default.SystemHost2;
                 String systemHost3 = Sample.WCF.Factory.Properties.Settings.Default.SystemHost3;

                 if (!string.IsNullOrEmpty(systemHost1))
                 {
                     Factory<IHelloSystem>.SetRemoteHost(systemHost1);
                     Factory<IHelloSystem>.Proxy.CacheRefresh();
                 }

                 if (!string.IsNullOrEmpty(systemHost2))
                 {
                     Factory<IHelloSystem>.SetRemoteHost(systemHost2);
                     Factory<IHelloSystem>.Proxy.CacheRefresh();
                 }

                 if (!string.IsNullOrEmpty(systemHost3))
                 {
                     Factory<IHelloSystem>.SetRemoteHost(systemHost3);
                     Factory<IHelloSystem>.Proxy.CacheRefresh();
                 }
             }
             catch (Exception ex)
             {
                 Factory<IHelloSystem>.DisposeProxy();
                 throw;
             }
             finally
             {
                 Factory<IHelloSystem>.SetRemoteHost(systemHost);
             }
         }
        #endregion

    }
}
