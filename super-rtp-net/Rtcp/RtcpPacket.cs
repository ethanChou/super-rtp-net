using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace super.rtp.net
{
    public abstract class RtcpPacket : DisposeObject
    {
        private uint version;
        private bool padding;
        private RtcpType packetType;

        public uint Version
        {
            get { return version; }
            set { version = value; }
        }

        public bool Padding
        {
            get { return padding; }
            set { padding = value; }
        }

        public RtcpType PacketType
        {
            get { return packetType; }
            set { packetType = value; }
        }

        public abstract void ToByteArray(byte[] buffer, ref uint offset);

		public abstract void ParseData(byte[] buffer, ref uint offset);

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("Version: {0}", this.Version));
			stringBuilder.AppendLine(string.Format("Padding: {0}", this.Padding));
			stringBuilder.AppendLine(string.Format("Type: {0}", this.PacketType));
			return stringBuilder.ToString();
		}

        protected RtcpPacket()
		{
			this.Version = 2;
			this.Padding = false;
		}

        ~RtcpPacket() { }
    }
}
