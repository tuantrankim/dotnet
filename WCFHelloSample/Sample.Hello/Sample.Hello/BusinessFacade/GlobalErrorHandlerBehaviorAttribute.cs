using System;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Sample.Hello.BusinessFacade
{
    public class GlobalErrorHandlerBehaviorAttribute : Attribute, IServiceBehavior
    {
        private readonly Type errorHandlerType;
        public GlobalErrorHandlerBehaviorAttribute(Type errorHandlerType)
        {
            this.errorHandlerType = errorHandlerType;
        }
        public void AddBindingParameters(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase, System.Collections.ObjectModel.Collection<ServiceEndpoint> endpoints, System.ServiceModel.Channels.BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        {
            IErrorHandler handler = (IErrorHandler)Activator.CreateInstance(this.errorHandlerType);
            foreach (ChannelDispatcherBase channelDispatcherBase in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher channelDispatcher = channelDispatcherBase as ChannelDispatcher;
                if (channelDispatcher != null)
                {
                    channelDispatcher.ErrorHandlers.Add(handler);
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, System.ServiceModel.ServiceHostBase serviceHostBase)
        { }
    }
}
