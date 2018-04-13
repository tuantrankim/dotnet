using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Security.Cryptography;

namespace WcfServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            WCFService.ServiceWrapper<WCFService.IEmployeeService>.internalService = WCFService.EmployeeService.Instance;
            using (ServiceHost host = new ServiceHost(typeof(WCFService.EmployeeService)))
            {
                //Add endpoint behavior
                ServiceEndpoint endpoint = host.Description.Endpoints[0];
                endpoint.Behaviors.Add(new EndpointBehaviorSecurity());

                host.Open();
                Console.WriteLine("Host start @ "+ DateTime.Now.ToString());
                Console.ReadLine();
            }
        }
    }

    public class EndpointBehaviorSecurity : IDispatchMessageInspector, IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            //clientRuntime.MessageInspectors.Add(new MessageInspectorAddHeader());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void Validate(ServiceEndpoint endpoint)
        { }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            try
            {
                if (!IsSecured()) throw new FaultException("Security check failure");
            }
            catch
            {
                throw new FaultException("Security check failure");
            }

            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }

        private bool IsSecured()
        {
            string IP = GetClientIP();
            string versionNumber = "";
            string processID = "";
            string clientToken = GetClientToken();
            
            string identity = GetClientIdentity();
            string token = CreateToken(identity, "scretkey");
            
            bool isSecured = token.Equals(clientToken);
            if (!isSecured)
            {
                string logMessage = String.Format("Security check fail [{0}]", IP + "," + identity);
            }
            return isSecured;
        }

        public string GetClientIdentity()
        {
            try
            {
                if (ServiceSecurityContext.Current == null)
                    return "No Security Context";
                return ServiceSecurityContext.Current.WindowsIdentity.Name;

            }
            catch (Exception ex)
            {
                return "N/A";
            }
        }
        private string GetClientIP()
        {
            try
            {
                MessageProperties messageProperties = OperationContext.Current.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpointProperty = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                return endpointProperty.Address; // +":" + endpointProperty.Port;
            }
            catch (Exception)
            {
                return "";
            }
        }
        private string GetClientToken()
        {
            string token = "";

            try
            {
                if (OperationContext.Current != null)
                {
                    MessageHeaders messageHeaders = OperationContext.Current.IncomingMessageHeaders;
                    if (messageHeaders != null)
                    {
                        token = messageHeaders.GetHeader<string>("String", "System");
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return token;
        }

        private string CreateToken(string message, string secret)
        {
            secret = secret ?? "";
            var encoding = new System.Text.ASCIIEncoding();
            byte[] keyByte = encoding.GetBytes(secret);
            byte[] messageBytes = encoding.GetBytes(message);
            using (var hmacsha256 = new HMACSHA256(keyByte))
            {
                byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
                return Convert.ToBase64String(hashmessage);
            }
        }
    }
}
