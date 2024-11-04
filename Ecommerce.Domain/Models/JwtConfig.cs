namespace Ecommerce.Domain.Models
{
    public class JwtConfig
    {
        public string Secret { get; set; }
        public int ExpirationMinutes { get; set; }
    }
}
