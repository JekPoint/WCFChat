using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using WCFChatBase;
using WCFChatBase.Interface;

namespace WCFChatServer
{
    /// <summary>
    /// Сервер сообщений
    /// </summary>
    internal class ChatServer
    {
        private readonly string serverIp;
        private readonly int serverExchangeTcpPort;
        private readonly ServiceHost host;

        /// <summary>
        /// Конструктор чат сервера сообщений
        /// </summary>
        /// <param name="serverHttpPort">Http порт для получения сообщений</param>
        /// <param name="serverTcpPort">TCP порт для работы с файлами</param>
        /// <param name="serverExchangeTcpPort">TCP порт для работы с метаданными</param>
        /// <param name="serverIp">IP адрес сервера</param>
        public ChatServer(int serverHttpPort, int serverTcpPort, int serverExchangeTcpPort, string serverIp = "localhost")
        {
            this.serverIp = serverIp;
            ServiceMessage.AppendLine($"Server Ip {serverIp}");

            this.serverExchangeTcpPort = serverExchangeTcpPort;
            var tcpAddress = new Uri($"net.tcp://{serverIp}:{serverTcpPort}/WPFHost");
            ServiceMessage.AppendLine($"net.tcp        - net.tcp://{serverIp}:{serverTcpPort}/WPFHost");

            var httpAddress = new Uri($"http://{serverIp}:{serverHttpPort}/WPFHost");
            ServiceMessage.AppendLine($"http           - http://{serverIp}:{serverHttpPort}/WPFHost");

            var serverListener = new Uri($"net.pipe://{serverIp}/WPFHost");
            ServiceMessage.AppendLine($"serverListener - net.pipe://{serverIp}/WPFHost");

            Uri[] baseAddresses = { tcpAddress, httpAddress, serverListener };
            host = new ServiceHost(typeof(ChatService), baseAddresses);

            var namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            host.AddServiceEndpoint(typeof(IChatService), namedPipeBinding, "");
        }

        private void OnChatServerEvent(object sender, ChatServiceEventArgs e)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Статус работы сервера
        /// </summary>
        public bool Open { get; private set; }

        /// <summary>
        /// Сервисные сообщения, в случае вне штатных ситуаций 
        /// </summary>
        public StringBuilder ServiceMessage { get; private set; } = new StringBuilder();

        /// <summary>
        /// Максимальное количество
        /// - подключенных клиентов
        /// - сообщений одновременно обрабатываемых сервером
        /// </summary>
        public int MaxConcurrentCalls { get; set; } = 100;

        /// <summary>
        /// Максимальное количество одновременно открытых сессий
        /// </summary>
        public int MaxConcurrentSessions { get; set; } = 100;

        /// <summary>
        /// Максимальный объем буфера для работы с файлами
        /// </summary>
        public int MaxSizeBuffer { get; set; } = 67108864;

        /// <summary>
        /// Инициализация и запуск сервера сообщений
        /// </summary>
        public void StartServer()
        {
            ThrottlingBehaviorInit();
            TcpBindingInit();
            MetadataBehaviorInit();

            ServiceMessage.AppendLine($"MaxConcurrentCalls {MaxConcurrentCalls}");
            ServiceMessage.AppendLine($"MaxConcurrentSessions {MaxConcurrentSessions}");
            ServiceMessage.AppendLine($"MaxConcurrentSessions {MaxConcurrentSessions}");
            ServiceMessage.AppendLine($"MaxSizeBuffer {MaxSizeBuffer}");
            try
            {
                host.Open();
                Open = true;   
                ServiceMessage.AppendLine("Start Ok");
            }
            catch (Exception ex)
            {
                Open = false;
                ServiceMessage.AppendLine(ex.Message);
            }
        }

        private void MetadataBehaviorInit()
        {
            var mBehave = new ServiceMetadataBehavior();
            host.Description.Behaviors.Add(mBehave);
            host.AddServiceEndpoint(typeof(IMetadataExchange),
                MetadataExchangeBindings.CreateMexTcpBinding(),
                $"net.tcp://{serverIp}:{serverExchangeTcpPort}/WPFHost/mex");
            ServiceMessage.AppendLine($"Behavior       - net.tcp://{serverIp}:{serverExchangeTcpPort}/WPFHost/mex");
        }

        private void ThrottlingBehaviorInit()
        {
            var throttle = host.Description.Behaviors.Find<ServiceThrottlingBehavior>();
            if (throttle != null)
                return;

            throttle = new ServiceThrottlingBehavior
            {
                MaxConcurrentCalls = MaxConcurrentCalls,
                MaxConcurrentSessions = MaxConcurrentSessions
            };
            host.Description.Behaviors.Add(throttle);
        }

        private void TcpBindingInit()
        {
            var tcpBinding = new NetTcpBinding(SecurityMode.None, true)
            {
                MaxBufferPoolSize = MaxSizeBuffer,
                MaxBufferSize = MaxSizeBuffer,
                MaxReceivedMessageSize = MaxSizeBuffer,
                TransferMode = TransferMode.Buffered,
                ReaderQuotas =
                {
                    MaxArrayLength = MaxSizeBuffer,
                    MaxBytesPerRead = MaxSizeBuffer,
                    MaxStringContentLength = MaxSizeBuffer
                },
                MaxConnections = MaxConcurrentCalls,
                ReceiveTimeout = new TimeSpan(20, 0, 0),
                ReliableSession = {Enabled = true, InactivityTimeout = new TimeSpan(20, 0, 10)}
            };
            host.AddServiceEndpoint(typeof(IChatService), tcpBinding, "tcp");
        }
    }
}
