using PropertyChanged;
using SqlSugar;
using System.ComponentModel;

namespace Another_Mirai_Native.Model
{
    [SugarTable("log")]
    [AddINotifyPropertyChangedInterface]
    public class LogModel
    {
        private const int DetailNoWrapPreviewMaxLength = 200;

        private string _detail = "";

        private string? _detailNoWrapCache;

        [SugarColumn(IsIdentity = true, IsPrimaryKey = true)]
        public int id { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        public long time { get; set; }

        /// <summary>
        /// 日志等级
        /// </summary>
        public int priority { get; set; }

        /// <summary>
        /// 日志来源
        /// </summary>
        public string source { get; set; } = "";

        /// <summary>
        /// 日志处理状态
        /// </summary>
        public string status { get; set; } = "";

        /// <summary>
        /// 日志类型
        /// </summary>
        public string name { get; set; } = "";

        /// <summary>
        /// 日志内容
        /// </summary>
        public string detail
        {
            get
            {
                return _detail;
            }
            set
            {
                _detail = value ?? "";
                _detailNoWrapCache = null;
            }
        }

        [SugarColumn(IsIgnore = true)]
        public string detailNoWrap
        {
            get
            {
                if (_detailNoWrapCache != null)
                {
                    return _detailNoWrapCache;
                }

                string content = detail.Replace("\r", " ").Replace("\n", " ");
                if (content.Length > DetailNoWrapPreviewMaxLength)
                {
                    content = content.Substring(0, DetailNoWrapPreviewMaxLength) + "...";
                }

                _detailNoWrapCache = content;
                return _detailNoWrapCache;
            }
        }

        public override string ToString()
        {
            return $"{detail} [{source}]";
        }
    }
}