using System.Security.Cryptography;
using System.Text;

namespace AqLife.Shared.Utils
{
    public static class FastHash
    {
        public static string GetSha256Hash(string rawData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = SHA256.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
        public static string GetSha384Hash(string rawData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = SHA384.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }
        public static string GetSha512Hash(string rawData)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(rawData);
            byte[] hashBytes = SHA512.HashData(bytes);
            return Convert.ToHexString(hashBytes);
        }

    }
}
