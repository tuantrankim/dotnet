using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    public class ServiceWrapper<T> : IDisposable where T : class
    {
        ChannelFactory<T> factory;
        private T channel;

        private readonly NetTcpBinding binding;
        private readonly EndpointAddress endpoint;

        private readonly object lockObject = new object();
        private bool disposed;
        public static T internalService;
        
        public ServiceWrapper(String serviceEndpoint)
        {
            if (internalService == null)
            {
                binding = new NetTcpBinding();
                binding.TransactionFlow = true;

                endpoint = new EndpointAddress(serviceEndpoint);
                disposed = false;
            }
        }

        public T Channel
        {
            get
            {
                if (internalService != null)
                {
                    return internalService;
                }
                if (disposed)
                {
                    throw new ObjectDisposedException("Resource ServiceWrapper<" + typeof(T) + "> has been disposed");
                }

                lock (lockObject)
                {
                    if (factory == null)
                    {
                        factory = new ChannelFactory<T>(binding, endpoint);

                        //Add behavior to factory
                        factory.Endpoint.Behaviors.Add(new EndpointBehaviorAddHeader());
                        channel = factory.CreateChannel();
                    }
                }
                return channel;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    lock (lockObject)
                    {
                        if (channel != null)
                        {
                            ((IClientChannel)channel).Close();
                        }
                        if (factory != null)
                        {
                            factory.Close();
                        }
                    }

                    channel = null;
                    factory = null;
                    disposed = true;
                }
            }
        }



    }
    public class EndpointBehaviorAddHeader : IEndpointBehavior
    {
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        { }
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageInspectorAddHeader());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        { }

        public void Validate(ServiceEndpoint endpoint)
        { }
    }

    public class MessageInspectorAddHeader : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            try
            {

                string identity = Environment.UserDomainName + "\\" + Environment.UserName;
                string token = CreateToken(identity, "scretkey");
                MessageHeader hrd = MessageHeader.CreateHeader("String", "System", token);

                request.Headers.Add(hrd);
            }
            catch { }
            return null;
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
