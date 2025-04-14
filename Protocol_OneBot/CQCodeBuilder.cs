using Another_Mirai_Native.DB;
using Another_Mirai_Native.Model;
using System.Text;

namespace Another_Mirai_Native.Protocol.OneBot
{
    public partial class OneBotAPI
    {
        public string RebuildMessage(string parsedMessage)
        {
            var splits = parsedMessage.SplitV2("\\[CQ:.*?\\]");
            StringBuilder stringBuilder = new();
            foreach (var s in splits)
            {
                if (!s.StartsWith("[CQ:"))
                {
                    stringBuilder.Append(s);
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
                        if (int.TryParse(cqcode.Items["id"], out int faceId))
                        {
                            stringBuilder.Append($"[CQ:face,id={faceId}]");
                        }
                        else
                        {
                            LogHelper.Error("构建消息", $"表情ID无法转换为有效数字：{cqcode.Items["id"]}");
                        }
                        break;

                    case Model.Enums.CQCodeType.Image:
                        (string imageFile, string imageSubType) = HandleFileDownload(cqcode);

                        // 转换完毕
                        if (string.IsNullOrEmpty(imageFile))
                        {
                            LogHelper.Error("构建消息", $"图片File字段转换失败：{cqcode.Items["file"]}");
                            break;
                        }

                        // 检查图片是否下载成功
                        string picPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "image", cqcode.Items["file"]);
                        if (!File.Exists(picPath) && File.Exists(picPath + ".cqimg"))
                        {
                            LogHelper.Error("构建消息", $"从缓存下载图片失败：{picPath}");
                            break;
                        }

                        if (string.IsNullOrEmpty(imageSubType))
                        {
                            // 直接使用file字段
                            stringBuilder.Append($"[CQ:image,file={imageFile}]");
                        }
                        else
                        {
                            // 使用subType字段
                            stringBuilder.Append($"[CQ:image,file={imageFile},sub_type={imageSubType}]");
                        }
                        break;

                    case Model.Enums.CQCodeType.Record:
                        (string recordFile, _) = HandleFileDownload(cqcode);
                        // 转换完毕
                        if (string.IsNullOrEmpty(recordFile))
                        {
                            LogHelper.Error("构建消息", $"音频File字段转换失败：{cqcode.Items["file"]}");
                            break;
                        }

                        // 检查是否下载成功
                        string recordPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "record", recordFile);

                        if (!File.Exists(recordPath) && File.Exists(recordPath + ".cqimg"))
                        {
                            LogHelper.Error("构建消息", $"下载音频失败：{recordPath}");
                            break;
                        }

                        stringBuilder.Append($"[CQ:record,file={recordFile}]");
                        break;

                    case Model.Enums.CQCodeType.At:
                        if (long.TryParse(cqcode.Items["qq"], out long qq))
                        {
                            stringBuilder.Append($"[CQ:at,qq={qq}]");
                        }
                        break;

                    case Model.Enums.CQCodeType.Reply:
                        if (int.TryParse(cqcode.Items["id"], out int id))
                        {
                            stringBuilder.Append($"[CQ:reply,id={id}]");
                        }
                        break;

                    default:
                        stringBuilder.Append(s);
                        break;
                }
            }
            return stringBuilder.ToString();
        }

        private (string file, string subType) HandleFileDownload(CQCode cqcode)
        {
            string imageFile = cqcode.Items["file"];
            string url = cqcode.Items.TryGetValue("url", out string? u) ? u : string.Empty;
            string folderName = cqcode.Function.ToString().ToLower();

            if (imageFile.StartsWith("http") || !string.IsNullOrEmpty(url))
            {
                return HandleHttpFile(cqcode, folderName);
            }
            else if (imageFile.StartsWith("base64://"))
            {
                return HandleBase64File(cqcode, folderName);
            }
            else if (imageFile.StartsWith("file:///"))
            {
                return HandleLocalFile(cqcode, folderName);
            }
            else
            {
                return HandleHashFile(cqcode);
            }
        }

        private (string file, string subType) HandleHashFile(CQCode cqcode)
        {
            // file字段为hash值，应当直接返回
            string file = cqcode.Items["file"];
            string subType = cqcode.Items.TryGetValue("sub_type", out string? s) ? s : string.Empty;

            return (Path.GetFileNameWithoutExtension(file), subType);
        }

        private (string file, string subType) HandleLocalFile(CQCode cqcode, string folderName)
        {
            // file字段为本地文件路径，应当复制文件到图片目录
            string file = cqcode.Items["file"].Replace("file://", "");
            string subType = cqcode.Items.TryGetValue("sub_type", out string? s) ? s : string.Empty;
            string fileName = Path.GetFileName(file);
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", folderName, fileName);
            if (File.Exists(filePath))
            {
                return (fileName, subType);
            }
            else
            {
                // 复制文件到图片目录
                try
                {
                    File.Copy(file, filePath, true);
                    return (fileName, subType);
                }
                catch (Exception ex)
                {
                    LogHelper.Error("构建消息", $"复制文件失败：{ex.Message}");
                    return ("", "");
                }
            }
        }

        private (string file, string subType) HandleBase64File(CQCode cqcode, string folderName)
        {
            // file字段为base64编码的图片，应当解码并保存到图片目录
            string base64 = cqcode.Items["file"].Replace("base64://", "");
            string subType = cqcode.Items.TryGetValue("sub_type", out string? s) ? s : string.Empty;
            byte[] imageBytes = Convert.FromBase64String(base64);
            string fileName = $"{base64.MD5()}.jpg";
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", folderName, fileName);
            File.WriteAllBytes(filePath, imageBytes);
            return (fileName, subType);
        }

        private (string file, string subType) HandleHttpFile(CQCode cqcode, string folderName)
        {
            // file字段为http链接，应当写入cqimg文件
            string u = cqcode.Items.TryGetValue("url", out string? o) ? o : string.Empty;
            string file = cqcode.Items["file"];
            string url = string.IsNullOrEmpty(u) ? file : u;

            string subType = cqcode.Items.TryGetValue("sub_type", out string? s) ? s : string.Empty;
            string hash = url.MD5();
            string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", folderName, Path.ChangeExtension(hash, ".cqimg"));
            if (File.Exists(filePath))
            {
                return (hash, subType);
            }
            else
            {
                File.WriteAllText(filePath, $"[{folderName}]\nmd5=0\nsize=0\nurl={url}");
                return (hash, subType);
            }
        }

        private string UnescapeRawMessage(string? msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return "";
            }

            return msg!.Replace("&#91;", "[").Replace("&#93;", "]").Replace("&#44;", ",").Replace("&amp;", "&");
        }

        private string EscapeRawMessage(string? msg)
        {
            if (string.IsNullOrEmpty(msg))
            {
                return "";
            }

            return msg!.Replace("&", "&amp;").Replace("[", "&#91;").Replace("]", "&#93;").Replace(",", "&#44;");
        }
    }
}
