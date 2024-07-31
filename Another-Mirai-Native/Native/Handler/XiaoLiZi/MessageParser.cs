using Another_Mirai_Native.Model;
using System.Text;

namespace Another_Mirai_Native.Native.Handler.XiaoLiZi
{
    public static class MessageParser
    {
        public static string ParseFromCQCode(string message)
        {
            var splits = message.SplitV2("\\[CQ:.*?\\]");
            StringBuilder sb = new();
            foreach (var s in splits)
            {
                if (s.StartsWith("[CQ:"))
                {
                    var cqcode = CQCode.Parse(s).FirstOrDefault();
                    if (cqcode != null)
                    {
                        string? value = "";
                        switch (cqcode.Function)
                        {
                            case Model.Enums.CQCodeType.Face:
                                sb.Append($"[bq{cqcode.Items["id"]}]");
                                break;

                            case Model.Enums.CQCodeType.Image:
                                if (cqcode.Items.TryGetValue("id", out value))
                                {
                                    sb.Append($"[pic,hash={value}]");
                                }
                                else if (cqcode.Items.TryGetValue("file", out value))
                                {
                                    sb.Append($"[picFile,path={Path.Combine("data", "image", value)}]");
                                }
                                break;

                            case Model.Enums.CQCodeType.Record:
                                if (cqcode.Items.TryGetValue("id", out value))
                                {
                                    sb.Append($"[audio,hash={value}]");
                                }
                                else if (cqcode.Items.TryGetValue("file", out value))
                                {
                                    sb.Append($"[AudioFile,path={Path.Combine("data", "record", value)}]");
                                }
                                break;

                            case Model.Enums.CQCodeType.At:
                                sb.Append($"[@{cqcode.Items["qq"]}]");
                                break;

                            case Model.Enums.CQCodeType.Reply:
                                sb.Append($"[Reply,Req={cqcode.Items["id"]}]");
                                break;

                            case Model.Enums.CQCodeType.Shake:
                                sb.Append($"[Shake]");
                                break;

                            default:
                                break;
                        }
                    }
                }
                else
                {
                    sb.Append(s);
                }
            }
            return sb.ToString();
        }

        public static string ParseToCQCode(string message)
        {
            var splits = message.SplitV2("\\[.*?\\]");
            StringBuilder sb = new();
            foreach (var s in splits)
            {
                if (!s.StartsWith("["))
                {
                    sb.Append(s);
                    continue;
                }
                if (s.StartsWith("[@"))
                {
                    sb.Append(s.Replace("[@", "[CQ:at,qq="));
                    continue;
                }
                if (s.StartsWith("[bq"))
                {
                    sb.Append(s.Replace("[bq", "[CQ:face,id="));
                    continue;
                }
                var items = s.Trim().Substring(1, s.Length - 2).Split(',');
                var keyValues = GetKeyValues(items);
                if (items.Length >= 2)
                {
                    string? value = "";
                    switch (items[0])
                    {
                        case "bigFace":
                            if (keyValues.TryGetValue("Id", out value))
                            {
                                sb.Append($"[CQ:bFace,id={value}]");
                            }
                            break;

                        case "smallFace":
                            if (keyValues.TryGetValue("Id", out value))
                            {
                                sb.Append($"[CQ:sface,id={value}]");
                            }
                            break;

                        case "picFile":
                        case "pic":
                            if (keyValues.TryGetValue("path", out value))
                            {
                                value = Helper.GetRelativePath(value, Environment.CurrentDirectory);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    sb.Append($"[CQ:image,file={value}]");
                                }
                            }
                            else if (keyValues.TryGetValue("hash", out value))
                            {
                                sb.Append($"[CQ:image,id={value}]");
                            }
                            break;

                        case "AudioFile":
                            if (keyValues.TryGetValue("path", out value))
                            {
                                value = Helper.GetRelativePath(value, Environment.CurrentDirectory);
                                if (!string.IsNullOrEmpty(value))
                                {
                                    sb.Append($"[CQ:record,file={value}]");
                                }
                            }
                            else if (keyValues.TryGetValue("hash", out value))
                            {
                                sb.Append($"[CQ:record,id={value}]");
                            }
                            break;

                        case "flashPicFile":
                            if (keyValues.TryGetValue("path", out value))
                            {
                                sb.Append($"[CQ:image,file={value},flash=true]");
                            }
                            break;

                        case "Shake":
                            sb.Append($"[CQ:shake]");
                            break;

                        case "Reply":
                            if (keyValues.TryGetValue("Req", out value))
                            {
                                sb.Append($"[CQ:reply,id={value}]");
                            }
                            break;

                        default:
                            break;
                    }
                }
            }
            return sb.ToString();
        }

        private static Dictionary<string, string> GetKeyValues(string[] items)
        {
            Dictionary<string, string> r = [];
            foreach (var item in items)
            {
                if (item.Contains("="))
                {
                    var split = item.Split('=');
                    if (split.Length > 1)
                    {
                        var key = split[0];
                        r[key] = split[1];
                    }
                }
            }
            return r;
        }
    }
}