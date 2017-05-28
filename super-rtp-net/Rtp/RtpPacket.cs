using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
	public class RtpPacket
	{
		private int version;
		private bool marker;
		private int payloadtype;
		private short sequencenumber;
		private int timestamp;
		private uint ssrc;
		private List<uint> csrc;
		private byte[] data;
		private uint csrcCount;

		public virtual uint HeaderSize
		{
			get
			{
				uint num = 12u;
				if (this.csrc != null)
				{
					num += (uint)(this.csrc.Count * 4);
				}
				return num;
			}
		}
		public byte[] DataPointer
		{
			get
			{
				return this.data;
			}
			set
			{
				this.data = value;
			}
		}
		public uint DataSize
		{
			get
			{
				uint result;
				if (this.data == null)
				{
					result = 0u;
				}
				else
				{
					result = (uint)this.data.Length;
				}
				return result;
			}
		}
		public uint SSRC
		{
			get
			{
				return this.ssrc;
			}
			set
			{
				this.ssrc = value;
			}
		}
		public int Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				this.timestamp = value;
			}
		}
		public short SequenceNumber
		{
			get
			{
				return this.sequencenumber;
			}
			set
			{
				this.sequencenumber = value;
			}
		}
		public int PayloadType
		{
			get
			{
				return this.payloadtype;
			}
			set
			{
				this.payloadtype = value;
			}
		}
		public bool Marker
		{
			get
			{
				return this.marker;
			}
			set
			{
				this.marker = value;
			}
		}
		public int Version
		{
			get
			{
				return this.version;
			}
			set
			{
				this.version = value;
			}
		}
		public RtpPacket(byte[] data)
		{
			this.DefaultInit();
			this.ParseData(data);
		}
		public RtpPacket(RtpPacket packet)
		{
			this.DefaultInit();
			this.Version = packet.Version;
			this.Marker = packet.Marker;
			this.PayloadType = packet.PayloadType;
			this.SequenceNumber = packet.SequenceNumber;
			this.Timestamp = packet.Timestamp;
			this.SSRC = packet.SSRC;
			this.DataPointer = packet.DataPointer;
		}
		public RtpPacket()
		{
			this.DefaultInit();
		}
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Version: {0}", this.Version));
			stringBuilder.AppendLine(string.Format("Marker: {0}", this.Marker));
			stringBuilder.AppendLine(string.Format("PayloadType: {0}", this.PayloadType));
			stringBuilder.AppendLine(string.Format("SequenceNum: {0}", (ushort)this.SequenceNumber));
			stringBuilder.AppendLine(string.Format("Timestamp: {0}", this.Timestamp));
			stringBuilder.AppendLine(string.Format("SSRC: {0}", this.SSRC));
			if (this.data != null)
			{
				stringBuilder.AppendLine(string.Format("PayloadSize: {0}", this.DataSize));
			}
			return stringBuilder.ToString();
		}
		public virtual void ToByteArray(byte[] buffer, ref uint offset)
		{
			int count = this.csrc.Count;
			buffer[offset] = (byte)(this.version << 6 | (count & 15));
			offset += 1u;
			buffer[offset] = (byte)(Convert.ToInt32(this.marker) << 7 | (this.payloadtype & 127));
			offset += 1u;
			buffer[offset] = (byte)(this.sequencenumber >> 8);
			offset += 1u;
			buffer[offset] = (byte)(this.sequencenumber & 255);
			offset += 1u;
			buffer[offset] = (byte)(this.timestamp >> 24 & 255);
			offset += 1u;
			buffer[offset] = (byte)(this.timestamp >> 16 & 255);
			offset += 1u;
			buffer[offset] = (byte)(this.timestamp >> 8 & 255);
			offset += 1u;
			buffer[offset] = (byte)(this.timestamp & 255);
			offset += 1u;
			buffer[offset] = (byte)(this.ssrc >> 24 & 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.ssrc >> 16 & 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.ssrc >> 8 & 255u);
			offset += 1u;
			buffer[offset] = (byte)(this.ssrc & 255u);
			offset += 1u;
			if (this.csrc.Count > 0)
			{
				List<uint>.Enumerator enumerator = this.csrc.GetEnumerator();
				while (enumerator.MoveNext())
				{
					uint current = enumerator.Current;
					buffer[offset] = (byte)(current >> 24 & 255u);
					offset += 1u;
					buffer[offset] = (byte)(current >> 16 & 255u);
					offset += 1u;
					buffer[offset] = (byte)(current >> 8 & 255u);
					offset += 1u;
					buffer[offset] = (byte)(current & 255u);
					offset += 1u;
				}
			}
			Array.Copy(this.data, 0, buffer, offset, this.data.Length);
			offset = (uint)(offset + this.data.Length);
		}
		protected virtual void DefaultInit()
		{
			this.version = 2;
			this.marker = false;
			this.payloadtype = 0;
			this.sequencenumber = 0;
			this.timestamp = 0;
			this.ssrc = 0u;
			this.csrc = new List<uint>();
			this.csrcCount = 0u;
			this.data = null;
		}
		protected virtual void ParseData(byte[] buffer)
		{
			int num = 0;
			this.version = buffer[num] >> 6;
			bool flag = Convert.ToBoolean(buffer[num] >> 5 & 1);
			bool flag2 = Convert.ToBoolean(buffer[num] >> 4 & 1);
			int num2 = (int)(buffer[num] & 15);
			num++;
			this.marker = Convert.ToBoolean(buffer[num] >> 7);
			this.payloadtype = (int)(buffer[num] & 127);
			num++;
			short num3 = (short)(buffer[num] << 8);
			num++;
			short num4 = (short)buffer[num];
			num++;
			this.sequencenumber = (short)(num3 | num4);
			int num5 = (int)buffer[num] << 24;
			num++;
			int num6 = (int)buffer[num] << 16;
			num++;
			int num7 = (int)buffer[num] << 8;
			num++;
			int num8 = (int)buffer[num];
			num++;
			this.timestamp = (num5 | num6 | num7 | num8);
			int num9 = (int)buffer[num] << 24;
			num++;
			int num10 = (int)buffer[num] << 16;
			num++;
			int num11 = (int)buffer[num] << 8;
			num++;
			int num12 = (int)buffer[num];
			num++;
			this.ssrc = (uint)(num9 | num10 | num11 | num12);
			this.csrc.Clear();
			for (int i = 0; i < num2; i++)
			{
				int num13 = (int)buffer[num] << 24;
				num++;
				int num14 = (int)buffer[num] << 16;
				num++;
				int num15 = (int)buffer[num] << 8;
				num++;
				int num16 = (int)buffer[num];
				num++;
				uint item = (uint)(num13 | num14 | num15 | num16);
				this.csrc.Add(item);
			}
			if (flag2)
			{
				num++;
				num += (int)buffer[num];
			}
			this.data = new byte[buffer.Length - num];
			Array.Copy(buffer, num, this.data, 0, this.data.Length);
		}
	}
}
