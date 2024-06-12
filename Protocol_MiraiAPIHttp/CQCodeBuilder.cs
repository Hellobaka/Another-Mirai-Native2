using Another_Mirai_Native.DB;
using Another_Mirai_Native.Enums;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using Another_Mirai_Native.Protocol.MiraiAPIHttp.MiraiAPIResponse;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Another_Mirai_Native.Protocol.MiraiAPIHttp
{
    /// <summary>
    /// 辅助CQ码构建与解析的类
    /// </summary>
    public static class CQCodeBuilder
    {
        /// <summary>
        /// CQ码转消息链
        /// </summary>
        /// <param name="message">CQ码文本</param>
        /// <returns>转换后的消息链数组</returns>
        public static List<IMiraiMessageBase> BuildMessageChains(string message, out int quoteId)
        {
            quoteId = -1;
            List<IMiraiMessageBase> result = new();
            var list = CQCode.Parse(message);// 通过工具函数提取所有的CQ码
            foreach (var item in list)
            {
                message = message.Replace(item.ToString(), "<!cqCode!>");// 将CQ码的位置使用占空文本替换
            }
            var p = message.Split("<!cqCode!>");
            int cqCode_index = 0;
            for (int i = 0; i < p.Length; i++)
            {
                IMiraiMessageBase messageBase;
                if (p[i] == "<!cqCode!>")
                {
                    messageBase = ParseCQCode2MiraiMessageBase(list[cqCode_index], out quoteId);// 将CQ码转换为消息链对象
                    cqCode_index++;
                    if (messageBase == null)
                    {
                        continue;
                    }
                }
                else
                {
                    messageBase = new MiraiMessageTypeDetail.Plain { text = p[i] };// 将文本转换为消息链对象
                }
                result.Add(messageBase);
            }
            return result;
        }

        /// <summary>
        /// 消息链转CQ码
        /// </summary>
        /// <param name="chainMsg">从Mirai发送来的消息链</param>
        /// <returns>转换CQ码后的结果</returns>
        public static string Parse(List<IMiraiMessageBase> chainMsg)
        {
            StringBuilder Result = new();
            foreach (var item in chainMsg)
            {
                switch (item.messageType)
                {
                    case MiraiMessageType.Source:
                        break;

                    case MiraiMessageType.Quote:
                        var quote = (MiraiMessageTypeDetail.Quote)item;
                        Result.Append($"[CQ:reply,id={quote.id}]");
                        break;

                    case MiraiMessageType.At:
                        var at = (MiraiMessageTypeDetail.At)item;
                        Result.Append(CQCode.CQCode_At(at.target));
                        break;

                    case MiraiMessageType.AtAll:
                        Result.Append(CQCode.CQCode_AtAll());
                        break;

                    case MiraiMessageType.Face:
                        var face = (MiraiMessageTypeDetail.Face)item;
                        Result.Append(CQCode.CQCode_Face(face.faceId));
                        break;

                    case MiraiMessageType.Plain:
                        var plain = (MiraiMessageTypeDetail.Plain)item;
                        Result.Append(plain.text);
                        break;

                    case MiraiMessageType.Image:
                        var image = (MiraiMessageTypeDetail.Image)item;
                        string imgId;
                        if (image.imageId.StartsWith("http"))
                        {
                            imgId = image.imageId.MD5();
                        }
                        else
                        {
                            imgId = image.imageId.Replace("-", "").Replace("{", "").Replace("}", "").Split('.').First();
                        }
                        Directory.CreateDirectory("data\\image");
                        File.WriteAllText($"data\\image\\{imgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={image.url}");
                        Result.Append($"[CQ:image,file={imgId}]");
                        break;

                    case MiraiMessageType.FlashImage:
                        var flashImage = (MiraiMessageTypeDetail.FlashImage)item;
                        string flashImgId = flashImage.imageId.Replace("-", "").Replace("{", "").Replace("}", "").Split('.').First();
                        Directory.CreateDirectory("data\\image");
                        File.WriteAllText($"data\\image\\{flashImgId}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={flashImage.url}");
                        Result.Append($"[CQ:image,file={flashImgId},flash=true]");
                        break;

                    case MiraiMessageType.Voice:
                        var voice = (MiraiMessageTypeDetail.Voice)item;
                        string voiceId = voice.voiceId.Replace(".amr", "");
                        Directory.CreateDirectory("data\\record");
                        File.WriteAllText($"data\\record\\{voiceId}.cqrecord", $"[record]\nurl={voice.url}");
                        Result.Append($"[CQ:record,file={voice.voiceId}]");
                        break;

                    case MiraiMessageType.Xml:
                        var xml = (MiraiMessageTypeDetail.Xml)item;
                        Result.Append($"[CQ:rich,type=xml,content={EscapeRawMessage(xml.xml)}]");
                        break;

                    case MiraiMessageType.Json:
                        var json = (MiraiMessageTypeDetail.Json)item;
                        Result.Append($"[CQ:rich,type=json,content={EscapeRawMessage(json.json)}]");
                        break;

                    case MiraiMessageType.App:
                        var app = (MiraiMessageTypeDetail.App)item;
                        Result.Append($"[CQ:rich,type=app,content={EscapeRawMessage(app.content)}]");
                        break;

                    case MiraiMessageType.Poke:
                        var poke = (MiraiMessageTypeDetail.Poke)item;
                        Result.Append($"[CQ:poke,name={poke.name},]");
                        break;

                    case MiraiMessageType.Dice:
                        var dice = (MiraiMessageTypeDetail.Dice)item;
                        Result.Append($"[CQ:dice,point={dice.value}]");
                        break;

                    case MiraiMessageType.MarketFace:
                        var marketFace = (MiraiMessageTypeDetail.MarketFace)item;
                        Result.Append($"[CQ:bigface,id={marketFace.id}]");
                        break;

                    case MiraiMessageType.MusicShare:
                        var musicShare = (MiraiMessageTypeDetail.MusicShare)item;
                        Result.Append(CQCode.CQCode_DIYMusic(musicShare.jumpUrl, musicShare.musicUrl, musicShare.title, musicShare.brief, musicShare.pictureUrl));
                        break;

                    case MiraiMessageType.Forward:
                    case MiraiMessageType.File:
                    case MiraiMessageType.MiraiCode:
                    default:
                        break;
                }
            }
            return Result.ToString();
        }

        /// <summary>
        /// 将Mirai发送的 json 数组, 转换为消息链对象
        /// </summary>
        /// <param name="json">json数组</param>
        public static List<IMiraiMessageBase> ParseJArray2MiraiMessageBaseList(JArray json)
        {
            List<IMiraiMessageBase> chainMsg = new();
            foreach (var item in json)
            {
                MiraiMessageType msgType = Helper.String2Enum<MiraiMessageType>(item["type"].ToString());
                switch (msgType)
                {
                    case MiraiMessageType.Source:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Source>());
                        break;

                    case MiraiMessageType.Quote:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Quote>());
                        break;

                    case MiraiMessageType.At:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.At>());
                        break;

                    case MiraiMessageType.AtAll:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.AtAll>());
                        break;

                    case MiraiMessageType.Face:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Face>());
                        break;

                    case MiraiMessageType.Plain:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Plain>());
                        break;

                    case MiraiMessageType.Image:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Image>());
                        break;

                    case MiraiMessageType.FlashImage:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.FlashImage>());
                        break;

                    case MiraiMessageType.Voice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Voice>());
                        break;

                    case MiraiMessageType.Xml:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Xml>());
                        break;

                    case MiraiMessageType.Json:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Json>());
                        break;

                    case MiraiMessageType.App:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.App>());
                        break;

                    case MiraiMessageType.Poke:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Poke>());
                        break;

                    case MiraiMessageType.Dice:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Dice>());
                        break;

                    case MiraiMessageType.MarketFace:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MarketFace>());
                        break;

                    case MiraiMessageType.MusicShare:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MusicShare>());
                        break;

                    case MiraiMessageType.Forward:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.Forward>());
                        break;

                    case MiraiMessageType.File:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.File>());
                        break;

                    case MiraiMessageType.MiraiCode:
                        chainMsg.Add(item.ToObject<MiraiMessageTypeDetail.MiraiCode>());
                        break;

                    default:
                        break;
                }
            }
            return chainMsg;
        }

        /// <summary>
        /// 将CQ码转换为消息链对象
        /// </summary>
        /// <param name="cqCode">需要转换的CQ码对象</param>
        private static IMiraiMessageBase ParseCQCode2MiraiMessageBase(CQCode cqCode, out int quoteId)
        {
            quoteId = -1;
            switch (cqCode.Function)
            {
                case CQCodeType.Face:
                    return new MiraiMessageTypeDetail.Face { faceId = Convert.ToInt32(cqCode.Items["id"]) };

                case CQCodeType.Bface:
                    return new MiraiMessageTypeDetail.MarketFace { id = Convert.ToInt32(cqCode.Items["id"]) };

                case CQCodeType.Image:
                    string picPath = cqCode.Items["file"];
                    // 以下为两个纠错路径, 防止拼接路径时出现以下两种情况
                    // basePath + "\foo.jpg"
                    // basePath + "foo.jpg"
                    string picPathA = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image") + picPath;
                    string picPathB = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath);
                    if (File.Exists(picPathA))
                    {
                        picPath = picPathA;
                    }
                    else if (File.Exists(picPathB))
                    {
                        picPath = picPathB;
                    }
                    else
                    {
                        // 若以上两个路径均不存在, 判断对应的 cqimg 文件是否存在
                        picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\image", picPath + ".cqimg");
                        if (!File.Exists(picPath))
                        {
                            LogHelper.WriteLog(LogLevel.Warning, "发送图片", "文件不存在", "");
                            return null;
                        }
                        string picTmp = File.ReadAllText(picPath);
                        // 分离 cqimg 文件中的 url
                        picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                        return cqCode.Items.ContainsKey("flash")
                            ? new MiraiMessageTypeDetail.FlashImage { url = picTmp }
                            : new MiraiMessageTypeDetail.Image { url = picTmp };
                    }
                    // 将图片转换为 base64
                    string picBase64 = Helper.ParsePic2Base64(picPath);
                    if (string.IsNullOrEmpty(picBase64))
                    {
                        return null;
                    }
                    if (cqCode.Items.ContainsKey("flash"))
                    {
                        return new MiraiMessageTypeDetail.FlashImage { base64 = picBase64 };
                    }
                    return new MiraiMessageTypeDetail.Image { base64 = picBase64 };

                case CQCodeType.Record:
                    string recordPath = cqCode.Items["file"];
                    recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"data\record", recordPath);
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
                                return null;
                            }
                        }
                        recordPath = new FileInfo(recordPath).FullName;
                        return new MiraiMessageTypeDetail.Voice { base64 = Helper.ParsePic2Base64(recordPath) };
                    }
                    else if (File.Exists(recordPath + ".cqrecord"))
                    {
                        string recordUrl = File.ReadAllText(recordPath + ".cqrecord").Replace("[record]\nurl=", "");
                        return new MiraiMessageTypeDetail.Voice { url = recordUrl };
                    }
                    else
                    {
                        return null;
                    }
                case CQCodeType.At:
                    return new MiraiMessageTypeDetail.At { target = Convert.ToInt64(cqCode.Items["qq"]), display = "" };

                case CQCodeType.Dice:
                    return new MiraiMessageTypeDetail.Dice { value = Convert.ToInt32(cqCode.Items["point"]) };

                case CQCodeType.Music:
                    return new MiraiMessageTypeDetail.MusicShare { brief = cqCode.Items["content"], jumpUrl = cqCode.Items["url"], musicUrl = cqCode.Items["audio"], pictureUrl = cqCode.Items["imageUrl"], title = cqCode.Items["title"] };

                case CQCodeType.Rich:
                    return (object)cqCode.Items["type"] switch
                    {
                        "xml" => new MiraiMessageTypeDetail.Xml { xml = UnescapeRawMessage(cqCode.Items["content"]) },
                        "json" => new MiraiMessageTypeDetail.Json { json = UnescapeRawMessage(cqCode.Items["content"]) },
                        "app" => new MiraiMessageTypeDetail.App { content = UnescapeRawMessage(cqCode.Items["content"]) },
                        _ => null,
                    };

                case CQCodeType.Reply:
                    quoteId = Convert.ToInt32(cqCode.Items["id"]);
                    return null;

                default:
                    return null;
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