using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using XamlAnimatedGif;

namespace Another_Mirai_Native.UI.Controls
{
    /// <summary>
    /// SendImageSelector.xaml 的交互逻辑
    /// </summary>
    public partial class FaceImageSelector : UserControl
    {
        public FaceImageSelector()
        {
            InitializeComponent();
            BuildCacheImages();
        }

        public event EventHandler ImageSelected;

        public string SelectedImageCQCode { get; set; }

        private string CollectedImagePath { get; } = Path.Combine("data", "image", "collected");

        private void MainContainer_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (FaceItem.IsSelected)
            {
                FaceContainer.Visibility = Visibility.Visible;
                ImageConatainer.Visibility = Visibility.Collapsed;
            }
            else
            {
                FaceContainer.Visibility = Visibility.Collapsed;
                ImageConatainer.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            MainScroll.ScrollToTop();

            FaceContainer_Common.Children.Clear();
            foreach (var item in UIConfig.Instance.UsedFaceId)
            {
                var element = ChatDetailListItem_Common.BuildFaceElement(item, false);
                if (element != null)
                {
                    FaceContainer_Common.Children.Add(BuildFaceElement(element, item));
                }
            }

            ImageConatainer.Children.Clear();
            Directory.CreateDirectory(CollectedImagePath);

            foreach (var item in Directory.GetFiles(CollectedImagePath))
            {
                if (Uri.TryCreate(Path.GetFullPath(item), UriKind.Absolute, out Uri? uri))
                {
                    Image image = new()
                    {
                        Width = 80,
                        Height = 80,
                        Stretch = Stretch.UniformToFill,
                        Margin = new Thickness(5),
                        Tag = Path.GetFileName(item)
                    };
                    AnimationBehavior.SetSourceUri(image, uri);
                    AnimationBehavior.SetRepeatBehavior(image, RepeatBehavior.Forever);
                    Button button = new()
                    {
                        Content = image,
                        Background = Brushes.Transparent
                    };
                    button.Click += (_, _) =>
                    {
                        SelectedImageCQCode = $"[CQ:image,file=collected\\{image.Tag}]";
                        ImageSelected?.Invoke(button, new EventArgs());
                    };
                    ImageConatainer.Children.Add(button);
                }
            }
        }

        private void BuildCacheImages()
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                var allFaces = GetAllResourceImages();
                foreach (var face in allFaces.OrderBy(x => x.Key))
                {
                    var element = ChatDetailListItem_Common.BuildFaceElement(face.Key, false);
                    if (element != null)
                    {
                        FaceContainer_All.Children.Add(BuildFaceElement(element, face.Key));
                    }
                }
            });
        }

        private Button BuildFaceElement(Image img, int id)
        {
            Button button = new()
            {
                Content = img,
                Background = Brushes.Transparent
            };
            button.Click += (_, _) =>
            {
                SelectedImageCQCode = $"[CQ:face,id={id}]";
                ImageSelected?.Invoke(button, new EventArgs());
                UpdateUsedFaceList(id);
            };
            return button;
        }

        private void UpdateUsedFaceList(int id)
        {
            List<int> usedFaceId = UIConfig.Instance.UsedFaceId;
            if (usedFaceId.Count > 10)
            {
                usedFaceId.RemoveAt(usedFaceId.Count - 1);
            }
            if (usedFaceId.Contains(id))
            {
                usedFaceId.Remove(id);
            }
            usedFaceId.Insert(0, id);

            UIConfig.Instance.SetConfig("UsedFaceId", usedFaceId);
        }

        private static Dictionary<int, string> GetAllResourceImages()
        {
            var images = new Dictionary<int, string>();
            var assembly = Assembly.GetExecutingAssembly();
            string resourceName = assembly.GetName().Name + ".g.resources";
            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream == null)
            {
                return [];
            }
            using var reader = new System.Resources.ResourceReader(stream);

            foreach (DictionaryEntry entry in reader)
            {
                var resourceKey = (string)entry.Key;
                if (resourceKey.StartsWith("resources/qq-face") && resourceKey.EndsWith(".png")
                    && int.TryParse(System.IO.Path.GetFileNameWithoutExtension(resourceKey), out int faceId))
                {
                    images.Add(faceId, $"pack://application:,,,/Resources/qq-face/{faceId}.png");
                }
            }
            return images;
        }
    }
}