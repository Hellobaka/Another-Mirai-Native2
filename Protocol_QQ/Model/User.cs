namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class User
    {
        /// <summary>
        /// 用户 id
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string username { get; set; }

        /// <summary>
        /// 用户头像地址
        /// </summary>
        public string avatar { get; set; }

        /// <summary>
        /// 是否是机器人
        /// </summary>
        public bool bot { get; set; }

        /// <summary>
        /// 特殊关联应用的 openid，需要特殊申请并配置后才会返回。如需申请，请联系平台运营人员。
        /// </summary>
        public string union_openid { get; set; }

        /// <summary>
        /// 机器人关联的互联应用的用户信息，与union_openid关联的应用是同一个。如需申请，请联系平台运营人员。
        /// </summary>
        public string union_user_account { get; set; }
    }
}