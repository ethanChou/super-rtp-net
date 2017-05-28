using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
namespace super.rtp.net
{
    public class RtpSender : DisposeObject
    {
        private object _objLock = new object();
        private readonly List<RtpParticipant> pc = new List<RtpParticipant>();
        private uint packetsSent;
        private IPEndPoint localEP;
        private UdpSocket client;
        private bool isDisposed;
        private Mutex mutex;
        public uint PacketsSent
        {
            get
            {
                return this.packetsSent;
            }
        }
        public RtpSender(IPEndPoint localEndpoint)
        {
            this.localEP = localEndpoint;
            this.DefaultInit();
            this.NetworkInit();
        }
        public RtpSender()
        {
            this.localEP = new IPEndPoint(IPAddress.Any, 0);
            this.DefaultInit();
            this.NetworkInit();
        }

        ~RtpSender()
        {
            if (this.client != null)
            {
                IDisposable disposable = this.client;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            Dispose(false);
        }

        public void AddParticipant(RtpParticipant participant)
        {
            lock (_objLock)
            {
                try
                {
                    participant.OnRTCPTimer = new RtpParticipant.RTCPTimerCallBack(this.OnRTCPTimerCallBack);
                    this.pc.Add(participant);
                }
                catch
                {
                }
            }
        }

        public bool RemoveParticipant(RtpParticipant participant)
        {
            lock (_objLock)
            {
                int c = this.pc.RemoveAll(t => t.SSRC == participant.SSRC);
                return c > 0 ? true : false;
            }
        }

        public void Send(RtpPacket packet)
        {
            try
            {
                foreach (var item in this.pc)
                {
                    RtpParticipant current = item;
                    if (current.Active)
                    {
                        IPEndPoint rTPEndpoint = current.RTPEndpoint;
                        byte[] array = current.PrepareSendRTP(packet);
                        if (array != null)
                        {
                            this.client.Send(array, rTPEndpoint);
                            short num = current.SequenceNumber;
                            num += 1;
                            current.SequenceNumber = num;
                            uint num2 = current.PacketCount;
                            num2 += 1u;
                            current.PacketCount = num2;
                            current.OctetCount += packet.DataSize;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public bool OnRTCPTimerCallBack(RtpParticipant participant)
        {
            return true;
        }

        protected virtual void NetworkInit()
        {
            this.client = new UdpSocket(this.localEP);
        }

        protected virtual void DefaultInit()
        {
            this.packetsSent = 0u;
            this.client = null;
            this.mutex = new Mutex();
        }

    }
}
