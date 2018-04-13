using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WCFService.EmployeeService)))
            {
                //Instead of specify behavior in app.config. You can do by coding
                ServiceMetadataBehavior serviceBehavior = new ServiceMetadataBehavior() { HttpGetEnabled = true };
                host.Description.Behaviors.Add(serviceBehavior);
                host.AddServiceEndpoint(typeof(WCFService.IEmployeeService),
                new BasicHttpBinding(),"EmployeeService");
                host.Open();
                Console.WriteLine("Host start @ "+ DateTime.Now.ToString());
                Console.ReadLine();
            }
        }
    }
}
