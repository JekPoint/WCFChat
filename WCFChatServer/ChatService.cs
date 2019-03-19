using WCFChatBase;

namespace WCFChatServer
{
    public class ChatService : IChatService
    {
        public double GetSum(double i, double j)
        {
            return i + j;
        }

        public double GetMult(double i, double j)
        {
            return i * j;
        }
    }
}
