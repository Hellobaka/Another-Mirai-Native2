using Another_Mirai_Native.Abstractions.Enums;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 表示 CQ码 的类
    /// </summary>
    internal class CQCode
    {
        #region --字段--

        private static readonly Lazy<Regex[]> _regices = new(InitializeRegex);

        private string _originalString;

        private MessageItemType _type;

        #endregion --字段--

        #region --属性--

        /// <summary>
        /// 获取一个值, 指示当前实例的功能
        /// </summary>
        public MessageItemType Function
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
        public CQCode(MessageItemType type, params KeyValuePair<string, string>[] keyValues)
        {
            this._type = type;
            this.Items = new Dictionary<string, string>(keyValues.Length);
            foreach (KeyValuePair<string, string> item in keyValues)
            {
                this.Items.Add(item.Key, item.Value);
            }

            this._originalString = string.Empty;
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

            if (!System.Enum.TryParse<MessageItemType>(match.Groups[1].Value, true, out _type))
            {
                this._type = MessageItemType.Unknown;    // 解析不出来的时候, 直接给一个默认
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
            return code.Function == MessageItemType.Image;
        }

        /// <summary>
        /// 判断是否是语音 <see cref="CQCode"/>
        /// </summary>
        /// <param name="code">要判断的 <see cref="CQCode"/> 实例</param>
        /// <returns>如果是语音 <see cref="CQCode"/> 返回 <see langword="true"/> 否则返回 <see langword="false"/></returns>
        public static bool EqualIsRecordCQCode(CQCode code)
        {
            return code.Function == MessageItemType.Record;
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

        public static string RemoveAllCQCodes(string msg)
        {
            return _regices.Value[0].Replace(msg, "");
        }

        /// <summary>
        /// 确定指定的对象是否等于当前对象
        /// </summary>
        /// <param name="obj">要与当前对象进行比较的对象</param>
        /// <returns>如果指定的对象等于当前对象，则为 <code>true</code>，否则为 <code>false</code></returns>
        public override bool Equals(object? obj)
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
            if (string.IsNullOrEmpty(this._originalString))
            {
                if (this.Items.Count == 0)
                {
                    // 特殊CQ码, 抖动窗口
                    this._originalString = string.Format("[CQ:{0}]", _type.ToString().ToLower());
                }
                else
                {
                    // 普通CQ码, 带参数
                    StringBuilder builder = new();
                    builder.Append("[CQ:");
                    builder.Append(this._type.ToString().ToLower());   // function
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
            return
            [
                new(@"\[CQ:([A-Za-z]*)(?:(,[^\[\]]+))?\]", RegexOptions.Compiled),    // 匹配CQ码
                new(@",([A-Za-z_]+)=([^,\[\]]+)", RegexOptions.Compiled)               // 匹配键值对
            ];
        }

        #endregion --私有方法--

        #region --CQ码类方法--

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
                MessageItemType.At,
                new KeyValuePair<string, string>("qq", Convert.ToString(qqId)));
        }

        /// <summary>
        /// 获取酷Q "At全体成员" 代码
        /// </summary>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_AtAll()
        {
            return new CQCode(
                MessageItemType.At,
                new KeyValuePair<string, string>("qq", "all"));
        }

        /// <summary>
        /// 获取酷Q "表情" 代码
        /// </summary>
        /// <param name="face">表情枚举</param>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Face(int face)
        {
            return new CQCode(
                MessageItemType.Face,
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
                MessageItemType.Image,
                new KeyValuePair<string, string>("file", path));
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
                MessageItemType.Record,
                new KeyValuePair<string, string>("file", path));
        }

        /// <summary>
        /// 获取酷Q "戳一戳" 代码
        /// </summary>
        /// <returns>返回 <see cref="CQCode"/> 对象</returns>
        public static CQCode CQCode_Shake()
        {
            return new CQCode(MessageItemType.Shake);
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