using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace RealWord.Utils.Utils
{
    public static class Hash
    {
        public static string GetHash(this string source)
        {
            SHA1 sha1Hash = SHA1.Create();
            byte[] sourceBytes = Encoding.UTF8.GetBytes(source);
            byte[] hashBytes = sha1Hash.ComputeHash(sourceBytes);
            string hash = BitConverter.ToString(hashBytes).Replace("-", String.Empty);

            return hash;
        }
    }
}
