using System.ServiceModel;

namespace Sample.Hello.Common.Interface
{
    [ServiceContract]
    public partial interface IHelloSystemWeb
    {
        [OperationContract]
        string Echo(string str);

    }
}
