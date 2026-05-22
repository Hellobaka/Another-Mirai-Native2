using Another_Mirai_Native.Abstractions.Enums;
using Another_Mirai_Native.Abstractions.Models;
using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using System.IO;
using System.Text;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public static class MessageChainParser
    {
        public static event Action<MessageChain, FileEntity> OnFileUploaded;

        public static string ParseMessageChainToCQCode(MessageChain chain)
        {
            StringBuilder message = new();
            foreach (var item in chain)
            {
                if (item is FaceEntity face)
                {
                    message.Append($"[CQ:face,id={face.FaceId}]");
                }
                else if (item is FileEntity file)
                {
                    Task.Run(() => OnFileUploaded?.Invoke(chain, file));
                }
                else if (item is ForwardEntity forward)
                {
                    int id = MessageCacher.GetMessageId(forward.MessageId, forward.Sequence);
                    if (id != 0)
                    {
                        message.Append($"[CQ:reply,id={id}]");
                    }
                }
                else if (item is GreyTipEntity greyTip)
                {
                    // ignore
                }
                else if (item is GroupReactionEntity groupReaction)
                {
                    // ignore
                }
                else if (item is ImageEntity image)
                {
                    string imgId = ChatHistoryHelper.CacheMessageFile(CachedFileType.Image, image.ImageUrl).Result
                        ?? BitConverter.ToString(image.ImageMd5).ToUpper().Replace("-", "");
                    message.Append($"[CQ:image,file={imgId},sub_type={image.SubType}]");
                }
                else if (item is JsonEntity json)
                {
                    message.Append($"[CQ:json,content={EscapeRawMessage(json.Json)}]");
                }
                else if (item is KeyboardEntity keyboard)
                {
                    // ignore
                }
                else if (item is LightAppEntity lightApp)
                {
                    message.Append($"[CQ:app,content={EscapeRawMessage(lightApp.Payload)}]");
                }
                else if (item is LongMsgEntity longMsg)
                {
                    // ignore
                }
                else if (item is MarkdownEntity markdown)
                {
                    // ignore
                }
                else if (item is MarketfaceEntity marketface)
                {
                    message.Append($"[CQ:face,id={marketface.EmojiId},name={marketface.Key},desc={marketface.Summary}]");
                }
                else if (item is MentionEntity mention)
                {
                    if (mention.Uin == 0)
                    {
                        message.Append("[CQ:at,qq=all]");
                    }
                    else
                    {
                        message.Append($"[CQ:at,qq={mention.Uin}]");
                    }
                }
                else if (item is MultiMsgEntity multiMsg)
                {
                    // ignore
                }
                else if (item is PokeEntity poke)
                {
                    message.Append("[CQ:poke]");
                }
                else if (item is RecordEntity record)
                {
                    string voiceId = ChatHistoryHelper.CacheMessageFile(CachedFileType.Record, record.AudioUrl).Result
                        ?? BitConverter.ToString(record.AudioMd5).ToUpper().Replace("-", "");
                    message.Append($"[CQ:record,file={voiceId}]");
                }
                else if (item is SpecialPokeEntity specialPoke)
                {
                    message.Append("[CQ:poke]");
                }
                else if (item is TextEntity text)
                {
                    message.Append(EscapeRawMessage(text.Text));
                }
                else if (item is VideoEntity video)
                {
                    string videoId = ChatHistoryHelper.CacheMessageFile(CachedFileType.Video, video.VideoUrl).Result
                        ?? video.VideoHash;
                    message.Append($"[CQ:video,file={videoId}]");
                }
                else if (item is XmlEntity xml)
                {
                    message.Append($"[CQ:xml,content={EscapeRawMessage(xml.Xml)}]");
                }
            }
            return message.ToString();
        }

        public static void ParseCQCodeToMessageChain(Lagrange.Core.Message.MessageBuilder builder, string message)
        {
            var splits = message.SplitV2("\\[CQ:.*?\\]");
            foreach (var s in splits)
            {
                if (!s.StartsWith("[CQ:"))
                {
                    builder.Text(s);
                    continue;
                }
                var cqcode = CQCode.Parse(s).FirstOrDefault();
                if (cqcode == null)
                {
                    continue;
                }
                switch (cqcode.Function)
                {
                    case MessageItemType.Face:
                        if (ushort.TryParse(cqcode.Items["id"], out ushort faceId))
                        {
                            builder.Face(faceId);
                        }
                        else
                        {
                            LogHelper.Error("构建消息", $"表情ID无法转换为有效数字：{cqcode.Items["id"]}");
                        }
                        break;

                    case MessageItemType.Image:
                        string file = cqcode.Items["file"];
                        string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image", file);
                        if (File.Exists(picPath))
                        {
                            builder.Image(picPath);
                        }
                        else
                        {
                            var cacheImagePath = CachedFile.GetCachedImageByHash(file);
                            if (cacheImagePath == null)
                            {
                                LogHelper.Error("构建消息", $"图片文件不存在：{picPath}");
                            }
                            else
                            {
                                string baseDirectory = Helper.GetCacheDirectoryByCachedFileType(CachedFileType.Image);
                                builder.Image(Path.Combine(baseDirectory, cacheImagePath.FileName));
                            }
                        }
                        break;

                    case MessageItemType.Record:
                        string recordHash = cqcode.Items["file"];
                        string recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "record", recordHash);
                        if (File.Exists(recordPath))
                        {
                            string extension = new FileInfo(recordPath).Extension;
                            if (extension != ".silk")
                            {
                                if (SilkConverter.SilkEncode(recordPath, extension))
                                {
                                    recordPath = recordPath.Replace(extension, ".silk");
                                }
                                else
                                {
                                    LogHelper.Error("构建消息", $"转换音频到Silk格式失败：{recordPath}");
                                    break;
                                }
                            }
                            recordPath = Path.GetFullPath(recordPath);
                            builder.Record(recordPath);
                        }
                        else
                        {
                            var cachedRecord = CachedFile.GetCachedRecordByHash(recordHash);
                            if (cachedRecord == null)
                            {
                                LogHelper.Error("构建消息", $"音频文件不存在：{recordHash}");
                            }
                            else
                            {
                                string baseDirectory = Helper.GetCacheDirectoryByCachedFileType(CachedFileType.Record);
                                builder.Record(Path.Combine(baseDirectory, cachedRecord.FileName));
                            }
                        }
                        break;

                    case MessageItemType.At:
                        uint qq = uint.TryParse(cqcode.Items["qq"], out uint l) ? l : 0;
                        builder.Mention(qq);
                        break;

                    case MessageItemType.Reply:
                        if (int.TryParse(cqcode.Items["id"], out int msgId))
                        {
                            var msg = MessageCacher.GetMessageById(msgId);
                            if (msg != null)
                            {
                                builder.Forward(msg);
                            }
                            else
                            {
                                LogHelper.Error("构建消息", $"回复消息ID无法从缓存查询到消息：{cqcode.Items["id"]}");
                            }
                        }
                        else
                        {
                            LogHelper.Error("构建消息", $"回复消息ID无法转换为有效数字：{cqcode.Items["id"]}");
                        }
                        break;

                    default:
                        break;
                }
            }
        }
        private static string UnescapeRawMessage(string msg)
        {
            return msg.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").Replace("&amp;", "&");
        }

        private static string EscapeRawMessage(string msg)
        {
            return msg.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;").Replace(",", "&#44;");
        }

    }
}
