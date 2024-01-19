using Another_Mirai_Native.Model.Enums;
using System.Text;
using System.Text.RegularExpressions;

namespace Another_Mirai_Native.Model
{
    /// <summary>
    /// 表示 CQ码 的类
    /// </summary>
    public class CQCode
    {
        #region --字段--

        private static readonly Lazy<Regex[]> _regices = new(InitializeRegex);

        private string _originalString;

        private CQCodeType _type;

        #endregion --字段--

        #region --属性--

        /// <summary>
        /// 获取一个值, 指示当前实例的功能
        /// </summary>
        public CQCodeType Function
        { get { return _type; } }

        /// <summary>
        /// 获取一个值, 指示当前实例是否属于图片 <see cref="CQCode"/>
        /// </summary>
        public bool IsImageCQCode
        { get { return EqualIsImageCQCode(this); } }

        /// <summary>
        /// 获取一个值, 指示当前实例是否属于语音 <see cref="CQCode"/>
        /// </summary>
        public bool IsRecordCQCode
        { get { return EqualIsRecordCQCode(this); } }

        /// <summary>
        /// 获取当前实例所包含的所有项目
        /// </summary>
        public Dictionary<string, string> Items { get; }

        #endregion --属性--

        #region --构造函数--

        /// <summary>
        /// 初始化 <see cref="CQCode"/> 类的新实例
        /// </summary>
        /// <param name="type">CQ码类型</param>
        /// <param name="keyValues">包含的键值对</param>
        public CQCode(CQCodeType type, params KeyValuePair<string, string>[] keyValues)
        {
            this._type = type;
            this.Items = new Dictionary<string, string>(keyValues.Length);
            foreach (KeyValuePair<string, string> item in keyValues)
            {
                this.Items.Add(item.Key, item.Value);
            }

            this._originalString = null;
        }

        /// <summary>
        /// 使用 CQ码 字符串初始化 <see cref="CQCode"/> 类的新实例
        /// </summary>
        /// <param name="str">CQ码字符串 或 包含CQ码的字符串</param>
        private CQCode(string str)
        {
            this._originalString = str;

            #region --解析 CqCode--

            Match match = _regices.Value[0].Match(str);
            if (!match.Success)
            {
                throw new FormatException("无法解析所传入的字符串, 字符串非CQ码格式!");
            }

            #endregion --解析 CqCode--

            #region --解析CQ码类型--

            if (!System.Enum.TryParse<CQCodeType>(match.Groups[1].Value, true, out _type))
            {
                this._type = CQCodeType.Unknown;    // 解析不出来的时候, 直接给一个默认
            }

            #endregion --解析CQ码类型--

            #region --解析键值对--

            MatchCollection collection = _regices.Value[1].Matches(match.Groups[2].Value);
            this.Items = new Dictionary<string, string>(collection.Count);
            foreach (Match item in collection)
            {
                this.Items.Add(item.Groups[1].Value, CQDeCode(item.Groups[2].Value));
            }

            #endregion --解析键值对--
        }

        #endregion --构造函数--

        #region --公开方法--

        /// <summary>
        /// 判断是否是图片 <see cref="CQCode"/>
        /// </summary>
        /// <param name="code">要判断的 <see cref="CQCode"/> 实例</param>
        /// <returns>如果是图片 <see cref="CQCode"/> 返回 <see langword="true"/> 否则返回 <see langword="false"/></returns>
        public static bool EqualIsImageCQCode(CQCode code)
        {
            return code.Function == CQCodeType.Image;
        }

        /// <summary>
        /// 判断是否是语音 <see cref="CQCode"/>
        /// </summary>
        /// <param name="code">要判断的 <see cref="CQCode"/> 实例</param>
        /// <returns>如果是语音 <see cref="CQCode"/> 返回 <see langword="true"/> 否则返回 <see langword="false"/></returns>
        public static bool EqualIsRecordCQCode(CQCode code)
        {
            return code.Function == CQCodeType.Record;
        }

