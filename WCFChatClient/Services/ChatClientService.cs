using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.Text;
using Catel.Data;
using WCFChatBase;
using WCFChatClient.Chat;
using WCFChatClient.Services.Interfaces;

namespace WCFChatClient.Services
{
    public class ChatClientService : ModelBase, IChatServiceCallback, IChatClientService
    {
        private ChatServiceClient chatClient;
        private Client receiver = null;
        private Client localClient;
        private readonly string receiverFilesPath = @"C:\Chat_Files";
        private delegate void FaultedInvoker();

        public ChatClientService()
        {}

        private void HandleProxy()
        {
            if (chatClient == null) 
                return;

            switch (chatClient.State)
            {
                case CommunicationState.Closed:
                    chatClient = null;
                    //chatListBoxMsgs.Items.Clear();
                    //chatListBoxNames.Items.Clear();
                    //loginLabelStatus.Content = "Disconnected";
                    //ShowChat(false);
                    //ShowLogin(true);
                    //loginButtonConnect.IsEnabled = true;
                    break;
                case CommunicationState.Closing:
                    break;
                case CommunicationState.Created:
                    break;
                case CommunicationState.Faulted:
                    chatClient.Abort();
                    chatClient = null;
                    //chatListBoxMsgs.Items.Clear();
                    //chatListBoxNames.Items.Clear();
                    //ShowChat(false);
                    //ShowLogin(true);
                    //loginLabelStatus.Content = "Disconnected";
                    //loginButtonConnect.IsEnabled = true;
                    break;
                case CommunicationState.Opened:
                    //ShowLogin(false);
                    //ShowChat(true);

                    //chatLabelCurrentStatus.Content = "online";
                    //chatLabelCurrentUName.Content = localClient.Name;

                    //Dictionary<int, Image> images = GetImages();
                    //Image img = images[loginComboBoxImgs.SelectedIndex];
                    //chatCurrentImage.Source = img.Source;
                    break;
                case CommunicationState.Opening:
                    break;
            }
        }

        public StringBuilder ServiceMessage { get; set; } = new StringBuilder();

        public List<Message> ChatMessage { get; set; } = new List<Message>();

        public bool Connected { get; private set; }

        public List<Client> UserList { get; set; } = new List<Client>();

        public string UserWriting { get; set; }


        public void Connect(string userName, string serverIp)
        {
            try
            {
                localClient = new Client();
                localClient.Name = userName;
                var context = new InstanceContext(this);
                chatClient = new ChatServiceClient(context);

                //As the address in the configuration file is set to localhost
                //we want to change it so we can call a service in internal 
                //network, or over internet
                var servicePath = chatClient.Endpoint.ListenUri.AbsolutePath;
                var serviceListenPort = chatClient.Endpoint.Address.Uri.Port.ToString();

                chatClient.Endpoint.Address = new EndpointAddress($"net.tcp://{serverIp}:{serviceListenPort}{servicePath}");


                chatClient.Open();

                chatClient.InnerDuplexChannel.Faulted += (sender, args) => HandleProxy();
                chatClient.InnerDuplexChannel.Opened += (sender, args) => HandleProxy();
                chatClient.InnerDuplexChannel.Closed += (sender, args) => HandleProxy();
                chatClient.ConnectAsync(localClient);
                Connected = true;
            }
            catch (Exception exception)
            {
                ServiceMessage.AppendLine(exception.Message);
                Connected = false;
            }
        }

        public void SendMessage(string textMessage)
        {
            if (chatClient == null || textMessage == "") 
                return;

            if (chatClient.State == CommunicationState.Faulted)
            {
                HandleProxy();
                return;
            }
            
            var message = new Message();
            message.Sender = localClient.Name;
            message.Content = textMessage;
            chatClient.SayAsync(message);
            chatClient.IsWritingAsync(null);
        }

        public void SendFile(string filePath)
        {
            if (receiver == null) 
                return;

            var fileInfo = new FileInfo(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);;
            try
            {
                var buffer = new byte[fileStream.Length];

                fileStream.Read(buffer, 0, buffer.Length);

                var fMsg = new FileMessage();
                fMsg.FileName = fileInfo.Name;
                fMsg.Sender = localClient.Name;
                fMsg.Data = buffer;
                chatClient.SendFileAsync(fMsg, receiver);
            }
            catch (Exception exception)
            {
                ServiceMessage.AppendLine(exception.Message);
            }
            finally
            {
                fileStream.Close();
            }
        }

        public void RefreshClients(Client[] clients)
        {
            UserList.Clear();
            UserList.AddRange(clients);
        }

        public void Receive(Message msg)
        {
            foreach (var client in UserList)
            {
                if (client.Name == msg.Sender)
                {
                    ChatMessage.Add(msg);
                }
            }
        }

        public void ReceiveWhisper(Message msg, Client receiver)
        {
            foreach (var client in UserList)
            {
                if (client.Name == msg.Sender)
                {
                    ChatMessage.Add(msg);
                }
            }
        }

        public void IsWritingCallback(Client client)
        {
            if (client == null)
            {
                UserWriting = "";
            }
            else
            {
                UserWriting += client.Name + " is writing a message.., ";
            }
        }

        public void ReceiverFile(FileMessage fileMsg, Client reciver)
        {
            try
            {
                var fileStream = new FileStream(receiverFilesPath + fileMsg.FileName, FileMode.Create, FileAccess.ReadWrite);
                fileStream.Write(fileMsg.Data, 0, fileMsg.Data.Length);
                ServiceMessage.AppendLine($"Received file, {fileMsg.FileName}");
            }
            catch (Exception exception)
            {
                ServiceMessage.AppendLine(exception.Message);
            }
        }

        public void UserJoin(Client client)
        {
            UserList.Add(client);
        }

        public void UserLeave(Client client)
        {
            UserList.Remove(client);
        }
    }
}
