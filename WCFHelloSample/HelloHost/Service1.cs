using System;
using System.ServiceProcess;
using System.ServiceModel;


namespace HelloHost
{
    public partial class Service1 : ServiceBase
    {
        ServiceHost host;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Type serviceType = typeof(Sample.Hello.BusinessFacade.HelloSystem);
            //System.Threading.Thread.Sleep(10000);
            //Sample.Hello.BusinessFacade.HelloCache.Refresh();


            Sample.Hello.BusinessFacade.HelloSystem.lockObject = new object();
            //Sample.Hello.BusinessFacade.HelloSystem.callCounter = 0;

            host = new ServiceHost(serviceType);
            host.Open();

        }

        protected override void OnStop()
        {
            host.Close();
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }
    }
}
