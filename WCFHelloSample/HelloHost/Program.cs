using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;

namespace HelloHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            
            //Force to use security protocol TSL 1.2
            //System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            
            //Or add the value to registry SchUseStrongCrypto 
            //[HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\.NETFramework\v4.0.30319]
            //"SchUseStrongCrypto"=dword:00000001  
            //[HKEY_LOCAL_MACHINE\SOFTWARE\Wow6432Node\Microsoft\.NETFramework\v4.0.30319]
            //"SchUseStrongCrypto"=dword:00000001

            // More than one user Service may run within the same process. To add
            // another service to this process, change the following line to
            // create a second service object. For example,
            //
            //   ServicesToRun = new ServiceBase[] {new Service1(), new MySecondUserService()};
            //
            ServicesToRun = new ServiceBase[] { new Service1() };
            
            

            ServiceBase.Run(ServicesToRun);
        }
    }
}