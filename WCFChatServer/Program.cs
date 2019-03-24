using System;

namespace WCFChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("====================================");
            Console.WriteLine("==============Start Server==========");
            var chatServer = new ChatServer(7878, 7879, 7880);
            chatServer.StartServer();
            Console.WriteLine(chatServer.ServiceMessage);
            Console.ReadLine();
            Console.WriteLine("==============Stop Server==========");
            Console.ReadLine();
        }
    }
}
