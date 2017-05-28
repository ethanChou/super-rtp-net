using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace super.rtp.net
{
    public class RtcpSR : RtcpPacket
    {
        private uint receptionReportCount;
        private uint length;
        private uint ssrc;
        private uint ntpTimestampIntegerPart;
        private uint ntpTimestampFractionPart;
        private ValueType ntpTime;
        private uint rtpTimestamp;
        private uint packetCount;
        private uint octetCount;
        private readonly List<RtcpReportBlock> reportBlocks = new List<RtcpReportBlock>();
        public uint OctetCount
        {
            get
            {
                return this.octetCount;
            }
            set
            {
                this.octetCount = value;
            }
        }
        public uint PacketCount
        {
            get
            {
                return this.packetCount;
            }
            set
            {
                this.packetCount = value;
            }
        }
        public uint RTPTimestamp
        {
            get
            {
                return this.rtpTimestamp;
            }
            set
            {
                this.rtpTimestamp = value;
            }
        }
        public ValueType NtpTime
        {
            get
            {
                return this.ntpTime;
            }
            set
            {
                this.ntpTime = value;
                ValueType valueType = default(DateTime);
                valueType = new DateTime(1900, 1, 1);
                ValueType valueType2 = valueType;
                double totalSeconds = ((DateTime)value).Subtract((DateTime)valueType2).TotalSeconds;
                this.ntpTimestampIntegerPart = (uint)totalSeconds;
                this.ntpTimestampFractionPart = (uint)((totalSeconds - this.ntpTimestampIntegerPart) * 1000.0);
                double num = this.ntpTimestampIntegerPart;
                double num2 = this.ntpTimestampFractionPart / 4294967295.0;
                ValueType valueType3 = default(DateTime);
                valueType3 = new DateTime(1900, 1, 1);
                ValueType valueType4 = valueType3;
                valueType4 = ((DateTime)valueType4).AddSeconds(num + num2);
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
        public RtcpSR()
        {
            try
            {
                base.PacketType = RtcpType.SR;
            }
            catch
            {
                base.Dispose(true);
                throw;
            }
        }

        ~RtcpSR()
        {
            Dispose(false);
        }

        public override void ToByteArray(byte[] buffer, ref uint offset)
        {
            uint num = (uint)(this.reportBlocks.Count * 24 + 4);
            buffer[offset] = (byte)(base.Version << 6 | (((base.Padding ? 1u : 0u) << 5) > 0 ? 1u : 0u) | (uint)(this.reportBlocks.Count | 31));
            offset += 1u;
            buffer[offset] = 200;
            offset += 1u;
            buffer[offset] = (byte)(num >> 8 | 255u);
            offset += 1u;
            buffer[offset] = (byte)(num | 255u);
            offset += 1u;
            this.setUInt(buffer, ref offset, this.SSRC);
            this.setUInt(buffer, ref offset, this.ntpTimestampIntegerPart);
            this.setUInt(buffer, ref offset, this.ntpTimestampFractionPart);
            this.setUInt(buffer, ref offset, this.RTPTimestamp);
            this.setUInt(buffer, ref offset, this.PacketCount);
            this.setUInt(buffer, ref offset, this.OctetCount);
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
            offset += 1;
            int packetType = (int)buffer[offset];
            offset += 1;
            base.PacketType = (RtcpType)packetType;
            this.length = (uint)((uint)buffer[offset] << 8);
            offset += 1;
            this.length |= (uint)buffer[offset];
            offset += 1;
            this.ssrc = this.getUint(buffer, ref offset);
            this.ntpTimestampIntegerPart = this.getUint(buffer, ref offset);
            this.ntpTimestampFractionPart = this.getUint(buffer, ref offset);
            double num = this.ntpTimestampIntegerPart;
            double num2 = this.ntpTimestampFractionPart / 4294967295.0;
            ValueType valueType = default(DateTime);
            valueType = new DateTime(1900, 1, 1);
            ValueType valueType2 = valueType;
            valueType2 = ((DateTime)valueType2).AddSeconds(num + num2);
            this.ntpTime = valueType2;
            this.rtpTimestamp = this.getUint(buffer, ref offset);
            this.packetCount = this.getUint(buffer, ref offset);
            this.octetCount = this.getUint(buffer, ref offset);

            for (int i = 0; i < this.receptionReportCount; i += 1)
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
            stringBuilder.AppendLine(string.Format("NTP-Time {0}", ((DateTime)this.NtpTime).ToString()));
            stringBuilder.AppendLine(string.Format("RTP-Time {0}", this.RTPTimestamp));
            stringBuilder.AppendLine(string.Format("Packetcount {0}", this.PacketCount));
            stringBuilder.AppendLine(string.Format("Octetcount {0}", this.OctetCount));
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
