using Ala.Backend.Application.Abstractions.Infrastructure.Services.Mail;
using Ala.Backend.Application.Common.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ala.Backend.Infrastructure.Services.Mail
{
    public class TemplateService : ITemplateService
    {
        private readonly ILogger<TemplateService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IMemoryCache _cache;

        public TemplateService(ILogger<TemplateService> logger, IWebHostEnvironment env, IMemoryCache cache)
        {
            _logger = logger;
            _env = env;
            _cache = cache;
        }

        public async Task<string> RenderAsync(string templateName, Dictionary<string, string> replacements, CancellationToken cancellationToken = default)
        {
            var commonReplacements = GetCommonReplacements();
            foreach (var item in commonReplacements)
            {
                replacements.TryAdd(item.Key, item.Value);
            }

            replacements.Remove("Logo");

            string? layoutHtml = await ReadFileAsync("layout.html", cancellationToken);
            if (string.IsNullOrEmpty(layoutHtml))
            {
                _logger.LogError("Ana mail şablonu (layout.html) bulunamadı.");
                throw new InternalServerException("Mail şablonu sunucuda bulunamadı.");
            }

            string? contentHtml = await ReadFileAsync($"{templateName}.html", cancellationToken);
            if (string.IsNullOrEmpty(contentHtml))
            {
                _logger.LogError("Mail içerik şablonu bulunamadı: {Name}", templateName);
                throw new InternalServerException("Mail içerik şablonu bulunamadı.");
            }

            string fullHtml = layoutHtml.Replace("{{BODY_CONTENT}}", contentHtml);
            fullHtml = fullHtml.Replace("{{CurrentYear}}", DateTime.Now.Year.ToString());

            foreach (var item in replacements)
            {
                fullHtml = fullHtml.Replace($"{{{{{item.Key}}}}}", item.Value);
            }

            return fullHtml;
        }

        private async Task<string?> ReadFileAsync(string fileName, CancellationToken cancellationToken)
        {
            return await _cache.GetOrCreateAsync<string?>($"EmailTemplate_{fileName}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(12);

                string? path = GetTemplatePath(fileName);
                if (path == null) return null;

                return await File.ReadAllTextAsync(path, cancellationToken);
            });
        }

        private string? GetTemplatePath(string fileName)
        {
            var rootPath = _env?.WebRootPath ?? Path.Combine(_env?.ContentRootPath ?? AppDomain.CurrentDomain.BaseDirectory, "wwwroot");

            string wwwrootTemplatePath = Path.Combine(rootPath, "EmailTemplates", fileName);
            string buildTemplatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailTemplates", fileName);

            if (File.Exists(wwwrootTemplatePath)) return wwwrootTemplatePath;
            if (File.Exists(buildTemplatePath)) return buildTemplatePath;

            return null;
        }

        private Dictionary<string, string> GetCommonReplacements()
        {
            return new Dictionary<string, string>
            {
                { "Mail_Greeting", "Merhaba," },
                { "Mail_Body_FallbackInfo", "Eğer butona tıklayamıyorsanız, aşağıdaki bağlantıyı kopyalayıp tarayıcınızın adres çubuğuna yapıştırabilirsiniz:" },
                { "Mail_General_LogoAltName", "Şirket Logosu" },
                { "Footer_InfoOne", "Bu e-posta otomatik olarak gönderilmiştir, lütfen cevaplamayınız." },
                { "Footer_InfoTwo", "Bizi tercih ettiğiniz için teşekkür ederiz." },
                { "Footer_InfoThree", "© Tüm hakları saklıdır." }
            };
        }
    }
}