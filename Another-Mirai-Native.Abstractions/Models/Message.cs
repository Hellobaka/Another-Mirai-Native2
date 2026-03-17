using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models.MessageItem;
using Another_Mirai_Native.Abstractions.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Another_Mirai_Native.Abstractions.Models
{
    /// <summary>
    /// 描述消息的类
    /// </summary>
    public class Message
    {
        internal readonly IPluginApi pluginApi;

        /// <summary>
        /// 初始化 <see cref="Message"/> 实例。
        /// </summary>
        /// <param name="pluginApi">插件 API 实例。</param>
        /// <param name="id">消息唯一标识。</param>
        /// <param name="text">消息原文。</param>
        public Message(IPluginApi pluginApi, int id, string text)
        {
            this.pluginApi = pluginApi;
            Id = id;
            Text = text;
            BuildMessageChain();
        }

        internal IPluginApi PluginApi => pluginApi;

        /// <summary>
        /// 获取当前消息的全局唯一标识
        /// </summary>
        public int Id { get; private set; }

        /// <summary>
        /// 获取一个值, 指示当前消息是否发送成功
        /// </summary>
        public bool IsSuccess => Id != 0;

        /// <summary>
        /// 获取当前消息的原文
        /// </summary>
        public string Text { get; private set; }

        /// <summary>
        /// 获取当前消息解析后的消息链。
        /// </summary>
        public List<MessageItemBase> MessageChain { get; private set; } = [];

        /// <summary>
        /// 撤回消息
        /// </summary>
        /// <returns>消息撤回成功与否</returns>
        public bool RemoveMessage()
        {
            return PluginApi.MessageApi.DeleteMessage(Id);
        }

        /// <summary>
        /// 异步撤回消息
        /// </summary>
        /// <returns>消息撤回成功与否</returns>
        public Task<bool> RemoveMessageAsync()
        {
            return PluginApi.MessageApi.DeleteMessageAsync(Id);
        }

        private void BuildMessageChain()
        {
            var parts = Text.SplitV2("\\[CQ:.*?\\]");
            foreach (var item in parts)
            {
                if (item.StartsWith("[CQ:"))
                {
                    var cqcode = CQCode.Parse(item).FirstOrDefault();
                    if (cqcode == null)
                    {
                        continue;
                    }
                    switch (cqcode.Function)
                    {
                        case MessageItemType.Face:
                            if (int.TryParse(cqcode.Items["id"], out int id))
                            {
                                MessageChain.Add(new Face(id));
                            }
                            break;

                        case MessageItemType.Bface:
                            if (int.TryParse(cqcode.Items["id"], out id))
                            {
                                MessageChain.Add(new Face(id));
                            }
                            break;

                        case MessageItemType.Image:
                            string file = cqcode.Items["file"];
                            bool isFlash = cqcode.Items.ContainsKey("flash") && cqcode.Items["flash"] == "true";
                            bool isPath = file.Contains("\\");
                            bool isEmoji = cqcode.Items.ContainsKey("sub_type") && cqcode.Items["sub_type"] == "1";
                            if (isPath)
                            {
                                MessageChain.Add(new Image(filePath: file, isFlash: isFlash, isEmoji: isEmoji));
                            }
                            else
                            {
                                MessageChain.Add(new Image(hash: file, isFlash: isFlash, isEmoji: isEmoji));
                            }

                            break;

                        case MessageItemType.Record:
                            file = cqcode.Items["file"];
                            isPath = file.Contains("\\");
                            if (isPath)
                            {
                                MessageChain.Add(new Record(filePath: file));
                            }
                            else
                            {
                                MessageChain.Add(new Record(hash: file));
                            }
                            break;

                        case MessageItemType.At:
                            var qq = cqcode.Items["qq"];
                            if (qq == "all")
                            {
                                MessageChain.Add(new At(0, true));
                            }
                            else if (long.TryParse(qq, out long qqNum))
                            {
                                MessageChain.Add(new At(qqNum, false));
                            }
                            break;

                        case MessageItemType.Rps:
                            if (int.TryParse(cqcode.Items["type"], out int type))
                            {
                                MessageChain.Add(new RPS((RpsType)type));
                            }
                            break;

                        case MessageItemType.Shake:
                            MessageChain.Add(new Shake());
                            break;

                        case MessageItemType.Dice:
                            if (int.TryParse(cqcode.Items["type"], out type))
                            {
                                MessageChain.Add(new Dice(type));
                            }
                            break;

                        case MessageItemType.Poke:
                            MessageChain.Add(new Poke(cqcode.Items["name"]));
                            break;

                        case MessageItemType.Rich:
                            RichContentType richContentType = Enum.TryParse<RichContentType>(cqcode.Items["type"], true, out var result) ? result : RichContentType.Json;
                            MessageChain.Add(new RichContent(richContentType, cqcode.Items["content"]));
                            break;

                        case MessageItemType.Reply:
                            if (int.TryParse(cqcode.Items["id"], out id))
                            {
                                MessageChain.Add(new Reply(id));
                            }
                            break;

                        default:
                            // 无效的CQ码
                            continue;
                    }
                }
                else
                {
                    MessageChain.Add(new Text(item));
                }
            }
        }
    }
}
