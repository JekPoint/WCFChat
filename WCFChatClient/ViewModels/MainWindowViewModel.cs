using System.Collections.Generic;
using System.Threading.Tasks;
using Catel.Data;
using Catel.MVVM;
using WCFChatBase;
using WCFChatClient.Services.Interfaces;

namespace WCFChatClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IChatClientService chatClientService;

        public MainWindowViewModel(IChatClientService chatClientService)
        {
            this.chatClientService = chatClientService;
        }

        public override string Title { get { return "WCFChatClient"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        #region ChatMessages property

        /// <summary>
        /// Gets or sets the ChatMessages value.
        /// </summary>
        public List<Message> ChatMessages
        {
            get { return GetValue<List<Message>>(ChatMessagesProperty); }
            set { SetValue(ChatMessagesProperty, value); }
        }

        /// <summary>
        /// ChatMessages property data.
        /// </summary>
        public static readonly PropertyData ChatMessagesProperty = RegisterProperty("ChatMessages", typeof(List<Message>));

        #endregion

        #region UserMessage property

        /// <summary>
        /// Gets or sets the UserMessage value.
        /// </summary>
        public string UserMessage
        {
            get { return GetValue<string>(UserMessageProperty); }
            set { SetValue(UserMessageProperty, value); }
        }

        /// <summary>
        /// UserMessage property data.
        /// </summary>
        public static readonly PropertyData UserMessageProperty = RegisterProperty("UserMessage", typeof(string));

        #endregion

        #region UserLogin property

        /// <summary>
        /// Gets or sets the UserLogin value.
        /// </summary>
        public string UserLogin
        {
            get { return GetValue<string>(UserLoginProperty); }
            set { SetValue(UserLoginProperty, value); }
        }

        /// <summary>
        /// UserLogin property data.
        /// </summary>
        public static readonly PropertyData UserLoginProperty = RegisterProperty("UserLogin", typeof(string),"User");

        #endregion

        #region ServerIp property

        /// <summary>
        /// Gets or sets the ServerIp value.
        /// </summary>
        public string ServerIp
        {
            get { return GetValue<string>(ServerIpProperty); }
            set { SetValue(ServerIpProperty, value); }
        }

        /// <summary>
        /// ServerIp property data.
        /// </summary>
        public static readonly PropertyData ServerIpProperty = RegisterProperty("ServerIp", typeof(string), "localhost");

        #endregion

        #region ConnectionOpen command

        private Command _connectionOpenCommand;

        /// <summary>
        /// Gets the ConnectionOpen command
        /// </summary>
        public Command ConnectionOpenCommand
        {
            get { return _connectionOpenCommand ?? (_connectionOpenCommand = new Command(ConnectionOpen)); }
        }

        /// <summary>
        /// Method to invoke when the ConnectiionOpen command is executed.
        /// </summary>
        private void ConnectionOpen()
        {
            chatClientService.Connect(UserLogin, ServerIp);
        }

        #endregion

        #region SendMessage command

        private Command _sendMessageCommand;

        /// <summary>
        /// Gets the SendMessage command.
        /// </summary>
        public Command SendMessageCommand
        {
            get { return _sendMessageCommand ?? (_sendMessageCommand = new Command(SendMessage)); }
        }

        /// <summary>
        /// Method to invoke when the SendMessage command is executed.
        /// </summary>
        private void SendMessage()
        {
            chatClientService.SendMessage(UserMessage);
        }

        #endregion

        #region SendFile command

        private Command _sendFileCommand;

        /// <summary>
        /// Gets the SendFile command.
        /// </summary>
        public Command SendFileCommand
        {
            get { return _sendFileCommand ?? (_sendFileCommand = new Command(SendFile)); }
        }

        /// <summary>
        /// Method to invoke when the SendFile command is executed.
        /// </summary>
        private void SendFile()
        {
            // TODO: Handle command logic here
        }

        #endregion
       
        protected override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            // TODO: subscribe to events here
        }

        protected override async Task CloseAsync()
        {
            // TODO: unsubscribe from events here

            await base.CloseAsync();
        }
    }
}
