using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            using (ServiceHost host = new ServiceHost(typeof(WCFService.EmployeeService)))
            {
                host.Open();
                Console.WriteLine("Host start @ "+ DateTime.Now.ToString());
                Console.ReadLine();
            }
        }
    }
}
