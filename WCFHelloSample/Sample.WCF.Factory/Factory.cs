using System;
using System.Net.Security;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Serialization;

using Sample.WCF.Factory.Properties;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Reflection;
using System.Diagnostics;

namespace Sample.WCF
{
    public class Factory<T>
    {
        private static readonly Factory<T> instance = new Factory<T>();
        private T system = default(T);
        private object lockObject = new object();
        ChannelFactory<T> channelFactory = null;
        private DateTime lastCall;
        private readonly TimeSpan closeTimeout = TimeSpan.FromMinutes(10);
        private static EndpointAddress ep;
        private NetTcpBinding binding;

        static Factory()
        {
        }

        private Factory()
        {
            CreateBinding();
        }

        private void CreateBinding()
        {
            ep = GetEndPointAddress();
            //ep = new EndpointAddress(new Uri(Settings.Default.SystemHost + "/" + GetTypeName()),
            //                            EndpointIdentity.CreateUpnIdentity(Settings.Default.UserPrincipalName));
            //ep = new EndpointAddress(new Uri(Settings.Default.SystemHost + "/" + GetTypeName()),
            //                            EndpointIdentity.CreateSpnIdentity("Sample.Hello.BusinessFacade.HelloSystem"));

            //ep = new EndpointAddress(new Uri(Settings.Default.SystemHost + "/" + GetTypeName()),
            //                            EndpointIdentity.CreateDnsIdentity(Settings.Default.DNSName));

            binding = new NetTcpBinding(SecurityMode.Transport, false);
            binding.OpenTimeout = TimeSpan.FromSeconds(100);
            binding.SendTimeout = TimeSpan.FromSeconds(100);
            binding.MaxReceivedMessageSize = 20000000; //20 MB
            binding.MaxBufferSize = 20000000;
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Windows;
            binding.Security.Transport.ProtectionLevel = ProtectionLevel.EncryptAndSign;

            //Increase configuration of ReaderQuotas when deserializing body of reply message
            binding.ReaderQuotas.MaxArrayLength = 67108864;
            binding.ReaderQuotas.MaxStringContentLength = 81920;
            /*binding.ReaderQuotas.MaxBytesPerRead = 40960;
            binding.ReaderQuotas.MaxDepth = 320;
            binding.ReaderQuotas.MaxNameTableCharCount = 163840;
            binding.ReaderQuotas.MaxStringContentLength = 81920;*/
        }

        private static EndpointAddress GetEndPointAddress()
        {
            string serviceHost = string.Empty;
            if (GetTypeName() == Factory.ServiceSystem.IHQLSystem.ToString())
            {
                serviceHost = Settings.Default.HQLHost;
            }
            else
            {
                //  assume HelloSystem
                serviceHost = Settings.Default.SystemHost;
            }

            return new EndpointAddress(serviceHost + "/" + GetTypeName());
        }

        public static void SetRemoteHost(string host)
        {
            Settings.Default["SystemHost"] = host;
            DisposeProxy();
            ep = GetEndPointAddress();
        }

        public static T Proxy
        {
            get
            {
                return instance.GetSystem();
            }
        }

        public static void DisposeProxy()
        {
            try
            {
                instance.DisposeChannel();
            }
            catch {
                //suppress exception
            }
        }

        private T GetSystem()
        {
            if (system != null)
            {
                CommunicationState coummunicationState = ((ICommunicationObject)system).State;
                if (
                    channelFactory.State == CommunicationState.Opened
                    &&
                    //Allowing Created and Opening state because
                    //in case of nest proxy call, inner proxy doesn't dispose outer the proxy
                    (coummunicationState == CommunicationState.Opened
                    || coummunicationState == CommunicationState.Created
                    || coummunicationState == CommunicationState.Opening)
                    )
                {
                    // compare the last time the service was called and check against time out period
                    // to make the check fast, I replaced channelFactory.Endpoint.Binding.CloseTimeout
                    if (DateTime.Now.Subtract(lastCall) < closeTimeout)
                        return system;
                    else
                    {
                        // when it times out, dispose the channel so it can recreated
                        DisposeChannel();
                    }

                }
                else
                {
                    // if there is something wrong with the current channel factory then dispose it so it can recreated
                    DisposeChannel();
                }
            }

            if (system == null)
            {
                if (binding == null)
                    CreateBinding();

                lock (lockObject)
                {
                    channelFactory = new ChannelFactory<T>(binding, ep);
                    channelFactory.Endpoint.Behaviors.Add(new EndpointBehaviorAddHeader());

                    SetDataContractSerializerBehavior(channelFactory.Endpoint.Contract);

                    // Create an instance of the remote object
                    // If publishing only the interface, must use Activator.GetObject.
                    system = channelFactory.CreateChannel();
                }
            }
            lastCall = DateTime.Now;
            return system;
        }

        /// <summary>
        /// This is goiing to reset the attribute MaxItemsInObjectGraph in DataContractSerializerOperationBehavior
        /// We hit the default threshold of 65536, and hence needs to increase
        /// </summary>
        /// <param name="contractDescription"></param>
        private void SetDataContractSerializerBehavior(ContractDescription contractDescription)
        {
            foreach (OperationDescription operation in contractDescription.Operations)
            {
                DataContractSerializerOperationBehavior dcsob = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (dcsob != null)
                {
                    dcsob.MaxItemsInObjectGraph = 1000000;
                    //operation.Behaviors.Remove(dcsob);
                }
                else
                    operation.Behaviors.Add(new CustomDataContractSerializerOperationBehavior(operation));
            }
        }


        private void DisposeChannel()
        {
            lock (lockObject)
            {
                channelFactory.Abort();
                channelFactory.Close();
                system = default(T);
            }
        }

        private static string GetTypeName()
        {
            string s = typeof(T).ToString();
            return s.Substring(s.LastIndexOf(".") + 1);
        }
    }

    class CustomDataContractSerializerOperationBehavior : DataContractSerializerOperationBehavior
    {
        public CustomDataContractSerializerOperationBehavior(OperationDescription operationDescription) : base(operationDescription) { }

        public override XmlObjectSerializer CreateSerializer(Type type,
                                                                XmlDictionaryString name,
                                                                XmlDictionaryString ns,
                                                                IList<Type> knownTypes)
        {
            return new DataContractSerializer(type, name, ns, knownTypes,
                1000000 /*maxItemsInObjectGraph*/,
                false/*ignoreExtensionDataObject*/,
                true/*preserveObjectReferences*/,
                null/*dataContractSurrogate*/);
        }
    }

    public class EndpointBehaviorAddHeader : IEndpointBehavior
    {            
        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {}
        public void ApplyClientBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(new MessageInspectorAddHeader());
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, System.ServiceModel.Dispatcher.EndpointDispatcher endpointDispatcher)
        {}

        public void Validate(ServiceEndpoint endpoint)
        {}
    }

    public class MessageInspectorAddHeader : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        { }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string systemInfo = string.Empty;
            //systemInfo = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            systemInfo = Settings.Default.Version;
            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess != null)
                systemInfo += "," + currentProcess.Id.ToString();
            MessageHeader hrd = MessageHeader.CreateHeader("String", "System", systemInfo);

            request.Headers.Add(hrd);

            return null;
        }
    }
}
