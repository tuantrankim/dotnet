using System;
using System.ServiceModel;
using Sample.Hello.Common.Interface;


namespace Sample.Hello.BusinessFacade
{
    /*
     * This particular example tells WCF to manage a singleton instance of the service type and 
     * to allow multithreaded access to the instance. There is also an [OperationBehavior] attribute 
     * for controlling operation-level behavior. Behaviors influence processing within the host, but 
     * have no impact on the service contract whatsoever. Behaviors are one of the primary WCF 
     * extensibility points. Any class that implements IServiceBehavior can be applied to a service 
     * through the use of a custom attribute or configuration element.
    */
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
      ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    [GlobalErrorHandlerBehavior(typeof(GlobalErrorHandler))]
    public partial class HelloSystem : MarshalByRefObject, IHelloSystem, IHelloSystemWeb
    {
        static readonly HelloSystem instance = new HelloSystem();

        #region Global Cache
        public static object _lockObject;
        public static object lockObject
        {
            get
            {
                if (_lockObject == null) _lockObject = new object();
                return _lockObject;
            }
            set
            {
                _lockObject = value;
            }
        }
        #endregion Global Cache



        HelloSystem()
        {
        }

        static HelloSystem()
		{

		}

		public static HelloSystem Instance
		{
			get
			{
				return instance;
			}
        }

        public void Sleep(int miliSecond)
        {
            System.Threading.Thread.Sleep(miliSecond);
        }

        public string Echo(string str)
        {
            return str;
        }
         
    }
}
