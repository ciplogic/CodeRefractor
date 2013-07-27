#region Usings

using System;
using System.IO;

#endregion

namespace Mono.Reflection
{
    public sealed class Image : IDisposable
    {
        private long position;
        private Stream stream;

        private Image(Stream stream)
        {
            this.stream = stream;
            position = stream.Position;
            this.stream.Position = 0;
        }

        private bool Advance(int length)
        {
            if (stream.Position + length >= stream.Length)
                return false;

            stream.Seek(length, SeekOrigin.Current);
            return true;
        }

        private bool MoveTo(uint position)
        {
            if (position >= stream.Length)
                return false;

            stream.Position = position;
            return true;
        }

        void IDisposable.Dispose()
        {
            stream.Position = position;
        }

        private ushort ReadUInt16()
        {
            return (ushort) (stream.ReadByte()
                             | (stream.ReadByte() << 8));
        }

        private uint ReadUInt32()
        {
            return (uint) (stream.ReadByte()
                           | (stream.ReadByte() << 8)
                           | (stream.ReadByte() << 16)
                           | (stream.ReadByte() << 24));
        }

        private bool IsManagedAssembly()
        {
            if (stream.Length < 318)
                return false;
            if (ReadUInt16() != 0x5a4d)
                return false;
            if (!Advance(58))
                return false;
            if (!MoveTo(ReadUInt32()))
                return false;
            if (ReadUInt32() != 0x00004550)
                return false;
            if (!Advance(20))
                return false;
            if (!Advance(ReadUInt16() == 0x20b ? 222 : 206))
                return false;

            return ReadUInt32() != 0;
        }

        public static bool IsAssembly(string file)
        {
            if (file == null)
                throw new ArgumentNullException("file");

            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return IsAssembly(stream);
        }

        public static bool IsAssembly(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");
            if (!stream.CanRead)
                throw new ArgumentException("Can not read from stream");
            if (!stream.CanSeek)
                throw new ArgumentException("Can not seek in stream");

            using (var image = new Image(stream))
                return image.IsManagedAssembly();
        }
    }
}