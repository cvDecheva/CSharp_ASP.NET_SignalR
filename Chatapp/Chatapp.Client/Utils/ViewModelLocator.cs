using Chatapp.Client.Services;
using Chatapp.Client.ViewModels;
using Unity;

namespace Chatapp.Client.Utils
{
    public class ViewModelLocator
    {
        private UnityContainer container;

        public ViewModelLocator()
        {
            container = new UnityContainer();
            container.RegisterType<IChatService, ChatService>();
            container.RegisterType<IDialogService, DialogService>();           
        }

        public MainWindowViewModel MainVM
        {
            get { return container.Resolve<MainWindowViewModel>(); }
        }

        public SearchWindowViewModel SearchVM
        {
            get { return container.Resolve<SearchWindowViewModel>(); }
        }
    }
}
