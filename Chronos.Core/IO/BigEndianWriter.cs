using System;
using System.Collections;
using System.IO;
using System.Text;

namespace Chronos.Core.IO
{
    public class BigEndianWriter : IDisposable, IDataWriter
    {
        private BinaryWriter m_writer;
        public Stream BaseStream
        {
            get
            {
                return this.m_writer.BaseStream;
            }
        }
        public long BytesAvailable
        {
            get
            {
                return this.m_writer.BaseStream.Length - this.m_writer.BaseStream.Position;
            }
        }
        public long Position
        {
            get
            {
                return this.m_writer.BaseStream.Position;
            }
            set
            {
                this.m_writer.BaseStream.Position = value;
            }
        }
        public byte[] Data
        {
            get
            {
                long position = this.m_writer.BaseStream.Position;
                byte[] array = new byte[this.m_writer.BaseStream.Length];
                this.m_writer.BaseStream.Position = 0L;
                this.m_writer.BaseStream.Read(array, 0, (int)this.m_writer.BaseStream.Length);
                this.m_writer.BaseStream.Position = position;
                return array;
            }
        }

        public BigEndianWriter()
        {
            this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public BigEndianWriter(Stream stream)
        {
            this.m_writer = new BinaryWriter(stream, Encoding.UTF8);
        }

        private void WriteBigEndianBytes(byte[] endianBytes)
        {
            for (int i = endianBytes.Length - 1; i >= 0; i--)
            {
                this.m_writer.Write(endianBytes[i]);
            }
        }

        public void WriteShort(short @short)
        {
            m_writer.Write(@short);
        }

        public void WriteInt(int @int)
        {
            m_writer.Write(@int);
        }

        public void WriteLong(long @long)
        {
            m_writer.Write(@long);
        }

        public void WriteUShort(ushort @ushort)
        {
            m_writer.Write(@ushort);
        }

        public void WriteUInt(uint @uint)
        {
            m_writer.Write(@uint);
        }

        public void WriteULong(ulong @ulong)
        {
            m_writer.Write(@ulong);
        }

        public void WriteByte(byte @byte)
        {
            this.m_writer.Write(@byte);
        }

        public void WriteSByte(sbyte @byte)
        {
            this.m_writer.Write(@byte);
        }

        public void WriteFloat(float @float)
        {
            this.m_writer.Write(@float);
        }

        public void WriteBoolean(bool @bool)
        {
            m_writer.Write(@bool);
        }

        public void WriteChar(char @char)
        {
            m_writer.Write(@char);
        }

        public void WriteDouble(double @double)
        {
            m_writer.Write(@double);
        }

        public void WriteSingle(float single)
        {
            m_writer.Write(single);
        }

        public void WriteUTF(string str)
        {
            m_writer.Write((UInt16)str.Length);
            //if (str.Length > 0)
                m_writer.Write(Encoding.ASCII.GetBytes(str));
            //m_writer.Write(str);
            //byte[] bytes = Encoding.UTF8.GetBytes(str);
            //ushort num = (ushort)bytes.Length;
            //this.WriteUShort(num);
            //if (num > 0)
            //    m_writer.Write(bytes);
        }

        public void WriteUTFBytes(string str)
        {
            if (str.Length > 0)
                m_writer.Write(Encoding.ASCII.GetBytes(str));
            //byte[] bytes = Encoding.UTF8.GetBytes(str);
            //int num = bytes.Length;
            //for (int i = 0; i < num; i++)
            //{
            //    this.m_writer.Write(bytes[i]);
            //}
        }

        public void WriteBytes(byte[] data)
        {
            this.m_writer.Write(data);
        }
        //public void WriteX(int value)
        //{
        //    if (value == 0)
        //    {
        //        WriteUShort(1);

        //        return;
        //    }

        //    BitArray bits;

        //    var temp = new BitArray(new[] { value });

        //    temp = BitWriter.TrimZero(temp, false);

        //    if (temp.Count <= 13)
        //    {
        //        bits = BitWriter.TrimZero(BitWriter.AddFront(temp, 2), true);

        //        bits[0] = true;
        //    }
        //    else
        //    {
        //        bits = BitWriter.TrimZero(BitWriter.AddFront(temp, 8), true);

        //        bits[1] = true;
        //    }

        //    var bytes = BitWriter.GetBytes(BitWriter.TrimZero(bits, true));

        //    if (bytes.Length == 4)
        //        bytes = BitWriter.GetBytes(BitWriter.AddBack(BitWriter.TrimZero(bits, true), 8));

        //    WriteBytes(bytes);
        //}
        public void WriteLittleX(int value)
        {
            if ((value & 0x80000000) != 0L )
                value = -value;
            if ((value & 0xFFFFFFE0) != 0)
            {
                if ((value & 0xFFFFE000) != 0)
                {
                    WriteByte(2);
                    WriteUInt((uint)value);
                }
                else
                {
                    WriteShort((short)(4 * value | 1));
                }
            }
            else
            {
                WriteByte((byte)(4 * value));
            }
        }
        public void WriteX(int value)
        {
            if ((value & 0xFFFFFFC0) != 0x0)
            {
                if ((value & 0xFFFFC000) != 0x0)
                {
                    WriteByte(0x2);
                    WriteUInt((uint)value);
                }
                else
                {
                    WriteShort((short)(0x4 * value | 0x1));
                }
            }
            else
            {
                WriteByte((byte)(0x4 * value));
            }
        }

        public void WriteFormat(string format, params object[] values)
        {
            var formatArray = format.ToLowerInvariant().ToCharArray();

            if (formatArray.Length != values.Length)
                throw new ArgumentException("Format length doesn't match the number of values.");

            for (var i = 0; i < formatArray.Length; i++)
                WriteFormat(formatArray[i], values[i]);
        }

        public void WriteFormat(char format, object value)
        {
            switch (format)
            {
                case 'b':
                    WriteByte((byte)value);
                    break;
                case 'c':
                    WriteSByte((sbyte)value);
                    break;
                case 'f':
                    WriteFloat((float)value);
                    break;
                case 'i':
                    WriteInt((int)value);
                    break;
                case 's':
                    WriteUTF((string)value);
                    break;
                case 'u':
                    WriteUInt((uint)value);
                    break;
                case 'w':
                    WriteUShort((ushort)value);
                    break;
                case 'x':
                    WriteX((int)value);
                    break;
            }
        }
        public void Seek(int offset)
        {
            this.Seek(offset, SeekOrigin.Begin);
        }

        public void Seek(int offset, SeekOrigin seekOrigin)
        {
            this.m_writer.BaseStream.Seek((long)offset, seekOrigin);
        }

        public void Clear()
        {
            this.m_writer = new BinaryWriter(new MemoryStream(), Encoding.UTF8);
        }

        public void Dispose()
        {
            this.m_writer.Flush();
            this.m_writer.Dispose();
            this.m_writer = null;
        }
    }
}