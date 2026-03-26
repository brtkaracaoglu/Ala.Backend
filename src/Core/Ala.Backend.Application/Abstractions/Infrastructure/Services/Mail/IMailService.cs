
using Ala.Backend.Application.Common.Responses;

namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail
{
    public interface IMailService
    {
        Task<SuccessDetails> SendMailAsync(string to, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendPasswordResetMailAsync(string to, int userId, string fullName, string resetToken, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendEmailConfirmationMailAsync(string to, string fullName, string confirmationToken, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendInitialPasswordMailAsync(string to, string fullName, string userName, string password, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendResendConfirmationMailAsync(string to, string userName, string confirmationToken, CancellationToken cancellationToken = default);
        Task<SuccessDetails> SendAdminAlertAsync(string subject, string errorDetail, CancellationToken cancellationToken = default);
    }
}