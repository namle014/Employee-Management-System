using Newtonsoft.Json;

namespace OA.Core.Models
{
    public class FacebookUserData
    {
        public long Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        [JsonProperty("first_name")]
        public string FirstName { get; set; } = string.Empty;
        [JsonProperty("last_name")]
        public string LastName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Locale { get; set; } = string.Empty;
        public FacebookPictureData? Picture { get; set; }
    }
    public class FacebookPictureData
    {
        public FacebookPicture? Data { get; set; }
    }
    public class FacebookPicture
    {
        public int Height { get; set; }
        public int Width { get; set; }
        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }
        public string Url { get; set; } = string.Empty;
    }
    public class FacebookUserAccessTokenData
    {
        [JsonProperty("app_id")]
        public long AppId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Application { get; set; } = string.Empty;
        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; }
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
        [JsonProperty("user_id")]
        public long UserId { get; set; }
    }
    public class FacebookUserAccessTokenValidation
    {
        public FacebookUserAccessTokenData? Data { get; set; }
    }
    public class FacebookAppAccessToken
    {
        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;
    }
}
