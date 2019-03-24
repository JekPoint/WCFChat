using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using WCFChatBase;

namespace WCFChatClient.Services.Interfaces
{
    public interface IChatClientService
    {
        StringBuilder ServiceMessage { get; set; }

        List<Message> ChatMessage { get; set; }

        bool Connected { get; }

        List<Client> UserList { get; set; }

        string UserWriting { get; set; }

        void Connect(string userName, string serverIp);

        void SendMessage(string textMessage);
        void SendFile(string filePath);
        void RefreshClients(Client[] clients);
        void Receive(Message msg);
        void ReceiveWhisper(Message msg, Client receiver);
        void IsWritingCallback(Client client);
        void ReceiverFile(FileMessage fileMsg, Client reciver);
        void UserJoin(Client client);
        void UserLeave(Client client);
        string ToString();
        IDisposable SuspendChangeCallbacks();
        IDisposable SuspendChangeNotifications(bool raiseOnResume);
        bool IsPropertyRegistered(string name);
        void GetObjectData(SerializationInfo info, StreamingContext context);
        event PropertyChangedEventHandler PropertyChanged;
    }
}