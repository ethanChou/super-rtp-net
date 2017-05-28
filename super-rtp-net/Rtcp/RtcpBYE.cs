using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
namespace super.rtp.net
{
    public class RtcpBYE : RtcpPacket
    {
        private uint sourceCount;
        private string reason;
        private uint length;
        private readonly List<uint> ssrcList = new List<uint>();
        public string Reason
        {
            get
            {
                return this.reason;
            }
            set
            {
                this.reason = value;
            }
        }
        public RtcpBYE()
        {
            base.PacketType = RtcpType.BYE;
            this.reason = null;
        }

        ~RtcpBYE()
        {
            Dispose(false);
        }

        public override void ToByteArray(byte[] buffer, ref uint offset)
        {
            int num = 0;
            num += this.ssrcList.Count * 4;

            if (!string.IsNullOrEmpty(this.Reason))
            {
                num += Encoding.UTF8.GetByteCount(this.Reason);
            }
            buffer[offset] = (byte)(base.Version << 6 | (((base.Padding ? 1u : 0u) << 5) > 0 ? 1u : 0u) | (uint)(this.ssrcList.Count & 31));
            offset += 1u;
            buffer[offset] = 203;
            offset += 1u;
            buffer[offset] = (byte)(num >> 8 & 255);
            offset += 1u;
            buffer[offset] = (byte)(num & 255);
            offset += 1u;
            List<uint>.Enumerator enumerator = this.ssrcList.GetEnumerator();
            while (enumerator.MoveNext())
            {
                uint current = enumerator.Current;
                this.setUInt(buffer, ref offset, current);
            }
            if (string.IsNullOrEmpty(this.Reason))
            {
                byte[] bytes = Encoding.UTF8.GetBytes(this.Reason);
                buffer[offset] = (byte)bytes.Length;
                offset += 1u;
                Array.Copy(bytes, 0, buffer, offset, bytes.Length);
                offset = (uint)(offset + bytes.Length);
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
            this.sourceCount = (uint)(buffer[offset] & 31);
            offset += 1u;
            int packetType = (int)buffer[offset];
            offset += 1u;
            base.PacketType = (RtcpType)packetType;
            this.length = (uint)((uint)buffer[offset] << 8);
            offset += 1u;
            this.length |= (uint)buffer[offset];
            offset += 1u;
            if (this.sourceCount != 0u)
            {
                for (uint num = 0u; num < this.sourceCount; num += 1u)
                {
                    this.ssrcList.Add(this.getUint(buffer, ref offset));
                }
                if (offset != this.length)
                {
                    uint count = (uint)buffer[offset];
                    offset += 1u;
                    this.Reason = Encoding.UTF8.GetString(buffer, (int)offset, (int)count);
                }
            }
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
            stringBuilder.AppendLine(string.Format("SourceCount: {0}", this.sourceCount));
            if (this.sourceCount != 0u)
            {
                List<uint>.Enumerator enumerator = this.ssrcList.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    uint current = enumerator.Current;
                    stringBuilder.AppendLine(string.Format("SSRC: {0}", current));
                }
            }
            if (!string.IsNullOrEmpty(this.Reason))
            {
                stringBuilder.AppendLine(string.Format("Reason: {0}", this.Reason));
            }
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
