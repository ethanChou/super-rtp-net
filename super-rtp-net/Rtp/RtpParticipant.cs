using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Timers;

namespace super.rtp.net
{
	public class RtpParticipant : DisposeObject
	{
		public delegate bool RTCPTimerCallBack(RtpParticipant participant);
		public RtpParticipant.RTCPTimerCallBack OnRTCPTimer;
		private uint ssrc;
		private short seqNum;
		private uint packetCount;
		private uint dataCount;
		private bool active;
		private IPEndPoint rtpEndpoint;
		private IPEndPoint rtcpEndpoint;
		private bool isRTCPActive;
		private uint tp;
		private uint tn;
		private uint tc;
		private uint pmembers;
		private uint members;
		private uint senders;
		private uint rtcp_bw;
		private bool we_sent;
		private uint avg_rtcp_size;
		private bool initial;
		private Timer timer;

		public bool Active
		{
			get
			{
				return this.active;
			}
			set
			{
				this.active = value;
			}
		}
		public uint OctetCount
		{
			get
			{
				return this.dataCount;
			}
			set
			{
				this.dataCount = value;
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
		public bool IsRTCPActive
		{
			get
			{
				return this.isRTCPActive;
			}
		}
		public IPEndPoint RTCPEndpoint
		{
			get
			{
				return this.rtcpEndpoint;
			}
			set
			{
				this.rtcpEndpoint = value;
			}
		}
		public IPEndPoint RTPEndpoint
		{
			get
			{
				return this.rtpEndpoint;
			}
			set
			{
				this.rtpEndpoint = value;
			}
		}
		public virtual short SequenceNumber
		{
			get
			{
				return this.seqNum;
			}
			set
			{
				this.seqNum = value;
			}
		}
		public virtual uint SSRC
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

		public RtpParticipant(IPEndPoint RTPEndpoint, IPEndPoint RTCPEndpoint)
		{
			this.isRTCPActive = false;
			this.RTCPEndpoint = RTCPEndpoint;
			this.RTPEndpoint = RTPEndpoint;
			this.Init();
		}

		public RtpParticipant(IPEndPoint RTPEndpoint)
		{
			this.isRTCPActive = false;
			this.RTCPEndpoint = null;
			this.RTPEndpoint = RTPEndpoint;
			this.Init();
		}

        ~RtpParticipant()
		{
            Dispose(false);
		}

		public virtual void Init()
		{
			Random random = new Random();
			this.ssrc = (uint)random.Next();
			this.seqNum = (short)random.Next();
			this.PacketCount = 0u;
			this.OctetCount = 0u;
			this.Active = true;
			this.OnRTCPTimer = null;
			this.tp = 0u;
			this.tc = 0u;
			this.tn = 2500u;
			this.initial = true;
			this.pmembers = 1u;
			this.members = 1u;
			this.we_sent = false;
			this.rtcp_bw = 1000u;
			this.avg_rtcp_size = 200u;
			int num;
			if (this.RTCPEndpoint != null)
			{
				num = 1;
			}
			else
			{
				num = 0;
			}
			this.isRTCPActive = (num != 0);
			if (this.isRTCPActive)
			{
				this.timer = new Timer();
				this.timer.Interval = this.tn;
				this.timer.Elapsed += new ElapsedEventHandler(this.OnFireRTCPTimer);
			}
		}
		public virtual byte[] PrepareSendRTP(RtpPacket packet)
		{
			byte[] array = null;
			uint sSRC = packet.SSRC;
			short sequenceNumber = packet.SequenceNumber;
			packet.SSRC = this.SSRC;
			packet.SequenceNumber = this.SequenceNumber;
			array = new byte[packet.DataSize + 1000u];
			uint newSize = 0u;
			packet.ToByteArray(array, ref newSize);
			Array.Resize<byte>(ref array, (int)newSize);
			packet.SSRC = sSRC;
			packet.SequenceNumber = sequenceNumber;
			return array;
		}
		protected void OnFireRTCPTimer(object source, ElapsedEventArgs e)
		{
			if (!(this.OnRTCPTimer == null))
			{
				bool flag = this.OnRTCPTimer(this);
			}
		}
	}
}
