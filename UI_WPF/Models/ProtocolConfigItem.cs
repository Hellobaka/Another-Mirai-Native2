using System;

namespace Another_Mirai_Native.UI.Models
{
    public class ProtocolConfigItem
    {
        public string Key { get; set; } = string.Empty;

        public string DescriptionTitle { get; set; } = string.Empty;

        public string DescriptionSubtitle { get; set; } = string.Empty;

        public Type ValueType { get; set; }

        public Type DisplayControl { get; set; }

        public Array DisplayValues { get; set; }
    }
}