        /// <summary>
        /// 从字符串中解析出所有的 CQ码, 转换为 <see cref="CQCode"/> 集合
        /// </summary>
        /// <param name="source">原始字符串</param>
        /// <returns>返回等效的 <see cref="List{CqCode}"/></returns>
        public static List<CQCode> Parse(string source)
        {
            MatchCollection collection = _regices.Value[0].Matches(source);
            List<CQCode> codes = new(collection.Count);
            foreach (Match item in collection)
            {
                codes.Add(new CQCode(item.Groups[0].Value));
            }
            return codes;
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为 <code>true</code>，否则为 <code>false</code></returns>
        public override bool Equals(object obj)
        {
            return obj is CQCode code ? string.Equals(this._originalString, code._originalString) : base.Equals(obj);
        }

        /// <summary>
        /// 返回该字符串的哈希代码
        /// </summary>
        /// <returns> 32 位有符号整数哈希代码</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode() & this._originalString.GetHashCode();
        }

        /// <summary>
        /// 处理返回用于发送的字符串
        /// </summary>
        /// <returns>用于发送的字符串</returns>
        public string ToSendString()
        {
            return this.ToString();
        }

        /// <summary>
        /// 返回此实例等效的CQ码形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this._originalString == null)
            {
                if (this.Items.Count == 0)
                {
                    // 特殊CQ码, 抖动窗口
                    this._originalString = string.Format("[CQ:{0}]", _type.GetDescription());
                }
                else
                {
                    // 普通CQ码, 带参数
                    StringBuilder builder = new();
                    builder.Append("[CQ:");
                    builder.Append(this._type.GetDescription());   // function
                    foreach (KeyValuePair<string, string> item in this.Items)
                    {
                        builder.AppendFormat(",{0}={1}", item.Key, CQEnCode(item.Value, true));
                    }
                    builder.Append("]");
                    this._originalString = builder.ToString();
                }
            }
            return this._originalString;
        }

        #endregion --公开方法--

        #region --私有方法--

        /// <summary>
        /// 延时初始化正则表达式
        /// </summary>
        /// <returns></returns>
        private static Regex[] InitializeRegex()
        {
            // 此处延时加载, 以提升运行速度
            return new Regex[]
            {
                new(@"\[CQ:([A-Za-z]*)(?:(,[^\[\]]+))?\]", RegexOptions.Compiled),    // 匹配CQ码
                new(@",([A-Za-z]+)=([^,\[\]]+)", RegexOptions.Compiled)               // 匹配键值对
            };
        }

        #endregion --私有方法--

        #region --CQ码类方法--

        /// <summary>
        /// 获取酷Q "匿名" 代码
        /// </summary>
        /// <param name="forced">强制发送, 若本参数为 <code>true</code> 发送失败时将转换为普通消息</param>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Anonymous(bool forced = false)
        {
            CQCode code = new(CQCodeType.Anonymous);
            if (forced)
            {
                code.Items.Add("ignore", "true");
            }
            return code;
        }

        /// <summary>
        /// 获取酷Q "At某人" 代码
        /// </summary>
        /// <param name="qqId">QQ号</param>
        /// <exception cref="ArgumentOutOfRangeException">参数: qqId 超出范围</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_At(long qqId)
        {
            return qqId < 0
                ? throw new ArgumentOutOfRangeException("qqId")
                : new CQCode(
                CQCodeType.At,
                new KeyValuePair<string, string>("qq", Convert.ToString(qqId)));
        }

        /// <summary>
        /// 获取酷Q "At全体成员" 代码
        /// </summary>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_AtAll()
        {
            return new CQCode(
                CQCodeType.At,
                new KeyValuePair<string, string>("qq", "all"));
        }

