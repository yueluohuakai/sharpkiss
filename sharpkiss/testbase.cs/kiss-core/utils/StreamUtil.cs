using System.IO;

namespace Kiss.Utils
{
    public static class StreamUtil
    {
        public static byte[] ToBytes(this Stream stream)
        {
            byte[] content;

            if (stream == null)
                content = new byte[0];
            else if (stream is MemoryStream)
                content = ((MemoryStream)stream).ToArray();
            else if (stream.CanRead && stream.CanSeek)
            {
                content = new byte[stream.Length];
                stream.Position = 0;
                stream.Read(content, 0, (int)stream.Length);
            }
            else
                content = new byte[0];

            return content;
        }
    }
}
