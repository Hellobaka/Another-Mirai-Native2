using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Another_Mirai_Native.UI.Pages
{
    public class InvokeArugment
    {
        public string Name { get; set; }

        public Type TargetType { get; set; }

        public string Value { get; set; }
    }

    public class ProtocolMethod
    {
        public MethodInfo Method { get; set; }

        public string MethodDescription { get; set; }

        public string MethodName { get; set; }

        public string MethodNameDisplay => MethodName.Substring(0, 1);
    }

    /// <summary>
    /// Test_APIPage.xaml 的交互逻辑
    /// </summary>
    public partial class Test_APIPage : Page, INotifyPropertyChanged
    {
        public Test_APIPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<ProtocolMethod> DisplayProtocolMethods { get; set; } = [];

        public bool FormLoaded { get; set; }

        public List<InvokeArugment> InvokeArugments { get; set; } = [];

        public List<ProtocolMethod> ProtocolMethods { get; set; } = [];

        private ProtocolMethod CurrentMethod { get; set; }

        private Debouncer SearchDebounder { get; set; } = new();

        /// <summary>
        /// MVVM
        /// </summary>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void BuildArgumentList()
        {
            ArgumentList.Children.Clear();
            foreach (var item in InvokeArugments)
            {
                ArgumentList.Children.Add(new TextBlock() { Text = item.Name, Margin = new Thickness(0, 5, 0, 0) });
                TextBox inputTextbox = new TextBox() { Tag = item, Text = item.Value, Margin = new Thickness(0, 3, 0, 0) };
                inputTextbox.TextChanged += (sender, _) =>
                {
                    InvokeArugment arugment = (sender as TextBox).Tag as InvokeArugment;
                    arugment.Value = (sender as TextBox).Text;
                };
                ArgumentList.Children.Add(inputTextbox);
            }
        }

        private void FilterMethods()
        {
            Dispatcher.BeginInvoke(() =>
            {
                DisplayProtocolMethods = ProtocolMethods.Where(x => x.MethodName.ToLower().Contains(SearchTextValue.Text.ToLower())).ToList();
                OnPropertyChanged(nameof(DisplayProtocolMethods));
            });
        }

        private async void InvokeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentMethod == null || CurrentMethod.Method == null)
            {
                DialogHelper.ShowSimpleDialog("调用测试方法", $"未选择测试方法");
                return;
            }
            List<object> arugments = [];
            foreach (var item in InvokeArugments)
            {
                item.Value = item.Value.Trim();
                try
                {
                    switch (item.TargetType.Name)
                    {
                        case "Int64":
                            arugments.Add(long.Parse(item.Value));
                            break;

                        case "Int32":
                            arugments.Add(int.Parse(item.Value));
                            break;

                        case "String":
                            arugments.Add(item.Value);
                            break;

                        case "Boolean":
                            arugments.Add(bool.Parse(item.Value));
                            break;

                        default:
                            break;
                    }
                }
                catch
                {
                    DialogHelper.ShowSimpleDialog("测试参数转换", $"参数转换失败:\n\n{item.Name} 的值 {item.Value} 无法转换为 {item.TargetType}");
                    return;
                }
            }

            if (!await DialogHelper.ShowConfirmDialog("调用测试方法", $"确定要调用测试方法 {CurrentMethod.MethodName} 吗？请二次确认输入的参数。"))
            {
                return;
            }
            CallStatus.Visibility = Visibility.Visible;
            FunctionInvokeResult.Text = "调用中...";
            try
            {
                object? ret = null;
                await Task.Run(() => ret = CurrentMethod.Method.Invoke(ProtocolManager.Instance.CurrentProtocol, arugments.ToArray()));

                if (ret is IDictionary dict)
                {
                    StringBuilder sb = new();
                    foreach (DictionaryEntry item in dict)
                    {
                        sb.AppendLine($"{item.Key} = {item.Value}");
                    }
                    FunctionInvokeResult.Text = sb.ToString();
                    return;
                }
                if (ret is IList list)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var item in list)
                    {
                        sb.AppendLine(item.ToString());
                    }
                    FunctionInvokeResult.Text = sb.ToString();
                    return;
                }
                FunctionInvokeResult.Text = ret.ToString();
            }
            catch (Exception ex)
            {
                DialogHelper.ShowSimpleDialog("调用测试方法", $"调用目标发生了异常: {ex.Message}\n{ex.StackTrace}");
            }
            finally
            {
                CallStatus.Visibility = Visibility.Collapsed;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (FormLoaded)
            {
                return;
            }
            FormLoaded = true;
            ProtocolMethods.Clear();
            var interfaceList = typeof(IProtocol).GetMethods();
            foreach (var item in ProtocolManager.Instance.CurrentProtocol.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance).OrderBy(x => x.Name))
            {
                if (!interfaceList.Any(x => x.Name == item.Name) || item.Name.StartsWith("get_") || item.Name.StartsWith("set_"))
                {
                    continue;
                }
                ProtocolMethods.Add(new ProtocolMethod
                {
                    MethodName = item.Name,
                    MethodDescription = $"{item.GetParameters().Length}个参数 返回值 {item.ReturnType.Name}",
                    Method = item
                });
            }
            DisplayProtocolMethods = [.. ProtocolMethods];
            OnPropertyChanged(nameof(DisplayProtocolMethods));
        }

        private void ProtocolMethodList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count <= 0 || e.AddedItems[0] is not ProtocolMethod method)
            {
                return;
            }
            CurrentMethod = method;
            InvokeArugments.Clear();
            foreach (var item in method.Method.GetParameters())
            {
                InvokeArugments.Add(new InvokeArugment
                {
                    Name = item.Name + $" ({item.ParameterType.Name})",
                    TargetType = item.ParameterType
                });
            }
            BuildArgumentList();
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            BuildArgumentList();
        }

        private void SearchTextValue_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!FormLoaded)
            {
                return;
            }
            SearchDebounder.Debounce(FilterMethods, 500);
        }
    }
}