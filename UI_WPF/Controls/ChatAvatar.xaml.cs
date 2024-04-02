using Another_Mirai_Native.UI.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// ChatAvatar.xaml 的交互逻辑
    /// </summary>
    public partial class ChatAvatar : UserControl
    {
        public static readonly DependencyProperty ItemProperty =
            DependencyProperty.Register(
                "Item",
                typeof(ChatListItemViewModel),
                typeof(ChatAvatar),
                new PropertyMetadata(new ChatListItemViewModel(), OnItemChanged));

        public ChatAvatar()
        {
            //DataContext = this;
            InitializeComponent();
        }

        public enum AvatarTypes
        {
            QQGroup,

            QQPrivate,

            Fallback
        }

        public AvatarTypes AvatarType { get; set; } = AvatarTypes.Fallback;

        public Brush FallbackBrush { get; set; }

        public string FallbackName { get; set; } = "";

        public long Id { get; set; }

        public bool IsRound { get; set; }

        public ChatListItemViewModel Item
        {
            get { return (ChatListItemViewModel)GetValue(ItemProperty); }
            set { SetValue(ItemProperty, value); }
        }

        private static Color[] Colors { get; set; } = new Color[]
        {
            Color.FromRgb(255, 87, 51), // Crimson
            Color.FromRgb(52, 152, 219), // SkyBlue
            Color.FromRgb(46, 204, 113), // Emerald
            Color.FromRgb(155, 89, 182), // Amethyst
            Color.FromRgb(52, 73, 94), // Asphalt
            Color.FromRgb(22, 160, 133), // Turquoise
            Color.FromRgb(39, 174, 96), // Nephritis
            Color.FromRgb(41, 128, 185), // BelizeHole
            Color.FromRgb(142, 68, 173), // Wisteria
            Color.FromRgb(241, 196, 15), // Sunflower
            Color.FromRgb(230, 126, 34), // Carrot
            Color.FromRgb(231, 76, 60), // Alizarin
            Color.FromRgb(236, 240, 241), // Clouds
            Color.FromRgb(149, 165, 166), // Concrete
            Color.FromRgb(243, 156, 18), // Orange
            Color.FromRgb(211, 84, 0), // Pumpkin
            Color.FromRgb(192, 57, 43), // Pomegranate
            Color.FromRgb(189, 195, 199), // Silver
            Color.FromRgb(127, 140, 141), // Asbestos
            Color.FromRgb(44, 62, 80) // MidnightBlue
        };

        private static List<long> LoadedItems { get; set; } = new();

        public void Init()
        {
            Container.CornerRadius = IsRound ? new CornerRadius(double.MaxValue) : new CornerRadius(Width * 0.1);
            FallbackBrush = new SolidColorBrush(Colors[new Random(Id.GetHashCode()).Next(Colors.Length)]);
            Container.Background = FallbackBrush;
            FallbackDisplay.FontSize = Width * 0.35;
            FallbackDisplay.Text = FallbackName?.Length > 2 ? FallbackName.Substring(0, 2) : FallbackName ?? Id.ToString().Substring(0, 2);
        }

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ChatAvatar control = (ChatAvatar)d;
            ChatListItemViewModel newValue = (ChatListItemViewModel)e.NewValue;

            control.Id = newValue.Id;
            control.FallbackName = newValue.GroupName;
            control.AvatarType = newValue.AvatarType;
            if (LoadedItems.Any(x => x == newValue.Id) is false)
            {
                LoadedItems.Add(newValue.Id);
            }
            control.GetDisplayImage(newValue.Id);
            control.Init();
        }

        private void GetDisplayImage(long id)
        {
            string url = "";
            switch (AvatarType)
            {
                case AvatarTypes.QQPrivate:
                    url = $"https://q.qlogo.cn/g?b=qq&nk={id}&s=160";
                    break;

                case AvatarTypes.QQGroup:
                    url = $"http://p.qlogo.cn/gh/{id}/{id}/0";
                    break;

                case AvatarTypes.Fallback:
                    return;
            }
            Task.Run(async () =>
            {
                await Dispatcher.BeginInvoke(async () =>
                {
                    var imgPath = await ChatDetailListItem_Common.DownloadImageAsync(url);
                    if (imgPath != null)
                    {
                        Container.Background = new ImageBrush(new BitmapImage(new Uri(imgPath)))
                        {
                            Stretch = Stretch.UniformToFill,
                            AlignmentX = AlignmentX.Left,
                            AlignmentY = AlignmentY.Top,
                            TileMode = TileMode.None
                        };
                        FallbackDisplay.Visibility = Visibility.Collapsed;
                    }
                });
            });
        }
    }
}