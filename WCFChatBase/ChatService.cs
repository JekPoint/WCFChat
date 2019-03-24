using System;
using System.Collections.Generic;
using System.ServiceModel;
using WCFChatBase.Interface;

namespace WCFChatBase
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single,
    ConcurrencyMode = ConcurrencyMode.Multiple, UseSynchronizationContext = false)]
    public class ChatService : IChatService
    {
        private readonly Dictionary<Client, IChatCallback> clients = new Dictionary<Client, IChatCallback>();

        private readonly List<Client> clientList = new List<Client>();

        private static IChatCallback CurrentCallback => OperationContext.Current.GetCallbackChannel<IChatCallback>();

        object syncObj = new object();

        public event ChatServerEventHandler ChatServerEvent = delegate{};

        private bool SearchClientsByName(string name)
        {
            foreach (var c in clients.Keys)
            {
                if (c.Name == name)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Connect(Client client)
        {
            if (clients.ContainsValue(CurrentCallback) || SearchClientsByName(client.Name))
                return false;

            lock (syncObj)
            {
                clients.Add(client, CurrentCallback);
                clientList.Add(client);

                foreach (var key in clients.Keys)
                {
                    var callback = clients[key];
                    try
                    {
                        callback.RefreshClients(clientList);
                        callback.UserJoin(client);
                    }
                    catch
                    {
                        clients.Remove(key);
                        return false;
                    }
                }
            }
            return true;
        }

        public void Say(Message msg)
        {
            lock (syncObj)
            {
                foreach (var callback in clients.Values)
                {
                    callback.Receive(msg);
                }
            }
        }

        public void Whisper(Message msg, Client receiver)
        {
            foreach (var rec in clients.Keys)
            {
                if (rec.Name != receiver.Name)
                    continue;

                var callback = clients[rec];
                callback.ReceiveWhisper(msg, rec);

                foreach (var sender in clients.Keys)
                {
                    if (sender.Name != msg.Sender)
                        continue;

                    var senderCallback = clients[sender];
                    senderCallback.ReceiveWhisper(msg, rec);
                    return;
                }
            }
        }

        public bool SendFile(FileMessage fileMsg, Client receiver)
        {
            foreach (var client in clients.Keys)
            {
                if (client.Name != receiver.Name)
                    continue;
                var msg = new Message
                    { Sender = fileMsg.Sender, Content = "I'M SENDING FILE.. " + fileMsg.FileName};

                var rcvrCallback = clients[client];

                rcvrCallback.ReceiveWhisper(msg, receiver);
                rcvrCallback.ReceiverFile(fileMsg, receiver);

                foreach (var sender in clients.Keys)
                {
                    if (sender.Name != fileMsg.Sender)
                        continue;

                    clients[sender].ReceiveWhisper(msg, receiver);
                    return true;
                }
            }
            return false;
        }

        public void IsWriting(Client client)
        {
            lock (syncObj)
            {
                foreach (var callback in clients.Values)
                {
                    callback.IsWritingCallback(client);
                }
            }
        }

        public void Disconnect(Client client)
        {
            foreach (var c in clients.Keys)
            {
                if (client.Name != c.Name)
                    continue;

                lock (syncObj)
                {
                    clients.Remove(c);
                    clientList.Remove(c);

                    foreach (var callback in clients.Values)
                    {
                        callback.RefreshClients(clientList);
                        callback.UserLeave(client);
                    }
                }
                return;
            }
        }

        public void SendStatusMessageEx(string msg, DateTime datetime)
        {
            ChatServerEvent(this, new ChatServiceEventArgs(msg, datetime));
        }
    }
}
