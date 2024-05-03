namespace Another_Mirai_Native.Model
{
    public class UpdateModel
    {
        public string LatestVersion { get; set; } = "";

        public DateTime ReleaseTime { get; set; }

        public List<UpdateItem> Items { get; set; } = new();

        public class UpdateItem
        {
            public string Version { get; set; } = "";

            public DateTime ReleaseTime { get; set; }

            public string ReleaseDescription { get; set; } = "";

            public string DownloadUrl { get; set; } = "";
        }

        public void Rebuild()
        {
            foreach (var item in Items)
            {
                var s = item.ReleaseDescription.Split('\n');
                item.ReleaseDescription = "";
                foreach (var i in s)
                {
                    item.ReleaseDescription += $"· {i}\n";
                }
            }
        }
    }
}
