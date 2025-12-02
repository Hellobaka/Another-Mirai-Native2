using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Another_Mirai_Native.UI.Controls.Chat
{
    /// <summary>
    /// SendImageSelector.xaml 的交互逻辑
    /// </summary>
    public partial class FaceImageSelector : UserControl
    {
        private const int FaceRowSize = 8;
        private const int ImageRowSize = 4;

        private Dictionary<int, string> _faceResourceCache = [];

        public FaceImageSelector()
        {
            InitializeComponent();
            DataContext = this;
            BuildCacheImages();
        }

        public event EventHandler? ImageSelected;

        public string SelectedImageCQCode { get; set; } = string.Empty;

        public ObservableCollection<IReadOnlyList<FaceResourceItem>> CommonFaceRows { get; } = new();

        public ObservableCollection<IReadOnlyList<FaceResourceItem>> AllFaceRows { get; } = new();

        public ObservableCollection<IReadOnlyList<CollectedImageItem>> CollectedImageRows { get; } = new();

        private string CollectedImagePath { get; } = Path.Combine("data", "image", "collected");

        private void MainContainer_SelectionChanged(ModernWpf.Controls.NavigationView sender, ModernWpf.Controls.NavigationViewSelectionChangedEventArgs args)
        {
            if (FaceItem.IsSelected)
            {
                FaceScroll.Visibility = Visibility.Visible;
                ImageList.Visibility = Visibility.Collapsed;
            }
            else
            {
                FaceScroll.Visibility = Visibility.Collapsed;
                ImageList.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            FaceScroll.ScrollToTop();
            RefreshCommonFaceRows();
            LoadCollectedImages();
        }

        private void FaceButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not FaceResourceItem face)
            {
                return;
            }

            SelectedImageCQCode = $"[CQ:face,id={face.Id}]";
            ImageSelected?.Invoke(sender, EventArgs.Empty);
            UpdateUsedFaceList(face.Id);
            RefreshCommonFaceRows();
            e.Handled = true;
        }

        private void ImageButton_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is not CollectedImageItem image)
            {
                return;
            }

            SelectedImageCQCode = $"[CQ:image,file=collected\\{image.FileName}]";
            ImageSelected?.Invoke(sender, EventArgs.Empty);
            e.Handled = true;
        }

        private void FaceScroll_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (FaceScroll.Visibility != Visibility.Visible)
            {
                return;
            }

            FaceScroll.ScrollToVerticalOffset(FaceScroll.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void BuildCacheImages()
        {
            MainWindow.Instance.Dispatcher.BeginInvoke(() =>
            {
                _faceResourceCache = GetAllResourceImages();
                PopulateAllFaceRows();
                RefreshCommonFaceRows();
            });
        }

        private void PopulateAllFaceRows()
        {
            AllFaceRows.Clear();
            if (_faceResourceCache.Count == 0)
            {
                return;
            }

            var faces = _faceResourceCache
                .OrderBy(x => x.Key)
                .Select(x => new FaceResourceItem(x.Key, new Uri(x.Value, UriKind.Absolute)))
                .ToList();

            foreach (var chunk in Chunk(faces, FaceRowSize))
            {
                AllFaceRows.Add(chunk);
            }
        }

        private void RefreshCommonFaceRows()
        {
            CommonFaceRows.Clear();
            if (_faceResourceCache.Count == 0)
            {
                return;
            }

            var faces = new List<FaceResourceItem>();
            foreach (var id in UIConfig.Instance.UsedFaceId)
            {
                if (_faceResourceCache.TryGetValue(id, out var path))
                {
                    faces.Add(new FaceResourceItem(id, new Uri(path, UriKind.Absolute)));
                }
            }

            foreach (var chunk in Chunk(faces, FaceRowSize))
            {
                CommonFaceRows.Add(chunk);
            }
        }

        private void LoadCollectedImages()
        {
            CollectedImageRows.Clear();
            Directory.CreateDirectory(CollectedImagePath);
            var items = new List<CollectedImageItem>();

            foreach (var file in Directory.GetFiles(CollectedImagePath))
            {
                var fullPath = Path.GetFullPath(file);
                if (Uri.TryCreate(fullPath, UriKind.Absolute, out var uri))
                {
                    items.Add(new CollectedImageItem(Path.GetFileName(file), uri));
                }
            }

            foreach (var chunk in Chunk(items, ImageRowSize))
            {
                CollectedImageRows.Add(chunk);
            }
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

            foreach (System.Collections.DictionaryEntry entry in reader)
            {
                var resourceKey = (string)entry.Key;
                if (resourceKey.StartsWith("resources/qq-face") && resourceKey.EndsWith(".png")
                    && int.TryParse(Path.GetFileNameWithoutExtension(resourceKey), out int faceId))
                {
                    images[faceId] = $"pack://application:,,,/Resources/qq-face/{faceId}.png";
                }
            }
            return images;
        }

        private static IEnumerable<IReadOnlyList<T>> Chunk<T>(IEnumerable<T> items, int size)
        {
            if (size <= 0)
            {
                yield break;
            }

            var buffer = new List<T>(size);
            foreach (var item in items)
            {
                buffer.Add(item);
                if (buffer.Count == size)
                {
                    yield return buffer.ToArray();
                    buffer.Clear();
                }
            }

            if (buffer.Count > 0)
            {
                yield return buffer.ToArray();
            }
        }

        public sealed class FaceResourceItem
        {
            public FaceResourceItem(int id, Uri source)
            {
                Id = id;
                Source = source;
            }

            public int Id { get; }

            public Uri Source { get; }
        }

        public sealed class CollectedImageItem
        {
            public CollectedImageItem(string fileName, Uri source)
            {
                FileName = fileName;
                Source = source;
            }

            public string FileName { get; }

            public Uri Source { get; }
        }
    }
}
