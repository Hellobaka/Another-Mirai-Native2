﻿using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Native;
using Lagrange.Core.Message;
using Lagrange.Core.Message.Entity;
using System.Text;

namespace Another_Mirai_Native.Protocol.LagrangeCore
{
    public static class MessageChainPaser
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
                    // ignore
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
                    string imgId = BitConverter.ToString(image.ImageMd5).ToUpper().Replace("-", "");
                    Directory.CreateDirectory("data\\image");
                    File.WriteAllText($"data\\image\\{imgId}.cqimg", $"[image]\nmd5={imgId}\nsize={image.ImageSize}\nurl={image.ImageUrl}");
                    message.Append($"[CQ:image,file={imgId}]");
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
                    string voiceId = BitConverter.ToString(record.AudioMd5).ToUpper().Replace("-", "");
                    Directory.CreateDirectory("data\\record");
                    File.WriteAllText($"data\\record\\{voiceId}.cqrecord", $"[record]\nurl={record.AudioUrl}");
                    message.Append($"[CQ:record,file={voiceId}]");
                }
                else if (item is SpecialPokeEntity specialPoke)
                {
                    message.Append("[CQ:poke]");
                }
                else if (item is TextEntity text)
                {
                    message.Append(text.Text);
                }
                else if (item is VideoEntity video)
                {
                    string videoId = video.VideoHash;
                    Directory.CreateDirectory("data\\video");
                    File.WriteAllText($"data\\video\\{videoId}.cqvideo", $"[video]\nurl={video.VideoUrl}\nlength={video.VideoLength}");
                    message.Append($"[CQ:video,file={videoId}]");
                }
                else if (item is XmlEntity xml)
                {
                    message.Append($"[CQ:xml,content={EscapeRawMessage(xml.Xml)}]");
                }
            }
            return message.ToString();
        }

        public static void ParseCQCodeToMessageChain(MessageBuilder builder, string message)
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
                    case Model.Enums.CQCodeType.Face:
                        if (ushort.TryParse(cqcode.Items["id"], out ushort faceId))
                        {
                            builder.Face(faceId);
                        }
                        else
                        {
                            LogHelper.Error("构建消息", $"表情ID无法转换为有效数字：{cqcode.Items["id"]}");
                        }
                        break;

                    case Model.Enums.CQCodeType.Image:
                        string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image", cqcode.Items["file"]);
                        if (File.Exists(picPath))
                        {
                            builder.Image(picPath);
                        }
                        else if (File.Exists(picPath + ".cqimg"))
                        {
                            string picUrl = File.ReadAllText(picPath).Split('\n').Last().Replace("url=", "");
                            string? path = Helper.DownloadImageAsync(picUrl, picPath).Result;
                            if (string.IsNullOrEmpty(path))
                            {
                                LogHelper.Error("构建消息", $"从缓存下载图片失败：{picPath}");
                                break;
                            }
                            builder.Image(path);
                        }
                        else
                        {
                            LogHelper.Error("构建消息", $"图片文件不存在：{picPath}");
                        }
                        break;

                    case Model.Enums.CQCodeType.Record:
                        string recordPath = cqcode.Items["file"];
                        recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "record", recordPath);
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
                        else if (File.Exists(recordPath + ".cqrecord"))
                        {
                            recordPath += ".amr";
                            string picUrl = File.ReadAllText(recordPath).Split('\n').Last().Replace("url=", "");
                            string cachePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "record", "cache");
                            bool download = Helper.DownloadFile(picUrl, Path.GetFileName(recordPath), cachePath, true).Result;
                            if (!download)
                            {
                                LogHelper.Error("构建消息", $"从缓存下载音频失败：{recordPath}");
                                break;
                            }
                            builder.Record(Path.Combine(cachePath, Path.GetFileName(recordPath)));
                        }
                        break;

                    case Model.Enums.CQCodeType.At:
                        uint qq = uint.TryParse(cqcode.Items["qq"], out uint l) ? l : 0;
                        builder.Mention(qq);
                        break;

                    case Model.Enums.CQCodeType.Reply:
                        if (uint.TryParse(cqcode.Items["id"], out uint msgId))
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