        /// <summary>
        /// 获取酷Q "音乐自定义" 代码
        /// </summary>
        /// <param name="url">分享链接, 点击后进入的页面 (歌曲介绍)</param>
        /// <param name="musicUrl">歌曲链接, 音频链接 (mp3链接)</param>
        /// <param name="title">标题, 建议12字以内</param>
        /// <param name="content">简介, 建议30字以内</param>
        /// <param name="imageUrl">封面图片链接, 留空为默认</param>
        /// <exception cref="ArgumentException">参数: url 或 musicUrl 是空字符串或为 null</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_DIYMusic(string url, string musicUrl, string title = null, string content = null, string imageUrl = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("分享链接不能为空", "url");
            }

            if (string.IsNullOrEmpty(musicUrl))
            {
                throw new ArgumentException("歌曲链接不能为空", "musicUrl");
            }

            CQCode code = new(
                CQCodeType.Music,
                new KeyValuePair<string, string>("type", "custom"),
                new KeyValuePair<string, string>("url", url),
                new KeyValuePair<string, string>("audio", musicUrl));
            if (!string.IsNullOrEmpty(title))
            {
                code.Items.Add("title", title);
            }

            if (!string.IsNullOrEmpty(content))
            {
                code.Items.Add("content", content);
            }

            if (!string.IsNullOrEmpty(imageUrl))
            {
                code.Items.Add("imageUrl", imageUrl);
            }
            return code;
        }

        /// <summary>
        /// 获取酷Q "Emoji" 代码
        /// </summary>
        /// <param name="id">Emoji的Id</param>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Emoji(int id)
        {
            return new CQCode(
                CQCodeType.Emoji,
                new KeyValuePair<string, string>("id", Convert.ToString(id)));
        }

        /// <summary>
        /// 获取酷Q "表情" 代码
        /// </summary>
        /// <param name="face">表情枚举</param>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Face(int face)
        {
            return new CQCode(
                CQCodeType.Face,
                new KeyValuePair<string, string>("id", Convert.ToString(face)));
        }

        /// <summary>
        /// 获取酷Q "图片" 代码
        /// </summary>
        /// <param name="path">图片的路径, 将图片放在 酷Q\data\image 下, 并填写相对路径. 如 酷Q\data\image\1.jpg 则填写 1.jpg</param>
        /// <exception cref="ArgumentException">参数: path 是空字符串或为 null</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Image(string path)
        {
            return string.IsNullOrEmpty(path)
                ? throw new ArgumentException("路径不能为空", "path")
                : new CQCode(
                CQCodeType.Image,
                new KeyValuePair<string, string>("file", path));
        }

        /// <summary>
        /// 获取酷Q "音乐" 代码
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="style"></param>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Music(long id, string type = "qq", int style = 0)
        {
            return new CQCode(
                CQCodeType.Music,
                new KeyValuePair<string, string>("id", Convert.ToString(id)),
                new KeyValuePair<string, string>("type", type),
                new KeyValuePair<string, string>("style", Convert.ToString(style)));
        }

        /// <summary>
        /// 获取酷Q "语音" 代码
        /// </summary>
        /// <param name="path">语音的路径, 将音频放在 酷Q\data\record 下, 并填写相对路径. 如 酷Q\data\record\1.amr 则填写 1.amr</param>
        /// <exception cref="ArgumentException">参数: path 是空字符串或为 null</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Record(string path)
        {
            return string.IsNullOrEmpty(path)
                ? throw new ArgumentException("语音路径不允许为空", "path")
                : new CQCode(
                CQCodeType.Record,
                new KeyValuePair<string, string>("file", path));
        }

        /// <summary>
        /// 获取酷Q "戳一戳" 代码
        /// </summary>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Shake()
        {
            return new CQCode(CQCodeType.Shake);
        }

        /// <summary>
        /// 获取酷Q "好友名片分享" 代码
        /// </summary>
        /// <param name="qqId">QQ号码</param>
        /// <exception cref="ArgumentOutOfRangeException">参数: qqId 超出范围</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_ShareFriendCard(long qqId)
        {
            return qqId < 0
                ? throw new ArgumentOutOfRangeException("qqId")
                : new CQCode(CQCodeType.Contact,
                new KeyValuePair<string, string>("type", "qq"),
                new KeyValuePair<string, string>("id", Convert.ToString(qqId)));
        }

        /// <summary>
        /// 获取酷Q "位置分享" 代码
        /// </summary>
        /// <param name="site">地点, 建议12字以内</param>
        /// <param name="detail">详细地址, 建议20字以内</param>
        /// <param name="lat">维度</param>
        /// <param name="lon">经度</param>
        /// <param name="zoom">放大倍数, 默认: 15倍</param>
        /// <exception cref="ArgumentException">参数: site 或 detail 是空字符串或为 null</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_ShareGPS(string site, string detail, double lat, double lon, int zoom = 15)
        {
            if (string.IsNullOrEmpty(site))
            {
                throw new ArgumentException("分享的地点不能为空", "site");
            }

            return string.IsNullOrEmpty(detail)
                ? throw new ArgumentException("详细地址不能为空", "detail")
                : new CQCode(
                CQCodeType.Location,
                new KeyValuePair<string, string>("lat", Convert.ToString(lat)),
                new KeyValuePair<string, string>("lon", Convert.ToString(lon)),
                new KeyValuePair<string, string>("zoom", Convert.ToString(zoom)),
                new KeyValuePair<string, string>("title", site),
                new KeyValuePair<string, string>("content", detail));
        }

        /// <summary>
        /// 获取酷Q "群名片分享" 代码
        /// </summary>
        /// <param name="groupId">群组</param>
        /// <exception cref="ArgumentOutOfRangeException">参数: groupId 超出范围</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_ShareGroupCard(long groupId)
        {
            return groupId < 0
                ? throw new ArgumentOutOfRangeException("groupId")
                : new CQCode(CQCodeType.Contact,
                new KeyValuePair<string, string>("type", "group"),
                new KeyValuePair<string, string>("group", Convert.ToString(groupId)));
        }

        /// <summary>
        /// 获取酷Q "链接分享" 代码
        /// </summary>
        /// <param name="url">分享的链接</param>
        /// <param name="title">显示的标题, 建议12字以内</param>
        /// <param name="content">简介信息, 建议30字以内</param>
        /// <param name="imageUrl">分享的图片链接, 留空则为默认图片</param>
        /// <exception cref="ArgumentException">参数: url 是空字符串或为 null</exception>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_ShareLink(string url, string title, string content, string imageUrl = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentException("分享的链接为空", "url");
            }

            CQCode code = new(
                CQCodeType.Share,
                new KeyValuePair<string, string>("url", url));

            if (!string.IsNullOrEmpty(title))
            {
                code.Items.Add("title", title);
            }
            if (!string.IsNullOrEmpty(content))
            {
                code.Items.Add("content", content);
            }
            if (!string.IsNullOrEmpty(imageUrl))
            {
                code.Items.Add("image", imageUrl);
            }

            return code;
        }

        /// <summary>
        /// 获取字符串副本的非转义形式
        /// </summary>
        /// <param name="source">欲反转义的原始字符串</param>
        /// <exception cref="ArgumentNullException">参数: source 为 null</exception>
        /// <returns>返回反转义的字符串副本</returns>
        public static string CQDeCode(string source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            StringBuilder builder = new(source);
            builder = builder.Replace("&#91;", "[");
            builder = builder.Replace("&#93;", "]");
            builder = builder.Replace("&#44;", ",");
            builder = builder.Replace("&amp;", "&");
            return builder.ToString();
        }

        /// <summary>
        /// 获取字符串副本的转义形式
        /// </summary>
        /// <param name="source">欲转义的原始字符串</param>
        /// <param name="enCodeComma">是否转义逗号, 默认 <code>false</code></param>
        /// <exception cref="ArgumentNullException">参数: source 为 null</exception>
        /// <returns>返回转义后的字符串副本</returns>
        public static string CQEnCode(string source, bool enCodeComma)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }
            StringBuilder builder = new(source);
            builder = builder.Replace("&", "&amp;");
            builder = builder.Replace("[", "&#91;");
            builder = builder.Replace("]", "&#93;");
            if (enCodeComma)
            {
                builder = builder.Replace(",", "&#44;");
            }
            return builder.ToString();
        }

        #endregion --CQ码类方法--
    }
}