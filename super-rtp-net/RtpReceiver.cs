using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;

namespace super.rtp.net
{
    public class RtpReceiver : DisposeObject
    {
        private object _objLock = new object();
        public delegate void AddRtpPacketDelegate(RtpPacket packet);
        public delegate void AddRTCPPacketDelegate(RtcpCompoundPacket packet);
        public RtpReceiver.AddRtpPacketDelegate AddRtpPacket;
        public RtpReceiver.AddRTCPPacketDelegate AddRTCPPacket;
        private Mutex mutex;
        private Socket[] RTPsockets;
        private Socket[] RTCPsockets;
        private List<ParticipantContainer> participants;
        private bool isTerminated;
        private string ReceiverName;
        public virtual string Name
        {
            get
            {
                return this.ReceiverName;
            }
            set
            {
                this.ReceiverName = value;
            }
        }
        public RtpReceiver()
        {
            this.mutex = new Mutex();
            this.participants = new List<ParticipantContainer>();
            this.RTPsockets = null;
            this.AddRtpPacket = null;
            this.isTerminated = false;
            this.ReceiverName = "UDP/IP Receiver";
        }

        ~RtpReceiver()
        {
            this.isTerminated = true;
            IDisposable disposable = this.participants as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
            IDisposable disposable2 = this.mutex;
            if (disposable2 != null)
            {
                disposable2.Dispose();
            }
            Dispose(false);
        }
        public virtual void AddParticipant(RtpParticipant participant)
        {
            lock (_objLock)
            {
                try
                {
                    Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    socket.Bind(participant.RTPEndpoint);
                    ParticipantContainer participantContainer = new ParticipantContainer(participant, socket);
                    this.participants.Add(participantContainer);
                    this.CreateSocketArrays();
                    if (participant.IsRTCPActive)
                    {
                        Socket socket2 = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                        socket2.Bind(participant.RTCPEndpoint);
                        participantContainer.RtcpSocket = socket2;
                    }
                }
                catch
                {
                }
            }
        }
        public static void ThreadProc(object obj)
        {
            (obj as RtpReceiver).Run();
        }
        protected virtual void Run()
        {
            while (!this.isTerminated)
            {
                this.SelectReadSockets();
            }
        }
        protected virtual void SelectReadSockets()
        {
            byte[] array = null;
            byte[] buffer = null;
            try
            {
                if (this.AddRtpPacket != null)
                {
                    Socket.Select(this.RTPsockets, null, null, 1);
                    if (this.RTPsockets.Length != 0)
                    {
                        for (int i = 0; i < this.RTPsockets.Length; i++)
                        {
                            Socket socket = this.RTPsockets[i];
                            if (socket != null)
                            {
                                array = new byte[4096];
                                int newSize = socket.Receive(array);
                                Array.Resize<byte>(ref array, newSize);
                                RtpPacket packet = new RtpPacket(array);
                                this.AddRtpPacket(packet);
                            }
                        }
                    }
                }


                if (this.AddRTCPPacket != null)
                {

                    if (this.RTCPsockets != null)
                    {
                        Socket.Select(this.RTCPsockets, null, null, 1);
                        if (this.RTCPsockets.Length != 0)
                        {
                            for (int j = 0; j < this.RTCPsockets.Length; j++)
                            {
                                Socket socket2 = this.RTCPsockets[j];
                                if (socket2 != null)
                                {
                                    buffer = new byte[4096];
                                    int newSize2 = socket2.Receive(buffer);
                                    Array.Resize<byte>(ref buffer, newSize2);
                                    RtcpCompoundPacket rTCPCompoundPacket = new RtcpCompoundPacket();
                                    uint num = 0u;
                                    rTCPCompoundPacket.ParseData(buffer, ref num);
                                    this.AddRTCPPacket(rTCPCompoundPacket);
                                }
                            }
                        }
                    }
                }

                this.CreateSocketArrays();
            }
            catch
            {
                throw;
            }
        }
        private void CreateSocketArrays()
        {
            if (this.RTPsockets != null)
            {
                this.RTPsockets = null;
            }
            this.RTPsockets = new Socket[this.participants.Count];
            if (this.RTCPsockets != null)
            {
                this.RTCPsockets = null;
            }
            this.RTCPsockets = new Socket[this.participants.Count];
            int num = 0;
            int num2 = 0;
            foreach (var item in this.participants)
            {
                ParticipantContainer current = item;
                this.RTPsockets[(int)num] = current.RTPSocket;
                if (current.RtcpSocket != null)
                {
                    this.RTCPsockets[(int)num2] = current.RtcpSocket;
                    num2 += 1;
                }
                num += 1;
            }
            if (num2 == 0)
            {
                this.RTCPsockets = null;
                this.RTCPsockets = null;
            }
            else
            {
                Array.Resize<Socket>(ref this.RTCPsockets, (int)num2);
            }
        }

    }
}
