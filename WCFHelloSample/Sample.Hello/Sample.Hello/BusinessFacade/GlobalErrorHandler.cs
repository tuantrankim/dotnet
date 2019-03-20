using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Sample.Hello.BusinessFacade
{
    public class GlobalErrorHandler : IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            //You should implement your logging here
            if (error is FaultException) return false;
            else return true;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            if (error is FaultException) return;
            FaultException faultException = new FaultException(error.Message);
            MessageFault messageFault = faultException.CreateMessageFault();
            fault = System.ServiceModel.Channels.Message.CreateMessage(version, messageFault, faultException.Action);
        }
    }
}
