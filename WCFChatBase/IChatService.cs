using System.ServiceModel;

namespace WCFChatBase
{
    [ServiceContract]
    public interface IChatService
    {

        [OperationContract]
        double GetSum(double i, double j);


        [OperationContract]
        double GetMult(double i, double j);
    }
}
