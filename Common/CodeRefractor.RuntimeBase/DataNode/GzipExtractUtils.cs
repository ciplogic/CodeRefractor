#region Usings

using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

#endregion

namespace CodeRefractor.DataNode
{
    internal static class GzipExtractUtils
    {
        public static byte[] Compress(this byte[] sBytes)
        {
            var mStream = new MemoryStream(sBytes);
            var outStream = new MemoryStream();

            using (var tinyStream = new GZipStream(outStream, CompressionMode.Compress))
            {
                mStream.CopyTo(tinyStream);
            }

            return outStream.ToArray();
        }

        public static byte[] Decompress(this byte[] sBytes)
        {
            var mStream = new MemoryStream(sBytes);
            var result = new List<byte>();
            using (var csStream = new GZipStream(mStream, CompressionMode.Decompress))
            {
                var buffer = new byte[1024];
                int nRead;
                while ((nRead = csStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    if (nRead == buffer.Length)
                    {
                        result.AddRange(buffer);
                    }
                    else
                    {
                        for (var i = 0; i < nRead; i++)
                            result.Add(buffer[i]);
                    }
                }
            }

            return result.ToArray();
        }
    }
}