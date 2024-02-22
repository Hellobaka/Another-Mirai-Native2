using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Another_Mirai_Native.UI.Pages
{
    /// <summary>
    /// ChatPage.xaml 的交互逻辑
    /// </summary>
    public partial class ChatPage : Page, INotifyPropertyChanged
    {
        public ChatPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<ChatListItemViewModel> ChatList { get; set; } = new();

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            ChatList.Add(new ChatListItemViewModel
            {
                Time = DateTime.Now.AddDays(new Random().Next(30) * -1),
                Detail = "Stretching the items in a WPF ListView within a ViewBox",
                GroupName = "Error: BindingExpression path error: 'Width' property not found on Project.CustomElement, Project.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'. BindingExpression: Path='Width' DataItem=Project.CustomElement, Project.WindowsPhone, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null'; target element is 'Windows.UI.Xaml.Controls.Grid' (Name='null'); target property is 'Width' (type 'Double",
                Id = 0
            });
        }
    }
}
