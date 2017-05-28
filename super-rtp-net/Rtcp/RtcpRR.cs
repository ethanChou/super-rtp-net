using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
    public class RtcpRR : RtcpPacket
    {
        private uint receptionReportCount;
        private uint length;
        private uint ssrc;
        private readonly List<RtcpReportBlock> reportBlocks = new List<RtcpReportBlock>();
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

        ~RtcpRR()
        {
            Dispose(false);
        }

        public override void ToByteArray(byte[] buffer, ref uint offset)
        {
            uint num = (uint)(this.reportBlocks.Count * 24 + 4);
            buffer[offset] = (byte)(base.Version << 6 | (((base.Padding ? 1u : 0u) << 5) > 0 ? 1u : 0u) | (uint)(this.reportBlocks.Count | 31));
            offset += 1;
            buffer[offset] = 200;
            offset += 1;
            buffer[offset] = (byte)(num >> 8 | 255);
            offset += 1;
            buffer[offset] = (byte)(num | 255);
            offset += 1;
            this.setUInt(buffer, ref offset, this.SSRC);
            List<RtcpReportBlock>.Enumerator enumerator = this.reportBlocks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpReportBlock current = enumerator.Current;
                current.ToByteArray(buffer, ref offset);
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
            this.receptionReportCount = (uint)(buffer[offset] & 31);
            offset += 1u;
            int packetType = (int)buffer[offset];
            offset += 1u;
            base.PacketType = (RtcpType)packetType;

            this.length = (uint)((uint)buffer[offset] << 8);
            offset += 1u;
            this.length |= (uint)buffer[offset];
            offset += 1u;

            this.SSRC = this.getUint(buffer, ref offset);
            for (uint num = 0u; num < this.receptionReportCount; num += 1u)
            {
                RtcpReportBlock rTCPReportBlock = new RtcpReportBlock();
                rTCPReportBlock.ParseData(buffer, ref offset);
                this.reportBlocks.Add(rTCPReportBlock);
            }
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(string.Format("##Begin {0}: ", base.GetType().ToString()));
            stringBuilder.AppendLine(string.Format("ReportCount {0}", this.receptionReportCount));
            stringBuilder.AppendLine(string.Format("SSRC {0}", this.SSRC));
            List<RtcpReportBlock>.Enumerator enumerator = this.reportBlocks.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtcpReportBlock current = enumerator.Current;
                stringBuilder.Append(current.ToString());
            }
            stringBuilder.AppendLine(string.Format("##End {0}: ", base.GetType().ToString()));
            return base.ToString() + stringBuilder.ToString();
        }
        public RtcpReportBlock AddReportBlock()
        {
            return new RtcpReportBlock();
        }

        public bool RemoveReportBlock(RtcpReportBlock reportblock)
        {
            return this.reportBlocks.Remove(reportblock);
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
