using Microsoft.Extensions.Options;

namespace Ala.Backend.Infrastructure.Settings;

public sealed class JwtSettingsValidator : IValidateOptions<JwtSettings>
{
    public ValidateOptionsResult Validate(string? name, JwtSettings options)
    {
        if (string.IsNullOrWhiteSpace(options.Issuer))
            return ValidateOptionsResult.Fail("JwtSettings:Issuer zorunlu.");

        if (string.IsNullOrWhiteSpace(options.Audience))
            return ValidateOptionsResult.Fail("JwtSettings:Audience zorunlu.");

        if (string.IsNullOrWhiteSpace(options.SigningKey))
            return ValidateOptionsResult.Fail("JwtSettings:SigningKey zorunlu.");

        if (!TryDecodeBase64(options.SigningKey, out var signingKeyBytes))
            return ValidateOptionsResult.Fail("JwtSettings:SigningKey geçerli bir Base64 olmalı.");

        if (signingKeyBytes.Length < 32)
            return ValidateOptionsResult.Fail("JwtSettings:SigningKey decode edilince en az 32 byte olmalı.");

        if (options.AccessTokenExpirationMinutes <= 0)
            return ValidateOptionsResult.Fail("AccessTokenExpirationMinutes 0'dan büyük olmalı.");

        if (options.RefreshTokenExpirationDays <= 0)
            return ValidateOptionsResult.Fail("RefreshTokenExpirationDays 0'dan büyük olmalı.");

        if (options.ClockSkewSeconds < 0)
            return ValidateOptionsResult.Fail("ClockSkewSeconds negatif olamaz.");

        if (options.UseEncryption)
        {
            if (string.IsNullOrWhiteSpace(options.EncryptionKey))
                return ValidateOptionsResult.Fail("UseEncryption=true ise EncryptionKey zorunlu.");

            if (!TryDecodeBase64(options.EncryptionKey, out var encryptionKeyBytes))
                return ValidateOptionsResult.Fail("EncryptionKey geçerli bir Base64 olmalı.");

            if (encryptionKeyBytes.Length != 32)
                return ValidateOptionsResult.Fail("EncryptionKey decode edilince tam 32 byte olmalı.");
        }

        return ValidateOptionsResult.Success;
    }

    private static bool TryDecodeBase64(string value, out byte[] bytes)
    {
        try
        {
            bytes = Convert.FromBase64String(value);
            return true;
        }
        catch
        {
            bytes = Array.Empty<byte>();
            return false;
        }
    }
}