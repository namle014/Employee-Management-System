namespace OA.Core.Models
{
    public class GoogleAppSettings
    {
        //public static GoogleAppSettings appSettings { get; set; }
        public string? JwtSecret { get; set; }
        public string? GoogleClientId { get; set; }
        public string? GoogleClientSecret { get; set; }
        public string? JwtEmailEncryption { get; set; }

    }
}
