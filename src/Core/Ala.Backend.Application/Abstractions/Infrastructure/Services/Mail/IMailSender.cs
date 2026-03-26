
namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail
{
    public interface IMailSender
    {
        Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default);

    }
}
