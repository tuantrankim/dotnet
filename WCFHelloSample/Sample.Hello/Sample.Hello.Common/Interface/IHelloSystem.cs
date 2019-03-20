using System.ServiceModel;

namespace Sample.Hello.Common.Interface
{
    [ServiceContract]
    public partial interface IHelloSystem
    {

        [OperationContract]
        void Sleep(int miliSecond);
    
    }
}
