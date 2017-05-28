using System;
using System.Net.Sockets;

namespace super.rtp.net
{
	internal class ParticipantContainer
	{
		private Socket rtpSocket;
		private Socket rtcpSocket;
		private RtpParticipant participant;
        public RtpParticipant Participant
		{
			get
			{
				return this.participant;
			}
		}
		public Socket RtcpSocket
		{
			get
			{
				return this.rtcpSocket;
			}
			set
			{
				this.rtcpSocket = value;
			}
		}
		public Socket RTPSocket
		{
			get
			{
				return this.rtpSocket;
			}
			set
			{
				this.rtpSocket = value;
			}
		}
        public ParticipantContainer(RtpParticipant p, Socket socket)
		{
			this.rtcpSocket = null;
			this.rtpSocket = socket;
			this.participant = p;
		}
		public ParticipantContainer()
		{
			this.rtpSocket = null;
			this.rtcpSocket = null;
			this.participant = null;
		}
	}
}
