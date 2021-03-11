using System;
using System.IO;
using System.Text;

namespace TerraZ.Client
{
	// Token: 0x02000008 RID: 8
	public class PacketWriter
	{
		// Token: 0x0600000D RID: 13 RVA: 0x000020C1 File Offset: 0x000002C1
		public PacketWriter()
		{
			this.Stream = new MemoryStream();
			this.Writer = new BinaryWriter(this.Stream);
			this.Writer.BaseStream.Position = 3L;
		}

		// Token: 0x0600000E RID: 14 RVA: 0x0000249C File Offset: 0x0000069C
		public PacketWriter SetType(short type)
		{
			long position = this.Writer.BaseStream.Position;
			this.Writer.BaseStream.Position = 2L;
			this.Writer.Write(type);
			this.Writer.BaseStream.Position = position;
			return this;
		}

		// Token: 0x0600000F RID: 15 RVA: 0x000020F7 File Offset: 0x000002F7
		public PacketWriter PackByte(byte num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000010 RID: 16 RVA: 0x00002106 File Offset: 0x00000306
		public PacketWriter PackInt16(short num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000011 RID: 17 RVA: 0x00002115 File Offset: 0x00000315
		public PacketWriter PackUInt16(ushort num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002124 File Offset: 0x00000324
		public PacketWriter PackInt32(int num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002133 File Offset: 0x00000333
		public PacketWriter PackUInt32(uint num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002142 File Offset: 0x00000342
		public PacketWriter PackUInt64(ulong num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002151 File Offset: 0x00000351
		public PacketWriter PackSingle(float num)
		{
			this.Writer.Write(num);
			return this;
		}

		// Token: 0x06000016 RID: 22 RVA: 0x00002160 File Offset: 0x00000360
		public PacketWriter PackString(string str)
		{
			this.Writer.Write(str);
			return this;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000024EC File Offset: 0x000006EC
		private void UpdateLength()
		{
			long position = this.Writer.BaseStream.Position;
			this.Writer.BaseStream.Position = 0L;
			this.Writer.Write((short)position);
			this.Writer.BaseStream.Position = position;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x0000253C File Offset: 0x0000073C
		public static string ByteArrayToString(byte[] ba)
		{
			StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
			foreach (byte b in ba)
			{
				stringBuilder.AppendFormat("{0:x2}", b);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x0000216F File Offset: 0x0000036F
		public byte[] GetByteData()
		{
			this.UpdateLength();
			return this.Stream.ToArray();
		}

		// Token: 0x04000009 RID: 9
		private MemoryStream Stream;

		// Token: 0x0400000A RID: 10
		private BinaryWriter Writer;
	}
}
