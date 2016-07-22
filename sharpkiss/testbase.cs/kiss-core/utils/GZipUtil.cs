using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace Kiss.Utils
{
    public static class GZipUtil
    {
        /// <summary>
        /// Takes a binary input buffer and GZip encodes the input
        /// </summary>
        public static byte[] GZipMemory(byte[] buffer)
        {
            byte[] result;
            using (MemoryStream ms = new MemoryStream())
            {
                using (GZipStream GZip = new GZipStream(ms, CompressionMode.Compress))
                {
                    GZip.Write(buffer, 0, buffer.Length);
                }

                result = ms.ToArray();
            }

            return result;
        }

        /// <summary>
        /// Takes a string input and GZip encodes the input
        /// </summary>
        /// <param name="Input"></param>
        /// <returns></returns>
        public static byte[] GZipMemory(string Input)
        {
            return GZipMemory(Encoding.UTF8.GetBytes(Input));
        }

        public static byte[] GZipMemory(string Input, Encoding encoding)
        {
            return GZipMemory(encoding.GetBytes(Input));
        }
    }
}
