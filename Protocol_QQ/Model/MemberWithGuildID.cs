using System;
using System.Collections.Generic;

namespace Another_Mirai_Native.Protocol.QQ.Model
{
    public class MemberWithGuildID
    {
        /// <summary>
        /// 频道id
        /// </summary>
        public string guild_id { get; set; }

        /// <summary>
        /// 用户的频道基础信息
        /// </summary>
        public User user { get; set; }

        /// <summary>
        /// 用户的昵称
        /// </summary>
        public string nick { get; set; }

        /// <summary>
        /// 用户在频道内的身份
        /// </summary>
        public List<string> roles { get; set; }

        /// <summary>
        /// 用户加入频道的时间
        /// </summary>
        public DateTime joined_at { get; set; }
    }
}