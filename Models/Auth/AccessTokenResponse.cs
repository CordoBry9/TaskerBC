
namespace TaskerBC.Models.Auth
{
    public class AccessTokenResponse
    {
        public string? TokenType { get; set; }
        public string? AccessToken { get; set; }
        public int ExpiresIn { get; set; }
        public string? RefreshToken { get; set; }

        public DateTimeOffset? ExpiresAt { get; set; }
    }
}
