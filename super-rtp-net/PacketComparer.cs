using System;
using System.Collections.Generic;

namespace super.rtp.net
{
	internal class PacketComparer : IComparer<RtpPacket>
	{
        public virtual int Compare(RtpPacket x, RtpPacket y)
		{
			return (int)((ushort)x.SequenceNumber - (ushort)y.SequenceNumber);
		}
	}
}
