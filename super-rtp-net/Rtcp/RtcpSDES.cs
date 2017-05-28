using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
    public class RtcpSDES : RtcpPacket
    {
        private uint sourceCount;
        private uint length;
        private readonly List<RtcpSDESChunk> chunks_ = new List<RtcpSDESChunk>();
        public uint SourceCount
        {
            get
            {
                return this.sourceCount;
            }
        }

        public RtcpSDES()
        {
            base.PacketType = RtcpType.SDES;
        }

        ~RtcpSDES()
        {
            Dispose(false);
        }

        public override void ToByteArray(byte[] buffer, ref uint offset)
        {
            buffer[offset] = (byte)(base.Version << 6 | (((base.Padding ? 1u : 0u) << 5) > 0 ? 1u : 0u) | (uint)(this.chunks_.Count & 31));
            offset += 1u;
            buffer[offset] = 202;
            offset += 1u;
            int num = (int)offset;
            buffer[offset] = 0;
            offset += 1u;
            buffer[offset] = 0;
            offset += 1u;
            int num2 = (int)offset;
            List<RtcpSDESChunk>.Enumerator enumerator = this.chunks_.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpSDESChunk current = enumerator.Current;
                current.ToByteArray(buffer, ref offset);
            }
            buffer[num] = (byte)((uint)(offset - num2) >> 8 & 255u);
            buffer[num + 1] = (byte)(offset - num2 & 255);
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
            for (uint num = 0u; num < this.sourceCount; num += 1u)
            {
                RtcpSDESChunk rTCPSDESChunk = new RtcpSDESChunk();
                rTCPSDESChunk.ParseData(buffer, ref offset);
                this.chunks_.Add(rTCPSDESChunk);
            }
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
            stringBuilder.AppendLine(string.Format("SourceCount: {0}", this.sourceCount));
            List<RtcpSDESChunk>.Enumerator enumerator = this.chunks_.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpSDESChunk current = enumerator.Current;
                stringBuilder.Append(current.ToString());
            }
            stringBuilder.AppendLine(string.Format("##End {0}: ", base.GetType().ToString()));
            return base.ToString() + stringBuilder.ToString();
        }

    }
}
