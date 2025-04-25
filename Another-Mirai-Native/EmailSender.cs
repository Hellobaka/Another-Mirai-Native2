using Another_Mirai_Native.Config;
using Another_Mirai_Native.DB;
using System.Net;
using System.Net.Mail;

namespace Another_Mirai_Native
{
    public class EmailSender
    {
        private string SmtpHost { get; set; } = "smtp.qq.com";
     
        private ushort SmtpPort { get; set; } = 587;
     
        private string FromEmail { get; set; }
     
        private string ToEmail { get; set; }
     
        private string Username { get; set; }
     
        private string Password { get; set; }

        public EmailSender()
        {
            SmtpHost = AppConfig.Instance.OfflineActionEmail_SMTPServer;
            SmtpPort = AppConfig.Instance.OfflineActionEmail_SMTPPort;
            FromEmail = AppConfig.Instance.OfflineActionEmail_SMTPSenderEmail;
            ToEmail = AppConfig.Instance.OfflineActionEmail_SMTPReceiveEmail;
            Username = AppConfig.Instance.OfflineActionEmail_SMTPUsername;
            Password = AppConfig.Instance.OfflineActionEmail_SMTPPassport;

            if (string.IsNullOrEmpty(SmtpHost)
                || string.IsNullOrEmpty(FromEmail)
                || string.IsNullOrEmpty(Username) 
                || string.IsNullOrEmpty(Password))
            {
                LogHelper.Error("邮件发送", $"SMTP 参数缺失");
            }

            if (string.IsNullOrEmpty(ToEmail))
            {
                ToEmail = Username;
            }
        }

        public bool SendEmail(string subject, string body, bool htmlContent)
        {
            try
            {
                using var client = new SmtpClient(SmtpHost, SmtpPort);
                client.Credentials = new NetworkCredential(Username, Password);
                client.EnableSsl = true;
                var mailMessage = new MailMessage(FromEmail, ToEmail, subject, htmlContent ? HtmlBody.Replace(HtmlContentTemplate, body) : body);
               
                client.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                LogHelper.Error("邮件发送", $"发送邮件时发生异常: {ex}");
                return false;
            }
        }

        private const string HtmlContentTemplate = "{{template}}";

        private const string HtmlBody = @"<!DOCTYPE html>
<html>
<head>
    <meta charset=""UTF-8"">
    <title>Bot 离线通知</title>
    <style>
        body {
            background: #f7f9fa;
            font-family: ""Segoe UI"", Arial, Helvetica, sans-serif;
            padding: 40px 0;
        }
        .container {
            max-width: 400px;
            margin: 0 auto;
            background: #fff;
            border-radius: 16px;
            box-shadow: 0 4px 24px rgba(80, 80, 80, 0.09);
            padding: 32px 24px;
            text-align: center;
        }
        .status-icon {
            font-size: 48px;
            color: #bbbbbb;
            margin-bottom: 18px;
        }
        .title {
            font-size: 20px;
            font-weight: 600;
            color: #222e3c;
            margin-bottom: 8px;
        }
        .desc {
            font-size: 15px;
            color: #444;
            margin-top: 0;
        }
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""status-icon"">🤖</div>
        <div class=""title"">{{template}}</div>
        <p class=""desc"">您的机器人已经离线一段时间。建议登录服务器进行维护。</p>
    </div>
</body>
</html>
";
    }
}
