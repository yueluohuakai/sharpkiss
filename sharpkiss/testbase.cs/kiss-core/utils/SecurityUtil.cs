using System;
using System.Security.Cryptography;
using System.Text;

namespace Kiss.Utils
{
    public static class SecurityUtil
    {
        public static string MD5_Hash(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] bytes_in = UTF8Encoding.Default.GetBytes(str);
            byte[] bytes_out = md5.ComputeHash(bytes_in);
            string str_out = BitConverter.ToString(bytes_out).Replace("-", "");
            return str_out;
        }
    }
}
