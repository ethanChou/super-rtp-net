using System;
using System.Runtime.InteropServices;

namespace super.rtp.net
{
    /// <summary>
    /// RTP Payload Format for ITU-T Rec. H.263 Video
    /// https://tools.ietf.org/html/rfc4629
    /// </summary>
	public class Rtp4629 : RtpPacket
	{
		private bool pictureStartCompression;
		private bool vrcPresent;
		private uint plen;
		private uint pebit;

		public override uint HeaderSize
		{
			get
			{
				return base.HeaderSize + 2u;
			}
		}
		public uint Pebit
		{
			get
			{
				return this.pebit;
			}
			set
			{
				this.pebit = value;
			}
		}
		public uint Plen
		{
			get
			{
				return this.plen;
			}
			set
			{
				this.plen = value;
			}
		}
		public bool VrcPresent
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.vrcPresent;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.vrcPresent = value;
			}
		}
		public bool PictureStartCompression
		{
			[return: MarshalAs(UnmanagedType.U1)]
			get
			{
				return this.pictureStartCompression;
			}
			[param: MarshalAs(UnmanagedType.U1)]
			set
			{
				this.pictureStartCompression = value;
			}
		}
		public Rtp4629(RtpPacket packet) : base(packet)
		{
			this.ParseData(packet.DataPointer, 0u);
		}
		public Rtp4629()
		{
			this.PictureStartCompression = false;
			this.VrcPresent = false;
			this.Plen = 0u;
			this.Pebit = 0u;
		}
		public override void ToByteArray(byte[] buffer, ref uint offset)
		{
			uint num = 2u;
			base.ToByteArray(buffer, ref offset);
			Array.Copy(buffer, (int)base.HeaderSize, buffer, (int)(base.HeaderSize + num), (int)(offset - base.HeaderSize));
			int num2;
			if (this.PictureStartCompression)
			{
				num2 = 4;
			}
			else
			{
				num2 = 0;
			}
			buffer[(int)base.HeaderSize] = (byte)num2;
			buffer[(int)(base.HeaderSize + 1u)] = 0;
			offset += num;
		}
		protected virtual void ParseData(byte[] buffer, uint offset)
		{
			ushort num = (ushort)(buffer[(int)offset] << 8);
			offset += 1u;
			ushort num2 = (ushort)buffer[(int)offset];
			offset += 1u;
			ushort num3 =(ushort)(num | num2);
			this.PictureStartCompression = ((byte)(num3 >> 10 & 1) != 0);
			this.VrcPresent = ((byte)(num3 >> 9 & 1) != 0);
			this.Plen = (uint)(num3 >> 2 & 63);
			this.Pebit = (uint)(num3 & 7);
			uint num4 = 0u;
			uint num5 = (uint)(buffer.Length - (int)offset);
			if (this.PictureStartCompression)
			{
				num5 += 2u;
				num4 = 2u;
			}
			base.DataPointer = new byte[num5];
			Array.Copy(buffer, (int)offset, base.DataPointer, (int)num4, base.DataPointer.Length - (int)num4);
			if (this.PictureStartCompression)
			{
				base.DataPointer[0] = 0;
				base.DataPointer[1] = 0;
			}
		}
	}
}
