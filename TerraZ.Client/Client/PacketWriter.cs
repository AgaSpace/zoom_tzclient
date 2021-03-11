using System;
using System.IO;
using System.Text;

namespace TerraZ.Client
{
	public class PacketWriter
	{
		public PacketWriter()
		{
			this.Stream = new MemoryStream();
			this.Writer = new BinaryWriter(this.Stream);
			this.Writer.BaseStream.Position = 3L;
		}
		public PacketWriter SetType(short type)
		{
			long position = this.Writer.BaseStream.Position;
			this.Writer.BaseStream.Position = 2L;
			this.Writer.Write(type);
			this.Writer.BaseStream.Position = position;
			return this;
		}
		public PacketWriter PackByte(byte num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackInt16(short num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackUInt16(ushort num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackInt32(int num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackUInt32(uint num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackUInt64(ulong num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackSingle(float num)
		{
			this.Writer.Write(num);
			return this;
		}
		public PacketWriter PackString(string str)
		{
			this.Writer.Write(str);
			return this;
		}
		private void UpdateLength()
		{
			long position = this.Writer.BaseStream.Position;
			this.Writer.BaseStream.Position = 0L;
			this.Writer.Write((short)position);
			this.Writer.BaseStream.Position = position;
		}
		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		public byte[] GetByteData()
		{
			this.UpdateLength();
			return this.Stream.ToArray();
		}

		private MemoryStream Stream;
		private BinaryWriter Writer;
	}
}
