using System;

namespace WCFChatBase
{
    /// <summary>
    /// Событие, получения сообщения сервером
    /// </summary>
    public class ChatServiceEventArgs
    {
        public ChatServiceEventArgs(string msg, DateTime datetime)
        {
            Message = msg;
            Date    = datetime;
        }

        /// <summary>
        /// Сообщение переданое на сервер
        /// </summary>
        public string   Message { get; set; }

        /// <summary>
        /// Время передачи сообщения
        /// </summary>
        public DateTime Date    { get; set; }
    }

    public delegate void ChatServerEventHandler(object sender, ChatServiceEventArgs e);
}
