using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;

namespace super.rtp.net
{
    public class RtpSession : DisposeObject
    {
        public delegate void NewSSRC_Callback(uint SSRC);
        public delegate bool NewRtpPacket_Callback(RtpPacket packet);
        public delegate void NewRTCPPacket_Callback(RtcpCompoundPacket packet);

        public RtpSession.NewSSRC_Callback NewSSRC;
        public RtpSession.NewRtpPacket_Callback NewRtpPacket;
        public RtpSession.NewRTCPPacket_Callback NewRTCPPacket;

        private readonly List<RtpSender> senders = new List<RtpSender>();
        private readonly List<RtpReceiver> receivers = new List<RtpReceiver>();
        private readonly List<RtpPacket> packets = new List<RtpPacket>();
        private readonly List<uint> ssrcs = new List<uint>();
        private uint packetsSent;
        private Mutex mutex;
        private object _objLock = new object();

        public uint ReceivedPacketCount
        {
            get
            {
                return (uint)this.packets.Count;
            }
        }

        public RtpPacket this[uint ssrc]
        {
            get
            {
                RtpPacket p = this.packets.Find(t => t.SSRC == ssrc);
                this.packets.Remove(p);
                return p;
            }
        }
        public uint PacketsSent
        {
            get
            {
                return this.packetsSent;
            }
        }
        public RtpSession()
        {
            this.packetsSent = 0u;
            this.NewSSRC = null;
            this.NewRtpPacket = null;
            this.mutex = new Mutex();
        }
        ~RtpSession()
        {
            IDisposable disposable = this.mutex;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }
        public void AddSender(RtpSender sender)
        {
            this.senders.Add(sender);
        }
        public bool RemoveSender(RtpSender sender)
        {
            return this.senders.Remove(sender);
        }
        public void AddReceiver(RtpReceiver receiver)
        {
            receiver.AddRtpPacket = new RtpReceiver.AddRtpPacketDelegate(this.AddReceivedRtpPacket);
            receiver.AddRTCPPacket = new RtpReceiver.AddRTCPPacketDelegate(this.AddReceivedRTCPPacket);
            this.receivers.Add(receiver);
            new Thread(new ParameterizedThreadStart(RtpReceiver.ThreadProc))
            {
                Name = receiver.Name
            }.Start(receiver);
        }
        public void SendPacket(RtpPacket packet)
        {
            List<RtpSender>.Enumerator enumerator = this.senders.GetEnumerator();
            while (enumerator.MoveNext())
            {
                RtpSender current = enumerator.Current;
                current.Send(packet);
            }
        }
        public RtpPacket GetNextPacket()
        {
            if (this.packets.Count <= 0)
            {
                return null;
            }
            RtpPacket p = this.packets[0];
            this.packets.Remove(p);
            return p;
        }
        public RtpFrame GetNextFrame(uint ssrc, int timestamp)
        {
            RtpFrame frame = null;
            frame = new RtpFrame();
            uint num = 0u;

            foreach (var item in this.packets)
            {
                RtpPacket current = item;
                if (current.SSRC == ssrc && current.Timestamp == timestamp)
                {
                    num += 1u;
                    frame.AddPacket(current);
                }
            }
            Console.WriteLine("packets found {0}", num);
            if (frame.PacketCount != 0)
            {
                return frame;
            }
            else
            {
                return null;
            }
        }

        private void AddReceivedRtpPacket(RtpPacket packet)
        {
            if (!this.ssrcs.Contains(packet.SSRC))
            {
                if (this.NewSSRC != null)
                {
                    this.ssrcs.Add(packet.SSRC);
                    this.NewSSRC(packet.SSRC);
                }
            }

            this.packets.Add(packet);

            if (this.NewRtpPacket != null)
            {
                bool flag2 = this.NewRtpPacket(packet);
                if (!flag2)
                {
                    if (this.packets.Contains(packet))
                    {
                        this.packets.Remove(packet);
                    }
                }
            }
        }
        private void AddReceivedRTCPPacket(RtcpCompoundPacket packet)
        {
            string value = packet.ToString();
            Console.WriteLine(value);
        }
    }
}
