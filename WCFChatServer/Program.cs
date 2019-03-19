using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using WCFChatBase;

namespace WCFChatServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // Инициализируем службу, указываем адрес, по которому она будет доступна
            ServiceHost host = new ServiceHost(typeof(ChatService), new Uri("http://localhost:8000/MyService"));
            // Добавляем конечную точку службы с заданным интерфейсом, привязкой (создаём новую) и адресом конечной точки
            host.AddServiceEndpoint(typeof(IChatService), new BasicHttpBinding(), "");
            // Запускаем службу
            host.Open();
            Console.WriteLine("Сервер запущен");
            Console.ReadLine();
            // Закрываем службу
            host.Close();
        }
    }
}
