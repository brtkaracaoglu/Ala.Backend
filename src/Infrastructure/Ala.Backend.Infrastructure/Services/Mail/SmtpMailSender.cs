using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Utils;

namespace Ala.Backend.Infrastructure.Services.Mail
{
    public class SmtpMailSender : IMailSender
    {
        private readonly MailSettings _settings;
        private readonly ILogger<SmtpMailSender> _logger;
        private readonly IWebHostEnvironment _env;

        public SmtpMailSender(IOptions<MailSettings> options, ILogger<SmtpMailSender> logger, IWebHostEnvironment env)
        {
            _settings = options.Value;
            _logger = logger;
            _env = env;
        }

        public async Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default)
        {
            var mail = new MimeMessage();
            mail.From.Add(new MailboxAddress(_settings.DisplayName, _settings.Username));

            foreach (var to in tos.Distinct())
            {
                mail.To.Add(MailboxAddress.Parse(to));
            }

            mail.Subject = subject;

            if (isBodyHtml)
                mail.Body = CreateBodyWithEmbeddedImage(body);
            else
                mail.Body = new TextPart("plain") { Text = body };

            try
            {
                using var smtp = new SmtpClient();
                smtp.Timeout = 10000;

                _logger.LogInformation(
                    "SMTP bağlantısı kuruluyor. Host:{Host} Port:{Port}",
                    _settings.Host,
                    _settings.Port);

                var secureOption = _settings.EnableSsl
                    ? SecureSocketOptions.StartTls
                    : SecureSocketOptions.Auto;

                if (_settings.IgnoreSslErrors || (_env?.IsDevelopment() ?? false))
                {
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                }

                await smtp.ConnectAsync(_settings.Host, _settings.Port, secureOption, cancellationToken);
                await smtp.AuthenticateAsync(_settings.Username, _settings.Password, cancellationToken);
                await smtp.SendAsync(mail, cancellationToken);
                await smtp.DisconnectAsync(true, cancellationToken);

                _logger.LogInformation(
                    "Mail başarıyla gönderildi. Subject:{Subject} Tos:{Tos}",
                    subject,
                    string.Join(",", tos));
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "SMTP mail gönderimi başarısız. Host:{Host} Port:{Port} User:{User}",
                    _settings.Host,
                    _settings.Port,
                    _settings.Username);

                // Eğer IntegrationException iki parametre almıyorsa, kendi exception sınıfınızı aşağıdaki gibi standartlaştırabilirsiniz
                throw new Exception("hata", ex);
            }
        }

        private MimeEntity CreateBodyWithEmbeddedImage(string htmlBody)
        {
            var builder = new BodyBuilder();

            if (htmlBody.Contains("{{Logo}}"))
            {
                // _env null ise doğrudan root klasörüne bakması için güvenlik eklendi
                var webRootPath = _env?.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var logoPath = Path.Combine(webRootPath, "images", "europower.png");

                if (File.Exists(logoPath))
                {
                    var logoImage = builder.LinkedResources.Add(logoPath);
                    logoImage.ContentId = MimeUtils.GenerateMessageId();
                    htmlBody = htmlBody.Replace("{{Logo}}", $"cid:{logoImage.ContentId}");
                }
                else
                {
                    // Sadece log atıyoruz ve etiketi temizliyoruz ki mailde "{{Logo}}" yazısı görünmesin
                    _logger.LogWarning("Logo dosyası sunucuda bulunamadı. Aranan Yol: {LogoPath}", logoPath);
                    htmlBody = htmlBody.Replace("{{Logo}}", string.Empty);
                }
            }

            builder.HtmlBody = htmlBody;
            return builder.ToMessageBody();
        }
    }
}