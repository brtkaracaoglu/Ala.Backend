namespace Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail
{
    public interface ITemplateService
    {
        Task<string> RenderAsync(string templateName, Dictionary<string, string> replacements, CancellationToken cancellationToken = default);
    }
}
