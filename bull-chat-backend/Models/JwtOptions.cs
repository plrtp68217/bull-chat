namespace bull_chat_backend.Models
{
    public class JwtOptions
    {
        public required string SecretKey { get; init; }
        public int ExpiredHours { get; init; }
        public required string Audience { get; init; }
        public required string Issuer { get; init; }

        public override string ToString()
        {
            return
                $"{nameof(SecretKey)}:{SecretKey}\n" +
                $"{nameof(ExpiredHours)}:{ExpiredHours}\n" +
                $"{nameof(Audience)}:{Audience}\n" +
                $"{nameof(Issuer)}:{Issuer}\n";
        }
    }
}