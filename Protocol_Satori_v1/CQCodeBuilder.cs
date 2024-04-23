using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using Another_Mirai_Native.Model.Enums;
using Another_Mirai_Native.Native;
using HtmlAgilityPack;
using System.Text.RegularExpressions;

namespace Another_Mirai_Native.Protocol.Satori
{
    public class CQCodeBuilder
    {
        public static string RawParseToCQCode(string content, string platform)
        {
            return platform switch
            {
                "chronocat" => RawParseToCQCode_Chronocat(content),
                _ => string.Empty,
            };
        }

        public static string CQCodeParseToRaw(string content, string platform)
        {
            return platform switch
            {
                "chronocat" => CQCodeParseToRaw_Chronocat(content),
                _ => string.Empty,
            };
        }

        private static string CQCodeParseToRaw_Chronocat(string content)
        {
            Regex regex = new("(\\[CQ:.*?,.*?\\])");
            var cqCodeCaptures = regex.Matches(content).Cast<Match>().Select(m => m.Value).ToList();
            var ls = CQCode.Parse(content);

            var s = regex.Split(content).ToList();
            s.RemoveAll(string.IsNullOrEmpty);
            string result = "";
            foreach (var item in s)
            {
                if (cqCodeCaptures.Contains(item))
                {
                    var cqcode = CQCode.Parse(item).FirstOrDefault();
                    if (cqcode == null)
                    {
                        continue;
                    }

                    if (cqcode.Function == CQCodeType.Image)
                    {
                        string picPath = cqcode.Items["file"];

                        string picPathCombined = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image", picPath);
                        if (File.Exists(picPathCombined))
                        {
                            picPath = picPathCombined;
                        }
                        else
                        {
                            // 判断对应的 cqimg 文件是否存在
                            picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image", picPath + ".cqimg");
                            if (!File.Exists(picPath))
                            {
                                LogHelper.WriteLog(LogLevel.Warning, "发送图片", "文件不存在", "");
                                continue;
                            }
                            string picTmp = File.ReadAllText(picPath);
                            // 分离 cqimg 文件中的 url
                            picTmp = picTmp.Split('\n').Last().Replace("url=", "");
                            result += $"<img src=\"{picTmp}\"/>";
                            continue;
                        }

                        // 将图片转换为 base64
                        string picBase64 = Helper.ParsePic2Base64(picPath);
                        if (string.IsNullOrEmpty(picBase64))
                        {
                            continue;
                        }
                        result += $"<img src=\"data:image/{Path.GetExtension(picPath).Replace(".", "")};base64,{picBase64}\"/>";
                    }
                    else if(cqcode.Function == CQCodeType.At)
                    {
                        result += $"<at id=\"{cqcode.Items["qq"]}\">";
                    }
                    else if(cqcode.Function == CQCodeType.Face)
                    {
                        result += $"<chronocat:face id=\"{cqcode.Items["id"]}\">";
                    }
                    else if(cqcode.Function == CQCodeType.Record)
                    {
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
                                    continue;
                                }
                            }
                            recordPath = new FileInfo(recordPath).FullName;
                            result += $"<img src=\"data:audio;base64,{Helper.ParsePic2Base64(recordPath)}\"/>";
                        }
                        else if (File.Exists(recordPath + ".cqrecord"))
                        {
                            string recordUrl = File.ReadAllText(recordPath + ".cqrecord").Replace("[record]\nurl=", "");
                            result += $"<img src=\"{recordUrl}\"/>";
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                else
                {
                    result += item;
                }
            }
            return result;
        }

        private static string RawParseToCQCode_Chronocat(string content)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            string result = "";
            foreach (var item in doc.DocumentNode.ChildNodes)
            {
                if (item.Name == "#text")
                {
                    result += item.InnerText;
                }
                else
                {
                    string name = item.Name.Replace("#", "").Split(':').Last();
                    string url, hash;
                    switch (name)
                    {
                        case "face":
                            string id = item.Attributes["id"].Value;
                            if (id == "358") // Dice
                            {
                                result += $"[CQ:point,point={item.Attributes["unsafe-result-id"].Value}]";
                            }
                            else if (id == "359")
                            {
                                result += $"[CQ:face,id={id}]";
                            }
                            else
                            {
                                result += $"[CQ:face,id={id}]";
                            }
                            break;

                        case "img":
                            url = item.Attributes["src"].Value;
                            hash = url.MD5();
                            Directory.CreateDirectory("data\\image");
                            File.WriteAllText($"data\\image\\{hash}.cqimg", $"[image]\nmd5=0\nsize=0\nurl={url}");

                            result += $"[CQ:image,file={hash}]";
                            break;

                        case "audio":
                            url = item.Attributes["src"].Value;
                            hash = url.MD5();
                            Directory.CreateDirectory("data\\record");
                            File.WriteAllText($"data\\record\\{hash}.cqrecord", $"[record]\nmd5=0\nsize=0\nurl={url}");

                            result += $"[CQ:record,file={hash}]";
                            break;

                        case "at":
                            result += $"[CQ:at,qq={item.Attributes["id"].Value}]";
                            break;

                        default:
                            break;
                    }
                }
            }
            return result;
        }
    }
}