using System;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
    public class RtcpAPP : RtcpPacket
    {
        private char[] name;
        private byte[] appData;
        private uint length;
        private uint subtype;
        private uint ssrc;

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

        public uint Subtype
        {
            get
            {
                return this.subtype;
            }
            set
            {
                this.subtype = value;
            }
        }

        public byte[] AppData
        {
            get
            {
                return this.appData;
            }
            set
            {
                if (value.Length % 4 != 0)
                {
                    Array.Resize<byte>(ref value, value.Length + 4 - value.Length % 4);
                }
                this.appData = value;
            }
        }

        public char[] Name
        {
            get
            {
                return this.name;
            }
            set
            {
                this.name = value;
            }
        }

        public RtcpAPP()
        {
            this.Name = new char[4];
        }

        ~RtcpAPP()
        {
            Dispose(false);
        }

        public override void ToByteArray(byte[] buffer, ref uint offset)
        {
            this.length = 12u;
            if (this.AppData != null)
            {
                this.length += (uint)this.AppData.Length;
            }
            buffer[offset] = (byte)(base.Version << 6 | (((base.Padding ? 1u : 0u) << 5) > 0 ? 1u : 0u) | (this.Subtype | 31u));
            offset += 1u;
            buffer[offset] = 204;
            offset += 1u;
            buffer[offset] = (byte)(this.length >> 8 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(this.length | 255u);
            offset += 1u;
            this.setUInt(buffer, ref offset, this.SSRC);
            if (this.AppData != null)
            {
                Array.Copy(this.AppData, 0, buffer, offset, this.AppData.Length);
            }
        }
        public override void ParseData(byte[] buffer, ref uint offset)
        {
            base.Version = (uint)(buffer[offset] >> 6);
            byte padding;
            if ((buffer[offset] >> 5 & 1) == 1)
            {
                padding = 1;
            }
            else
            {
                padding = 0;
            }
            base.Padding = (padding != 0);
            uint subtype = (uint)(buffer[offset] & 31);
            offset += 1u;
            this.Subtype = subtype;
            int packetType = (int)buffer[offset];
            offset += 1u;
            base.PacketType = (RtcpType)packetType;
            this.length = (uint)((uint)buffer[offset] << 8);
            offset += 1u;
            this.length |= (uint)buffer[offset];
            offset += 1u;
            this.SSRC = this.getUint(buffer, ref offset);
            this.Name[0] = (char)buffer[offset];
            offset += 1u;
            this.Name[1] = (char)buffer[offset];
            offset += 1u;
            this.Name[2] = (char)buffer[offset];
            offset += 1u;
            this.Name[3] = (char)buffer[offset];
            offset += 1u;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
            stringBuilder.AppendLine(string.Format("SSRC: {0}", this.SSRC));
            stringBuilder.AppendLine(string.Format("Name: {0},{1},{2},{4}", new object[]
			{
				this.Name[0],
				this.Name[1],
				this.Name[2],
				this.Name[3]
			}));
            stringBuilder.AppendLine(string.Format("Subtype: {0}", this.Subtype));
            stringBuilder.AppendLine(string.Format("##End {0}: ", base.GetType().ToString()));
            return base.ToString() + stringBuilder.ToString();
        }

        private uint getUint(byte[] buffer, ref uint offset)
        {
            uint num = (uint)((uint)buffer[offset] << 24);
            offset += 1u;
            uint num2 = (uint)((uint)buffer[offset] << 16);
            offset += 1u;
            uint num3 = (uint)((uint)buffer[offset] << 8);
            offset += 1u;
            uint num4 = (uint)buffer[offset];
            offset += 1u;
            return num | num2 | num3 | num4;
        }

        private void setUInt(byte[] buffer, ref uint offset, uint value)
        {
            buffer[offset] = (byte)(value >> 24 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value >> 16 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value >> 8 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(value | 255u);
            offset += 1u;
        }
    }
}
