using Microsoft.IdentityModel.Tokens;
namespace OA.Core.Configurations
{
    public class ConnectionStrings
    {
        public string? DefaultConnection { get; set; }
    }
    public class UploadConfigurations
    {
        public string TempFolder { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public int ChunkSize { get; set; }
    }
    public class FacebookAuthSettings
    {
        public string? AppId { get; set; }
        public string? AppSecret { get; set; }
    }
    public class JwtIssuerOptions
    {
        /// <summary>
        /// 4.1.1.  "iss" (Issuer) Claim - The "iss" (issuer) claim identifies the principal that issued the JWT.
        /// </summary>
        public string? Issuer { get; set; }
        /// <summary>
        /// 4.1.2.  "sub" (Subject) Claim - The "sub" (subject) claim identifies the principal that is the subject of the JWT.
        /// </summary>
        public string? Subject { get; set; }
        /// <summary>
        /// 4.1.3.  "aud" (Audience) Claim - The "aud" (audience) claim identifies the recipients that the JWT is intended for.
        /// </summary>
        public string? Audience { get; set; }
        /// <summary>
        /// Url Client Admin
        /// </summary>
        public string? UrlClientAdmin { get; set; }
        /// <summary>
        /// Url Client EndUser
        /// </summary>
        public string? UrlClientEndUser { get; set; }
        /// <summary>
        /// 4.1.4.  "exp" (Expiration Time) Claim - The "exp" (expiration time) claim identifies the expiration time on or after which the JWT MUST NOT be accepted for processing.
        /// </summary>
        public DateTime Expiration => IssuedAt.Add(ValidFor);
        /// <summary>
        /// 4.1.5.  "nbf" (Not Before) Claim - The "nbf" (not before) claim identifies the time before which the JWT MUST NOT be accepted for processing.
        /// </summary>
        public DateTime NotBefore => DateTime.UtcNow;
        /// <summary>
        /// 4.1.6.  "iat" (Issued At) Claim - The "iat" (issued at) claim identifies the time at which the JWT was issued.
        /// </summary>
        public DateTime IssuedAt => DateTime.UtcNow;
        /// <summary>
        /// Set the timespan the token will be valid for (default is 120 min)
        /// </summary>
        public TimeSpan ValidFor { get; set; } = TimeSpan.FromMinutes(60);
        /// <summary>
        /// "jti" (JWT ID) Claim (default ID is a GUID)
        /// </summary>
        public Func<Task<string>> JtiGenerator =>
          () => Task.FromResult(Guid.NewGuid().ToString());
        /// <summary>
        /// The signing key to use when generating tokens.
        /// </summary>
        public SigningCredentials? SigningCredentials { get; set; }
    }
    public class SMSoptions
    {
        public string? SMSAccountIdentification { get; set; }
        public string? SMSAccountPassword { get; set; }
        public string? SMSAccountFrom { get; set; }
    }
    public class Urls
    {
        public string? urls { get; set; }
    }
}
