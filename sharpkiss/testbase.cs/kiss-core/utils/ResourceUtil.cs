using System.IO;
using System.Reflection;
using System.Text;

namespace Kiss.Utils
{
    /// <summary>
    /// resource util
    /// </summary>
    public static class ResourceUtil
    {
        /// <summary>
        /// load text from specified assembly
        /// </summary>
        /// <param name="asm"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadTextFromAssembly(Assembly asm, string name)
        {
            AssertUtils.ArgumentNotNull(asm, "asm");
            AssertUtils.ArgumentHasText(name, "name");

            return Encoding.UTF8.GetString(LoadBufferFromAssembly(asm, name));
        }

        /// <summary>
        /// load text from the calling assembly
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string LoadTextFromAssembly(string name)
        {
            return LoadTextFromAssembly(Assembly.GetCallingAssembly(), name);
        }

        public static byte[] LoadBufferFromAssembly(Assembly assembly, string resource)
        {
            byte[] output = null;

            using (Stream stream = assembly.GetManifestResourceStream(resource))
            {
                if (stream == null)
                    return output;

                if (stream.Length <= 3)
                {
                    output = new byte[stream.Length];
                    stream.Read(output, 0, output.Length);
                }
                else
                {
                    using (BinaryReader rdr = new BinaryReader(stream, Encoding.UTF8))
                    {
                        using (var ms = new MemoryStream())
                        {
                            byte[] bom = rdr.ReadBytes(3);

                            if (!(bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF))
                                ms.Write(bom, 0, bom.Length);

                            int readLength;
                            var buffer = new byte[4096];
                            do
                            {
                                readLength = rdr.Read(buffer, 0, buffer.Length);
                                ms.Write(buffer, 0, readLength);
                            }
                            while (readLength != 0);

                            output = ms.ToArray();
                        }
                    }
                }
            }

            return output;
        }
    }
}
