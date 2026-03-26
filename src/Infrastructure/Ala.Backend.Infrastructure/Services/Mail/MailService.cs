using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Common.Responses;
using Ala.Backend.Infrastructure.Settings;
using Microsoft.Extensions.Options;

namespace Ala.Backend.Infrastructure.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly IMailSender _mailSender;
        private readonly ITemplateService _templateService;
        private readonly MailSettings _settings;

        public MailService(
            IMailSender mailSender,
            ITemplateService templateService,
            IOptions<MailSettings> options)
        {
            _mailSender = mailSender;
            _templateService = templateService;
            _settings = options.Value;
        }

        public async Task<SuccessDetails> SendMailAsync(string to, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default)
        {
            return await SendMailAsync(new[] { to }, subject, body, isBodyHtml, cancellationToken);
        }

        public async Task<SuccessDetails> SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default)
        {
            await _mailSender.SendEmailAsync(tos, subject, body, isBodyHtml, cancellationToken);
            return ResultResponse.Success("E-posta başarıyla gönderildi.");
        }

        public async Task<SuccessDetails> SendPasswordResetMailAsync(string to, int userId, string fullName, string resetToken, CancellationToken cancellationToken = default)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Mail_Title_PasswordReset", "Şifre Sıfırlama" },
                { "Mail_Body_PasswordReset_Info", "Şifrenizi sıfırlamak için aşağıdaki butona tıklayabilirsiniz." },
                { "Mail_Button_ResetPassword", "Şifremi Sıfırla" },
                { "UserName", fullName },
                { "Url", $"{_settings.ClientUrl}/auth/reset-password?uid={userId}&token={resetToken}" }
            };

            string subject = "Şifre Sıfırlama Talebi";
            string body = await _templateService.RenderAsync("BodyContent/password-reset", replacements, cancellationToken);

            return await SendMailAsync(to, subject, body, true, cancellationToken);
        }

        public async Task<SuccessDetails> SendEmailConfirmationMailAsync(string to, string fullName, string confirmationToken, CancellationToken cancellationToken = default)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Mail_Title_EmailConfirmation", "E-posta Doğrulama" },
                { "Mail_Body_EmailConfirmation_Info", "Hesabınızı aktifleştirmek için lütfen e-posta adresinizi doğrulayın." },
                { "Mail_Button_EmailConfirmation", "E-postayı Doğrula" },
                { "FullName", fullName },
                { "Url", $"{_settings.ClientUrl}/auth/confirm-email?token={confirmationToken}" }
            };

            string subject = "E-posta Adresinizi Doğrulayın";
            string body = await _templateService.RenderAsync("BodyContent/email-confirmation", replacements, cancellationToken);

            return await SendMailAsync(to, subject, body, true, cancellationToken);
        }

        public async Task<SuccessDetails> SendInitialPasswordMailAsync(string to, string fullName, string userName, string password, CancellationToken cancellationToken = default)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Mail_Title_InitialPassword", "Hesabınız Oluşturuldu" },
                { "Mail_Body_InitialPassword_Info", "Sisteme giriş yapabilmeniz için geçici şifreniz oluşturulmuştur." },
                { "Mail_Label_Username", "Kullanıcı Adı" },
                { "Mail_Label_TemporaryPassword", "Geçici Şifre" },
                { "Mail_Button_InitialPassword", "Giriş Yap" },
                { "FullName", fullName },
                { "UserName", userName },
                { "Password", password },
                { "LoginUrl", $"{_settings.ClientUrl}/login" }
            };

            string subject = "Hesap Bilgileriniz ve Geçici Şifreniz";
            string body = await _templateService.RenderAsync("BodyContent/initial-password", replacements, cancellationToken);

            return await SendMailAsync(to, subject, body, true, cancellationToken);
        }

        public async Task<SuccessDetails> SendResendConfirmationMailAsync(string to, string fullName, string confirmationToken, CancellationToken cancellationToken = default)
        {
            var replacements = new Dictionary<string, string>
            {
                { "Mail_Title_ResendConfirmation", "E-posta Doğrulama (Tekrar)" },
                { "Mail_Body_ResendConfirmation_Info", "E-posta doğrulama linkiniz yenilenmiştir. Aşağıdaki butona tıklayarak işleminizi tamamlayabilirsiniz." },
                { "Mail_Button_ResendConfirmation", "E-postayı Doğrula" },
                { "FullName", fullName },
                { "Url", $"{_settings.ClientUrl}/auth/confirm-email?token={confirmationToken}&email={to}" }
            };

            string subject = "Yeni E-posta Doğrulama Linkiniz";
            string body = await _templateService.RenderAsync("BodyContent/resend-confirmation", replacements, cancellationToken);

            return await SendMailAsync(to, subject, body, true, cancellationToken);
        }

        public async Task<SuccessDetails> SendAdminAlertAsync(string subject, string errorDetail, CancellationToken cancellationToken = default)
        {
            var to = !string.IsNullOrEmpty(_settings.AdminEmail) ? _settings.AdminEmail : "brtkaracaoglu@gmail.com";

            string labelTitle = "Sistem Uyarı Bildirimi";

            var replacements = new Dictionary<string, string>
            {
                { "MailTitle", labelTitle },
                { "Subject", subject },
                { "Date", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss") },
                { "ErrorDetail", errorDetail },
                { "Label_Subject", "Konu" },
                { "Label_Date", "Tarih" },
                { "Label_ErrorDetail", "Hata Detayı" },
                { "InfoMessage", "Bu mesaj sistem tarafından otomatik olarak oluşturulmuştur." }
            };

            string body = await _templateService.RenderAsync("BodyContent/admin-alert", replacements, cancellationToken);

            return await SendMailAsync(new[] { to }, $"🚨 {labelTitle}: {subject}", body, true, cancellationToken);
        }
    }
}
