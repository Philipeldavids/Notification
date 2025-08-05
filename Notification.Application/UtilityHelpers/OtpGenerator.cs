using System.Security.Cryptography;

namespace Notification.Application.UtilityHelpers
{
    public static class OtpGenerator
    {
        public static string GenerateNumericOtp(int length = 6)
        {
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            return string.Concat(bytes.Select(b => (b % 10).ToString()));
        }
    }
}