using System.Collections.Generic;
using Catel.MVVM;
using System.Threading.Tasks;
using Catel.Data;

namespace WCFChatClient.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {}

        public override string Title { get { return "WCFChatClient"; } }

        // TODO: Register models with the vmpropmodel codesnippet
        // TODO: Register view model properties with the vmprop or vmpropviewmodeltomodel codesnippets
        // TODO: Register commands with the vmcommand or vmcommandwithcanexecute codesnippets

        #region ChatMessage property

        /// <summary>
        /// Gets or sets the ChatMessage value.
        /// </summary>
        public List<string> ChatMessage
        {
            get { return GetValue<List<string>>(ChatMessageProperty); }
            set { SetValue(ChatMessageProperty, value); }
        }

        /// <summary>
        /// ChatMessage property data.
        /// </summary>
        public static readonly PropertyData ChatMessageProperty = RegisterProperty("ChatMessage", typeof(List<string>));

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
