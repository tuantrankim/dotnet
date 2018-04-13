using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFService
{
    public class ServiceWrapper<T> : IDisposable where T : class
    {
        ChannelFactory<T> factory;
        private T channel;

        private readonly BasicHttpBinding binding;
        private readonly EndpointAddress endpoint;

        private readonly object lockObject = new object();
        private bool disposed;

        
        public ServiceWrapper(String serviceEndpoint)
        {
            binding = new BasicHttpBinding();
            endpoint = new EndpointAddress(serviceEndpoint);
            disposed = false;
        }

        public T Channel
        {
            get
            {
                if (disposed)
                {
                    throw new ObjectDisposedException("Resource ServiceWrapper<" + typeof(T) + "> has been disposed");
                }

                lock (lockObject)
                {
                    if (factory == null)
                    {
                        factory = new ChannelFactory<T>(binding, endpoint);
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
}
