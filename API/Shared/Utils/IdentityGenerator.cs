using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyLife.Shared.Utils
{
    public static class IdentityGenerator
    {
        private const string UpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string LowerCase = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Special = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        /// <summary>
        /// 生成满足复杂性要求的随机登录名 [cite: 16]
        /// 长度: 6-12, 包含: 大写、小写、数字、特殊字符
        /// </summary>
        public static string GenerateSecureLoginName()
        {
            // 1. 确定长度 (6-12)
            int length = RandomNumberGenerator.GetInt32(6, 13);

            var res = new StringBuilder();

            // 2. 核心契约：确保每种字符类型至少出现一次 [cite: 22]
            res.Append(LowerCase[RandomNumberGenerator.GetInt32(LowerCase.Length)]);
            res.Append(UpperCase[RandomNumberGenerator.GetInt32(UpperCase.Length)]);
            res.Append(Digits[RandomNumberGenerator.GetInt32(Digits.Length)]);
            res.Append(Special[RandomNumberGenerator.GetInt32(Special.Length)]);

            // 3. 填充剩余长度
            string allChars = LowerCase + UpperCase + Digits + Special;
            while (res.Length < length)
            {
                res.Append(allChars[RandomNumberGenerator.GetInt32(allChars.Length)]);
            }

            // 4. 打乱顺序 (Fisher-Yates Shuffle) 增强不可预测性 [cite: 34]
            char[] array = res.ToString().ToCharArray();
            int n = array.Length;
            while (n > 1)
            {
                int k = RandomNumberGenerator.GetInt32(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }

            return new string(array);
        }
    }
}
